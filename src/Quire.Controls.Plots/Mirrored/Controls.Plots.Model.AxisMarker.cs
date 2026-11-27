using System.ComponentModel;
using System.Globalization;
using System.Xml;
using Quire.Calculation.Core;
using Quire.Controls.Plots.Presenter;
using Quire.PersistentDataObjects;
using Quire.PlotDataObjects;

namespace Quire.Controls.Plots.Model;

public partial class AxisMarker : BaseMarker, IAxisMarker, IBaseMarker, INotifyPropertyChanged
{
    public static partial class PropertyNames
    {
        public const string Value = "Value";

        public const string LineStyle = "LineStyle";

        public const string LineWeight = "LineWeight";
    }

    private double _value;

    private LineStyle _lineStyle;

    private double _lineWeight;

    private readonly RealNumberCalculatable _realNumberCalculatable = new RealNumberCalculatable();

    public double Value
    {
        get
        {
            return _value;
        }
        set
        {
            if (value != _value)
            {
                _value = value;
                OnPropertyChanged("Value");
            }
        }
    }

    public LineStyle LineStyle
    {
        get
        {
            return _lineStyle;
        }
        set
        {
            if (value != _lineStyle)
            {
                _lineStyle = value;
                OnPropertyChanged("LineStyle");
            }
        }
    }

    public double LineWeight
    {
        get
        {
            return _lineWeight;
        }
        set
        {
            if (value != _lineWeight)
            {
                _lineWeight = value;
                OnPropertyChanged("LineWeight");
            }
        }
    }

    public RealNumberCalculatable RealNumberCalculatable => _realNumberCalculatable;

    public AxisMarker()
    {
        Calculatable = RealNumberCalculatable;
        RealNumberCalculatable.PropertyChanged += CalculatablePropertyChanged;
    }

    public AxisMarker(axisBaseTypeMarker dataObject, IResultManager resultManager)
        : this()
    {
        LineStyle = PlotDataObjectCreator.ConvertLineStyle(dataObject.linestyle);
        LineWeight = dataObject.lineweight;
        Color = TraceColorTable.parseColor(dataObject.color);
        ShowCallout = dataObject.display;
        if (dataObject.InnerXml != null && dataObject.InnerXml.Length != 0)
        {
            DebugTools.DevAssert(dataObject.InnerXml.Length == 1, null);
            ExpressionXml = dataObject.InnerXml[0];
        }
        else if (dataObject.EquationSExpression != null && dataObject.EquationSExpression.Length != 0)
        {
            DebugTools.DevAssert(dataObject.EquationSExpression.Length == 1, null);
            EquationSExpression = dataObject.EquationSExpression[0];
        }
        if (dataObject.valueSpecified)
        {
            Value = dataObject.value;
        }
        resultManager.SetResultIfRefIdSpecified(dataObject.resultRef, dataObject.resultRefSpecified, Calculatable);
    }

    public axisBaseTypeMarker GetDataObject(IResultManager resultManager)
    {
        int num = ((IsUserSetValue && resultManager != null) ? resultManager.CreateResultRefId(RealNumberCalculatable) : (-1));
        axisBaseTypeMarker markerData = new axisBaseTypeMarker
        {
            linestyle = PlotDataObjectCreator.ConvertLineStyle(LineStyle),
            lineweight = LineWeight,
            color = Color.ToString(CultureInfo.InvariantCulture),
            display = ShowCallout,
            value = Value,
            valueSpecified = true,
            resultRef = num,
            resultRefSpecified = (num != -1)
        };
        if (IsUserSetValue)
        {
            markerData.InnerXml = new XmlNode[1] { ExpressionXml };
            markerData.EquationSExpression = new string[1] { EquationSExpression };
        }
        else
        {
            markerData.InnerXml = null;
            markerData.EquationSExpression = null;
        }
        return markerData;
    }

    private void CalculatablePropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == "Value")
        {
            Value = RealNumberCalculatable.Value;
        }
    }
}
