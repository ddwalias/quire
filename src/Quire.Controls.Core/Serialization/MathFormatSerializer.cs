#nullable disable
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Quire;

namespace Quire.Controls.Core.Serialization;

public partial class MathFormatSerializer : XElementSerializable
{
    private static readonly XmlBooleanConverter converter = new XmlBooleanConverter();

    private bool _ignoreUnsetProperty = true;

    public MathFormat MathFormat { get; set; }

    public MathFormatSerializer(string localName, string nameSpace, MathFormat mathFormat, bool ignoreUnsetProperty = true)
        : base(XName.Get(localName, nameSpace))
    {
        MathFormat = mathFormat;
        _ignoreUnsetProperty = ignoreUnsetProperty;
    }

    protected override void CreateChildren(XElement thisElement = null)
    {
        IList<object> list = new List<object>();
        if (!_ignoreUnsetProperty || MathFormat.IsFontFamilyNameSet)
        {
            list.Add(new PropertySerializableToXmlAttribute(MathFormat, "FontFamilyName", "FontFamily"));
        }
        if (!_ignoreUnsetProperty || MathFormat.IsFontSizeSet)
        {
            list.Add(new PropertySerializableToXmlAttribute(MathFormat, "FontSize", "FontSize"));
        }
        if (!_ignoreUnsetProperty || MathFormat.IsBoldSet)
        {
            list.Add(new PropertySerializableToXmlAttribute(MathFormat, "IsBold", "Bold", converter));
        }
        if (!_ignoreUnsetProperty || MathFormat.IsItalicSet)
        {
            list.Add(new PropertySerializableToXmlAttribute(MathFormat, "IsItalic", "Italic", converter));
        }
        if (!_ignoreUnsetProperty || MathFormat.IsUnderlineSet)
        {
            list.Add(new PropertySerializableToXmlAttribute(MathFormat, "IsUnderline", "Underline", converter));
        }
        if (!_ignoreUnsetProperty || MathFormat.IsFontColorSet)
        {
            list.Add(new PropertySerializableToXmlAttribute(MathFormat, "FontColor", "ForegroundColor"));
        }
        if (!_ignoreUnsetProperty || MathFormat.IsHighlightColorSet)
        {
            list.Add(new PropertySerializableToXmlAttribute(MathFormat, "HighlightColor", "BackgroundColor"));
        }
        list.OfType<PropertySerializableToXmlAttribute>().Each((PropertySerializableToXmlAttribute item) =>
        {
            item.DontResetValue = true;
        });
        Children = new object[list.Count];
        list.CopyTo(Children, 0);
    }
}
