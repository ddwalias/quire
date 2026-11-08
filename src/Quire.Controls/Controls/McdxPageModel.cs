#nullable disable
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Linq;
using Quire.Controls.Core.Serialization;
using Quire.Mirror;
using Quire.PersistentData;
using Quire.PersistentDataObjects;

namespace Quire.Controls;

public partial class McdxPageModel
{
    [MirrorVerified("PtcControlsMcdxPageModelMechanicalDifferentialTests")]
    private static Dictionary<string, object> AttributeNameToDefaultValueDictionary { get; }

    static McdxPageModel()
    {
        AttributeNameToDefaultValueDictionary = new Dictionary<string, object>();
        PropertyInfo[] properties = typeof(presentationPageModel).GetProperties();
        foreach (PropertyInfo propertyInfo in properties)
        {
            if (XElementSerializable.GetXmlAttributeDefaultValue(propertyInfo, out var defaultVal))
            {
                AttributeNameToDefaultValueDictionary.Add(XElementSerializable.GetXmlAttributeName(propertyInfo), defaultVal);
            }
        }
    }

    public McdxPageModel(IWorksheetPersistentData worksheetData, XName name)
        : base(name, null)
    {
        Children = new object[21]
        {
            new PropertySerializableToXmlAttribute(worksheetData, "LayoutSize", "paper-code"),
            new PropertySerializableToXmlAttribute(worksheetData, "GridSize", "grid-size"),
            new PropertySerializableToXmlAttribute(worksheetData, "PageOrientation", "orientation"),
            new PropertySerializableToXmlAttribute(worksheetData, "MarginType", "margin-type"),
            new PropertySerializableToXmlAttribute(worksheetData, "StartingPageNumber", "starting-page-number"),
            new PropertySerializableToXmlAttribute(worksheetData, "TotalPagesOffset", "total-pages-offset"),
            new PropertySerializableToXmlAttribute(worksheetData, "PageMargin", "page-margin", new ThicknessNullableConverter()),
            GetPlotBackgroundSerializableAttribute(worksheetData),
            GetBooleanPropertySerializableAttribute(worksheetData, "DisplayGrid", "show-grid"),
            GetBooleanPropertySerializableAttribute(worksheetData, "IsSolveBlockLabelVisible", "is-solve-block-label-visible"),
            GetBooleanPropertySerializableAttribute(worksheetData, "DisplayHFGrid", "show-hf-grid"),
            new PropertySerializableToXmlAttribute(worksheetData, "PageBackgroundColor", "page-background-color", new ColorNullableConverter()),
            GetBooleanPropertySerializableAttribute(worksheetData, "DisplayFrame", "show-page-frame"),
            GetBooleanPropertySerializableAttribute(worksheetData, "DisplayHeaderFrame", "show-header-frame"),
            GetBooleanPropertySerializableAttribute(worksheetData, "DisplayFooterFrame", "show-footer-frame"),
            GetBooleanPropertySerializableAttribute(worksheetData, "UseMajorGridLines", "useMajorGridLines"),
            GetBooleanPropertySerializableAttribute(worksheetData, "UseHFMajorGridLines", "useHFMajorGridLines"),
            GetBooleanPropertySerializableAttribute(worksheetData, "ShowIOTags", "show-input-output-tag"),
            GetBooleanPropertySerializableAttribute(worksheetData, "OleObjectAutoResize", "ole-object-preserve-aspect-ratio"),
            GetBooleanPropertySerializableAttribute(worksheetData, "SpellCheckEnabled", "spell-check-enabled"),
            GetBooleanPropertySerializableAttribute(worksheetData, "IsFirstPageHeaderFooterDifferent", "is-first-page-header-footer-different")
        };
    }

    private PropertySerializableToXmlAttribute GetPlotBackgroundSerializableAttribute(IWorksheetPersistentData worksheetData)
    {
        return new PropertySerializableToXmlAttribute(worksheetData, "PlotBackgroundType", "plot-background-type", new PlotBackgroundTypeToXmlValueConverter("plot-background-type", AttributeNameToDefaultValueDictionary));
    }

    private PropertySerializableToXmlAttribute GetBooleanPropertySerializableAttribute(IWorksheetPersistentData worksheetData, string propertyName, string attributeName)
    {
        return new PropertySerializableToXmlAttribute(worksheetData, propertyName, attributeName, new BooleanConverterWithDefaultValue(attributeName, AttributeNameToDefaultValueDictionary));
    }
}
