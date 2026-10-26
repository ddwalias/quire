#nullable disable
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Linq;
using Quire.Controls.Core.Serialization;
using Quire.PersistentData;
using Quire.PersistentDataObjects;

namespace Quire.Controls.Core.Settings;

public partial class TextRenderingSerializer : XElementSerializable
{
    internal const string LocalName = "textRendering";

    public static readonly XNamespace Ns;

    private readonly TextStyleCollectionSerializable _textStyles = new TextStyleCollectionSerializable(Ns);

    public TextStyleCollectionSerializable TextStyles => _textStyles;

    public IWorksheetPersistentData WorksheetData { get; }

    private static XmlLanguageConverterWithDefaultValue XmlProofingLanguageConverter { get; }

    public TextRenderingSerializer(IWorksheetPersistentData worksheetData)
        : base(XName.Get("textRendering", Ns.NamespaceName), Array.Empty<object>())
    {
        WorksheetData = worksheetData;
        if (WorksheetData.TextStyles != null)
        {
            foreach (KeyValuePair<string, ITextStyleObject> textStyle in WorksheetData.TextStyles)
            {
                _textStyles.Add(textStyle.Key, textStyle.Value);
            }
        }
    }

    static TextRenderingSerializer()
    {
        Ns = "http://schemas.ptc.com/mathcad/settings/presentation10";
        PropertyInfo propertyInfo;
        try
        {
            propertyInfo = typeof(presentationTextRendering).GetProperty("ProofingLanguage");
        }
        catch (Exception ex)
        {
            propertyInfo = null;
            DebugTools.FailAssertion(ex);
        }
        string xmlAttributeName = XElementSerializable.GetXmlAttributeName(propertyInfo);
        if (xmlAttributeName != null)
        {
            DebugTools.DevAssert(XElementSerializable.GetXmlAttributeDefaultValue(propertyInfo, out var defaultVal), null);
            XmlProofingLanguageConverter = new XmlLanguageConverterWithDefaultValue(xmlAttributeName, defaultVal);
        }
    }

    protected override void CreateChildren(XElement thisElement)
    {
        List<object> list = new List<object> { TextStyles };
        PropertySerializableToXmlAttribute proofingLanguageSerializableAttribute = GetProofingLanguageSerializableAttribute(WorksheetData, "ProofingLanguage");
        if (proofingLanguageSerializableAttribute != null)
        {
            list.Add(proofingLanguageSerializableAttribute);
        }
        Children = list.ToArray();
    }

    private PropertySerializableToXmlAttribute GetProofingLanguageSerializableAttribute(IWorksheetPersistentData worksheetData, string propertyName)
    {
        if (DebugTools.AssertNotNull(XmlProofingLanguageConverter, null).Failed)
        {
            return null;
        }
        try
        {
            return new PropertySerializableToXmlAttribute(worksheetData, propertyName, XmlProofingLanguageConverter.AttributeName, XmlProofingLanguageConverter);
        }
        catch (Exception ex)
        {
            DebugTools.FailAssertion(ex);
            return null;
        }
    }
}
