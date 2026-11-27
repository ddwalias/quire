#nullable disable
#pragma warning disable

using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;
using System.Xml;
using Quire.Calculation.Core;
using Quire.Controls.Core;
using Quire.Controls.Core.Serialization;
using Quire.Controls.Internal;
using Quire.Controls.Plots.Presenter;
using Quire.Controls.Plots.ViewModel;
using Quire.PersistentDataObjects;
using Quire.Undo;
using Quire.Wpf;
using Quire.Xml;

namespace Quire.Controls.Plots;

public partial class PlotEquation : PlotEquationBase, IPlotEquationScaled, IPlotEquation, IDelegateFocus, IInputElement, IPlotEquationBase, ICurrentMathFormatProperty, INotifyPropertyChanged, IMathFormatOwner, ISupportCustomInitialization, IMathFormatForDeserialization, IComponentConnector
{
    private PreemptiveCommandBinding _navigateToEquationStart;

    private RubberBandHighlighter _errorFrame;

    private IPlotEquationViewModelBase _viewModel;

    private readonly PropertyChangedNotificationHandler _propertyChangedNotificationHandler = new PropertyChangedNotificationHandler();

    private static readonly DependencyPropertyKey _scalingHeaderVisibilityPropertyKey;

    public static readonly DependencyProperty ScalingHeaderVisibilityProperty;

    private static readonly Type _typeOfThis;

    public static readonly DependencyProperty CalcIsDisabledProperty;

    public static readonly DependencyProperty DisplayNumberFormatProperty;

    internal const string EmptySExpression = "@PLACEHOLDER";

    private void ComputeBaseLine(double expHeight, double scalingHeight, double zEqBaseLineY)
    {
        if (expHeight < scalingHeight)
        {
            zEqBaseLineY += (scalingHeight - expHeight) / 2.0;
        }
        TextBaselineOffset = zEqBaseLineY;
    }

    protected override Size MeasureOverride(Size constraint)
    {
        Size result = base.MeasureOverride(constraint);
        ComputeBaseLine(_exp.DesiredSize.Height, _scaling.DesiredSize.Height, _exp.TextBaselineY);
        return result;
    }

    public PlotEquation()
    {
        InitializeComponent();
        MathFormatDobjController mathFormat = new MathFormatDobjController(this);
        new MathFormatDobjController(_exp).SetPropertiesFrom(mathFormat);
        new MathFormatDobjController(_scaling).SetPropertiesFrom(mathFormat);
        _exp.RequestEvaluation += OnRequestEvaluation;
        _scaling.RequestEvaluation += OnRequestEvaluation;
        _exp.PreviewGotKeyboardFocus += OnPreviewGotKeyboardFocus;
        _scaling.PreviewGotKeyboardFocus += OnPreviewGotKeyboardFocus;
        _exp.RequestFocus += OnRequestFocus;
        _scaling.RequestFocus += OnRequestFocus;
        AttachSExpressionValueChanged();
        _scaling.ExpressionValueChanged += (IEquationControl scalingEquationControl) =>
        {
            FireScalingChanged();
        };
        BindCommands();
        ActiveEquation = _exp;
        ScalingChangedByUser = false;
        AddHandler(CommonRoutedEvents.TraversedFocusEvent, new CustomRoutedEventHandler<FocusNavigationDirection>(OnTraversedFocusEvent), handledEventsToo: true);
        _propertyChangedNotificationHandler.AddPropertyChangedNotificationHandler("PlotCalculatableError", PlotCalculatableErrorChanged);
    }

    static PlotEquation()
    {
        _scalingHeaderVisibilityPropertyKey = DependencyProperty.RegisterReadOnly("ScalingHeaderVisibility", typeof(Visibility), typeof(PlotEquation), new FrameworkPropertyMetadata(Visibility.Hidden));
        ScalingHeaderVisibilityProperty = _scalingHeaderVisibilityPropertyKey.DependencyProperty;
        _typeOfThis = typeof(PlotEquation);
        CalcIsDisabledProperty = EquationControl.CalcIsDisabledProperty.AddOwner(_typeOfThis);
        DisplayNumberFormatProperty = EquationControl.DisplayNumberFormatProperty.AddOwner(_typeOfThis);
    }

