using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xml.Linq;
using Quire.Calculation;
using Quire.Controls.Core;
using Quire.Controls.Plots;
using Quire.Core;
using Quire.EquationEditor.Tree.Transform;
using Quire.PersistentDataObjects;

namespace Quire.Controls.Worksheet;

/// <summary>
/// The worksheet surface: pages with a grid, a crosshair insertion point, and freely positioned regions.
/// Typing on empty space creates a math region at the crosshair (<c>"</c> creates a text region);
/// click a region to edit it, drag it to move it, and drag on empty space to select several.
/// </summary>
public class WorksheetControl : UserControl, IDisposable
{
    public const double PageWidth = 816, PageHeight = 1056, Margin_ = 48, Grid = 8;
    private const double ContentWidth = PageWidth - 2 * Margin_, ContentHeight = PageHeight - 2 * Margin_;
    private const string ClipboardFormat = "Quire.Regions";

    private readonly WorksheetModel _model = new();
    private readonly CalculationController _controller;
    private readonly IControlFactory _factory = ControlFactory.Instance;
    private readonly Canvas _canvas;
    private readonly Canvas _pages;
    private readonly ScaleTransform _zoom = new(1, 1);
    private readonly Path _crosshair;
    private readonly Rectangle _rubberBand;
    private readonly ScrollViewer _scroll;
    private readonly List<RegionControl> _selection = [];
    private readonly Stack<string> _undo = new(), _redo = new();

    private Point _cursor = new(0, 0);
    private Point? _dragStart;
    private Dictionary<RegionControl, Point>? _dragOrigins;
    private bool _dragMoved, _rubberBanding, _restoring;
    private RegionControl? _pressedRegion;
    private string _lastSnapshot = "";
    private int _calcBusy;

    public WorksheetControl()
    {
        _controller = new CalculationController();
        _controller.BusyChanged += (_, busy) =>
        {
            _calcBusy = Math.Max(0, _calcBusy + (busy ? 1 : -1));
            StatusChanged?.Invoke(this, EventArgs.Empty);
        };

        _canvas = new Canvas
        {
            Width = ContentWidth, Height = ContentHeight,
            Background = GridBrush(), ClipToBounds = false, Focusable = true, FocusVisualStyle = null,
        };
        _pages = new Canvas { Width = PageWidth, Height = PageHeight, Background = new SolidColorBrush(Color.FromRgb(0xE6, 0xE8, 0xEC)) };
        Canvas.SetLeft(_canvas, Margin_);
        Canvas.SetTop(_canvas, Margin_);

        _crosshair = new Path
        {
            Data = Geometry.Parse("M -6,0 L 6,0 M 0,-8 L 0,8"),
            Stroke = new SolidColorBrush(Color.FromRgb(0xD0, 0x20, 0x20)), StrokeThickness = 1.5, IsHitTestVisible = false,
        };
        _rubberBand = new Rectangle
        {
            Stroke = new SolidColorBrush(Color.FromRgb(0x3C, 0x8C, 0xDC)), StrokeDashArray = [3, 2],
            Fill = new SolidColorBrush(Color.FromArgb(0x20, 0x3C, 0x8C, 0xDC)), Visibility = Visibility.Collapsed, IsHitTestVisible = false,
        };
        Panel.SetZIndex(_rubberBand, 1000);
        _canvas.Children.Add(_crosshair);
        _canvas.Children.Add(_rubberBand);

        var host = new Grid { LayoutTransform = _zoom, HorizontalAlignment = HorizontalAlignment.Center, Margin = new Thickness(16) };
        host.Children.Add(_pages);
        _pages.Children.Add(_canvas);
        _scroll = new ScrollViewer
        {
            Content = host, HorizontalScrollBarVisibility = ScrollBarVisibility.Auto, VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
            Background = new SolidColorBrush(Color.FromRgb(0xD5, 0xD8, 0xDE)), Focusable = false,
        };
        Content = _scroll;

        _canvas.MouseLeftButtonDown += HandleCanvasMouseDown;
        _canvas.MouseMove += HandleCanvasMouseMove;
        _canvas.MouseLeftButtonUp += HandleCanvasMouseUp;
        _canvas.PreviewKeyDown += HandleKeyDown;
        _canvas.TextInput += HandleTextInput;
        PreviewMouseWheel += HandleMouseWheel;

        _controller.BindTo(_model);
        MoveCursor(new Point(0, 0));
        UpdatePages();
        Loaded += (_, _) => _canvas.Focus();
        _lastSnapshot = Snapshot();
    }

