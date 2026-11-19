using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using Quire.Calculation.Core;
using Quire.Controls.Core;
using Quire.Controls.Core.Whiteboard;
using Quire.Controls.Internal;
using Quire.Controls.Plots.PlotCommands;
using Quire.Controls.Plots.Presenter;
using Quire.Controls.Plots.ViewModel;
using Quire.Controls.Text;
using Quire.Core;
using Quire.EquationEditor.View;
using Quire.FunctionalitiesLimitation;
using Quire.Services;
using Quire.Undo;
using Quire.Wpf;

namespace Quire.Controls.Plots.View;

public abstract partial class PlotControlBase
{
    public IPlotEquation ActivePlotEquation
    {
        get
        {
            return _activePlotEquation;
        }
        set
        {
            if (_activePlotEquation != value)
            {
                _activePlotEquation = value;
                FirePropertyChanged("ActivePlotEquation");
            }
        }
    }
    public double VerticalAlignmentOffset => 0.0;
    public bool IsEmpty => false;
    public bool IgnoreForHorizontalMovementAndWidth => false;
    protected virtual IWhiteboardItem LastChildItem
    {
        get
        {
            throw new NotImplementedException();
        }
    }
    protected virtual IWhiteboardItem FirstChildItem
    {
        get
        {
            throw new NotImplementedException();
        }
    }
    protected EquationControl ActiveEquation
    {
        get
        {
            return _activeEquation;
        }
        set
        {
            if (_activeEquation != null)
            {
                _activeEquation.PropertyChanged -= EquationControlPropertyChangedHandler;
            }
            _activeEquation = value;
            if (_activeEquation != null)
            {
                _activeEquation.PropertyChanged += EquationControlPropertyChangedHandler;
            }
            EquationControlPropertyChangedHandler(_activeEquation, new PropertyChangedEventArgs("CurrentIdLabel"));
            EquationControlPropertyChangedHandler(_activeEquation, new PropertyChangedEventArgs("CurrentDisplayIdLabel"));
            UpdateMathFormat();
        }
    }
    public Size GridCellSize
    {
        get
        {
            try
            {
                return WorksheetPropertiesManager.GetWorksheetProperties(this).GridCellSize;
            }
            catch (DevAssertionException)
            {
                return new Size(WpfUtils.InchesToDips(0.2), WpfUtils.InchesToDips(0.2));
            }
        }
    }
    public IdLabels? CurrentIdLabel
    {
        get
        {
            return _currentIdLabel;
        }
        set
        {
            _currentIdLabel = value;
            if (_currentIdLabel == IdLabels.None)
            {
                _currentIdLabel = null;
            }
            FirePropertyChanged("CurrentIdLabel");
        }
    }
    public global::System.Nullable<global::Quire.Controls.Core.IdLabels> CurrentDisplayIdLabel { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); set => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }
    public BackgroundTypeE PlotBackgroundType
    {
        get
        {
            return (BackgroundTypeE)((DependencyObject)this).GetValue(PlotBackgroundTypeProperty);
        }
        set
        {
            ((DependencyObject)this).SetValue(PlotBackgroundTypeProperty, (object)value);
        }
    }
    public bool IsActiveRegion => WhiteboardDependencyProperties.GetIsActive(this);
    public virtual global::System.Collections.Generic.IEnumerable<global::Quire.Calculation.Core.ICalculatable> CalculatableChildren { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }
    public bool IsTabNavigatable => !CalcIsDisabled;
    public bool CalcIsDisabled
    {
        get
        {
            return (bool)((DependencyObject)this).GetValue(CalcIsDisabledProperty);
        }
        set
        {
            ((DependencyObject)this).SetValue(CalcIsDisabledProperty, (object)value);
        }
    }
    public bool HasOriginPositioning
    {
        get
        {
            return (bool)((DependencyObject)this).GetValue(HasOriginPositioningProperty);
        }
        set
        {
            ((DependencyObject)this).SetValue(HasOriginPositioningProperty, (object)value);
        }
    }
    public bool CanHideFromCalculationContext => true;
    public NumberFormat CurrentNumberFormat
    {
        get
        {
            return _currentNumberFormat;
        }
        set
        {
            if (_currentNumberFormat != value)
            {
                _currentNumberFormat = value;
                FirePropertyChanged("CurrentNumberFormat");
            }
        }
    }
    public NumberFormat DisplayNumberFormat
    {
        get
        {
            return (NumberFormat)((DependencyObject)this).GetValue(DisplayNumberFormatProperty);
        }
        set
        {
            ((DependencyObject)this).SetValue(DisplayNumberFormatProperty, (object)value);
        }
    }
    public RichTextTitle ActiveRichTextTitle
    {
        get
        {
            return _activeRichTextTitle;
        }
        set
        {
            if (_activeRichTextTitle != value)
            {
                _activeRichTextTitle = value;
                FirePropertyChanged("IsSubscript");
                FirePropertyChanged("IsSuperscript");
                FirePropertyChanged("ActiveRichTextTitle");
            }
        }
    }
    public PlotItem ActivePlotItem
    {
        get
        {
            return _activePlotItem;
        }
        set
        {
            if (_activePlotItem != value)
            {
                _activePlotItem = value;
                FirePropertyChanged("ActivePlotItem");
            }
        }
    }
    public PlotGraphAxis ActiveAxis
    {
        get
        {
            return _activeAxis;
        }
        set
        {
            if (_activeAxis != value)
            {
                _activeAxis = value;
                UpdateTargetAxes();
                FirePropertyChanged("ActiveAxis");
            }
        }
    }
    public global::Quire.Calculation.Core.EngineError CurrentError { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }
    protected virtual bool ErrorFrameShouldBeVisible => ErrorIsNotNull;
    public bool ShowAxisExpressions
    {
        get
        {
            return (bool)((DependencyObject)this).GetValue(ShowAxisExpressionsProperty);
        }
        set
        {
            ((DependencyObject)this).SetValue(ShowAxisExpressionsProperty, (object)value);
        }
    }
    public bool ShowGridLines
    {
        get
        {
            return (bool)((DependencyObject)this).GetValue(_showGridLinesProperty);
        }
        protected set
        {
            ((DependencyObject)this).SetValue(_showGridLinesProperty, (object)value);
        }
    }
    public Brush GridLineColor
    {
        get
        {
            return (Brush)((DependencyObject)this).GetValue(_gridLineColorProperty);
        }
        protected set
        {
            ((DependencyObject)this).SetValue(_gridLineColorProperty, (object)value);
        }
    }
    private bool CanExecuteCommandsOnTargetAxes => TargetAxes != null;
    public bool ShowTicks
    {
        get
        {
            return (bool)((DependencyObject)this).GetValue(_showTicksProperty);
        }
        protected set
        {
            ((DependencyObject)this).SetValue(_showTicksProperty, (object)value);
        }
    }
    public bool ShowNumbers
    {
        get
        {
            return (bool)((DependencyObject)this).GetValue(_showNumbersProperty);
        }
        protected set
        {
            ((DependencyObject)this).SetValue(_showNumbersProperty, (object)value);
        }
    }
    protected PlotGraphAxis[] Axes
    {
        get
        {
            return _axes;
        }
        set
        {
            if (_axes != value)
            {
                _axes = value;
                UpdateTargetAxes();
            }
        }
    }
    protected PlotGraphAxis[] TargetAxes
    {
        get
        {
            return _targetAxes;
        }
        set
        {
            if (_targetAxes != value)
            {
                if (_targetAxes != null)
                {
                    PlotGraphAxis[] targetAxes = _targetAxes;
                    foreach (PlotGraphAxis obj in targetAxes)
                    {
                        obj.NumberFormatChanged -= PlotGraphAxisNumberFormatChanged;
                        obj.ShowTicksChanged -= UpdateShowTicks;
                        obj.ShowNumbersChanged -= UpdateShowNumbers;
                        obj.ShowGridLinesChanged -= UpdateShowGridLines;
                    }
                }
                _targetAxes = value;
                if (_targetAxes != null)
                {
                    PlotGraphAxis[] targetAxes = _targetAxes;
                    foreach (PlotGraphAxis obj2 in targetAxes)
                    {
                        obj2.NumberFormatChanged += PlotGraphAxisNumberFormatChanged;
                        obj2.ShowTicksChanged += UpdateShowTicks;
                        obj2.ShowNumbersChanged += UpdateShowNumbers;
                        obj2.ShowGridLinesChanged += UpdateGridLines;
                    }
                }
                UpdateCurrentNumberFormat();
                UpdateShowTicks();
                UpdateShowNumbers();
                UpdateGridLines();
            }
        }
    }
    public int SelectedTracesNumOfPoints
    {
        get
        {
            return (int)((DependencyObject)this).GetValue(SelectedTracesNumOfPointsProperty);
        }
        set
        {
            ((DependencyObject)this).SetValue(SelectedTracesNumOfPointsProperty, (object)value);
        }
    }
    public bool IsNumberOfPointsAvailableBase
    {
        get
        {
            return (bool)((DependencyObject)this).GetValue(IsNumberOfPointsAvailableBaseProperty);
        }
        set
        {
            ((DependencyObject)this).SetValue(IsNumberOfPointsAvailableBaseProperty, (object)value);
        }
    }
    public IEnumerable<string> NumberOfPointsValuesCollection
    {
        get
        {
            return ((DependencyObject)this).GetValue(NumberOfPointsValuesCollectionProperty) as IEnumerable<string>;
        }
        set
        {
            ((DependencyObject)this).SetValue(NumberOfPointsValuesCollectionProperty, (object)value);
        }
    }
    protected internal bool HasWorksheetProperties => WorksheetPropertiesManager.HasWorksheetProperties(this);
    public MathFormat MathFormat
    {
        get
        {
            return _mathFormat;
        }
        private set
        {
            _mathFormat = value;
            FirePropertyChanged("CurrentMathFormat");
        }
    }
    public MathFormat CurrentMathFormat
    {
        get
        {
            return MathFormat;
        }
        set
        {
            throw new NotImplementedException();
        }
    }
    public TextFormat CurrentTextFormat
    {
        get
        {
            return ActiveRichTextTitle?.CurrentTextFormat;
        }
        set
        {
            if (ActiveRichTextTitle != null)
            {
                ActiveRichTextTitle.CurrentTextFormat = value;
            }
        }
    }
    public ITextStyleChangedHandler TextStyleChangedHandler
    {
        get
        {
            return _textStyleChangedHandler;
        }
        set
        {
            if (_textStyleChangedHandler == value)
            {
                return;
            }
            _textStyleChangedHandler = value;
            PropagateTextStyleChangedHandler();
        }
    }
    public TextFormat InitialTextFormat
    {
        get
        {
            return _initialTextFormat;
        }
        set
        {
            if (_initialTextFormat == value)
            {
                return;
            }
            _initialTextFormat = value;
            PropagateInitialTextFormat();
        }
    }
    public IEnumerable<ICurrentMathFormatProperty> MathFormatElements
    {
        get
        {
            return _mathFormatElements;
        }
        private set
        {
            if (_mathFormatElements == value)
            {
                return;
            }
            _mathFormatElements = value;
            MathFormat = (_mathFormatElements.IsNullOrEmpty() ? null : new MathFormat(this));
        }
    }
    public bool FirstPropertyForXamlSerialization
    {
        get
        {
            return true;
        }
        set
        {
            InCustomInitialization = true;
            ReadDeserializationFormatsForPaste();
        }
    }
    public virtual global::Quire.Controls.Core.IMathFormat DeserializeMathFormat { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); set => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }
    public virtual global::System.Collections.Generic.IDictionary<global::Quire.Controls.Core.IdLabels, global::Quire.Controls.Core.ExtendedFont> DeserializeLabeledIdFormat { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); set => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }
    public virtual bool InCustomInitialization { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); set => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }
    public IEnumerable<NodeMessageAdorner> NodeMessageAdorners
    {
        get
        {
            List<NodeMessageAdorner> list = new List<NodeMessageAdorner>();
            if (ErrorAdorner != null)
            {
                list.Add(ErrorAdorner);
            }
            return list;
        }
    }
    global::Quire.Controls.Core.IUIElementArrangeInfoProvider global::Quire.Controls.Core.IHasChildArrangeInfoProvider.ChildArrangeInfoProvider { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }
    public XmlLanguage CurrentProofingLanguage
    {
        get
        {
            return ActiveRichTextTitle?.CurrentProofingLanguage;
        }
        private set
        {
            if (ActiveRichTextTitle != null && !object.Equals(ActiveRichTextTitle.CurrentProofingLanguage, value))
            {
                ActiveRichTextTitle.Document.Language = value;
            }
        }
    }
    public bool IsSubscript
    {
        get
        {
            if (ActiveRichTextTitle == null)
            {
                return false;
            }
            return ActiveRichTextTitle.IsSubscript;
        }
    }
    public bool IsSuperscript
    {
        get
        {
            if (ActiveRichTextTitle == null)
            {
                return false;
            }
            return ActiveRichTextTitle.IsSuperscript;
        }
    }
    public virtual bool HighlightAll(string text, FindOptions options)
    {
        throw new NotImplementedException();
    }
    public virtual bool FindOncePrivate(string text, bool startFromCaret, global::System.Windows.Documents.LogicalDirection direction, global::Quire.Controls.Core.FindOptions options, global::System.Collections.Generic.List<global::Quire.Pair<int, int>> subscriptPositions) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private ISearchableRegion GetActiveSearchableRegion()
    {
        if (ActiveRichTextTitle != null)
        {
            return ActiveRichTextTitle;
        }
        return ActivePlotEquation?.FocusedEquation;
    }
    private global::System.Collections.Generic.IEnumerator<global::Quire.Controls.Core.IFindRestrictor> GetISearchableEnumerator(global::Quire.Controls.Core.ISearchableRegion from, global::System.Windows.Documents.LogicalDirection direction, bool findInMath, bool findInText) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private global::System.Collections.Generic.List<global::Quire.Controls.Core.IFindRestrictor> GetSearchableRegions(bool findInMath, bool findInText) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private global::System.Collections.Generic.List<global::Quire.Controls.Core.IFindRestrictor> SortRegionsByLocation(global::System.Collections.Generic.List<global::Quire.Controls.Core.IFindRestrictor> regions) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private Point? GetTopLeftPosition(ISearchableRegion region)
    {
        if (region is FrameworkElement { IsLoaded: not false } frameworkElement)
        {
            try
            {
                return frameworkElement.TranslatePoint(new Point(0.0, 0.0), this);
            }
            catch
            {
                return null;
            }
        }
        return null;
    }
    protected virtual void BindCommands() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    protected PlotControlBase() { }
    public virtual void DelegateFocus() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public void HandleMathFormatChanged(DependencyPropertyChangedEventArgs e)
    {
    }
    public void Activate(bool isActive)
    {
        if (isActive && !IsKeyboardFocusWithin)
        {
            AppServiceRegistry.GetService<IFrameworkElementFacade>().ExecuteOnLoad(this, DelegateFocus);
        }
    }
    public global::System.Nullable<global::System.Windows.Vector> NavigateIntoItem(global::Quire.Direction direction) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public void SimulateMouseClick(Point location, int numberOfClicks)
    {
        Activate(isActive: true);
    }
    private void EquationControlPropertyChangedHandler(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
    {
        ICurrentIdLabelProperty currentIdLabelProperty = sender as ICurrentIdLabelProperty;
        if (currentIdLabelProperty != null && propertyChangedEventArgs.PropertyName == "CurrentIdLabel")
        {
            CurrentIdLabel = currentIdLabelProperty.CurrentIdLabel;
        }
        if (currentIdLabelProperty != null && propertyChangedEventArgs.PropertyName == "CurrentDisplayIdLabel")
        {
            CurrentDisplayIdLabel = currentIdLabelProperty.CurrentDisplayIdLabel;
        }
    }
    protected override void OnPreviewGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
    {
        base.OnPreviewGotKeyboardFocus(e);
        ActiveEquation = (e.OriginalSource as DependencyObject).TryFindAncestorIncludingSelf<EquationControl>();
    }
    protected void FirePropertyChanged(string propertyName)
    {
        if (PropertyChanged != null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    private static void ChangeBackgroundCommandExecute(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private static void OnBackgroundTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
    {
        ((PlotControlBase)(object)d).OnBackgroundTypeChanged((BackgroundTypeE)args.NewValue);
    }
    protected virtual void OnBackgroundTypeChanged(BackgroundTypeE newValue)
    {
        if (BackgroundTypePropertyChanged != null)
        {
            BackgroundTypePropertyChanged(newValue);
        }
    }
    protected void FireCalculatableContainerCalculatableAdded(ICalculatable calculatable)
    {
        if (CalculatableAdded != null)
        {
            CalculatableAdded(this, calculatable);
        }
    }
    protected void FireCalculatableContainerCalculatableRemoved(ICalculatable calculatable)
    {
        if (CalculatableRemoved != null)
        {
            CalculatableRemoved(calculatable);
        }
    }
    protected virtual void FireCalculatableMoved(ICalculatable calculatable)
    {
        if (CalculatableMoved != null)
        {
            CalculatableMoved(calculatable);
        }
    }
    internal void FireHasOriginPositioningChanged()
    {
        if (HasOriginPositioningChanged != null)
        {
            HasOriginPositioningChanged(HasOriginPositioning);
        }
    }
    private static void ChangeOriginPositioningCommandExecute(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public virtual void ChangeOriginPositioning(bool newValue)
    {
    }
    protected void BindChangeOriginPositioningCommand() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    protected static void UpdateHasOriginPositioningOfAxes(bool newValue, PlotGraphAxis rangeAxis, PlotGraphAxis primaryResultAxis)
    {
        using (new CommandScope("Call undoable cmds for updating HasOriginPositioning of x and y axis"))
        {
            int index = rangeAxis.PositionTickMark;
            int index2 = primaryResultAxis.PositionTickMark;
            if (newValue)
            {
                primaryResultAxis.FindClosestTickValueToZero(ref index);
                rangeAxis.FindClosestTickValueToZero(ref index2);
            }
            new AxisSetTickNumberCommand(rangeAxis, index, newValue).Execute();
            new AxisSetTickNumberCommand(primaryResultAxis, index2, newValue).Execute();
        }
    }
    private void HandleLostFocus(object sender, RoutedEventArgs e)
    {
        DependencyObject val = e.OriginalSource as DependencyObject;
        IInputElement focusedElement = FocusManager.GetFocusedElement(FocusManager.GetFocusScope(val));
        DependencyObject newFocused = focusedElement as DependencyObject;
        HandleLostFocus(val, newFocused);
    }
    private void HandleGotFocus(object sender, global::System.Windows.RoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    protected virtual void HandleGotFocusOverride(DependencyObject newFocused)
    {
    }
    private static void OnToggleHideFromCalculationContext(object sender, ExecutedRoutedEventArgs e)
    {
        if (sender is PlotControlBase)
        {
            (sender as PlotControlBase).HideFromCalculationContext = !(sender as PlotControlBase).HideFromCalculationContext;
            e.Handled = true;
        }
    }
    protected static void HasOriginPositioningPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
    {
        if (dependencyObject is PlotControlBase && dependencyPropertyChangedEventArgs.NewValue is bool)
        {
            (dependencyObject as PlotControlBase).FireHasOriginPositioningChanged();
        }
    }
    protected static NumberFormat CreateLocalNumberFormatIfNeeded(PlotGraphAxis axis)
    {
        if (!axis.HasLocalValue(PlotGraphAxis.DisplayNumberFormatProperty))
        {
            DebugTools.DevAssert(axis.DisplayNumberFormat != null, null);
            axis.DisplayNumberFormat = ((axis.DisplayNumberFormat != null) ? axis.DisplayNumberFormat.Clone() : new NumberFormat());
        }
        return axis.DisplayNumberFormat;
    }
    protected void UpdateTargetAxes()
    {
        if (ActivePlotItem != null)
        {
            TargetAxes = null;
            return;
        }
        TargetAxes = ((IsActiveRegion && ActiveAxis != null) ? new PlotGraphAxis[1] { ActiveAxis } : Axes);
    }
    public bool ContainsError(EngineError error)
    {
        foreach (ICalculatable calculatableChild in CalculatableChildren)
        {
            if (calculatableChild is ICalculationErrorOwner calculationErrorOwner && calculationErrorOwner.ContainsError(error))
            {
                return true;
            }
        }
        return false;
    }
    public void ShowError(EngineError error)
    {
        foreach (ICalculatable calculatableChild in CalculatableChildren)
        {
            if (calculatableChild is ICalculationErrorOwner calculationErrorOwner && calculationErrorOwner.ContainsError(error))
            {
                calculationErrorOwner.ShowError(error);
                break;
            }
        }
    }
    protected void ShowHideErrorFrame()
    {
        if (ErrorFrameShouldBeVisible)
        {
            if (PlotErrorFrame == null)
            {
                PlotErrorFrame = PlotControl.CreateErrorFrame(this);
            }
        }
        else if (PlotErrorFrame != null)
        {
            PlotErrorFrame.Remove();
            PlotErrorFrame = null;
        }
    }
    protected void HideErrorMessage()
    {
        if (ErrorAdorner != null)
        {
            ErrorAdorner.Remove();
            ErrorAdorner = null;
        }
    }
    protected void ShowErrorMessage(string errorMessage)
    {
        ErrorAdorner = new NodeMessageAdorner(this, errorMessage, NodeMessageAdornerType.Error);
    }
    protected void ShowHideErrorMessage()
    {
        HideErrorMessage();
        string errorMessage = ErrorMessage;
        if (ErrorMessage != null)
        {
            ShowErrorMessage(errorMessage);
        }
    }
    protected static void ToggleTicksExecuted(object sender, ExecutedRoutedEventArgs executedRoutedEventArgs)
    {
        if (sender is PlotControlBase plotControlBase)
        {
            plotControlBase.ToggleTicks(executedRoutedEventArgs.Parameter);
        }
    }
    private bool GetShowGridLines() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private static void ToggleGridLinesExecuted(object sender, ExecutedRoutedEventArgs e)
    {
        if (sender is PlotControlBase plotControlBase)
        {
            plotControlBase.ToggleGridLines(e.Parameter);
        }
    }
    private Brush GetGridLineColor()
    {
        if (TargetAxes == null || TargetAxes.Length == 0)
        {
            return GridLineColor;
        }
        return new SolidColorBrush(TargetAxes[0].GridLineColor);
    }
    private void UpdateShowGridLines()
    {
        ShowGridLines = GetShowGridLines();
    }
    private void UpdateGridLineColor()
    {
        GridLineColor = GetGridLineColor();
    }
    private void UpdateGridLines()
    {
        UpdateShowGridLines();
        UpdateGridLineColor();
    }
    private void ToggleGridLines(object parameter) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private static void CanExecuteToggleGridLines(object sender, CanExecuteRoutedEventArgs e)
    {
        if (sender is PlotControlBase { ActiveAxis: not null } && (sender is XYPlotControl || sender is PolarPlotControl))
        {
            e.CanExecute = true;
        }
    }
    private static void ChangeGridLineColorExecuted(object sender, ExecutedRoutedEventArgs e)
    {
        if (sender is PlotControlBase plotControlBase && e.Parameter is SolidColorBrush parameter)
        {
            plotControlBase.ChangeGridLineColor(parameter);
        }
    }
    private void ChangeGridLineColor(object parameter) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private static void CanExecuteChangeGridLineColor(object sender, CanExecuteRoutedEventArgs e)
    {
        if (sender is PlotControlBase plotControlBase)
        {
            e.CanExecute = plotControlBase.ShowGridLines;
        }
    }
    private void ToggleTicks(object parameter) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private bool GetNewValue(bool currentValue, object parameter)
    {
        if (IsActiveRegion)
        {
            return !currentValue;
        }
        if (parameter is bool)
        {
            return !(bool)parameter;
        }
        return !currentValue;
    }
    private void ExecuteAxesSetPropertyCommand(bool newValue, Action<PlotGraphAxis, bool> set, Func<PlotGraphAxis, bool> get)
    {
        if (CanExecuteCommandsOnTargetAxes)
        {
            CommandBuffer.Execute(new AxesSetPropertyCommand(TargetAxes, newValue, set, get));
        }
    }
    private void ExecuteAxesSetPropertyCommand<T>(T newValue, Action<PlotGraphAxis, T> set, Func<PlotGraphAxis, T> get, string name = null)
    {
        if (CanExecuteCommandsOnTargetAxes)
        {
            CommandBuffer.Execute(new AxesSetPropertyCommand<T>(TargetAxes, newValue, set, get, name));
        }
    }
    private bool GetShowTicks() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private bool GetShowNumbers() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    protected static void CanExecuteCommandWithBooleanParameterOnTargetAxes(object sender, CanExecuteRoutedEventArgs e)
    {
        if (sender is PlotControlBase plotControlBase && e.Parameter is bool)
        {
            e.CanExecute = plotControlBase.CanExecuteCommandsOnTargetAxes;
        }
    }
    protected static void ToggleNumberedExecuted(object sender, ExecutedRoutedEventArgs executedRoutedEventArgs)
    {
        if (sender is PlotControlBase plotControlBase)
        {
            plotControlBase.ToggleNumbered(executedRoutedEventArgs.Parameter);
        }
    }
    private void ToggleNumbered(object parameter) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    protected static void IsActiveRegionChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
    {
        if (dependencyObject is PlotControlBase && dependencyPropertyChangedEventArgs.NewValue is bool)
        {
            (dependencyObject as PlotControlBase).IsActiveRegionChanged((bool)dependencyPropertyChangedEventArgs.NewValue);
        }
    }
    protected virtual void IsActiveRegionChanged(bool newValue)
    {
        UpdateTargetAxes();
    }
    private static void IsSelectedChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
    {
        if (dependencyObject is PlotControlBase && dependencyPropertyChangedEventArgs.NewValue is bool)
        {
            (dependencyObject as PlotControlBase).IsSelectedChanged((bool)dependencyPropertyChangedEventArgs.NewValue);
        }
    }
    protected virtual void IsSelectedChanged(bool newValue)
    {
        MathFormatElements = (newValue ? GetPlotEquations() : null);
    }
    public void OnWorksheetPropertiesChanged(string[] propertyNames, IWorksheetProperties oldProperties, IWorksheetProperties newProperties)
    {
        foreach (string text in propertyNames)
        {
            if (text == "GridCellSize")
            {
                FirePropertyChanged(text);
            }
        }
    }
    protected override bool ShouldSerializeProperty(global::System.Windows.DependencyProperty dp) => base.ShouldSerializeProperty(dp);
    public new bool ShouldSerializeResources()
    {
        return false;
    }
    protected void OnTraverseForward(object sender, ExecutedRoutedEventArgs e)
    {
        OnTraverse((FocusNavigationDirection)0);
    }
    protected void OnTraverseBackward(object sender, ExecutedRoutedEventArgs e)
    {
        OnTraverse((FocusNavigationDirection)1);
    }
    protected void OnCanTraverseForward(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = GetNextTraversalItem(1) != null;
    }
    protected void OnCanTraverseBackwards(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = GetNextTraversalItem(-1) != null;
    }
    protected virtual List<RichTextTitle> BuildLabelsList()
    {
        return null;
    }
    protected virtual List<EquationControl> CreateTickMarksAndMarkersList()
    {
        return null;
    }
    private UIElement GetNextTraversalItem(int step)
    {
        IList<EquationControl> list;
        bool isCyclic;
        if (ActivePlotEquation.ActiveEquation == ActiveEquation)
        {
            list = BuildEquationList();
            isCyclic = false;
        }
        else
        {
            list = CreateTickMarksAndMarkersList();
            isCyclic = true;
        }
        int nextTraversalIndex = GetNextTraversalIndex(list, ActiveEquation, step, isCyclic);
        if (nextTraversalIndex != -1)
        {
            return list[nextTraversalIndex];
        }
        return null;
    }
    public static int GetNextTraversalIndex(IList<EquationControl> items, EquationControl active, int step, bool isCyclic)
    {
        if (items == null)
        {
            return -1;
        }
        int num = items.IndexOf(active);
        int num2 = items.Count();
        int num3 = -1;
        int num4 = 1;
        int num5;
        do
        {
            num5 = num + step * num4;
            if (isCyclic)
            {
                num5 %= num2;
                if (num5 < 0)
                {
                    num5 += num2;
                }
            }
            if (num5 >= 0 && num5 < num2)
            {
                num3 = num5;
            }
            num4++;
        }
        while (num4 < num2 && num3 == -1 && num5 >= 0 && num5 < num2);
        return num3;
    }
    protected virtual void OnTraversedFocusEvent(object sender, CustomRoutedEventArgs<FocusNavigationDirection> args)
    {
    }
    private void OnTraverse(FocusNavigationDirection direction)
    {
        int step;
        if ((int)direction == 0)
        {
            step = 1;
        }
        else
        {
            DebugTools.DevAssert((int)direction == 1, null);
            step = -1;
        }
        UIElement nextTraversalItem = GetNextTraversalItem(step);
        DebugTools.DevAssert(nextTraversalItem != null, null);
        ActivateCommand.Execute(null, nextTraversalItem);
        nextTraversalItem.RaiseEvent(new CustomRoutedEventArgs<FocusNavigationDirection>(CommonRoutedEvents.TraversedFocusEvent, direction));
    }
    protected void AddEquationsFromList(EquationList equationList, List<EquationControl> list)
    {
        for (int num = 0; num < equationList.EquationCount; num++)
        {
            list.Add(equationList[num].Exp);
            if (!(equationList[num] is PlotEquationWOUnit))
            {
                list.Add(equationList[num].Scaling);
            }
        }
    }
    protected static void ActivateExecuted(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.OriginalSource is PlotControlBase)
        {
            PlotControlBase plotControlBase = e.OriginalSource as PlotControlBase;
            if (!plotControlBase.IsActiveRegion)
            {
                plotControlBase.DelegateFocus();
            }
        }
        else if (e.OriginalSource is UIElement)
        {
            (e.OriginalSource as UIElement).Focus();
        }
        else
        {
            DebugTools.DevAssert(false, null);
        }
    }
    private void UpdateShowNumbers()
    {
        ShowNumbers = GetShowNumbers();
    }
    private void UpdateShowTicks()
    {
        ShowTicks = GetShowTicks();
    }
    private void OnSetNumberFormatPropertyPreview(object sender, ExecutedRoutedEventArgs args)
    {
        using (new CommandScope(args.Command.ToString()))
        {
            PlotGraphAxis[] targetAxes = TargetAxes;
            for (int num = 0; num < targetAxes.Length; num++)
            {
                CreateLocalNumberFormatIfNeeded(targetAxes[num]).SetPropertyValueWithCommand((SetPropertyValueCommandParameter)args.Parameter);
            }
        }
        args.Handled = true;
        UpdateCurrentNumberFormat();
    }
    private void UpdateCurrentNumberFormat() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    protected void OnClearResultFormatPreview(object sender, ExecutedRoutedEventArgs args)
    {
        using (new CommandScope(args.Command.ToString()))
        {
            PlotGraphAxis[] targetAxes = TargetAxes;
            for (int num = 0; num < targetAxes.Length; num++)
            {
                targetAxes[num].ClearDisplayNumberFormatWithCommand();
            }
        }
        args.Handled = true;
    }
    private void CanClearResultFormat(object sender, global::System.Windows.Input.CanExecuteRoutedEventArgs args) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void PlotGraphAxisNumberFormatChanged(NumberFormat numberFormat, bool isLocal)
    {
        UpdateCurrentNumberFormat();
    }
    protected string GetErrorMessage(global::Quire.Calculation.Core.EngineError error) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public virtual void DeserializeEpilog()
    {
    }
    private void UpdateMathFormat() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    protected void SetActiveTitleForTextFormat(RichTextTitle title)
    {
        if (ActiveRichTextTitle == title)
        {
            return;
        }
        if (ActiveRichTextTitle != null)
        {
            ActiveRichTextTitle.PropertyChanged -= OnActiveTitlePropertyChanged;
        }
        ActiveRichTextTitle = title;
        if (ActiveRichTextTitle != null)
        {
            ActiveRichTextTitle.PropertyChanged += OnActiveTitlePropertyChanged;
        }
        FirePropertyChanged("CurrentTextFormat");
        FirePropertyChanged("CurrentProofingLanguage");
    }
    private void OnActiveTitlePropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
        case "CurrentTextFormat":
            FirePropertyChanged("CurrentTextFormat");
            break;
        case "CurrentProofingLanguage":
            FirePropertyChanged("CurrentProofingLanguage");
            break;
        case "IsSubscript":
        case "IsSuperscript":
            FirePropertyChanged(e.PropertyName);
            break;
        }
    }
    protected RichTextTitle FindRichTextTitleAncestor(DependencyObject element)
    {
        return element.TryFindAncestorIncludingSelf<RichTextTitle>();
    }
    public void ApplyTextFormatPropertyValueToControl(DependencyProperty property, object value, Block block = null)
    {
        ITextFormatCreation textFormatCreation = (ITextFormatCreation)ActiveRichTextTitle;
        if (textFormatCreation != null)
        {
            textFormatCreation.ApplyTextFormatPropertyValueToControl(property, value, block);
        }
    }
    public void SetTextFormatPropertyValue(DependencyProperty property, object value, bool isUndoRedo)
    {
        ITextFormatOwner textFormatOwner = (ITextFormatOwner)ActiveRichTextTitle;
        if (textFormatOwner != null)
        {
            textFormatOwner.SetTextFormatPropertyValue(property, value, isUndoRedo);
        }
    }
    private void HandleApplyTextStyleCommand(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void CanExecuteApplyTextStyleCommand(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = ActiveRichTextTitle != null;
        e.Handled = true;
    }
    protected virtual void PropagateTextStyleChangedHandler()
    {
    }
    protected virtual void PropagateInitialTextFormat()
    {
    }
    private void CanExecuteMathFormatCommandPreview(object sender, CanExecuteRoutedEventArgs args)
    {
        if (MathFormat != null)
        {
            return;
        }
        args.CanExecute = false;
        args.Handled = true;
    }
    private void ReadDeserializationFormatsForPaste()
    {
        if (DebugTools.DevAssert(MathFormatForPaste.Instance != null && MathFormatForPaste.Instance.DocumentMathFormat != null && MathFormatForPaste.Instance.DocumentLabeledIdFormat != null, null).Failed)
        {
            return;
        }
        DeserializeMathFormat = MathFormatForPaste.Instance.DocumentMathFormat;
        DeserializeLabeledIdFormat = MathFormatForPaste.Instance.DocumentLabeledIdFormat;
    }
    public string GetHelpContext()
    {
        if (ActivePlotEquation.FocusedEquation == null)
        {
            return null;
        }
        return ActivePlotEquation.FocusedEquation.GetHelpContext();
    }
}
