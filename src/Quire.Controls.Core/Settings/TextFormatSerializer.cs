#nullable disable
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Xml.Linq;
using Quire.Controls.Core.Serialization;

namespace Quire.Controls.Core.Settings;

public partial class TextFormatSerializer : XElementSerializable
{
    internal const string LocalName = "textStyle";

    protected ITextStyleObject _textFormat;

    public TextDecorationsToAttributesSerializer TextDecorationSerializer { get; set; }

    public string StyleKey { get; set; }

    public ITextStyleObject TextFormat
    {
        get
        {
            return _textFormat;
        }
        set
        {
            _textFormat = value;
            if (_textFormat is global::Quire.Core.TextFormat { Background: null } textFormat)
            {
                textFormat.Background = Brushes.Transparent;
            }
            TextDecorationSerializer = new TextDecorationsToAttributesSerializer(_textFormat.TextDecorations, Namespace);
        }
    }

    public TextFormatSerializer(ITextStyleObject textFormat, XNamespace ns, string styleKey)
        : base(ns + "textStyle", System.Array.Empty<object>())
    {
        StyleKey = styleKey;
        TextFormat = textFormat;
    }

    public override void ReadXml(XElement xElement)
    {
        base.ReadXml(xElement);
        TextDecorationsToAttributesSerializer textDecorationSerializer = TextDecorationSerializer;
        if (textDecorationSerializer?.TextDecorationsCollection?.Count > 0)
        {
            _textFormat.TextDecorations = TextDecorationSerializer.TextDecorationsCollection;
        }
    }

    protected override XElement GetThisElement(XElement xElement)
    {
        return xElement.Elements(Name).FirstOrDefault((XElement element) => element.Attribute("style-key")?.Value == StyleKey) ?? xElement.Elements(Name).FirstOrDefault((XElement element) => element.Attribute("name")?.Value == StyleKey);
    }

    protected override void CreateChildren(XElement thisElement)
    {
        if (thisElement != null && thisElement.Attribute("style-key") == null && thisElement.Attribute("name") != null)
        {
            thisElement.Add(new XAttribute("style-key", thisElement.Attribute("name").Value));
        }
        if (_textFormat is TextStyle textStyle)
        {
            List<object> list = new List<object>();
            list.Add(new PropertySerializableToXmlAttribute(_textFormat, "Name", "name"));
            list.Add(new PropertySerializableToXmlAttribute(_textFormat, "StyleKey", "style-key"));
            list.Add(new PropertySerializableToXmlAttribute(_textFormat, "BasedOnKey", "based-on-key"));
            if (textStyle.FontFamily != null || thisElement?.Attribute("font-family") != null)
            {
                list.Add(new PropertySerializableToXmlAttribute(_textFormat, "FontFamily", "font-family"));
            }
            if (textStyle.FontWeight.HasValue || thisElement?.Attribute("font-weight") != null)
            {
                list.Add(new PropertySerializableToXmlAttribute(_textFormat, "FontWeight", "font-weight"));
            }
            if (textStyle.FontSize.HasValue || thisElement?.Attribute("font-size") != null)
            {
                list.Add(new PropertySerializableToXmlAttribute(_textFormat, "FontSize", "font-size"));
            }
            if (textStyle.FontStyle.HasValue || thisElement?.Attribute("font-style") != null)
            {
                list.Add(new PropertySerializableToXmlAttribute(_textFormat, "FontStyle", "font-style"));
            }
            if (textStyle.TextDecorations != null || thisElement?.Attribute("underline") != null)
            {
                list.Add(new SerializableAsElementContents(TextDecorationSerializer));
            }
            if (textStyle.FontVariants.HasValue || thisElement?.Attribute("variants") != null)
            {
                list.Add(new PropertySerializableToXmlAttribute(_textFormat, "FontVariants", "variants"));
            }
            if (textStyle.Background != null || thisElement?.Attribute("background") != null)
            {
                list.Add(new PropertySerializableToXmlAttribute(_textFormat, "Background", "background"));
            }
            if (textStyle.Foreground != null || thisElement?.Attribute("foreground") != null)
            {
                list.Add(new PropertySerializableToXmlAttribute(_textFormat, "Foreground", "foreground"));
            }
            if (textStyle.Margin.HasValue || thisElement?.Attribute("margin") != null)
            {
                list.Add(new PropertySerializableToXmlAttribute(_textFormat, "Margin", "margin"));
            }
            if (textStyle.MarkerStyle.HasValue || thisElement?.Attribute("marker-style") != null)
            {
                list.Add(new PropertySerializableToXmlAttribute(_textFormat, "MarkerStyle", "marker-style"));
            }
            if (textStyle.TextIndent.HasValue || thisElement?.Attribute("text-indent") != null)
            {
                list.Add(new PropertySerializableToXmlAttribute(_textFormat, "TextIndent", "text-indent"));
            }
            if (textStyle.TextAlignment.HasValue || thisElement?.Attribute("text-align") != null)
            {
                list.Add(new PropertySerializableToXmlAttribute(_textFormat, "TextAlignment", "text-align"));
            }
            Children = list.ToArray();
        }
        else
        {
            Children = new object[15]
            {
                new PropertySerializableToXmlAttribute(_textFormat, "Name", "name"),
                new PropertySerializableToXmlAttribute(_textFormat, "FontFamily", "font-family"),
                new PropertySerializableToXmlAttribute(_textFormat, "FontSizeInPoints", "font-size"),
                new PropertySerializableToXmlAttribute(_textFormat, "FontWeight", "font-weight"),
                new PropertySerializableToXmlAttribute(_textFormat, "FontStyle", "font-style"),
                new SerializableAsElementContents(TextDecorationSerializer),
                new PropertySerializableToXmlAttribute(_textFormat, "Variants", "variants"),
                new PropertySerializableToXmlAttribute(_textFormat, "Background", "background"),
                new PropertySerializableToXmlAttribute(_textFormat, "Foreground", "foreground"),
                new PropertySerializableToXmlAttribute(_textFormat, "Margin", "margin"),
                new PropertySerializableToXmlAttribute(_textFormat, "BasedOnKey", "based-on-key"),
                new PropertySerializableToXmlAttribute(_textFormat, "StyleKey", "style-key"),
                new PropertySerializableToXmlAttribute(_textFormat, "MarkerStyle", "marker-style", new EnumConverter(typeof(TextMarkerStyle)), new SerializationInfo
                {
                    DefaultValue = "None",
                    Flags = SerializationFlags.DEFAULT_VALUE_FROM_PROPERTY
                }),
                new PropertySerializableToXmlAttribute(_textFormat, "TextIndent", "text-indent"),
                new PropertySerializableToXmlAttribute(_textFormat, "TextAlignment", "text-align")
            };
        }
    }
}