    // ---------------------------------------------------------------- public surface

    public WorksheetModel Model => _model;

    public event EventHandler? DocumentChanged;
    public event EventHandler? StatusChanged;
    public event EventHandler? SelectionChanged;

    public bool IsCalculating => _calcBusy > 0;

    public int ErrorCount => _model.Items.Count(r => r.HasError);

    public int RegionCount => _model.Items.Count;

    public bool CanUndo => _undo.Count > 0;
    public bool CanRedo => _redo.Count > 0;

    public IReadOnlyList<RegionControl> SelectedRegions => _selection;

    public RegionControl? EditingRegion => _model.Items.FirstOrDefault(r => r.IsEditing);

    public double Zoom
    {
        get => _zoom.ScaleX;
        set
        {
            var z = Math.Clamp(value, 0.5, 3);
            _zoom.ScaleX = _zoom.ScaleY = z;
            StatusChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public void Load(WorksheetDocument doc)
    {
        _restoring = true;
        try
        {
            ClearSelection();
            foreach (var r in _model.Items.ToArray()) RemoveRegionCore(r);
            _model.SetUnitSystem(doc.Settings.UnitSystem);
            _model.SetAutoCalc(doc.Settings.AutoCalc);
            _model.SetResultFormat(new ResultFormat(doc.Settings.Decimals, doc.Settings.ExponentialThreshold, doc.Settings.ShowTrailingZeros));
            foreach (var data in doc.Regions)
            {
                try { AddRegionCore(_factory.CreateRegion(data), new Point(data.Left, data.Top)); }
                catch (Exception ex) when (ex is ArgumentException or FormatException)
                {
                    AddRegionCore(new TextRegionControl(new TextRegionPersistentData
                    {
                        SectionXaml = TextRegionPersistentData.PlainSection($"[Region could not be loaded: {ex.Message}]"),
                    }), new Point(data.Left, data.Top));
                }
            }
            MoveCursor(new Point(0, _model.Items.Count == 0 ? 0 : Snap(_model.Items.Max(Bottom) + Grid * 2)));
            UpdatePages();
        }
        finally
        {
            _restoring = false;
        }
        _lastSnapshot = Snapshot();
        StatusChanged?.Invoke(this, EventArgs.Empty);
    }

    public WorksheetDocument ToDocument()
    {
        var doc = new WorksheetDocument { Settings = _model.Settings };
        foreach (var r in _model.Items)
        {
            if (r.IsEmpty) continue;
            var data = r.Save();
            var p = _model.GetItemWorksheetLocation(r);
            data.Top = p.Y;
            data.Left = p.X;
            doc.Regions.Add(data);
        }
        return doc;
    }

    public void SetUnitSystem(string name) { _model.SetUnitSystem(name); Changed(); }
    public void SetAutoCalc(bool on) { _model.SetAutoCalc(on); Changed(); }
    public void SetResultFormat(ResultFormat format) { _model.SetResultFormat(format); Changed(); }
    public void CalculateNow() => _model.CalculateNow();
    public void RecalculateAll() => _model.RecalculateAll();

    public Task WhenCalculated() => _controller.WhenIdle();

    /// <summary>The element holding every page, for printing or rendering the whole worksheet.</summary>
    public FrameworkElement PagesElement => _pages;

    /// <summary>Places the insertion crosshair (snapped to the grid).</summary>
    public void SetCursor(Point p) => MoveCursor(p);

    public Point InsertionPoint => _cursor;

    /// <summary>Moves a region; calculation order follows the new position.</summary>
    public void MoveRegion(RegionControl region, Point location)
    {
        location = new Point(Math.Max(0, Snap(location.X)), Math.Max(0, Snap(location.Y)));
        Canvas.SetLeft(region, location.X);
        Canvas.SetTop(region, location.Y);
        _model.MoveItem(region, location);
        UpdatePages();
        Changed();
    }

    public Point LocationOf(RegionControl region) => _model.GetItemWorksheetLocation(region);

    // ---------------------------------------------------------------- inserting

    public RegionControl InsertMath(string? initialText = null) => InsertAndEdit(_factory.CreateEquation(), initialText);
    public RegionControl InsertText() => InsertAndEdit(_factory.CreateText(), null);
    public RegionControl InsertXYPlot() => InsertAndEdit(_factory.CreateXYPlot(), null);
    public RegionControl InsertPolarPlot() => InsertAndEdit(_factory.CreatePolarPlot(), null);
    public RegionControl InsertSolveBlock() => InsertAndEdit(_factory.CreateSolveBlock(), null);

    /// <summary>Inserts palette text into the region being edited, or starts a new math region with it.</summary>
    public void InsertIntoEditor(string text)
    {
        var editing = EditingRegion;
        if (editing != null) editing.InsertText(text);
        else InsertMath(text);
    }

    private RegionControl InsertAndEdit(RegionControl region, string? initialText)
    {
        EndAllEditing();
        ClearSelection();
        AddRegionCore(region, _cursor);
        UpdatePages();
        region.BeginEdit(initialText);
        return region;
    }

    // ---------------------------------------------------------------- region bookkeeping

    private void AddRegionCore(RegionControl region, Point location)
    {
        location = new Point(Math.Max(0, location.X), Math.Max(0, location.Y));
        Canvas.SetLeft(region, location.X);
        Canvas.SetTop(region, location.Y);
        _canvas.Children.Add(region);
        _model.AddItemAtLocation(region, location);
        region.Changed += HandleRegionChanged;
        region.RequestLeave += HandleRegionLeave;
        region.EditingChanged += HandleRegionEditingChanged;
        region.PreviewMouseLeftButtonDown += HandleRegionMouseDown;
        region.SizeChanged += (_, _) => UpdatePages();
        if (region is CalculatableRegionControl c) c.RlChanged += (_, _) => StatusChanged?.Invoke(this, EventArgs.Empty);
    }

    private void RemoveRegionCore(RegionControl region)
    {
        region.Changed -= HandleRegionChanged;
        region.RequestLeave -= HandleRegionLeave;
        region.EditingChanged -= HandleRegionEditingChanged;
        region.PreviewMouseLeftButtonDown -= HandleRegionMouseDown;
        _selection.Remove(region);
        _model.RemoveItem(region);
        _canvas.Children.Remove(region);
    }

    private double Bottom(RegionControl r) => _model.GetItemWorksheetLocation(r).Y + r.ActualHeight;

    private void UpdatePages()
    {
        double bottom = _model.Items.Count == 0 ? 0 : _model.Items.Max(r => Bottom(r) + 40);
        bottom = Math.Max(bottom, _cursor.Y + 40);
        int pages = Math.Max(1, (int)Math.Ceiling((bottom + 2 * Margin_) / PageHeight));
        _pages.Height = pages * PageHeight;
        _canvas.Height = pages * PageHeight - 2 * Margin_;
        foreach (var old in _pages.Children.OfType<Shape>().Where(r => r.Tag as string == "page").ToArray()) _pages.Children.Remove(old);

        // one continuous sheet with dashed page-break lines, like a draft view
        var sheet = new Rectangle
        {
            Width = PageWidth, Height = _pages.Height, Fill = Brushes.White, Tag = "page", IsHitTestVisible = false,
            Effect = new System.Windows.Media.Effects.DropShadowEffect { BlurRadius = 6, ShadowDepth = 1, Opacity = 0.25 },
        };
        Panel.SetZIndex(sheet, -2);
        _pages.Children.Insert(0, sheet);
        for (int i = 1; i < pages; i++)
        {
            var line = new Line
            {
                X1 = 0, X2 = PageWidth, Y1 = i * PageHeight, Y2 = i * PageHeight, Tag = "page", IsHitTestVisible = false,
                Stroke = new SolidColorBrush(Color.FromRgb(0xB8, 0xC0, 0xCC)), StrokeDashArray = [6, 4], StrokeThickness = 1,
            };
            Panel.SetZIndex(line, -1);
            _pages.Children.Insert(1, line);
        }
    }

    private static Brush GridBrush()
    {
        var dot = new GeometryDrawing(new SolidColorBrush(Color.FromRgb(0xDD, 0xE1, 0xE8)), null, new RectangleGeometry(new Rect(0, 0, 1, 1)));
        var brush = new DrawingBrush(new DrawingGroup { Children = { new GeometryDrawing(Brushes.White, null, new RectangleGeometry(new Rect(0, 0, 16, 16))), dot } })
        {
            TileMode = TileMode.Tile, Viewport = new Rect(0, 0, 16, 16), ViewportUnits = BrushMappingMode.Absolute, Stretch = Stretch.None,
            Viewbox = new Rect(0, 0, 16, 16), ViewboxUnits = BrushMappingMode.Absolute,
        };
        brush.Freeze();
        return brush;
    }

    private static double Snap(double v) => Math.Round(v / Grid) * Grid;

    private void MoveCursor(Point p)
    {
        _cursor = new Point(Math.Clamp(Snap(p.X), 0, ContentWidth - Grid), Math.Max(0, Snap(p.Y)));
        Canvas.SetLeft(_crosshair, _cursor.X);
        Canvas.SetTop(_crosshair, _cursor.Y + 8);
        _crosshair.Visibility = EditingRegion == null ? Visibility.Visible : Visibility.Collapsed;
    }

    // ---------------------------------------------------------------- selection & editing

    private void Select(RegionControl r, bool add)
    {
        if (!add) ClearSelection();
        if (_selection.Contains(r)) return;
        _selection.Add(r);
        r.IsSelected = true;
        SelectionChanged?.Invoke(this, EventArgs.Empty);
    }

    public void ClearSelection()
    {
        foreach (var r in _selection) r.IsSelected = false;
        _selection.Clear();
        SelectionChanged?.Invoke(this, EventArgs.Empty);
    }

    public void SelectAll()
    {
        EndAllEditing();
        ClearSelection();
        foreach (var r in _model.Items) Select(r, add: true);
    }

    private void EndAllEditing()
    {
        foreach (var r in _model.Items.Where(r => r.IsEditing).ToArray()) r.EndEdit(commit: true);
    }

    private void HandleRegionEditingChanged(object? sender, EventArgs e)
    {
        var region = (RegionControl)sender!;
        _crosshair.Visibility = EditingRegion == null ? Visibility.Visible : Visibility.Collapsed;
        if (!region.IsEditing && region.IsEmpty && _model.Items.Contains(region))
        {
            // an empty region left behind after editing disappears, as in Mathcad
            Dispatcher.BeginInvoke(() =>
            {
                if (!region.IsEditing && region.IsEmpty && _model.Items.Contains(region))
                {
                    RemoveRegionCore(region);
                    Changed();
                }
            });
        }
        SelectionChanged?.Invoke(this, EventArgs.Empty);
    }

    private void HandleRegionLeave(object? sender, RegionLeaveEventArgs e)
    {
        var region = (RegionControl)sender!;
        if (!_model.Items.Contains(region)) return;
        var p = _model.GetItemWorksheetLocation(region);
        region.UpdateLayout();
        MoveCursor(e.Direction switch
        {
            LeaveDirection.Down => new Point(p.X, p.Y + region.ActualHeight + Grid * 2),
            LeaveDirection.Right => new Point(p.X + region.ActualWidth + Grid * 3, p.Y),
            _ => p,
        });
        UpdatePages();
        _canvas.Focus();
        Keyboard.Focus(_canvas);
    }

    private void HandleRegionChanged(object? sender, EventArgs e) => Changed();

    private void Changed()
    {
        if (_restoring) return;
        var snap = Snapshot();
        if (snap == _lastSnapshot) return;
        _undo.Push(_lastSnapshot);
        _redo.Clear();
        _lastSnapshot = snap;
        DocumentChanged?.Invoke(this, EventArgs.Empty);
        StatusChanged?.Invoke(this, EventArgs.Empty);
    }

    // ---------------------------------------------------------------- undo / redo

    private string Snapshot() => WorksheetPackage.ToXml(ToDocument()).ToString(SaveOptions.DisableFormatting);

    public void Undo() => Restore(_undo, _redo);
    public void Redo() => Restore(_redo, _undo);

    private void Restore(Stack<string> from, Stack<string> to)
    {
        EndAllEditing();
        if (from.Count == 0) return;
        to.Push(Snapshot());
        var xml = from.Pop();
        Load(WorksheetPackage.FromXml(XElement.Parse(xml)));
        DocumentChanged?.Invoke(this, EventArgs.Empty);
    }

    // ---------------------------------------------------------------- clipboard

    public void Copy()
    {
        if (_selection.Count == 0) return;
        var doc = new WorksheetDocument { Settings = _model.Settings };
        double minX = _selection.Min(r => _model.GetItemWorksheetLocation(r).X), minY = _selection.Min(r => _model.GetItemWorksheetLocation(r).Y);
        foreach (var r in _selection.OrderBy(r => _model.GetItemWorksheetLocation(r), WorksheetItemPositionComparer.Instance))
        {
            var data = r.Save();
            var p = _model.GetItemWorksheetLocation(r);
            data.Left = p.X - minX;
            data.Top = p.Y - minY;
            doc.Regions.Add(data);
        }
        var obj = new DataObject();
        obj.SetData(ClipboardFormat, WorksheetPackage.ToXml(doc).ToString());
        obj.SetText(string.Join(Environment.NewLine, _selection.Select(r => r switch
        {
            EquationControl eq => eq.SourceText,
            TextRegionControl t => t.PlainText,
            _ => "",
        }).Where(s => s.Length > 0)));
        Clipboard.SetDataObject(obj, copy: true);
    }

    public void Cut()
    {
        Copy();
        DeleteSelection();
    }

    public void Paste()
    {
        if (EditingRegion is { } editing && Clipboard.ContainsText())
        {
            editing.InsertText(Clipboard.GetText().Replace("\r", "").Replace("\n", " "));
            return;
        }
        EndAllEditing();
        if (Clipboard.GetData(ClipboardFormat) is string xml)
        {
            var doc = WorksheetPackage.FromXml(XElement.Parse(xml));
            ClearSelection();
            foreach (var data in doc.Regions)
            {
                data.Id = RegionPersistentData.NewId();
                var region = _factory.CreateRegion(data);
                AddRegionCore(region, new Point(_cursor.X + data.Left, _cursor.Y + data.Top));
                Select(region, add: true);
            }
            UpdatePages();
            Changed();
        }
        else if (Clipboard.ContainsText())
        {
            var region = (EquationControl)InsertMath();
            region.InsertText(Clipboard.GetText().Trim());
        }
    }

    public void DeleteSelection()
    {
        if (_selection.Count == 0) return;
        foreach (var r in _selection.ToArray()) RemoveRegionCore(r);
        ClearSelection();
        Changed();
    }

    // ---------------------------------------------------------------- mouse

    private void HandleCanvasMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.OriginalSource != _canvas) return;
        EndAllEditing();
        var p = e.GetPosition(_canvas);
        if ((Keyboard.Modifiers & ModifierKeys.Shift) == 0) ClearSelection();
        MoveCursor(p);
        _dragStart = p;
        _rubberBanding = true;
        _canvas.CaptureMouse();
        _canvas.Focus();
        e.Handled = true;
    }

