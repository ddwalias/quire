#nullable disable
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Quire.Controls.Core.Serialization;

namespace Quire.Controls.Core.Settings;

public partial class TextStyleCollectionSerializable : XElementSerializable
{
    internal const string LocalName = "textStyles";

    private readonly Dictionary<string, TextFormatSerializer> _styles = new Dictionary<string, TextFormatSerializer>();

    public TextStyleCollectionSerializable(XNamespace Ns)
        : base(Ns + "textStyles", System.Array.Empty<object>())
    {
    }

    public Dictionary<string, ITextStyleObject> ToTextFormats()
    {
        return _styles.ToDictionary(x => x.Key, x => x.Value.TextFormat);
    }

    public ITextStyleObject this[string styleName]
    {
        get
        {
            return _styles[styleName]?.TextFormat;
        }
        set
        {
            _styles[styleName] = new TextFormatSerializer(value, Namespace, styleName);
        }
    }

    public void Add(string key, ITextStyleObject value)
    {
        if (!_styles.ContainsKey(value.StyleKey))
        {
            if (value.StyleKey.IsNullOrEmpty())
            {
                value.StyleKey = key;
            }
            if (value.Name.IsNullOrEmpty())
            {
                value.Name = value.StyleKey;
            }
            _styles.Add(value.StyleKey, new TextFormatSerializer(value, Namespace, value.StyleKey));
        }
    }

    protected override void CreateChildren(XElement thisElement)
    {
        foreach (XElement item in thisElement.Descendants())
        {
            string text = ((item.Attribute("style-key") != null) ? item.Attribute("style-key")?.Value : item.Attribute("name")?.Value);
            switch (text)
            {
                case "headerLayoutStyle":
                case "footerLayoutStyle":
                case "areaLayoutStyle":
                    Add(text, new global::Quire.Core.TextFormat(text));
                    break;
                default:
                    Add(text, new TextStyle(item.Attribute("name")?.Value, text));
                    break;
            }
        }
        object[] array = new object[_styles.Count];
        int num = 0;
        foreach (KeyValuePair<string, TextFormatSerializer> style in _styles)
        {
            array[num++] = style.Value;
        }
        Children = array;
    }
}