    public override IEnumerable<IEquationControl> ContainedEquationControls
    {
        get
        {
            yield return _exp;
            yield return _scaling;
        }
    }

    protected override void OnSubEquationExpressionChanged(IEquationControl subEquation)
    {
        if (subEquation != ScalingEquation)
        {
            if (!EnvironmentScope.IsCurrentlyInsideUndoOrRedo)
            {
                string oldExp = ScalingEquation.EquationSExpression;
                Delegate0 undoMethod = () =>
                {
                    using (new CommandScope(new CompoundCommand()))
                    {
                        _scaling.EquationSExpression = oldExp;
                    }
                };
                CommandBuffer.Execute(new DummyChangeUnitsCommand(() => true, undoMethod));
            }
        }
        else
        {
            UserScalingExpression = null;
            ScalingChangedByUser = !IsScalingEmpty;
        }
        base.OnSubEquationExpressionChanged(subEquation);
    }

    private void PlotCalculatableErrorChanged()
    {
        if (PlotCalculatableError == null)
        {
            if (_errorFrame != null)
            {
                _errorFrame.Remove();
                _errorFrame = null;
            }
        }
        else if (_errorFrame == null)
        {
            _errorFrame = PlotControl.CreateErrorFrame(this);
        }
    }

    private void OnTraversedFocusEvent(object sender, CustomRoutedEventArgs<FocusNavigationDirection> args)
    {
        if (args.Parameter == FocusNavigationDirection.Next)
        {
            NavigateRegionStart();
        }
        else
        {
            NavigateRegionEnd();
        }
    }

    public Visibility ScalingHeaderVisibility
    {
        get
        {
            return (Visibility)GetValue(ScalingHeaderVisibilityProperty);
        }
        private set
        {
            SetValue(_scalingHeaderVisibilityPropertyKey, value);
        }
    }

    private void BindCommands()
    {
        CommandBindings.Add(new CommandBinding(EquationCommands.NavigateLeft, OnNavigateLeft, CanNavigateLeft));
        CommandBindings.Add(new CommandBinding(EquationCommands.NavigateRight, OnNavigateRight, CanNavigateRight));
        WpfUtils.AddPreemptiveCommandBinding(this, EquationCommands.Delete, HandleDelete, null, bindingMarksEventAsHandled: false);
        WpfUtils.AddPreemptiveCommandBinding(this, EquationCommands.Backspace, HandleBackspace, null, bindingMarksEventAsHandled: false);
        WpfUtils.AddPreemptiveCommandBinding(this, EquationCommands.NavigateToEquationEnd, HandleNavigateToEquationEnd, null, bindingMarksEventAsHandled: true);
        _navigateToEquationStart = WpfUtils.AddPreemptiveCommandBinding(this, EquationCommands.NavigateToEquationStart, HandleNavigateToEquationStart, null, bindingMarksEventAsHandled: true);
    }

    private void CanNavigateRight(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = CanNavigate(ActiveEquation, Direction.Right);
    }

    private void CanNavigateLeft(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = CanNavigate(ActiveEquation, Direction.Left);
    }

    private void OnNavigateRight(object sender, ExecutedRoutedEventArgs e)
    {
        if (((object)_exp).Equals(ActiveEquation))
        {
            NavigateToScaling();
        }
    }

    private void OnNavigateLeft(object sender, ExecutedRoutedEventArgs e)
    {
        if (((object)_scaling).Equals(ActiveEquation))
        {
            NavigateToExp();
        }
    }

    private void OnPreviewGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
    {
        if (sender is EquationControl)
        {
            ActiveEquation = (EquationControl)sender;
        }
    }

    private void HandleNavigateToEquationStart(object sender, ExecutedRoutedEventArgs e)
    {
        NavigateRegionStart();
    }

    private void HandleNavigateToEquationEnd(object sender, ExecutedRoutedEventArgs e)
    {
        NavigateRegionEnd();
    }

    private void HandleBackspace(object sender, ExecutedRoutedEventArgs e)
    {
        if (sender is PlotEquation && IsPlotEquationEmpty)
        {
            FireRequestRemove(this, Direction.Left);
            e.Handled = true;
        }
    }