    private void HandleRegionMouseDown(object sender, MouseButtonEventArgs e)
    {
        var region = (RegionControl)sender;
        if (region.IsEditing) return;
        bool add = (Keyboard.Modifiers & (ModifierKeys.Shift | ModifierKeys.Control)) != 0;
        foreach (var other in _model.Items.Where(r => r != region && r.IsEditing).ToArray()) other.EndEdit(commit: true);
        if (!region.IsSelected || add) Select(region, add);
        _pressedRegion = region;
        _dragStart = e.GetPosition(_canvas);
        _dragMoved = false;
        _dragOrigins = _selection.ToDictionary(r => r, r => _model.GetItemWorksheetLocation(r));
        _canvas.CaptureMouse();
        _canvas.Focus();
        e.Handled = true;
    }

    private void HandleCanvasMouseMove(object sender, MouseEventArgs e)
    {
        if (_dragStart is not { } start || e.LeftButton != MouseButtonState.Pressed) return;
        var p = e.GetPosition(_canvas);
        var delta = p - start;
        if (_rubberBanding)
        {
            var rect = new Rect(start, p);
            _rubberBand.Visibility = Visibility.Visible;
            Canvas.SetLeft(_rubberBand, rect.X);
            Canvas.SetTop(_rubberBand, rect.Y);
            _rubberBand.Width = rect.Width;
            _rubberBand.Height = rect.Height;
            if (rect.Width > 3 || rect.Height > 3)
                foreach (var r in _model.Items)
                {
                    var loc = _model.GetItemWorksheetLocation(r);
                    bool hit = rect.IntersectsWith(new Rect(loc, new Size(r.ActualWidth, r.ActualHeight)));
                    if (hit) Select(r, add: true);
                    else if (r.IsSelected && (Keyboard.Modifiers & ModifierKeys.Shift) == 0)
                    {
                        r.IsSelected = false;
                        _selection.Remove(r);
                        SelectionChanged?.Invoke(this, EventArgs.Empty);
                    }
                }
            return;
        }
        if (_dragOrigins == null || (!_dragMoved && Math.Abs(delta.X) < 3 && Math.Abs(delta.Y) < 3)) return;
        _dragMoved = true;
        foreach (var (r, origin) in _dragOrigins)
        {
            Canvas.SetLeft(r, Math.Max(0, Snap(origin.X + delta.X)));
            Canvas.SetTop(r, Math.Max(0, Snap(origin.Y + delta.Y)));
        }
    }

