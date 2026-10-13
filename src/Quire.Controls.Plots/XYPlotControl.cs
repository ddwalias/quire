using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Quire.Calculation.Core;
using Quire.Controls.Core;
using Quire.EquationEditor.Tree;
using Quire.EquationEditor.Tree.Parsing;
using Quire.EquationEditor.Tree.Transform;
using Quire.EquationEditor.View;
using Quire.PersistentDataObjects;

namespace Quire.Controls.Plots;

/// <summary>
/// XY or polar plot region. The y-axis holds one expression per trace and the x-axis one expression
/// (a range variable, a vector, or an undefined name for a QuickPlot over −10..10).
/// </summary>
public partial class XYPlotControl : CalculatableRegionControl
{
    private readonly PlotGraph _graph = new();
    private readonly StackPanel _yLabels = new() { VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(0, 0, 4, 0) };
    private readonly MathView _xLabel = new() { HorizontalAlignment = HorizontalAlignment.Center };
    private readonly TextBlock _xUnitText = new() { Foreground = Brushes.DimGray, FontSize = 11, VerticalAlignment = VerticalAlignment.Center };
    private readonly TextBlock _message = new() { Foreground = ErrorBrush, FontSize = 11, TextWrapping = TextWrapping.Wrap, Visibility = Visibility.Collapsed };
    private readonly Grid _editor;
    private readonly TextBox _yBox, _xBox, _xUnitBox, _yUnitBox, _xMinBox, _xMaxBox, _yMinBox, _yMaxBox;
    private readonly ComboBox _kindBox;

    private PlotRegionPersistentData _data;

    public XYPlotControl(PlotRegionPersistentData? data = null)
    {
        _data = data ?? new PlotRegionPersistentData();
        RegionId = _data.Id;

        _yBox = Box(multiline: true);
        _xBox = Box();
        _xUnitBox = Box(); _yUnitBox = Box();
        _xMinBox = Box(); _xMaxBox = Box(); _yMinBox = Box(); _yMaxBox = Box();
        _kindBox = new ComboBox { ItemsSource = new[] { "XY", "Polar" }, Margin = new Thickness(2), FontSize = 12 };
        _editor = BuildEditor();

        var plotRow = new DockPanel();
        DockPanel.SetDock(_yLabels, Dock.Left);
        plotRow.Children.Add(_yLabels);
        plotRow.Children.Add(_graph);

        var xRow = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Center };
        xRow.Children.Add(_xLabel);
        xRow.Children.Add(_xUnitText);

        var root = new StackPanel();
        root.Children.Add(plotRow);
        root.Children.Add(xRow);
        root.Children.Add(_message);
        root.Children.Add(_editor);
        Body = root;