    private void HandleDelete(object sender, ExecutedRoutedEventArgs e)
    {
        if (sender is PlotEquation && IsPlotEquationEmpty)
        {
            FireRequestRemove(this, Direction.Right);
            e.Handled = true;
        }
    }

    private bool CanNavigate(IEquationControl control, Direction direction)
    {
        if (control.Equals(_exp) && direction == Direction.Right)
        {
            return true;
        }
        if (control.Equals(_scaling) && direction == Direction.Left)
        {
            return true;
        }
        return false;
    }

    private void NavigateToExp()
    {
        _exp.Focus();
        _exp.NavigateRegionEnd();
    }

    private void NavigateToScaling()
    {
        _scaling.Focus();
        _scaling.NavigateRegionStart();
    }

    public void ShowOrHideScaling(bool isEquationListKeyboardFocusWithin)
    {
        if (!IsScalingEmpty)
        {
            ShowScaling();
            HideScalingHeader();
        }
        else if (IsKeyboardFocusWithin)
        {
            ShowScaling();
            ShowScalingHeader();
        }
        else if (isEquationListKeyboardFocusWithin)
        {
            ShowScaling();
            HideScalingHeader();
        }
        else
        {
            HideScaling();
            HideScalingHeader();
        }
    }

    private void ShowScaling()
    {
        _scaling.Opacity = 1.0;
    }

    private void HideScaling()
    {
        _scaling.Opacity = 0.0;
    }

    private void ShowScalingHeader()
    {
        ScalingHeaderVisibility = Visibility.Visible;
    }

    private void HideScalingHeader()
    {
        ScalingHeaderVisibility = Visibility.Hidden;
    }

    private void OnRequestEvaluation(ICalculatable calculatable)
    {
        Commit();
    }

    public static string FormatPlotEquationSExpression(string equationSExpression, string unitSExpression)
    {
        return string.Format("(@TRACE_EXPR {0} (@TRACE_UNIT {1}))", equationSExpression, unitSExpression);
    }

    public string ScalingRl
    {
        get
        {
            string rl = GetUserScalingExpression().Rl;
            if (!IsScalingEmpty && ScalingChangedByUser)
            {
                return rl;
            }
            return "(number 1)";
        }
    }

    public string EquationSExpression
    {
        get
        {
            return FormatPlotEquationSExpression(_exp.EquationSExpression, _scaling.EquationSExpression);
        }
        set
        {
            // Release IL requires callvirt on newobj without a local slot.
            Match match = (new Regex("^\\(@TRACE_EXPR (?<Exp>.*)\\(@TRACE_UNIT (?<Scaling>.*)\\)\\)$") switch { var r => r }).Match(value);
            if (match.Success)
            {
                string equationSExpression = match.Result("${Exp}");
                string scalingExpression = match.Result("${Scaling}");
                _exp.EquationSExpression = equationSExpression;
                _scaling.EquationSExpression = scalingExpression;
                return;
            }
            throw new FormatException("The SExp is not in a correct format. the format should be:(@TRACE_EXPR @PLACEHOLDER (@TRACE_UNIT @PLACEHOLDER))");
        }
    }

    [Obsolete("Usage of Exp property is deprecated. Use Expression instead.")]
    public EquationControl Exp => _exp;

    public override IEquationControl Expression => _exp;

    [Obsolete("Scaling EquationControl should not be exposed directly.")]
    public EquationControl Scaling => _scaling;

    public IWhiteboardItem ScalingItem => _scaling;

    public bool IsScalingEmpty => ScalingEquation.EquationSExpression == SExpressionUtils.PlaceholderNode;

    public bool IsExpEmpty => Expression.EquationSExpression == SExpressionUtils.PlaceholderNode;

    public bool IsPlotEquationEmpty
    {
        get
        {
            if (IsScalingEmpty)
            {
                return IsExpEmpty;
            }
            return false;
        }
    }

    public bool ScalingChangedByUser { get; private set; }

    public string UserScalingExpression { get; private set; }

    public string EmptyPlotEquationSExp => FormatPlotEquationSExpression("@PLACEHOLDER", "@PLACEHOLDER");