    private void HandleCanvasMouseUp(object sender, MouseButtonEventArgs e)
    {
        _canvas.ReleaseMouseCapture();
        if (_rubberBanding)
        {
            _rubberBanding = false;
            _rubberBand.Visibility = Visibility.Collapsed;
        }
        else if (_dragMoved && _dragOrigins != null)
        {
            foreach (var r in _dragOrigins.Keys.OrderBy(r => Canvas.GetTop(r)))
                _model.MoveItem(r, new Point(Canvas.GetLeft(r), Canvas.GetTop(r)));
            UpdatePages();
            Changed();
        }
        else if (_pressedRegion is { CanEdit: true } region && _selection.Count == 1 && (Keyboard.Modifiers & (ModifierKeys.Shift | ModifierKeys.Control)) == 0)
        {
            // a click without dragging edits the region
            ClearSelection();
            region.BeginEdit();
        }
        _dragStart = null;
        _dragOrigins = null;
        _pressedRegion = null;
        _dragMoved = false;
    }

    private void HandleMouseWheel(object sender, MouseWheelEventArgs e)
    {
        if ((Keyboard.Modifiers & ModifierKeys.Control) == 0) return;
        Zoom += e.Delta > 0 ? 0.1 : -0.1;
        e.Handled = true;
    }

