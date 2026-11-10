using System;
using System.Collections;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Quire.Calculation.Core;
using Quire.Controls.ControlAdorners;
using Quire.Controls.Core;
using Quire.Controls.Core.IORegions;
using Quire.Controls.InputControls;
using Quire.Core;
using Quire.EquationEditor.Tree;
using Quire.EquationEditor.View;
using Quire.Matrices;
using Quire.Wpf;

namespace Quire.Controls.Internal;

internal partial class VisualInputScriptableControlNodeBase
{
    public static readonly Type TypeOfThis = typeof(VisualInputScriptableControlNodeBase);
    protected double _availableWidthCache;
    protected double _availableHeightCache;
    protected ContextMenu _contextMenu;
    public static RoutedEvent ContentChangedEvent;
    protected EquationControl _eq;
    public static readonly RoutedEvent RequestActivateEquationForComboBoxNodeEvent = EventManager.RegisterRoutedEvent("RequestActivateEquationForComboBoxNode", RoutingStrategy.Bubble, typeof(RoutedEventHandler), TypeOfThis);
    private const int pastingAttachedPropertyDelay = 5;
    private const double headingFontSizeSmallerBy = 2.0;
    private const double headingFontOpacity = 0.6;
    private const double headingCellHorizontalPadding = 2.0;
    private const string rowHeadingName = "RowHeading";
    private Visibility _headingsVisibility;
    public static readonly string VISUAL_NAME = "VisualScripting";
    private double _effectiveBaselineY;
    private MatrixNodeInfo _matrixInfo;
    protected MathNode _mathNode;
    private StackPanel _rowHeading;
    private StackPanel _columnHeading;
    private string[] _rowIndices;
    private string[] _columnIndices;
    protected VerticalFence _leftBracket;
    protected VerticalFence _rightBracket;
    protected double _opacity = 1.0;
    protected VisualMatrixContents _matrixGrid;
    protected ScrollViewer _scrollViewer;
    protected Grid _grid;
    protected Border _border;
    protected ToggleButton _toggle;
    protected Control _inputControl;
    private double _rowHeadingTop;
    private double _columnHeadingLeft;
    private double _matrixTop;
    private double _matrixLeft;
    private double _matrixContentTop;
    private double _matrixContentLeft;
    private IVisualMathNode[] _children;
    protected VisualMathRegionNode _region;
    protected IInputScriptableWithInputNodeInfo _scriptableInfo;

    public InputScriptableControlViewModelWithInputs ViewModel => (InputScriptableControlViewModelWithInputs)(((IInputControl)_inputControl)?.ViewModel);

    protected void init(string mathNodeName, MathNode mathNode, VisualMathRegionNode region)
    {
        InitMathRegion(region);
        Name = VISUAL_NAME;
        InitNodeInfo(mathNode);
        _matrixGrid = new VisualMatrixContents();
        _matrixGrid.HorizontalAlignment = HorizontalAlignment.Left;
        ColumnCount = 1;
        _scrollViewer.Content = _matrixGrid;
        _scrollViewer.MinWidth = _inputControl.MinWidth;
        _grid.Children.Add(_inputControl);
        Grid.SetRow(_inputControl, 2);
        ConstructContextMenu();
        DataExchangeModel = ((IInputScriptableControlViewModel)_inputControl.DataContext).Model as IDataExchangeModel;
        Loaded += LoadedHandler;
    }

    public bool BlockFocus { get; set; }

    public bool IsAdornerFocused { get; }

    protected override void OnContextMenuOpening(ContextMenuEventArgs e)
    {
        EquationControl equationControl = GetEquationControl();
        if (equationControl != null && equationControl.IsSelected)
        {
            ContextMenu = null;
            return;
        }

        base.OnContextMenuOpening(e);
        ContextMenu = _contextMenu;
    }

    private void InitMathRegion(VisualMathRegionNode region)
    {
        _region = region;
        _region.Activated += HandleRegionActivated;
        _region.Deactivated += HandleRegionDeactivated;
    }

    private void InitNodeInfo(MathNode mathNode)
    {
        _mathNode = mathNode;
        _scriptableInfo = (IInputScriptableWithInputNodeInfo)mathNode.NodeInfo;
    }