    public void SetDataObject(plotEquation equationData)
    {
        if (AssertionE.Failed == (AssertionE)DebugTools.DevAssert(equationData.math.Length > 0, null))
        {
            return;
        }
        equationData.math[0].SetXmlNodeOrSExpression(SetExpressionXmlNode, SetExpressionSExpression);
        if (equationData.math.Length < 2)
        {
            return;
        }
        equationData.math[1].SetXmlNodeOrSExpression(SetScalingXmlNode, SetScalingSExpression);
        if (equationData.formattingOverride != null)
        {
            MathFormat.SetDataObject(equationData.formattingOverride);
        }
    }

    public void SetRegionXmlNode(XmlNode value)
    {
        if (value.ChildNodes.Count > 2)
        {
            XmlNode xmlNode = value.ChildNodes[2];
            if (DebugTools.DevAssert(xmlNode.Name == "formattingOverride", null).Passed)
            {
                using XmlNodeReader reader = new XmlNodeReader(xmlNode);
                new MathFormatSerializer("formattingOverride", xmlNode.NamespaceURI, MathFormat, ignoreUnsetProperty: false).ReadXml(reader);
            }
        }
        SetExpressionXmlNode(value.ChildNodes[0]);
        SetScalingXmlNode(value.ChildNodes[1]);
    }

    private void SetScalingXmlNode(XmlNode value)
    {
        if (value.FirstChild.LocalName != "placeholder")
        {
            _scaling.SetRegionXmlNode(value);
            UserScalingExpression = _scaling.EquationSExpression;
            ShowScaling();
        }
    }

    private void SetScalingSExpression(string value)
    {
        if (value != null && value != "@PLACEHOLDER")
        {
            _scaling.EquationSExpression = value;
            UserScalingExpression = _scaling.EquationSExpression;
            ShowScaling();
        }
    }

    private void SetExpressionXmlNode(XmlNode value)
    {
        Expression.SetRegionXmlNode(value);
    }

    private void SetExpressionSExpression(string value)
    {
        Expression.EquationSExpression = value;
    }

    public XmlNode GetRegionXmlNode()
    {
        XmlDocument xmlDocument = new XmlDocument();
        string namespaceURI = XmlWorksheetData.McdxNameSpaceManager.LookupNamespace(string.Empty);
        XmlElement xmlElement = xmlDocument.CreateElement("plotEquation", namespaceURI);
        xmlElement.AppendChild(xmlDocument.ImportNode(GetExpXml(), deep: true));
        XmlNode regionXmlNode = GetUserScalingExpression().GetRegionXmlNode();
        xmlElement.AppendChild(xmlDocument.ImportNode(regionXmlNode, deep: true));
        if (MathFormat.AreAnyPropertiesSet)
        {
            using XmlNodeWriter writer = new XmlNodeWriter(xmlElement);
            new MathFormatSerializer("formattingOverride", xmlElement.NamespaceURI, MathFormat).WriteXml(writer);
        }
        return xmlElement;
    }

    private IEquationControl GetUserScalingExpression()
    {
        IEquationControl result;
        if (UserScalingExpression == null)
        {
            result = _scaling;
        }
        else
        {
            using (new CommandScope(new CompoundCommand()))
            {
                result = new EquationControl
                {
                    AllowExpressionsOnly = true,
                    EquationSExpression = UserScalingExpression
                };
            }
        }
        return result;
    }

    private XmlNode GetExpXml()
    {
        return Expression.GetRegionXmlNode();
    }

    public bool AtLeftEdge
    {
        get
        {
            if (_scaling.IsKeyboardFocusWithin)
            {
                return false;
            }
            return _exp.AtLeftEdge;
        }
    }

    public string RegionErrorSExp
    {
        get
        {
            if (_exp.ErrorSExpression != string.Empty && _scaling.ErrorSExpression != string.Empty)
            {
                DebugTools.DevAssert(false, null);
            }
            if (_exp.ErrorSExpression != string.Empty)
            {
                return _exp.ErrorSExpression;
            }
            if (_scaling.ErrorSExpression != string.Empty)
            {
                return _scaling.ErrorSExpression;
            }
            return string.Empty;
        }
    }

