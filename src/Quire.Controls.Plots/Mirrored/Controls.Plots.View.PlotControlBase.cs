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
    public IdLabels? CurrentDisplayIdLabel
    {
        get
        {
            return _currentDisplayIdLabel;
        }
        set
        {
            IdLabels? idLabels = ((value == IdLabels.None) ? ((IdLabels?)null) : value);
            if (_currentDisplayIdLabel != idLabels)
            {
                _currentDisplayIdLabel = idLabels;
                FirePropertyChanged("CurrentDisplayIdLabel");
            }
        }
    }
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
    public EngineError CurrentError => ((ICalculationErrorOwner)WpfUtils.GetFocusedAncestorOfTypeElement<EquationControl>(this))?.CurrentError;
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
            if (_targetAxes == value)
            {
                return;
            }
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
            if (_textStyleChangedHandler != value)
            {
                _textStyleChangedHandler = value;
                PropagateTextStyleChangedHandler();
            }
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
            if (_initialTextFormat != value)
            {
                _initialTextFormat = value;
                PropagateInitialTextFormat();
            }
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
            if (_mathFormatElements != value)
            {
                _mathFormatElements = value;
                MathFormat = (_mathFormatElements.IsNullOrEmpty() ? null : new MathFormat((DependencyObject)(object)this));
            }
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
    private global::System.Nullable<global::System.Windows.Point> GetTopLeftPosition(global::Quire.Controls.Core.ISearchableRegion region) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    protected virtual void BindCommands() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    protected PlotControlBase() { }
    public virtual void DelegateFocus() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public void HandleMathFormatChanged(global::System.Windows.DependencyPropertyChangedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public void Activate(bool isActive) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public global::System.Nullable<global::System.Windows.Vector> NavigateIntoItem(global::Quire.Direction direction) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public void SimulateMouseClick(global::System.Windows.Point location, int numberOfClicks) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void EquationControlPropertyChangedHandler(object sender, global::System.ComponentModel.PropertyChangedEventArgs propertyChangedEventArgs) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    protected override void OnPreviewGotKeyboardFocus(global::System.Windows.Input.KeyboardFocusChangedEventArgs e) => base.OnPreviewGotKeyboardFocus(e);
    protected void FirePropertyChanged(string propertyName) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private static void ChangeBackgroundCommandExecute(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private static void OnBackgroundTypeChanged(global::System.Windows.DependencyObject d, global::System.Windows.DependencyPropertyChangedEventArgs args) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    protected virtual void OnBackgroundTypeChanged(global::Quire.Controls.Core.BackgroundTypeE newValue) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    protected void FireCalculatableContainerCalculatableAdded(global::Quire.Calculation.Core.ICalculatable calculatable) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    protected void FireCalculatableContainerCalculatableRemoved(global::Quire.Calculation.Core.ICalculatable calculatable) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    protected virtual void FireCalculatableMoved(global::Quire.Calculation.Core.ICalculatable calculatable) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    internal void FireHasOriginPositioningChanged() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private static void ChangeOriginPositioningCommandExecute(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public virtual void ChangeOriginPositioning(bool newValue) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    protected void BindChangeOriginPositioningCommand() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    protected static void UpdateHasOriginPositioningOfAxes(bool newValue, global::Quire.Controls.Plots.PlotGraphAxis rangeAxis, global::Quire.Controls.Plots.PlotGraphAxis primaryResultAxis) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void HandleLostFocus(object sender, global::System.Windows.RoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void HandleGotFocus(object sender, global::System.Windows.RoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    protected virtual void HandleGotFocusOverride(global::System.Windows.DependencyObject newFocused) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private static void OnToggleHideFromCalculationContext(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    protected static void HasOriginPositioningPropertyChanged(global::System.Windows.DependencyObject dependencyObject, global::System.Windows.DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    protected static global::Quire.Controls.Core.NumberFormat CreateLocalNumberFormatIfNeeded(global::Quire.Controls.Plots.PlotGraphAxis axis) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    protected void UpdateTargetAxes() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public bool ContainsError(global::Quire.Calculation.Core.EngineError error) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public void ShowError(global::Quire.Calculation.Core.EngineError error) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    protected void ShowHideErrorFrame() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    protected void HideErrorMessage() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    protected void ShowErrorMessage(string errorMessage) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    protected void ShowHideErrorMessage() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    protected static void ToggleTicksExecuted(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs executedRoutedEventArgs) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private bool GetShowGridLines() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private static void ToggleGridLinesExecuted(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private global::System.Windows.Media.Brush GetGridLineColor() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void UpdateShowGridLines() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void UpdateGridLineColor() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void UpdateGridLines() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void ToggleGridLines(object parameter) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private static void CanExecuteToggleGridLines(object sender, global::System.Windows.Input.CanExecuteRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private static void ChangeGridLineColorExecuted(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void ChangeGridLineColor(object parameter) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private static void CanExecuteChangeGridLineColor(object sender, global::System.Windows.Input.CanExecuteRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void ToggleTicks(object parameter) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private bool GetNewValue(bool currentValue, object parameter) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void ExecuteAxesSetPropertyCommand(bool newValue, global::System.Action<global::Quire.Controls.Plots.PlotGraphAxis, bool> set, global::System.Func<global::Quire.Controls.Plots.PlotGraphAxis, bool> get) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void ExecuteAxesSetPropertyCommand<T>(T newValue, global::System.Action<global::Quire.Controls.Plots.PlotGraphAxis, T> set, global::System.Func<global::Quire.Controls.Plots.PlotGraphAxis, T> get, string name = default) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private bool GetShowTicks() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private bool GetShowNumbers() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    protected static void CanExecuteCommandWithBooleanParameterOnTargetAxes(object sender, global::System.Windows.Input.CanExecuteRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    protected static void ToggleNumberedExecuted(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs executedRoutedEventArgs) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void ToggleNumbered(object parameter) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    protected static void IsActiveRegionChanged(global::System.Windows.DependencyObject dependencyObject, global::System.Windows.DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    protected virtual void IsActiveRegionChanged(bool newValue) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private static void IsSelectedChanged(global::System.Windows.DependencyObject dependencyObject, global::System.Windows.DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    protected virtual void IsSelectedChanged(bool newValue) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public void OnWorksheetPropertiesChanged(string[] propertyNames, global::Quire.Controls.Core.IWorksheetProperties oldProperties, global::Quire.Controls.Core.IWorksheetProperties newProperties) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    protected override bool ShouldSerializeProperty(global::System.Windows.DependencyProperty dp) => base.ShouldSerializeProperty(dp);
    public bool ShouldSerializeResources() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    protected void OnTraverseForward(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    protected void OnTraverseBackward(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    protected void OnCanTraverseForward(object sender, global::System.Windows.Input.CanExecuteRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    protected void OnCanTraverseBackwards(object sender, global::System.Windows.Input.CanExecuteRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    protected virtual global::System.Collections.Generic.List<global::Quire.Controls.Text.RichTextTitle> BuildLabelsList() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    protected virtual global::System.Collections.Generic.List<global::Quire.Controls.EquationControl> CreateTickMarksAndMarkersList() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private global::System.Windows.UIElement GetNextTraversalItem(int step) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public static int GetNextTraversalIndex(global::System.Collections.Generic.IList<global::Quire.Controls.EquationControl> items, global::Quire.Controls.EquationControl active, int step, bool isCyclic) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    protected virtual void OnTraversedFocusEvent(object sender, global::Quire.Wpf.CustomRoutedEventArgs<global::System.Windows.Input.FocusNavigationDirection> args) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void OnTraverse(global::System.Windows.Input.FocusNavigationDirection direction) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    protected void AddEquationsFromList(global::Quire.Controls.Plots.EquationList equationList, global::System.Collections.Generic.List<global::Quire.Controls.EquationControl> list) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    protected static void ActivateExecuted(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void UpdateShowNumbers() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void UpdateShowTicks() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void OnSetNumberFormatPropertyPreview(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs args) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void UpdateCurrentNumberFormat() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    protected void OnClearResultFormatPreview(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs args) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void CanClearResultFormat(object sender, global::System.Windows.Input.CanExecuteRoutedEventArgs args) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void PlotGraphAxisNumberFormatChanged(global::Quire.Controls.Core.NumberFormat numberFormat, bool isLocal) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    protected string GetErrorMessage(global::Quire.Calculation.Core.EngineError error) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public virtual void DeserializeEpilog() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void UpdateMathFormat() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    protected void SetActiveTitleForTextFormat(global::Quire.Controls.Text.RichTextTitle title) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void OnActiveTitlePropertyChanged(object sender, global::System.ComponentModel.PropertyChangedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    protected global::Quire.Controls.Text.RichTextTitle FindRichTextTitleAncestor(global::System.Windows.DependencyObject element) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public void ApplyTextFormatPropertyValueToControl(global::System.Windows.DependencyProperty property, object value, global::System.Windows.Documents.Block block = default) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public void SetTextFormatPropertyValue(global::System.Windows.DependencyProperty property, object value, bool isUndoRedo) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void HandleApplyTextStyleCommand(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void CanExecuteApplyTextStyleCommand(object sender, global::System.Windows.Input.CanExecuteRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    protected virtual void PropagateTextStyleChangedHandler() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    protected virtual void PropagateInitialTextFormat() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void CanExecuteMathFormatCommandPreview(object sender, global::System.Windows.Input.CanExecuteRoutedEventArgs args) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void ReadDeserializationFormatsForPaste() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public string GetHelpContext() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
}
