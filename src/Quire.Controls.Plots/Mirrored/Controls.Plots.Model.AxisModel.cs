#nullable disable
#pragma warning disable

using System.Collections.Generic;
using Quire.Controls.Plots.Presenter;

namespace Quire.Controls.Plots.Model;

internal partial class AxisModel
{
    public IPlotTitleModel AxisTitleModel { get; set; }

    public AxisMarkersCollection Markers => _markers;

    public IEnumerable<IEquationItem> Equations => _equations;

    public bool CanHaveMarkers => !IsDiscrete;

    public bool XIsString { get; set; }

    public bool YIsString { get; set; }

    public bool IsDiscrete { get; set; }

    public double LegendWidth { get; set; }

    public string AxisXmlNodeName
    {
        get
        {
            switch (Type)
            {
                case AxesType.X:
                    return "xAxis";
                case AxesType.Y:
                    return "yAxis";
                case AxesType.Z:
                    return "zAxis";
                case AxesType.Angular:
                    return "angularAxis";
                case AxesType.Radial:
                    return "radialAxis";
                default:
                    return "";
            }
        }
    }

    public string DomainXmlNodeName
    {
        get
        {
            switch (Type)
            {
                case AxesType.X:
                    return "xyDomain";
                case AxesType.Y:
                    return "xyDomain";
                case AxesType.Angular:
                    return "angularDomain";
                case AxesType.Radial:
                    return "radialDomain";
                case AxesType.Z:
                    return "zDomain";
                default:
                    return "";
            }
        }
    }
}
