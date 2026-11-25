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
using Quire.PersistentDataObjects;
using Quire.Wpf;
using Quire.Xml;

namespace Quire.Controls.Plots;

public partial class PlotEquationWOUnit : IPlotEquation
{
    private EngineError _plotCalculatableError;

    public event Action<IPlotEquation, Direction> RequestRemove;

    public event Action RequestEvaluation;

    private readonly EquationControl _scaling = new EquationControl
    {
        AllowExpressionsOnly = true
    };

    private static readonly Type _typeOfThis;

    public static readonly DependencyProperty CalcIsDisabledProperty;

    public static readonly DependencyProperty DisplayNumberFormatProperty;

    public PlotEquationWOUnit()
    {
        InitializeComponent();
        MathFormatDobjController mathFormat = new MathFormatDobjController(this);
        new MathFormatDobjController(_exp).SetPropertiesFrom(mathFormat);
        new MathFormatDobjController(_scaling).SetPropertiesFrom(mathFormat);
        _exp.RequestEvaluation += OnRequestEvaluation;
        _exp.PreviewGotKeyboardFocus += OnPreviewGotKeyboardFocus;
        AttachSExpressionValueChanged();
        BindCommands();
        ActiveEquation = _exp;
        AddHandler(CommonRoutedEvents.TraversedFocusEvent, new CustomRoutedEventHandler<FocusNavigationDirection>(OnTraversedFocusEvent), handledEventsToo: true);
    }

    static PlotEquationWOUnit()
    {
        _typeOfThis = typeof(PlotEquationWOUnit);
        CalcIsDisabledProperty = EquationControl.CalcIsDisabledProperty.AddOwner(_typeOfThis);
        DisplayNumberFormatProperty = EquationControl.DisplayNumberFormatProperty.AddOwner(_typeOfThis);
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

    private void BindCommands()
    {
        WpfUtils.AddPreemptiveCommandBinding(this, EquationCommands.Delete, HandleDelete, null, bindingMarksEventAsHandled: false);
        WpfUtils.AddPreemptiveCommandBinding(this, EquationCommands.Backspace, HandleBackspace, null, bindingMarksEventAsHandled: false);
        WpfUtils.AddPreemptiveCommandBinding(this, EquationCommands.NavigateToEquationEnd, HandleNavigateToEquationEnd, null, bindingMarksEventAsHandled: true);
        WpfUtils.AddPreemptiveCommandBinding(this, EquationCommands.NavigateToEquationStart, HandleNavigateToEquationStart, null, bindingMarksEventAsHandled: true);
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
        if (sender is PlotEquationWOUnit && IsPlotEquationEmpty)
        {
            FireRequestRemove(this, Direction.Left);
            e.Handled = true;
        }
    }

    private void HandleDelete(object sender, ExecutedRoutedEventArgs e)
    {
        if (sender is PlotEquationWOUnit && IsPlotEquationEmpty)
        {
            FireRequestRemove(this, Direction.Right);
            e.Handled = true;
        }
    }

    private void OnRequestEvaluation(ICalculatable calculatable)
    {
        if (!IsCommitted)
        {
            Commit();
        }
    }

    public string EquationSExpression
    {
        get
        {
            return string.Format("(@TRACE_EXPR {0})", _exp.EquationSExpression);
        }
        set
        {
            Regex regex = new Regex("^\\(@TRACE_EXPR (?<Exp>.*)\\)$");
            Match match = regex.Match(value);
            if (match.Success)
            {
                string equationSExpression = match.Result("${Exp}");
                _exp.EquationSExpression = equationSExpression;
                return;
            }
            throw new FormatException("The SExp is not in a correct format. the format should be:(@TRACE_EXPR @PLACEHOLDER )");
        }
    }

    [Obsolete("Usage of Exp property is deprecated. Use Expression instead.")]
    public EquationControl Exp => _exp;

    public override IEquationControl Expression => _exp;

    public IWhiteboardItem ExpressionItem => _exp;

    [Obsolete("Scaling EquationControl should not be exposed directly.")]
    public EquationControl Scaling => _scaling;

    public IWhiteboardItem ScalingItem => _scaling;

    public bool IsExpEmpty => Expression.EquationSExpression == SExpressionUtils.PlaceholderNode;

    public bool IsPlotEquationEmpty => IsExpEmpty;

    public void SetRegionXmlNode(XmlNode value)
    {
        if (value.ChildNodes.Count > 1)
        {
            XmlNode xmlNode = value.ChildNodes[1];
            if (DebugTools.DevAssert(xmlNode.Name == "formattingOverride", null).Passed)
            {
                using XmlNodeReader reader = new XmlNodeReader(xmlNode);
                new MathFormatSerializer("formattingOverride", xmlNode.NamespaceURI, MathFormat, ignoreUnsetProperty: false).ReadXml(reader);
            }
        }
        SetExpressionXmlNode(value.ChildNodes[0]);
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
        xmlElement.AppendChild(xmlDocument.ImportNode(Expression.GetRegionXmlNode(), deep: true));
        if (MathFormat.AreAnyPropertiesSet)
        {
            using XmlNodeWriter writer = new XmlNodeWriter(xmlElement);
            new MathFormatSerializer("formattingOverride", xmlElement.NamespaceURI, MathFormat).WriteXml(writer);
        }
        return xmlElement;
    }

    public plotEquation GetDataObject()
    {
        XmlNode regionXmlNode = Expression.GetRegionXmlNode();
        string equationSExpression = Expression.EquationSExpression;
        if (regionXmlNode == null || regionXmlNode.FirstChild == null)
        {
            DebugTools.DevAssert(false, null);
            return new plotEquation();
        }
        mathType[] math = new mathType[1]
        {
            new mathType
            {
                InnerXml = new XmlNode[1] { regionXmlNode.FirstChild },
                EquationSExpression = new string[1] { equationSExpression }
            }
        };
        return new plotEquation
        {
            math = math,
            formattingOverride = (MathFormat.AreAnyPropertiesSet ? MathFormat.GetDataObject() : null)
        };
    }

    public string EmptyPlotEquationSExp => "(@TRACE_EXPR @PLACEHOLDER )";

    public void SetDataObject(plotEquation equationData)
    {
        if (AssertionE.Failed == (AssertionE)DebugTools.DevAssert(equationData.math.Length > 0, null))
        {
            return;
        }
        equationData.math[0].SetXmlNodeOrSExpression(SetExpressionXmlNode, SetExpressionSExpression);
        if (equationData.formattingOverride != null)
        {
            MathFormat.SetDataObject(equationData.formattingOverride);
        }
    }

    public bool AtLeftEdge => _exp.AtLeftEdge;

    public void NavigateRegionEnd()
    {
        if (!_exp.IsKeyboardFocusWithin)
        {
            _exp.Focus();
        }
        _exp.NavigateRegionEnd();
    }

    public void NavigateRegionStart()
    {
        if (!_exp.IsKeyboardFocusWithin)
        {
            _exp.Focus();
        }
        _exp.NavigateRegionStart();
    }

    public string RegionErrorSExp
    {
        get
        {
            if (_exp.ErrorSExpression != string.Empty)
            {
                DebugTools.DevAssert(false, null);
            }
            if (_exp.ErrorSExpression != string.Empty)
            {
                return _exp.ErrorSExpression;
            }
            return string.Empty;
        }
    }

    public string RegionErrorCode
    {
        get
        {
            if (_exp.ErrorCode != string.Empty)
            {
                DebugTools.DevAssert(false, null);
            }
            if (_exp.ErrorCode != string.Empty)
            {
                return _exp.ErrorCode;
            }
            return string.Empty;
        }
    }

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
        Expression.RequestEvaluation -= OnRequestEvaluation;
        Expression.Commit();
        Expression.RequestEvaluation += OnRequestEvaluation;
        IsCommitted = true;
    }