    public string RegionErrorCode
    {
        get
        {
            if (_exp.ErrorCode != string.Empty && _scaling.ErrorCode != string.Empty)
            {
                DebugTools.DevAssert(false, null);
            }
            if (_exp.ErrorCode != string.Empty)
            {
                return _exp.ErrorCode;
            }
            if (_scaling.ErrorCode != string.Empty)
            {
                return _scaling.ErrorCode;
            }
            return string.Empty;
        }
    }

    public override IPlotEquationViewModelBase ViewModel
    {
        get
        {
            return _viewModel;
        }
        internal set
        {
            if (_viewModel != null)
            {
                _propertyChangedNotificationHandler.DetachPropertyChangedHandler(_viewModel);
            }
            _viewModel = value;
            _propertyChangedNotificationHandler.AttachPropertyChangedHandler(_viewModel);
            _propertyChangedNotificationHandler.UpdateAllProperties();
        }
    }

    public EngineError PlotCalculatableError
    {
        set
        {
            ViewModel.PlotCalculatableError = value;
        }
        get
        {
            return ViewModel.PlotCalculatableError;
        }
    }

    public double TextBaselineOffset { get; private set; }

    public EquationControl ActiveEquation { get; private set; }

    public IEquationControl FocusedEquation
    {
        get
        {
            if (!_exp.IsKeyboardFocusWithin)
            {
                if (!_scaling.IsKeyboardFocusWithin)
                {
                    return null;
                }
                return _scaling;
            }
            return _exp;
        }
    }

    public SynchronizationStatus SynchronizationStatus
    {
        get
        {
            if (_exp.SynchronizationStatus != SynchronizationStatus.Pending && _scaling.SynchronizationStatus != SynchronizationStatus.Pending)
            {
                return SynchronizationStatus.Synchronized;
            }
            return SynchronizationStatus.Pending;
        }
    }

    public CalculatingStatus CalculatingStatus
    {
        get
        {
            if (_exp.CalculatingStatus != CalculatingStatus.Calculating && _scaling.CalculatingStatus != CalculatingStatus.Calculating)
            {
                return CalculatingStatus.Idle;
            }
            return CalculatingStatus.Calculating;
        }
    }

    public override IEquationControl ScalingEquation => _scaling;

    public event Action<IPlotEquation, Direction> RequestRemove;

    public event Action RequestEvaluation;

    public event Action<IPlotEquationScaled> ScalingChanged;

    private void FireScalingChanged()
    {
        if (ScalingChanged != null)
        {
            ScalingChanged(this);
        }
    }

    public IList<Unit> Units
    {
        set
        {
            MergeScalingExpression(value);
        }
    }

    public int RegionErrorCount => _exp.ErrorCount + _scaling.ErrorCount;

    public IWhiteboardItem ExpressionItem => _exp;

    public NumberFormat DisplayNumberFormat
    {
        get
        {
            return (NumberFormat)GetValue(DisplayNumberFormatProperty);
        }
        set
        {
            SetValue(DisplayNumberFormatProperty, value);
            _exp.DisplayNumberFormat = DisplayNumberFormat;
            _scaling.DisplayNumberFormat = DisplayNumberFormat;
        }
    }

    public void Commit()
    {
        Commit(fireEvent: true);
    }

    public void Commit(bool fireEvent)
    {
        if (fireEvent)
        {
            FireRequestEvaluation();
        }
        CommitEquationAvoidingEvents(Expression);
        CommitEquationAvoidingEvents(ScalingEquation);
        IsCommitted = true;
    }

    private void CommitEquationAvoidingEvents(IEquationControl equationControl)
    {
        if (!equationControl.IsCommitted)
        {
            equationControl.RequestEvaluation -= OnRequestEvaluation;
            equationControl.Commit();
            equationControl.RequestEvaluation += OnRequestEvaluation;
        }
    }

    private void FireRequestEvaluation()
    {
        if (RequestEvaluation != null)
        {
            RequestEvaluation();
        }
    }

    private void FireRequestRemove(PlotEquation plotEquation, Direction direction)
    {
        if (RequestRemove != null)
        {
            RequestRemove(plotEquation, direction);
        }
    }

    public void DelegateFocus()
    {
        if (!ActiveEquation.Focus())
        {
            ActiveEquation.UpdateLayout();
            ActiveEquation.Focus();
        }
    }

