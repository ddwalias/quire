using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Xml;
using Quire.Controls.Plots.Presenter;
using Quire.PersistentDataObjects;
using Quire.Serialization;
using Quire.Xml;

namespace Quire.Controls.Plots.Model;

internal partial class PlotLegendModel
{
    private bool _isDirty;
    private bool _showBorder;
    private LegendPositions.PlotLegendPosition _position;
    private bool _showPlotLegend;
    private double _legendWidth = double.NaN;
    private readonly IList<IPlotLegendItem> _legendItems = new List<IPlotLegendItem>();

    public PlotLegendModel(PlotType plotType)
    {
        Position = LegendPositions.PlotLegendPosition.Below;
        PlotType = plotType;
    }

    public PlotType PlotType { get; }

    public bool IsDirty
    {
        get => _isDirty;
        set => _isDirty = value;
    }

    public bool ShowPlotLegend
    {
        get => _showPlotLegend;
        set
        {
            if (_showPlotLegend == value) return;
            _showPlotLegend = value;
            OnPropertyChanged("ShowPlotLegend");
        }
    }

    public LegendPositions.PlotLegendPosition Position
    {
        get => _position;
        set
        {
            if (_position == value) return;
            _position = value;
            OnPropertyChanged("Position");
        }
    }

    public bool ShowBorder
    {
        get => _showBorder;
        set
        {
            if (_showBorder == value) return;
            _showBorder = value;
            OnPropertyChanged("ShowBorder");
        }
    }

    public double LegendWidth
    {
        get => _legendWidth;
        set
        {
            if (_legendWidth.Equals(value)) return;
            _legendWidth = value;
            OnPropertyChanged("LegendWidth");
        }
    }

    public IEnumerable<IPlotLegendItem> PlotLegendItems => _legendItems;

    public event PropertyChangedEventHandler PropertyChanged;

    private void OnPropertyChanged(string propertyName)
    {
        if (PropertyChanged != null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public void AddLegendItem(IPlotLegendItem legendItem, int index)
    {
        _legendItems.Insert(index, legendItem);
    }

    public void RemoveLegendItem(IPlotLegendItem legendItem)
    {
        _legendItems.Remove(legendItem);
    }

    public IS11NProvider S11NProvider { get; set; }

    public IDeS11NProvider DeS11NProvider { get; set; }

    public IList<IRegionPersistentData> SetRegionPersistentData(IDeS11NProvider d11NProvider, object data)
    {
        if (data == null)
        {
            return null;
        }
        if (DebugTools.DevAssert(data is plotBaseTypeLegend, "Data should be of type plotBaseTypeLegend").Failed)
        {
            return null;
        }
        plotBaseTypeLegend plotBaseTypeLegend2 = (plotBaseTypeLegend)data;
        ShowPlotLegend = plotBaseTypeLegend2.display;
        ShowBorder = plotBaseTypeLegend2.showborder;
        Position = (Enum.TryParse<LegendPositions.PlotLegendPosition>(plotBaseTypeLegend2.position.ToString(), ignoreCase: true, out var result) ? result : LegendPositions.PlotLegendPosition.Below);
        LegendWidth = plotBaseTypeLegend2.legendWidth;
        return null;
    }

    public object GetRegionPersistentData(IS11NProvider s11NProvider)
    {
        plotBaseTypeLegendPosition result;
        return new plotBaseTypeLegend
        {
            display = ShowPlotLegend,
            showborder = ShowBorder,
            position = (Enum.TryParse<plotBaseTypeLegendPosition>(Position.ToString(), ignoreCase: true, out result) ? result : plotBaseTypeLegendPosition.below),
            legendWidth = LegendWidth
        };
    }

    private static string ConvertToPlotBaseTypeLegendPosition(LegendPositions.PlotLegendPosition position)
    {
        switch (position)
        {
            case LegendPositions.PlotLegendPosition.Below:
                return "below";
            case LegendPositions.PlotLegendPosition.TopLeft:
                return "top-left";
            case LegendPositions.PlotLegendPosition.TopRight:
                return "top-right";
            case LegendPositions.PlotLegendPosition.BottomLeft:
                return "bottom-left";
            case LegendPositions.PlotLegendPosition.BottomRight:
                return "bottom-right";
            default:
                throw new ArgumentOutOfRangeException("PlotLegendPosition");
        }
    }

    private static bool TryConvertToPlotLegendPosition(string value, out LegendPositions.PlotLegendPosition position)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            position = LegendPositions.PlotLegendPosition.Below;
            return false;
        }
        switch (value.Trim().ToLowerInvariant())
        {
            case "below":
                position = LegendPositions.PlotLegendPosition.Below;
                return true;
            case "top-left":
                position = LegendPositions.PlotLegendPosition.TopLeft;
                return true;
            case "top-right":
                position = LegendPositions.PlotLegendPosition.TopRight;
                return true;
            case "bottom-left":
                position = LegendPositions.PlotLegendPosition.BottomLeft;
                return true;
            case "bottom-right":
                position = LegendPositions.PlotLegendPosition.BottomRight;
                return true;
            default:
                throw new ArgumentOutOfRangeException("plotBaseTypeLegendPosition");
        }
    }

    public XmlNode RegionXml
    {
        get
        {
            XmlDocument xmlDocument = new XmlDocument();
            string namespaceURI = XmlWorksheetData.McdxNameSpaceManager.LookupNamespace(string.Empty);
            XmlElement xmlElement = xmlDocument.CreateElement("legend", namespaceURI);
            xmlElement.SetAttribute("display", XmlConvert.ToString(ShowPlotLegend));
            xmlElement.SetAttribute("show-border", XmlConvert.ToString(ShowBorder));
            xmlElement.SetAttribute("position", ConvertToPlotBaseTypeLegendPosition(Position));
            if (!double.IsNaN(LegendWidth))
            {
                xmlElement.SetAttribute("legendWidth", LegendWidth.ToString(CultureInfo.InvariantCulture));
            }
            return xmlElement;
        }
        set
        {
            if (value == null)
            {
                return;
            }
            XmlAttribute xmlAttribute = value.Attributes?["display"];
            if (xmlAttribute != null && bool.TryParse(xmlAttribute.Value, out var result))
            {
                ShowPlotLegend = result;
            }
            XmlAttribute xmlAttribute2 = value.Attributes?["show-border"];
            if (xmlAttribute2 != null && bool.TryParse(xmlAttribute2.Value, out var result2))
            {
                ShowBorder = result2;
            }
            XmlAttribute xmlAttribute3 = value.Attributes?["position"];
            if (xmlAttribute3 != null && TryConvertToPlotLegendPosition(xmlAttribute3.Value, out var position))
            {
                Position = position;
            }
            XmlAttribute xmlAttribute4 = value.Attributes?["legendWidth"];
            if (xmlAttribute4 != null && double.TryParse(xmlAttribute4.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out var result3))
            {
                LegendWidth = result3;
            }
        }
    }
}