    protected Control InitInputControl(string mathNodeName, MathNode mathNode)
    {
        return VisualInputControlFactory.CreateVisualInputControl(mathNodeName, mathNode.NodeInfo);
    }

    private void HandleRegionActivated()
    {
        ViewModel.IsMathRegionActive = true;
    }

    private void HandleRegionDeactivated()
    {
        ViewModel.IsMathRegionActive = false;
        HandleCommitNeeded();
    }

    ~VisualInputScriptableControlNodeBase()
    {
        Dispose();
    }

    private Visibility InitializeVisibility()
    {
        if (_scriptableInfo.IsInputsAreaShown)
        {
            return Visibility.Visible;
        }

        return Visibility.Collapsed;
    }

    private void ConstructContextMenu()
    {
        _contextMenu = (ContextMenu)FindResource("ScriptableControlContextMenu");
        for (int num = 0; num < _contextMenu.Items.Count; num++)
        {
            ((PrimeContextMenuItem)_contextMenu.Items[num]).CommandTarget = this;
        }
    }

    protected void BindCommands()
    {
        CommandBindings.Add(new CommandBinding(InputScriptingControlWithInputCommands.AddScriptingInputField, HandleAddInputs, CanAddInputs));
        CommandBindings.Add(new CommandBinding(InputScriptingControlWithInputCommands.RemoveScriptingInputField, HandleRemoveInputs, CanRemoveInputs));
        CommandBindings.Add(new CommandBinding(InputScriptingControlCommands.OpenEditingMode, HandleOpenEditingMode));
    }

    private void CanAddInputs(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = ViewModel.IsEnabled && !ViewModel.IsInputEnabled;
    }

    private void CanRemoveInputs(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = ViewModel.IsEnabled && ViewModel.IsInputEnabled;
    }

    protected virtual void HandleOpenEditingMode(object sender, ExecutedRoutedEventArgs routedEventArgs)
    {
        RaiseEvent(new RoutedEventArgs(RequestActivateEquationForComboBoxNodeEvent, this));
        Window window = VisualInputControlFactory.CreateInputControlEditor(((IInputControlNodeInfo)NodeInfo).InputControlType, ViewModel);
        Window mainWindow = Application.Current.MainWindow;
        if (mainWindow != null)
        {
            window.Owner = mainWindow;
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
        }

        IInputControlViewModelWithEditor inputControlViewModelWithEditor;
        if (window.ShowDialog() == true)
        {
            inputControlViewModelWithEditor = ViewModel as IInputControlViewModelWithEditor;
            if (inputControlViewModelWithEditor != null)
            {
                inputControlViewModelWithEditor.CommitEditorChanges();
                if (ViewModel.IsDirty)
                {
                    EquationControl equationControl = GetEquationControl();
                    if (equationControl != null)
                    {
                        equationControl._model.IsExpressionValueModified = true;
                    }
                }
            }
        }
    }

    private void HandleAddInputs(object sender, ExecutedRoutedEventArgs arg)
    {
        if (!ViewModel.IsMathRegionActive)
        {
            RaiseEvent(new RoutedEventArgs(RequestActivateEquationForComboBoxNodeEvent, this));
        }

        if (!ViewModel.IsInputEnabled)
        {
            ViewModel.IsInputEnabled = true;
            if (ViewModel.HasKeyboard)
            {
                _scrollViewer.Focus();
            }
        }

        EquationControl equationControl = GetEquationControl();
        if (equationControl != null)
        {
            equationControl._model.IsExpressionValueModified = true;
        }
    }

    private void HandleRemoveInputs(object sender, ExecutedRoutedEventArgs arg)
    {
        if (!ViewModel.IsMathRegionActive)
        {
            RaiseEvent(new RoutedEventArgs(RequestActivateEquationForComboBoxNodeEvent, this));
        }

        if (ViewModel.IsInputEnabled)
        {
            ViewModel.IsInputEnabled = false;
            EquationControl equationControl = GetEquationControl();
            if (equationControl != null)
            {
                equationControl._view.FireDeleteMatrixContents(_mathNode);
            }
        }
    }

