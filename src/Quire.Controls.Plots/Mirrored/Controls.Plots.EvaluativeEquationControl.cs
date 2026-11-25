namespace Quire.Controls.Plots;

using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Xml;
using Quire.Calculation.Core;
using Quire.Controls;
using Quire.Controls.Core;
using Quire.Controls.Plots.ViewModel;
using Quire.Undo;
using Quire.Wpf;

public partial class EvaluativeEquationControl : UserControl, IEquationProxy, IEquationSerializationProxy, IComponentConnector
{
    private readonly EquationControlWithTransform _expressionTransform;
    private readonly EquationControlWithTransform _resultTransform;
    private SExpressionFormatter _formatter;
    public static readonly DependencyProperty IsUserSetValueProperty;
    public static readonly DependencyProperty EquationProxyProperty;
    public static readonly DependencyProperty RealResultValueProperty;
    public static readonly DependencyProperty IsActiveProperty;
    public static readonly RoutedEvent RequestRemoveEvent;
    public static readonly DependencyProperty ErrorProperty;
    public static readonly DependencyProperty DisplayNumberFormatProperty;
    internal EquationControl _equationControl;
    internal EquationControl _formattedResult;
    private bool _contentLoaded;

    static EvaluativeEquationControl()
    {
        IsUserSetValueProperty = DependencyProperty.Register("IsUserSetValue", typeof(bool?), typeof(EvaluativeEquationControl), (PropertyMetadata)new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, IsUserSetValueChanged));
        EquationProxyProperty = DependencyProperty.Register("EquationProxy", typeof(IEquationProxy), typeof(EvaluativeEquationControl));
        RealResultValueProperty = DependencyProperty.Register("RealResultValue", typeof(double), typeof(EvaluativeEquationControl), (PropertyMetadata)new FrameworkPropertyMetadata((object)double.NaN, (PropertyChangedCallback)RealResultValueChanged));
        IsActiveProperty = DependencyProperty.Register("IsActive", typeof(bool), typeof(EvaluativeEquationControl), (PropertyMetadata)new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, IsActiveChanged));
        RequestRemoveEvent = EventManager.RegisterRoutedEvent("RequestRemove", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(MarkersViewModel));
        ErrorProperty = DependencyProperty.Register("Error", typeof(EngineError), typeof(EvaluativeEquationControl), (PropertyMetadata)new FrameworkPropertyMetadata((object)null, (PropertyChangedCallback)ErrorChanged));
        double num = WpfUtils.PointsToDips(FontConstants.DefaultMathFontSizeInPoints);
        Control.FontSizeProperty.OverrideMetadata(typeof(EvaluativeEquationControl), (PropertyMetadata)new FrameworkPropertyMetadata((object)num, (PropertyChangedCallback)FontSizeChanged));
        DisplayNumberFormatProperty = EquationControl.DisplayNumberFormatProperty.AddOwner(typeof(EvaluativeEquationControl), (PropertyMetadata)new FrameworkPropertyMetadata(DisplayNumberFormatChanged));
    }

    public EvaluativeEquationControl()
    {
        InitializeComponent();
        CreateFormatter(null);
        EquationProxy = this;
        _expressionTransform = new EquationControlWithTransform(_equationControl);
        _resultTransform = new EquationControlWithTransform(_formattedResult);
        _formattedResult.Loaded += FixCollapsedEqOnLoaded;
        _equationControl.Loaded += FixCollapsedEqOnLoaded;
    }

    private static void FixCollapsedEqOnLoaded(object sender, RoutedEventArgs e)
    {
        UIElement uIElement = sender as UIElement;
        if (AssertionE.Failed == (AssertionE)DebugTools.DevAssert(uIElement != null, null))
        {
            return;
        }
        if (Visibility.Collapsed == uIElement.Visibility && !uIElement.IsArrangeValid && uIElement.IsMeasureValid)
        {
            uIElement.InvalidateMeasure();
        }
    }

    private void EquationRequestEvaluation(ICalculatable obj)
    {
        if (RequestEvaluation != null)
        {
            RequestEvaluation(this);
        }
    }

    private void EditingEquationChanged(object sender, RoutedEventArgs e)
    {
        if (!EnvironmentScope.IsCurrentlyInsideUndoOrRedo)
        {
            IsUserSetValue = true;
        }
    }

    private void FormattedResultChanged(object sender, RoutedEventArgs e)
    {
        e.Handled = true;
    }

    public event Action<IEquationProxy> RequestEvaluation;

    public XmlNode EquationXmlNode
    {
        get
        {
            return _equationControl.GetRegionXmlNode().FirstChild;
        }
        set
        {
            XmlNode xmlNode = value.OwnerDocument.CreateElement("math", "ws");
            xmlNode.AppendChild(value);
            _equationControl.SetRegionXmlNode(xmlNode);
            UpdateError();
        }
    }

    public string EquationSExpression
    {
        get
        {
            return _equationControl.EquationSExpression;
        }
        set
        {
            using (new CommandScope(new CompoundCommand()))
            {
                _equationControl.EquationSExpression = value;
            }
            UpdateError();
        }
    }

    private void UpdateError()
    {
        _equationControl.SetError(Error);
    }

    private string ResultSExpression
    {
        get
        {
            return _formattedResult.EquationSExpression;
        }
        set
        {
            using (new CommandScope(new CompoundCommand()))
            {
                _formattedResult.EquationSExpression = value;
            }
        }
    }

    public string Rl => string.Format("(eval {0})", _equationControl.Rl);



    public bool? IsUserSetValue
    {
        get
        {
            return (bool?)GetValue(IsUserSetValueProperty);
        }
        set
        {
            SetValue(IsUserSetValueProperty, value);
        }
    }

    private void SetNonUserSetSExpressionWithoutNotification(string sExpression)
    {
        if (IsUserSetValue.HasValue && !IsUserSetValue.Value)
        {
            _equationControl.Changed -= EditingEquationChanged;
            EquationSExpression = sExpression;
            _equationControl.NavigateRegionEnd();
            _equationControl.Changed += EditingEquationChanged;
        }
    }

    public bool IsCleared => _equationControl.IsEmpty;

    public bool IsCommitted
    {
        get
        {
            return _equationControl.IsCommitted;
        }
        set
        {
            if (value)
            {
                _equationControl.RequestEvaluation -= EquationRequestEvaluation;
                _equationControl.Commit();
                _equationControl.RequestEvaluation += EquationRequestEvaluation;
            }
        }
    }

    protected override void OnGotFocus(RoutedEventArgs e)
    {
        base.OnGotFocus(e);
        _equationControl.Focus();
    }

    protected override bool ShouldSerializeProperty(DependencyProperty dp)
    {
        if (EquationProxyProperty == dp && EquationProxy == this)
        {
            return false;
        }
        return base.ShouldSerializeProperty(dp);
    }

    public IEquationProxy EquationProxy
    {
        get
        {
            return (IEquationProxy)GetValue(EquationProxyProperty);
        }
        set
        {
            SetValue(EquationProxyProperty, value);
        }
    }



    public double RealResultValue
    {
        get
        {
            return (double)GetValue(RealResultValueProperty);
        }
        set
        {
            SetValue(RealResultValueProperty, value);
        }
    }



    private void UpdateEquationVisibility()
    {
        bool flag = IsActive || Error != null;
        _equationControl.Visibility = (flag ? Visibility.Visible : Visibility.Collapsed);
        _formattedResult.Visibility = (flag ? Visibility.Collapsed : Visibility.Visible);
    }

    public bool IsActive
    {
        get
        {
            return (bool)GetValue(IsActiveProperty);
        }
        set
        {
            SetValue(IsActiveProperty, value);
        }
    }



    public EngineError Error
    {
        get
        {
            return (EngineError)GetValue(ErrorProperty);
        }
        set
        {
            SetValue(ErrorProperty, value);
        }
    }

    private static void DisplayNumberFormatChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        EvaluativeEquationControl evaluativeEquationControl = d as EvaluativeEquationControl;
        DebugTools.DevAssert(evaluativeEquationControl != null, null);
        NumberFormat numberFormat = e.OldValue as NumberFormat;
        if (null != numberFormat)
        {
            numberFormat.FormatChanged -= evaluativeEquationControl.DisplayNumberFormatChanged;
        }
        NumberFormat numberFormat2 = e.NewValue as NumberFormat;
        if (null != numberFormat2)
        {
            numberFormat2.FormatChanged += evaluativeEquationControl.DisplayNumberFormatChanged;
        }
        evaluativeEquationControl.CreateFormatter(numberFormat2);
        evaluativeEquationControl.DisplayNumberFormatChanged(string.Empty, null);
    }

    private void DisplayNumberFormatChanged(string propertyName, object newValue)
    {
        FormatResult(RealResultValue);
    }

    private void CreateFormatter(NumberFormat numberFormat)
    {
        _formatter = new SExpressionFormatter(numberFormat ?? new NumberFormat());
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
        }
    }

    private void FormatResult(double result)
    {
        if (AssertionE.Failed == (AssertionE)DebugTools.DevAssert(_formatter != null, null))
        {
            return;
        }
        string text = _formatter.FormatReal(result);
        try
        {
            ResultSExpression = text;
        }
        catch
        {
            text = result.ToString();
        }
        SetNonUserSetSExpressionWithoutNotification(text);
    }



    private void EquationControlRequestFocus()
    {
        Focus();
    }

    private void EquationControlRequestRemove(IWhiteboardItem arg1, Direction arg2)
    {
        RaiseEvent(new RoutedEventArgs(RequestRemoveEvent));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "10.0.11.0")]
    public void InitializeComponent()
    {
        if (_contentLoaded)
        {
            return;
        }
        _contentLoaded = true;
        Uri resourceLocator = new Uri("/Quire.Controls.Plots;component/evaluativeequationcontrol.xaml", UriKind.Relative);
        Application.LoadComponent(this, resourceLocator);
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "10.0.11.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
        switch (connectionId)
        {
            case 1:
                _equationControl = (EquationControl)target;
                _equationControl.RequestFocus += EquationControlRequestFocus;
                _equationControl.RequestRemove += EquationControlRequestRemove;
                _equationControl.RequestEvaluation += EquationRequestEvaluation;
                _equationControl.Changed += EditingEquationChanged;
                break;
            case 2:
                _formattedResult = (EquationControl)target;
                _formattedResult.Changed += FormattedResultChanged;
                break;
            default:
                _contentLoaded = true;
                break;
        }
    }
}