    // ---------------------------------------------------------------- keyboard

    private void HandleTextInput(object sender, TextCompositionEventArgs e)
    {
        if (EditingRegion != null || string.IsNullOrEmpty(e.Text) || char.IsControl(e.Text[0])) return;
        if (e.Text == "\"") InsertText();
        else InsertMath(e.Text);
        e.Handled = true;
    }

    private void HandleKeyDown(object sender, KeyEventArgs e)
    {
        if (EditingRegion != null) return;
        bool ctrl = (Keyboard.Modifiers & ModifierKeys.Control) != 0;
        switch (e.Key)
        {
            case Key.Delete or Key.Back when _selection.Count > 0:
                DeleteSelection();
                break;
            case Key.Left: MoveCursor(_cursor + new Vector(-Grid, 0)); break;
            case Key.Right: MoveCursor(_cursor + new Vector(Grid, 0)); break;
            case Key.Up: MoveCursor(_cursor + new Vector(0, -Grid)); break;
            case Key.Down: MoveCursor(_cursor + new Vector(0, Grid)); break;
            case Key.Enter: MoveCursor(new Point(0, _cursor.Y + Grid * 3)); UpdatePages(); break;
            case Key.Escape: ClearSelection(); break;
            case Key.A when ctrl: SelectAll(); break;
            default: return;
        }
        e.Handled = true;
    }

    public void Dispose()
    {
        EndAllEditing();
        _controller.Dispose();
    }
}