    public EquationControl ActiveEquation { get; private set; }

    public IEquationControl FocusedEquation
    {
        get
        {
            if (!_exp.IsKeyboardFocusWithin)
            {
                return null;
            }
            return _exp;
        }
    }

    public SynchronizationStatus SynchronizationStatus => _exp.SynchronizationStatus;

    public CalculatingStatus CalculatingStatus => _exp.CalculatingStatus;

    public IList<Unit> Units
    {
        set
        {
            if (!(value == null ? false : value.Count < 1))
            {
                throw new PlotInvalidResultException("PlotControl.Error.UnitsAreNotSupportedInRange");
            }
        }
    }

    public override IEquationControl ScalingEquation => _scaling;

    public int RegionErrorCount => _exp.ErrorCount;

    private void FireRequestEvaluation()
    {
        if (RequestEvaluation != null)
        {
            RequestEvaluation();
        }
    }

    private void FireRequestRemove(PlotEquationWOUnit plotEquation, Direction direction)
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

    public string ScalingRl => "(number 1)";

    private RubberBandHighlighter _errorFrame;

    public EngineError PlotCalculatableError
    {
        set
        {
            if (_plotCalculatableError != value)
            {
                _plotCalculatableError = value;
                if (value == null)
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
        }
        get
        {
            return _plotCalculatableError;
        }
    }

    public override IEnumerable<IEquationControl> ContainedEquationControls
    {
        get
        {
            yield return _exp;
        }
    }

    internal EquationControl _exp;

    private bool _contentLoaded;

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
        if (_contentLoaded)
        {
            return;
        }
        _contentLoaded = true;
        Uri resourceLocator = new Uri("/Quire.Controls.Plots;component/plotequationwounit.xaml", UriKind.Relative);
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
        if (connectionId == 1)
        {
            _exp = (EquationControl)target;
        }
        else
        {
            _contentLoaded = true;
        }
    }

    double IPlotEquation.ActualWidth => ActualWidth;

    double IPlotEquation.ActualHeight => ActualHeight;
}
