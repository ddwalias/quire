using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Xml;
using Quire.Controls.Text;
using Quire.PersistentDataObjects;
using Quire.Serialization;
using Quire.Xml;

namespace Quire.Controls.Plots.Model;

internal partial class PlotTitleModel
{
    private LegendPositions.PlotTitlePosition _position = LegendPositions.PlotTitlePosition.PlotBoundaryTop;
    private bool _showTitle;
    private bool _isDirty;
    private bool _showBorder;
    private bool _isVertical;

    public RichTextTitle Title { get; set; }
    public PlotTitleType TitleType { get; set; }

    public bool IsDirty
    {
        get
        {
            if (!_isDirty)
            {
                return Title.IsDirty;
            }
            return true;
        }
        set
        {
            _isDirty = value;
        }
    }

    public bool ShowTitle
    {
        get => _showTitle;
        set
        {
            if (_showTitle == value) return;
            _showTitle = value;
            OnPropertyChanged("ShowTitle");
        }
    }

    public LegendPositions.PlotTitlePosition Position
    {
        get => _position;
        set
        {
            if (_position == value) return;
            _position = value;
            OnPropertyChanged("Position");
        }
    }

    public bool IsVertical
    {
        get => _isVertical;
        set
        {
            if (_isVertical == value) return;
            _isVertical = value;
            OnPropertyChanged("IsVertical");
        }
    }

    public IS11NProvider S11NProvider { get; set; }
    public IDeS11NProvider DeS11NProvider { get; set; }

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

    public XmlNode RegionXmlNode
    {
        get => GetXmlNode(XmlWorksheetData.McdxNameSpaceManager.LookupNamespace(string.Empty));
        set => SetXmlNode(value);
    }

    private XmlNode GetXmlNode(string wsNameSpace)
    {
        XmlDocument xmlDocument = new XmlDocument();
        string qualifiedName = ((TitleType == PlotTitleType.Plot) ? "plotTitle" : "axisLabel");
        XmlElement xmlElement = xmlDocument.CreateElement(qualifiedName, wsNameSpace);
        xmlElement.SetAttribute("display", XmlConvert.ToString(ShowTitle));
        xmlElement.SetAttribute("show-border", XmlConvert.ToString(ShowBorder));
        xmlElement.SetAttribute("title-position", Position.ToString());
        xmlElement.SetAttribute("is-vertical", XmlConvert.ToString(IsVertical));
        if (Title == null || !IsDirty)
        {
            return xmlElement;
        }
        if (S11NProvider is IRegionType regionType)
        {
            S11NProvider.Helper.AssignNextRegionId(Title);
            regionType.regionid = S11NProvider.Helper.GetRegionId(Title).ToString();
        }
        if (Title.GetRegionPersistentData(S11NProvider) is textType textType2)
        {
            XmlElement xmlElement2 = xmlDocument.CreateElement("title", wsNameSpace);
            if (textType2.Any != null)
            {
                XmlNode newChild = xmlDocument.ImportNode(textType2.Any, deep: true);
                xmlElement2.AppendChild(newChild);
            }
            if (textType2.istextblock)
            {
                xmlElement2.SetAttribute("is-text-block", XmlConvert.ToString(textType2.istextblock));
            }
            if (!string.IsNullOrEmpty(textType2.itemidref))
            {
                xmlElement2.SetAttribute("item-idref", textType2.itemidref);
            }
            xmlElement.AppendChild(xmlElement2);
            textType2.PreSerialize();
        }
        return xmlElement;
    }

    private void SetXmlNode(XmlNode value)
    {
        XmlAttribute xmlAttribute = value.Attributes?["title-position"];
        if (xmlAttribute != null)
        {
            Position = (LegendPositions.PlotTitlePosition)Enum.Parse(typeof(LegendPositions.PlotTitlePosition), xmlAttribute.Value);
        }
        XmlAttribute xmlAttribute2 = value.Attributes?["display"];
        if (xmlAttribute2 != null)
        {
            ShowTitle = XmlConvert.ToBoolean(xmlAttribute2.Value);
        }
        XmlAttribute xmlAttribute3 = value.Attributes?["show-border"];
        if (xmlAttribute3 != null)
        {
            ShowBorder = XmlConvert.ToBoolean(xmlAttribute3.Value);
        }
        XmlAttribute xmlAttribute4 = value.Attributes?["is-vertical"];
        IsVertical = ((xmlAttribute4 != null) ? XmlConvert.ToBoolean(xmlAttribute4.Value) : (TitleType == PlotTitleType.YAxis));
        if (Title != null)
        {
            XmlNamespaceManager mcdxNameSpaceManager = XmlWorksheetData.McdxNameSpaceManager;
            XmlNode xmlNode = value.SelectSingleNode(XmlQualifiedName.ToString("title", "ws"), mcdxNameSpaceManager);
            if (xmlNode == null)
            {
                return;
            }
            textType textType2 = new textType();
            if (xmlNode.FirstChild != null)
            {
                textType2.Any = xmlNode.FirstChild as XmlElement;
            }
            XmlAttribute xmlAttribute5 = xmlNode.Attributes?["is-text-block"];
            if (xmlAttribute5 != null)
            {
                textType2.istextblock = XmlConvert.ToBoolean(xmlAttribute5.Value);
            }
            XmlAttribute xmlAttribute6 = xmlNode.Attributes?["item-idref"];
            if (xmlAttribute6 != null)
            {
                textType2.itemidref = xmlAttribute6.Value;
            }
            Title.SetRegionPersistentData(DeS11NProvider, textType2);
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public IList<IRegionPersistentData> SetRegionPersistentData(IDeS11NProvider d11NProvider, object data)
    {
        if (data == null)
        {
            return null;
        }
        if (DebugTools.DevAssert(data is plotTitleType, "Data should be of type plotTitleType").Failed)
        {
            return null;
        }
        plotTitleType plotTitleType2 = (plotTitleType)data;
        ShowTitle = plotTitleType2.display;
        ShowBorder = plotTitleType2.showborder;
        if (!string.IsNullOrEmpty(plotTitleType2.titleposition) && Enum.TryParse<LegendPositions.PlotTitlePosition>(plotTitleType2.titleposition, out var result))
        {
            Position = result;
        }
        IsVertical = plotTitleType2.isvertical;
        if (Title != null && plotTitleType2.title != null)
        {
            Title.SetRegionPersistentData(d11NProvider, plotTitleType2.title);
        }
        return null;
    }

    public object GetRegionPersistentData(IS11NProvider s11NProvider)
    {
        plotTitleType plotTitleType2 = new plotTitleType
        {
            display = ShowTitle,
            showborder = ShowBorder,
            titleposition = Position.ToString(),
            isvertical = IsVertical
        };
        if (Title != null)
        {
            textType title = Title.GetRegionPersistentData(s11NProvider) as textType;
            plotTitleType2.title = title;
        }
        return plotTitleType2;
    }

    public PlotTitleModel()
    {
    }
}