    private void OnRequestFocus()
    {
        if (IsVisible)
        {
            return;
        }
        PlotControl plotControl = this.TryFindVisualAncestor<PlotControl>();
        if (plotControl != null)
        {
            plotControl.DelegateFocus();
        }
    }

    private void MergeScalingExpression(IEnumerable<Unit> value)
    {
        using (new CommandScope(new CompoundCommand()))
        {
            try
            {
                DetachSExpressionValueChanged();
                _navigateToEquationStart.Remove();
                if (UserScalingExpression == null)
                {
                    UserScalingExpression = _scaling.EquationSExpression;
                }
                else
                {
                    _scaling.EquationSExpression = UserScalingExpression;
                }
                if (value.IsNotEmpty())
                {
                    MergeUnitMonomial(_scaling, value);
                }
            }
            finally
            {
                _navigateToEquationStart.Add();
                AttachSExpressionValueChanged();
            }
        }
    }

    private static void MergeUnitMonomial(EquationControl eq, IEnumerable<Unit> units)
    {
        EquationCommands.NavigateToEquationStart.Execute(null, eq);
        if (SExpressionUtils.PlaceholderNode != eq.CurrentNodeSExpression)
        {
            EquationCommands.InsertOperator.Execute("MULTIPLICATION", eq);
            EquationCommands.NavigateToEquationStart.Execute(null, eq);
        }
        eq.InsertUnitMonomial(units);
    }

    public plotEquation GetDataObject()
    {
        bool isInPrinting = DocumentAttachedProperties.GetIsInPrinting(this);
        XmlNode expXml = GetExpXml();
        string text = isInPrinting ? Expression.EquationSExpressionWithResolvedLabels : Expression.EquationSExpression;
        IEquationControl userScalingExpression = GetUserScalingExpression();
        XmlNode regionXmlNode = userScalingExpression.GetRegionXmlNode();
        string scalingExpression = isInPrinting ? userScalingExpression.EquationSExpressionWithResolvedLabels : userScalingExpression.EquationSExpression;
        if (regionXmlNode == null || regionXmlNode.FirstChild == null || expXml == null || expXml.FirstChild == null)
        {
            DebugTools.DevAssert(false, null);
            return new plotEquation();
        }
        mathType[] math = new mathType[2]
        {
            new mathType
            {
                InnerXml = new XmlNode[1] { expXml.FirstChild },
                EquationSExpression = new string[1] { text }
            },
            new mathType
            {
                InnerXml = new XmlNode[1] { regionXmlNode.FirstChild },
                EquationSExpression = new string[1] { scalingExpression }
            }
        };
        return new plotEquation
        {
            math = math,
            formattingOverride = (MathFormat.AreAnyPropertiesSet ? MathFormat.GetDataObject() : null)
        };
    }

    public void NavigateRegionEnd()
    {
        if (!_scaling.IsKeyboardFocusWithin)
        {
            _scaling.Focus();
        }
        _scaling.NavigateRegionEnd();
    }

    public void NavigateRegionStart()
    {
        if (!_exp.IsKeyboardFocusWithin)
        {
            _exp.Focus();
        }
        _exp.NavigateRegionStart();
    }

    internal EquationControl _exp;

    internal ScaledEquationControl _scaling;

    private bool _contentLoaded;

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
        if (_contentLoaded)
        {
            return;
        }
        _contentLoaded = true;
        Uri resourceLocator = new Uri("/Quire.Controls.Plots;component/plotequation.xaml", UriKind.Relative);
        Application.LoadComponent(this, resourceLocator);
    }

    [DebuggerNonUserCode]
    internal Delegate _CreateDelegate(Type delegateType, string handler)
    {
        return Delegate.CreateDelegate(delegateType, this, handler);
    }

    [DebuggerNonUserCode]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
        switch (connectionId)
        {
        case 1:
            _exp = (EquationControl)target;
            break;
        case 2:
            _scaling = (ScaledEquationControl)target;
            break;
        default:
            _contentLoaded = true;
            break;
        }
    }

    double IPlotEquation.ActualWidth => ActualWidth;

    double IPlotEquation.ActualHeight => ActualHeight;
}