    private void HandleAddOutput(object sender, ExecutedRoutedEventArgs arg)
    {
        if (ViewModel.IsLhsLabel)
        {
            EquationControl equationControl = GetEquationControl();
            if (equationControl != null)
            {
                equationControl.ScopeLhs();
            }

            EquationCommands.InsertMatrix.Execute(new Size(1.0, 2.0), null);
        }
        else
        {
            EquationCommands.InsertRow.Execute(Direction.Down, null);
        }
    }

    internal MatrixResizeAdorner ResizeAdorner { get; set; }

    protected virtual void OnControlLostKeyboardFocusOrDataChanged()
    {
        if (!(NodeInfo is IInputControlNodeInfo))
        {
            DebugTools.FailAssertion(NodeInfo?.GetType().Name);
        }
    }

    protected void HandleCommitNeeded()
    {
        EquationControl equationControl = GetEquationControl();
        if (equationControl != null && ViewModel.IsDirty && !equationControl._model.IsExpressionValueModified)
        {
            ViewModel.IsDirty = false;
            EquationCommands.Commit.Execute(new EquationCommands.CommitCommandParameter(true, equationControl.Error != null, equationControl.Error == null), this);
            HandleCalculatableStateChanged(CalculatableState.Commit);
        }
    }

    protected void HandleModelPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        EquationControl equationControl = GetEquationControl();
        if (equationControl != null)
        {
            switch (e.PropertyName)
            {
                case "EngineError":
                    equationControl.SetScriptingEngineError(ViewModel.Model.EngineError);
                    break;
                case "IsDirty":
                    RaiseEvent(new RoutedEventArgs(ContentChangedEvent));
                    break;
            }
        }
    }

    protected EquationControl GetEquationControl()
    {
        if (_eq == null)
        {
            _eq = this.TryFindVisualAncestor<EquationControl>();
            if (_eq == null)
            {
                _eq = this.TryFindVisualAncestor<EquationControl>();
            }

            if (_eq == null)
            {
                _eq = this.TryFindVisualAncestor<EquationControl>();
            }
        }

        return _eq;
    }

    private void LoadedHandler(object sender, RoutedEventArgs e)
    {
        Loaded -= LoadedHandler;
        double num = 0.0;
        double val = 0.0;
        if (!(ParentNode is VisualMathRegionNode) && ParentNode[0] is FrameworkElement frameworkElement)
        {
            num = frameworkElement.ActualWidth;
            val = frameworkElement.ActualHeight;
        }

        EquationControl equationControl = GetEquationControl();
        if (equationControl != null)
        {
            equationControl.ExpressionValueChanged += HandleSExpressionChanged;
            equationControl.IORegionChanged += HandleIORegionChanged;
            ViewModel.IsEnabled = !equationControl.CalcIsDisabled;
            ViewModel.IsMathRegionActive = _region.IsKeyboardFocused;
            equationControl.MinWidth = _inputControl.MinWidth + num + 10.0;
            equationControl.MinHeight = Math.Max(_inputControl.MinHeight, val);
            equationControl.IsResizableInputControl = true;
            for (int num2 = 0; num2 < 5; num2++)
            {
                if (DocumentAttachedProperties.GetIsInPasting(equationControl) || DocumentAttachedProperties.GetIsInPrinting(equationControl))
                {
                    HandleCalculatableStateChanged(CalculatableState.Pasting);
                    return;
                }
            }
        }
    }

    private void HandleCalculatableStateChanged(CalculatableState state)
    {
        ViewModel.State = state;
    }

    public override void HandleMathFormatChanged(DependencyPropertyChangedEventArgs e)
    {
        if (e.Property.Name == "FontSize")
        {
            ViewModel.FontSize = WpfUtils.PointsToDips((double)e.NewValue);
        }
    }

    private void HandleIORegionChanged(DependencyObject arg1, IORegionChangeType arg2)
    {
        if (arg2 == IORegionChangeType.CalcIsDisabledStatus && arg1 is EquationControl equationControl)
        {
            ViewModel.IsEnabled = !equationControl.CalcIsDisabled;
        }
    }

    private void HandleSExpressionChanged(IEquationControl eq)
    {
        ViewModel.HandleSExpressionChanged(eq);
    }

    public TableSize OriginalSize { get; set; }

    private void HookupMatrixResizeWidget(object sender, MouseEventArgs eventArgs)
    {
        eventArgs.Handled = true;
    }

    public void UpdateOnBorderResize(Vector offset, bool changeWidth, bool changeHeight, UIElementCollection uiElementCollection = null)
    {
        if (offset.X == 0.0)
        {
            _availableWidthCache = 0.0;
        }

        if (changeWidth && offset.X != 0.0)
        {
            if (ViewModel.Width + offset.X - _availableWidthCache > 0.0)
            {
                ViewModel.Width += offset.X - _availableWidthCache;
                _scrollViewer.Width = ViewModel.Width;
            }

            _availableWidthCache = offset.X;
        }

        if (offset.Y == 0.0)
        {
            _availableHeightCache = 0.0;
        }

        if (changeHeight && offset.Y != 0.0)
        {
            if (ViewModel.Height + offset.Y - _availableHeightCache > 0.0)
            {
                ViewModel.Height += offset.Y - _availableHeightCache;
            }

            _availableHeightCache = offset.Y;
        }
    }

    protected override Size MeasureStaticLayout(Size availableSize)
    {
        _inputControl.FontFamily = Font.FontFamily;
        _inputControl.FontSize = Font.FontSize;
        _inputControl.FontStretch = Font.FontStretch;
        _inputControl.FontStyle = Font.FontStyle;
        _inputControl.FontWeight = Font.FontWeight;
        _inputControl.Foreground = Font.Foreground;
        _scrollViewer.Measure(availableSize);
        _inputControl.Measure(availableSize);
        ScrollViewer scrollViewer = _scrollViewer;
        Size desiredSize = _inputControl.DesiredSize;
        scrollViewer.MaxWidth = desiredSize.Width;
        _grid.Measure(availableSize);
        _border.Measure(availableSize);
        return _border.DesiredSize;
    }

    public override double EffectiveBaselineY
    {
        get
        {
            Size desiredSize = DesiredSize;
            return desiredSize.Height;
        }
    }

    public override double EffectiveScriptBaselineY => 0.0;

    public override double TextBaselineY => _effectiveBaselineY;

    public int RowCount => _matrixGrid.Rows;

    public int ColumnCount
    {
        get => _matrixGrid.Columns;
        set => _matrixGrid.Columns = value;
    }

    public UIElementCollection Cells => _matrixGrid.Children;

    public StackPanel RowHeading => _rowHeading;

    public StackPanel ColumnHeading => _columnHeading;

    public override INodeInfo NodeInfo
    {
        get => base.NodeInfo;
        set
        {
            base.NodeInfo = value;
            _matrixInfo = (MatrixNodeInfo)base.NodeInfo;
            _children = new IVisualMathNode[_matrixInfo.ViewportSize.Rows * _matrixInfo.ViewportSize.Columns];
            ColumnCount = _matrixInfo.ViewportSize.Columns;
        }
    }

    public string[] MatrixHeadingRowIndices => _rowIndices;

    public string[] MatrixHeadingColumnIndices => _columnIndices;

    public TableCoordinate? ErrorCell { get; set; }

    public IVisualMathNode ErrorNode
    {
        get
        {
            if (ErrorCell.HasValue)
            {
                if (ErrorCell.Value.Row >= _matrixInfo.ViewportOffset.Row + _matrixInfo.ViewportSize.Rows || ErrorCell.Value.Column >= _matrixInfo.ViewportOffset.Column + _matrixInfo.ViewportSize.Columns)
                {
                    return Cells[_matrixGrid.CellCount - 1] as IVisualMathNode;
                }

                if (ErrorCell.Value.Row < _matrixInfo.ViewportOffset.Row || ErrorCell.Value.Column < _matrixInfo.ViewportOffset.Column)
                {
                    return Cells[0] as IVisualMathNode;
                }
            }

            return null;
        }
    }

    public void TryDragIntoHiddenPart(IVisualMathNode nodeHit, out int rowShift, out int columnShift)
    {
        rowShift = 0;
        columnShift = 0;
        if (_matrixInfo.ViewportSize == _matrixInfo.FullSize)
        {
            return;
        }

        if (_matrixInfo.IsInResult)
        {
            return;
        }

        int indexOfNode = GetIndexOfNode(nodeHit);
        if (indexOfNode == -1)
        {
            return;
        }

        int num = MatrixHelper.RowOf(indexOfNode, ColumnCount);
        int num2 = MatrixHelper.ColumnOf(indexOfNode, ColumnCount);
        if (num == 0 && _matrixInfo.RowsBeforeViewport > 0)
        {
            rowShift = -1;
        }
        else if (num == RowCount - 1 && _matrixInfo.RowsAfterViewport > 0)
        {
            rowShift = 1;
        }

        if (num2 == 0 && _matrixInfo.ColsBeforeViewport > 0)
        {
            columnShift = -1;
        }
        else if (num2 == ColumnCount - 1 && _matrixInfo.ColsAfterViewport > 0)
        {
            columnShift = 1;
        }
    }

    public Visibility HeadingsVisibility
    {
        get => _headingsVisibility;
        set
        {
            _headingsVisibility = value;
            SetHeadingsCellsVisibility();
        }
    }

    internal override HitTestResult GetNodeAtPoint(Point hitPoint)
    {
        if (_toggle.Visibility == Visibility.Visible)
        {
            Point val = TransformToDescendant(_toggle).Transform(hitPoint);
            if (val.X > 0.0 && val.X < 16.0 && val.Y > 0.0 && val.Y < 16.0)
            {
                _toggle.IsChecked = !_toggle.IsChecked;
                return new VisualMathNodeHitTestResult(this, Direction.Indeterminate, Direction.Indeterminate, hitPoint);
            }
        }

        if (_scrollViewer.Visibility == Visibility.Collapsed || hitPoint.Y > _scrollViewer.ActualHeight)
        {
            return new VisualMathNodeHitTestResult(this, Direction.Left, Direction.Indeterminate, hitPoint);
        }

        Point val2 = TransformToDescendant(_matrixGrid).Transform(hitPoint);
        if (val2.X < 0.0)
        {
            return new VisualMathNodeHitTestResult(this, Direction.Left, Direction.Left, hitPoint);
        }

        if (val2.X > _matrixGrid.ActualWidth)
        {
            val2.X = _matrixGrid.ActualWidth;
        }

        VisualMathNode visualMathNode = _matrixGrid.CellFromPoint(val2);
        GeneralTransform generalTransform = TransformToDescendant(visualMathNode);
        return visualMathNode.GetNodeAtPoint(generalTransform.Transform(hitPoint));
    }

    protected virtual void HandleGotFocus(object sender, RoutedEventArgs e)
    {
        ViewModel.IsMathRegionActive = true;
        BlockFocus = true;
        e.Handled = !ViewModel.HasKeyboard;
    }

    protected virtual void HandleLostFocus(object sender, RoutedEventArgs e)
    {
        if (!_region.IsKeyboardFocused && !_scrollViewer.IsKeyboardFocused)
        {
            ViewModel.IsMathRegionActive = false;
        }

        BlockFocus = false;
    }

    protected virtual void HandleMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.Source == _toggle)
        {
            _region.IsLogicalFocusWithin = false;
        }

        if (e.Source == _inputControl || e.Source == _scrollViewer)
        {
            BlockFocus = true;
            e.Handled = true;
        }
        else
        {
            e.Handled = false;
        }
    }

    public override IVisualMathNode this[int index]
    {
        get => _children[index];
        set => _children[index] = value;
    }

    private static bool IsResizeIndicator(IVisualTextNode text)
    {
        if (text != null)
        {
            return text.Text == '…'.ToString() || text.Text == '⋮'.ToString() || text.Text == '⋰'.ToString() || text.Text == '⋱'.ToString();
        }

        return false;
    }

    private void SetupResizeIndicatorBehaviour(VisualTextNode resizeIndicator)
    {
        if (!IsResizeIndicator(resizeIndicator))
        {
            return;
        }

        resizeIndicator.MouseDown += (s, e) =>
        {
            e.Handled = true;
        };
        resizeIndicator.MouseUp += HookupMatrixResizeWidget;
        resizeIndicator.Cursor = Cursors.Hand;
        resizeIndicator.Loaded += ResizeIndicatorOnLoaded;
    }

    private void ResizeIndicatorOnLoaded(object sender, RoutedEventArgs e)
    {
        VisualTextNode visualTextNode = sender as VisualTextNode;
        if (visualTextNode == null || !IsResizeIndicator(visualTextNode))
        {
            return;
        }

        visualTextNode.ToolTip = ResourceProvider<GenericResources>.Instance["MatrixResizeWidgetToolTip"];
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        double num = ExtendedFont.SafeComputeInkBoundingBox(Font.n_Run).Height;
        _matrixGrid.BaselineToMidPointAdjustment = num / 2.0;
        _matrixGrid.Measure(availableSize);
        MeasureStaticLayout(availableSize);
        _effectiveBaselineY = _matrixGrid.MatrixBaseline + _matrixContentTop;
        FireMeasureOverideCompleted();
        return _border.DesiredSize;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        ArrangeStaticLayout(finalSize);
        if (_matrixGrid != null)
        {
            _matrixGrid.Arrange(new Rect(new Point(_matrixContentLeft, _matrixContentTop), new Size(_grid.DesiredSize.Width, _matrixGrid.DesiredSize.Height)));
        }

        DebugTools.DevAssert(IsMeasureValid, null);
        return _border.DesiredSize;
    }

    protected override Size ArrangeStaticLayout(Size finalSize)
    {
        _border.Arrange(new Rect(new Point(0.0, 0.0), finalSize));
        return finalSize;
    }

    public IVisualMathNode LeftFence => _leftBracket;

    public IVisualMathNode RightFence => _rightBracket;

    public VisualMathNode[] NodeChildren
    {
        get
        {
            VisualMathNode[] array = new VisualMathNode[_matrixGrid.Children.Count];
            _matrixGrid.Children.CopyTo((UIElement[])array, 0);
            return array;
        }
    }

    public Size DesiredSizeWithoutFences => _matrixGrid.DesiredSize;

    public bool Horizontal => true;

    public void SetParensVisibility(Visibility visibility)
    {
        _opacity = visibility == Visibility.Visible ? 1 : 0;
        _leftBracket.Opacity = _opacity;
        _rightBracket.Opacity = _opacity;
    }

    private StackPanel CreateHeading(string panelName, ExtendedFont font)
    {
        StackPanel stackPanel = new StackPanel
        {
            Name = panelName,
            Orientation = Orientation.Vertical,
            Opacity = 0.6
        };
        TextElement.SetFontSize(stackPanel, font.FontSize - 2.0);
        TextElement.SetFontFamily(stackPanel, font.FontTypeface.FontFamily);
        return stackPanel;
    }

    private void ArrangeHeadingContent(StackPanel heading)
    {
        heading.Width = 0.0;
        heading.Height = 0.0;
        double fontSize = TextElement.GetFontSize(heading);
        for (int num = 0; num < heading.Children.Count; num++)
        {
            if (heading.Children[num] is Label label)
            {
                label.FontSize = fontSize;
                double val = 0.0;
                string text = label.Content.ToString();
                if (text != null && text.IndexOf('⋮') >= 0)
                {
                    val = 2.0 * ExtendedFont.SafeComputeInkBoundingBox(Font.j_Run).Height + 2.0 * _matrixGrid.RowSpacing;
                }

                if (heading.Name == "RowHeading")
                {
                    label.Height = Math.Max(val, _matrixGrid.GetRowHeight(num) + _matrixGrid.RowSpacing);
                    label.HorizontalContentAlignment = HorizontalAlignment.Right;
                    label.Padding = new Thickness(2.0, 0.0, 2.0, 0.0);
                }
                else
                {
                    label.Height = Math.Max(val, _matrixGrid.GetColumnWidth(num) + _matrixGrid.ColumnSpacing);
                    label.HorizontalContentAlignment = HorizontalAlignment.Left;
                    label.Padding = new Thickness(2.0, 0.0, 0.0, 0.0);
                }

                label.VerticalContentAlignment = VerticalAlignment.Center;
                label.Visibility = HeadingsVisibility;
                label.Measure(WpfUtils.SizeInfinity);
                if (heading.Width < label.DesiredSize.Width)
                {
                    heading.Width = label.DesiredSize.Width;
                }

                heading.Height += label.DesiredSize.Height;
            }
        }
    }

    private string[] GetMatrixRowOrColumnIndices(StackPanel heading)
    {
        ArrayList arrayList = new ArrayList();
        if (heading.Children.Count > 0)
        {
            Regex regex = new Regex("\\d");
            foreach (UIElement child in heading.Children)
            {
                if (!(child is Label label))
                {
                    continue;
                }

                string[] array = label.Content.ToString().Split('\n');
                foreach (string text in array)
                {
                    if (regex.IsMatch(text))
                    {
                        arrayList.Add(text);
                    }
                    else
                    {
                        arrayList.Add('…'.ToString());
                    }
                }
            }
        }
        else
        {
            int num2 = heading.Name == "RowHeading" ? _matrixInfo.ViewportSize.Rows : _matrixInfo.ViewportSize.Columns;
            int origin = _matrixInfo.Origin;
            for (int num3 = 0; num3 < num2; num3++)
            {
                arrayList.Add((num3 + origin).ToString());
            }
        }

        return (string[])arrayList.ToArray(typeof(string));
    }

    public void SetHeadingsCellsVisibility()
    {
        foreach (UIElement child in RowHeading.Children)
        {
            child.Visibility = HeadingsVisibility;
        }

        foreach (UIElement child2 in ColumnHeading.Children)
        {
            child2.Visibility = HeadingsVisibility;
        }
    }

    private void PopulateMatrixHeadings()
    {
        Label element;
        if (_matrixInfo.ViewportOffset.Row > 0 || _matrixInfo.Remainder.Rows > 0)
        {
            if (_matrixInfo.ViewportOffset.Row > 0)
            {
                string content = _matrixInfo.Origin + "\n⋮";
                element = new Label
                {
                    Content = content
                };
                RowHeading.Children.Add(element);
            }

            for (int num = 0; num < _matrixInfo.ViewportSize.Rows; num++)
            {
                element = new Label
                {
                    Content = _matrixInfo.Origin + _matrixInfo.ViewportOffset.Row + num
                };
                RowHeading.Children.Add(element);
            }

            if (_matrixInfo.Remainder.Rows > 0)
            {
                int num = _matrixInfo.ViewportSize.Rows - 1 + _matrixInfo.Remainder.Rows + _matrixInfo.ViewportOffset.Row + _matrixInfo.Origin;
                string content2 = " ⋮\n" + num;
                element = new Label
                {
                    Content = content2
                };
                RowHeading.Children.Add(element);
            }
        }

        if (_matrixInfo.ViewportOffset.Column > 0)
        {
            string content3 = _matrixInfo.Origin + "\n⋮";
            element = new Label
            {
                Content = content3
            };
            ColumnHeading.Children.Add(element);
        }

        for (int num2 = 0; num2 < _matrixInfo.ViewportSize.Columns; num2++)
        {
            element = new Label
            {
                Content = (_matrixInfo.Origin + _matrixInfo.ViewportOffset.Column + num2).ToString()
            };
            ColumnHeading.Children.Add(element);
        }

        if (_matrixInfo.Remainder.Columns > 0)
        {
            int num3 = _matrixInfo.ViewportSize.Columns - 1 + _matrixInfo.Remainder.Columns + _matrixInfo.ViewportOffset.Column + _matrixInfo.Origin;
            string content4 = " ⋮\n" + num3;
            element = new Label
            {
                Content = content4
            };
            ColumnHeading.Children.Add(element);
        }

        _rowIndices = GetMatrixRowOrColumnIndices(_rowHeading);
        _columnIndices = GetMatrixRowOrColumnIndices(_columnHeading);
    }

    public override void BuildChildren(IVisualMathNode[] children)
    {
        base.BuildChildren(children);
        ConstructMatrixGrid();
    }

    private void ConstructMatrixGrid()
    {
        if (_matrixInfo.IsInResult)
        {
            _matrixGrid.ColumnAlignment = ColumnAlignments.DecimalPointAlign;
        }

        MatrixDisplayHelper matrixDisplayHelper = new MatrixDisplayHelper(_matrixInfo);
        OriginalSize = _matrixInfo.FullSize;
        ColumnCount = matrixDisplayHelper.DisplaySize.Columns;
        for (int num = 0; num < matrixDisplayHelper.DisplaySize.Rows * matrixDisplayHelper.DisplaySize.Columns; num++)
        {
            TableCoordinate cellPosition = new TableCoordinate(num / ColumnCount, num % ColumnCount);
            IVisualMathNode visualMathNode = CheckIfSpecialCellNeeded(cellPosition, matrixDisplayHelper);
            if (visualMathNode != null)
            {
                _matrixGrid.Children.Add((UIElement)visualMathNode);
                VisualTextNode visualTextNode = visualMathNode as VisualTextNode;
                if (IsResizeIndicator(visualTextNode))
                {
                    SetupResizeIndicatorBehaviour(visualTextNode);
                }

                continue;
            }

            if (_matrixInfo.ViewportOffset.Row > 0)
            {
                cellPosition.Row -= 1;
            }

            if (_matrixInfo.ViewportOffset.Column > 0)
            {
                cellPosition.Column -= 1;
            }

            if (matrixDisplayHelper.ShowChildAsIs(cellPosition))
            {
                _matrixGrid.Children.Add((UIElement)this[cellPosition.Row * _matrixInfo.ViewportSize.Columns + cellPosition.Column]);
            }
        }
    }

    private IVisualMathNode CreateTextNode(string value)
    {
        IVisualTextNode visualTextNode = new VisualTextNode();
        visualTextNode.MathFormat.SetPropertiesFrom(MathFormat);
        DependencyObject val = visualTextNode as DependencyObject;
        if (val != null)
        {
            EquationControl.SetOverrideLabelColor(val, true);
            val.SetValue(AttachedProperties.LabeledIdFormatProperty, null);
        }

        visualTextNode.Text = value;
        return visualTextNode;
    }

    private IVisualMathNode CheckIfSpecialCellNeeded(TableCoordinate cellPosition, MatrixDisplayHelper displayHelper)
    {
        if (displayHelper.ShowDownRightEllipsis(cellPosition))
        {
            return CreateTextNode('⋱'.ToString());
        }

        if (displayHelper.ShowHorizontalEllipsis(cellPosition))
        {
            return CreateTextNode('…'.ToString());
        }

        if (displayHelper.ShowVerticalEllipsis(cellPosition))
        {
            return CreateTextNode('⋮'.ToString());
        }

        if (displayHelper.ShowEmptyCell(cellPosition))
        {
            return CreateTextNode(" ");
        }

        return null;
    }

    private int GetIndexOfNode(IVisualMathNode childNode)
    {
        if (!(childNode is VisualMathNode element))
        {
            return -1;
        }

        return _matrixGrid.Children.IndexOf(element);
    }

    public void Dispose()
    {
        if (ViewModel != null)
        {
            ((IInputScriptableControlModelWithInputs)ViewModel.Model).CommitNeeded -= HandleCommitNeeded;
            ((IInputScriptableControlModelWithInputs)ViewModel.Model).PropertyChanged -= HandleModelPropertyChanged;
            _region.Activated -= HandleRegionActivated;
            _region.Deactivated -= HandleRegionDeactivated;
        }

        EquationControl equationControl = GetEquationControl();
        if (equationControl != null)
        {
            equationControl.ExpressionValueChanged -= HandleSExpressionChanged;
            equationControl.IORegionChanged -= HandleIORegionChanged;
        }
    }

    public IVisualMathNode[] MathChildren => _children;

    public new UIElementCollection Children
    {
        get
        {
            if (_grid != null)
            {
                return _grid.Children;
            }

            return base.Children;
        }
    }

    public bool NavigateInto(Direction direction)
    {
        bool result = true;
        switch (direction)
        {
            case Direction.Left:
            case Direction.Right:
                if (_inputControl.IsVisible)
                {
                    ((Grid)((ContentControl)_inputControl).Content).Children[0].Focus();
                }

                break;
            default:
                result = false;
                break;
            case Direction.Indeterminate:
                break;
        }

        return result;
    }

    public bool IsEditorFrameVisible { get; }

    public Control InputControl => _inputControl;

    public IDataExchangeModel DataExchangeModel { get; private set; }

    public VisualInputScriptableControlNodeBase()
    {
    }

    event RoutedEventHandler IVisualNode.Loaded
    {
        add => base.Loaded += value;
        remove => base.Loaded -= value;
    }

    bool IVisualNode.IsLoaded => base.IsLoaded;
}