        ApplyData();
    }

    private static TextBox Box(bool multiline = false) => new()
    {
        FontFamily = new FontFamily("Cascadia Mono, Consolas"),
        FontSize = 13,
        Margin = new Thickness(2),
        MinWidth = 70,
        AcceptsReturn = multiline,
        TextWrapping = TextWrapping.NoWrap,
        MinLines = multiline ? 2 : 1,
    };

    private Grid BuildEditor()
    {
        var g = new Grid { Visibility = Visibility.Collapsed, Margin = new Thickness(0, 6, 0, 0), Background = new SolidColorBrush(Color.FromRgb(0xF4, 0xF7, 0xFB)) };
        g.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        g.ColumnDefinitions.Add(new ColumnDefinition());
        g.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        g.ColumnDefinitions.Add(new ColumnDefinition());
        void Row(int r, string l1, UIElement e1, string? l2 = null, UIElement? e2 = null)
        {
            g.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            Add(new TextBlock { Text = l1, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(4, 0, 4, 0), FontSize = 12 }, r, 0);
            Add(e1, r, 1);
            if (l2 != null) Add(new TextBlock { Text = l2, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(8, 0, 4, 0), FontSize = 12 }, r, 2);
            if (e2 != null) Add(e2, r, 3);
        }
        void Add(UIElement e, int r, int c)
        {
            Grid.SetRow(e, r);
            Grid.SetColumn(e, c);
            g.Children.Add(e);
        }
        Row(0, "y (one per line)", _yBox, "y unit", _yUnitBox);
        Row(1, "x", _xBox, "x unit", _xUnitBox);
        Row(2, "x min", _xMinBox, "x max", _xMaxBox);
        Row(3, "y min", _yMinBox, "y max", _yMaxBox);
        var buttons = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right };
        var ok = new Button { Content = "Apply", Margin = new Thickness(2), Padding = new Thickness(10, 1, 10, 1), IsDefault = false };
        ok.Click += (_, _) => { EndEdit(commit: true); RaiseLeave(LeaveDirection.Down); };
        buttons.Children.Add(_kindBox);
        buttons.Children.Add(ok);
        Row(4, "", buttons);
        Grid.SetColumnSpan(buttons, 3);
        g.PreviewKeyDown += (_, e) =>
        {
            if (e.Key == Key.Escape) { EndEdit(commit: false); RaiseLeave(LeaveDirection.Cancel); e.Handled = true; }
            if (e.Key == Key.Enter && e.Source != _yBox) { EndEdit(commit: true); RaiseLeave(LeaveDirection.Down); e.Handled = true; }
        };
        return g;
    }

    public PlotRegionPersistentData Data => _data;

    private void ApplyData()
    {
        _graph.Width = _data.Width;
        _graph.Height = _data.Height;
        _graph.Kind = _data.Kind;
        _graph.XMin = _data.XMin; _graph.XMax = _data.XMax; _graph.YMin = _data.YMin; _graph.YMax = _data.YMax;

        _yLabels.Children.Clear();
        for (int i = 0; i < _data.Y.Count; i++)
        {
            var row = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 1, 0, 1) };
            row.Children.Add(new Border { Width = 10, Height = 3, Background = new SolidColorBrush(PlotGraph.Palette[i % PlotGraph.Palette.Length]), VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(0, 0, 3, 0) });
            row.Children.Add(new MathView { Expression = _data.Y[i] });
            _yLabels.Children.Add(row);
        }
        if (_data.YUnit != null) _yLabels.Children.Add(new MathView { Expression = _data.YUnit, Opacity = 0.7 });
        _xLabel.Expression = _data.X ?? PlaceholderNode.Instance;
        _xUnitText.Text = "";
        Rl = BuildRl();
    }

    private string BuildRl()
    {
        if (_data.X == null || _data.Y.Count == 0) return "(noop)";
        var parts = new List<string>
        {
            $"(x {MathNodeToRlTransform.Expr(_data.X)})",
            $"(y {string.Join(" ", _data.Y.Select(MathNodeToRlTransform.Expr))})",
        };
        if (_data.XUnit != null) parts.Add($"(xunit {MathNodeToRlTransform.Expr(_data.XUnit)})");
        if (_data.YUnit != null) parts.Add($"(yunit {MathNodeToRlTransform.Expr(_data.YUnit)})");
        return $"(plot2d {string.Join(" ", parts)})";
    }

    // ---------------------------------------------------------------- editing

    public override void BeginEdit(string? initialText = null)
    {
        _yBox.Text = string.Join(Environment.NewLine, _data.Y.Select(MathNodeToLinearTransform.Write));
        _xBox.Text = _data.X == null ? "" : MathNodeToLinearTransform.Write(_data.X);
        _xUnitBox.Text = _data.XUnit == null ? "" : MathNodeToLinearTransform.Write(_data.XUnit);
        _yUnitBox.Text = _data.YUnit == null ? "" : MathNodeToLinearTransform.Write(_data.YUnit);
        _xMinBox.Text = Num(_data.XMin); _xMaxBox.Text = Num(_data.XMax);
        _yMinBox.Text = Num(_data.YMin); _yMaxBox.Text = Num(_data.YMax);
        _kindBox.SelectedIndex = _data.Kind == PlotKind.Polar ? 1 : 0;
        _editor.Visibility = Visibility.Visible;
        IsEditing = true;
        Dispatcher.BeginInvoke(() => (_data.Y.Count == 0 ? _yBox : _yBox).Focus(), System.Windows.Threading.DispatcherPriority.Input);
    }

    private static string Num(double? v) => v?.ToString("R", CultureInfo.InvariantCulture) ?? "";

    public override void EndEdit(bool commit)
    {
        if (!IsEditing) return;
        IsEditing = false;
        _editor.Visibility = Visibility.Collapsed;
        if (!commit) return;
        try
        {
            var next = new PlotRegionPersistentData
            {
                Id = RegionId, Top = _data.Top, Left = _data.Left, Width = _data.Width, Height = _data.Height,
                Kind = _kindBox.SelectedIndex == 1 ? PlotKind.Polar : PlotKind.XY,
                X = Expr(_xBox.Text), XUnit = Expr(_xUnitBox.Text), YUnit = Expr(_yUnitBox.Text),
                XMin = Limit(_xMinBox.Text), XMax = Limit(_xMaxBox.Text), YMin = Limit(_yMinBox.Text), YMax = Limit(_yMaxBox.Text),
            };
            foreach (var line in _yBox.Text.Split('\n').Select(l => l.Trim()).Where(l => l.Length > 0))
                next.Y.Add(LinearParser.ParseExpression(line));
            _data = next;
            ApplyData();
            RaiseChanged();
        }
        catch (ParseException ex)
        {
            ShowError($"Plot expression: {ex.Message}");
        }
        catch (FormatException)
        {
            ShowError("Axis limits must be numbers");
        }
    }

    private static MathExpr? Expr(string text) => string.IsNullOrWhiteSpace(text) ? null : LinearParser.ParseExpression(text.Trim());

    private static double? Limit(string text) =>
        string.IsNullOrWhiteSpace(text) ? null : double.Parse(text.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture);

    public override bool IsEmpty => _data.X == null && _data.Y.Count == 0;

    // ---------------------------------------------------------------- results

    protected override void OnResult(CalculationResult result)
    {
        _graph.Format = ResultFormat;
        if (result.Error != null)
        {
            ShowError(result.Error.Message);
            _graph.SetTraces([], []);
            return;
        }
        ShowError(null);
        if (result.Value is ITrace2DResultNode t)
        {
            _graph.SetTraces(t.Traces, _data.Y.Select(MathNodeToLinearTransform.Write).ToArray());
            var xu = t.Traces.FirstOrDefault()?.XUnits;
            _xUnitText.Text = _data.XUnit == null && xu is { Count: > 0 } ? "  (" + UnitText(xu) + ")" : "";
        }
        else _graph.SetTraces([], []);
    }

    private static string UnitText(IReadOnlyList<UnitPower> units) =>
        ResultFormatter.UnitExpression(units) is { } n ? MathNodeToLinearTransform.Write(n) : "";

    private void ShowError(string? text)
    {
        HasError = text != null;
        _message.Text = text ?? "";
        _message.Visibility = text == null ? Visibility.Collapsed : Visibility.Visible;
    }

    public override RegionPersistentData Save()
    {
        _data.Id = RegionId;
        return _data;
    }
}
