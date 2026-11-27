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
        if (Title.GetRegionPersistentData(S11NProvider) is textType titleData)
        {
            XmlElement titleElement = xmlDocument.CreateElement("title", wsNameSpace);
            if (titleData.Any != null)
            {
                XmlNode newChild = xmlDocument.ImportNode(titleData.Any, deep: true);
                titleElement.AppendChild(newChild);
            }
            if (titleData.istextblock)
            {
                titleElement.SetAttribute("is-text-block", XmlConvert.ToString(titleData.istextblock));
            }
            if (!string.IsNullOrEmpty(titleData.itemidref))
            {
                titleElement.SetAttribute("item-idref", titleData.itemidref);
            }
            xmlElement.AppendChild(titleElement);
            titleData.PreSerialize();
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
        XmlAttribute displayAttribute = value.Attributes?["display"];
        if (displayAttribute != null)
        {
            ShowTitle = XmlConvert.ToBoolean(displayAttribute.Value);
        }
        XmlAttribute showBorderAttribute = value.Attributes?["show-border"];
        if (showBorderAttribute != null)
        {
            ShowBorder = XmlConvert.ToBoolean(showBorderAttribute.Value);
        }
        XmlAttribute verticalAttribute = value.Attributes?["is-vertical"];
        IsVertical = ((verticalAttribute != null) ? XmlConvert.ToBoolean(verticalAttribute.Value) : (TitleType == PlotTitleType.YAxis));
        if (Title != null)
        {
            XmlNamespaceManager mcdxNameSpaceManager = XmlWorksheetData.McdxNameSpaceManager;
            XmlNode xmlNode = value.SelectSingleNode(XmlQualifiedName.ToString("title", "ws"), mcdxNameSpaceManager);
            if (xmlNode == null)
            {
                return;
            }
            textType titleData = new textType();
            if (xmlNode.FirstChild != null)
            {
                titleData.Any = xmlNode.FirstChild as XmlElement;
            }
            XmlAttribute textBlockAttribute = xmlNode.Attributes?["is-text-block"];
            if (textBlockAttribute != null)
            {
                titleData.istextblock = XmlConvert.ToBoolean(textBlockAttribute.Value);
            }
            XmlAttribute itemIdAttribute = xmlNode.Attributes?["item-idref"];
            if (itemIdAttribute != null)
            {
                titleData.itemidref = itemIdAttribute.Value;
            }
            Title.SetRegionPersistentData(DeS11NProvider, titleData);
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
        plotTitleType titleData = (plotTitleType)data;
        ShowTitle = titleData.display;
        ShowBorder = titleData.showborder;
        if (!string.IsNullOrEmpty(titleData.titleposition) && Enum.TryParse<LegendPositions.PlotTitlePosition>(titleData.titleposition, out var result))
        {
            Position = result;
        }
        IsVertical = titleData.isvertical;
        if (Title != null && titleData.title != null)
        {
            Title.SetRegionPersistentData(d11NProvider, titleData.title);
        }
        return null;
    }

    public object GetRegionPersistentData(IS11NProvider s11NProvider)
    {
        plotTitleType titleData = new plotTitleType
        {
            display = ShowTitle,
            showborder = ShowBorder,
            titleposition = Position.ToString(),
            isvertical = IsVertical
        };
        if (Title != null)
        {
            textType title = Title.GetRegionPersistentData(s11NProvider) as textType;
            titleData.title = title;
        }
        return titleData;
    }

    public PlotTitleModel()
    {
    }
}
