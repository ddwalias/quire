#nullable disable
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Xml;
using System.Xml.Linq;
using Quire;
using Quire.Core;
using Quire.Mirror;
using Quire.Wpf;

namespace Quire.Controls.Core.MC14Conversion;

public partial class Mathcad14TextStyle
{
    [MirrorVerified("PtcControlsCoreMC14ConversionMathcad14TextStyleMechanicalDifferentialTests")]
    private static readonly XNamespace _namespace = XNamespace.Get("http://schemas.mathsoft.com/worksheet30");

    [MirrorVerified("PtcControlsCoreMC14ConversionMathcad14TextStyleMechanicalDifferentialTests")]
    private Dictionary<string, Mathcad14TextStyle> _stylesDictionary;

    [MirrorVerified("PtcControlsCoreMC14ConversionMathcad14TextStyleMechanicalDifferentialTests")]
    private Mathcad14TextStyle _baseStyle;

    [MirrorVerified("PtcControlsCoreMC14ConversionMathcad14TextStyleMechanicalDifferentialTests")]
    public string Name { get; set; }

    [MirrorVerified("PtcControlsCoreMC14ConversionMathcad14TextStyleMechanicalDifferentialTests")]
    public string BaseStyleName { get; set; }

    [MirrorVerified("PtcControlsCoreMC14ConversionMathcad14TextStyleMechanicalDifferentialTests")]
    private Mathcad14TextStyle BaseStyle
    {
        get
        {
            if (_baseStyle == null && BaseStyleName != null)
            {
                try
                {
                    _baseStyle = _stylesDictionary[BaseStyleName];
                }
                catch (KeyNotFoundException)
                {
                }
            }
            return _baseStyle;
        }
    }

    [MirrorVerified("PtcControlsCoreMC14ConversionMathcad14TextStyleMechanicalDifferentialTests")]
    public Mathcad14TextStyle(Mathcad14TextStyle baseStyle)
    {
        _baseStyle = baseStyle;
        Name = baseStyle.Name;
    }

    [MirrorVerified("PtcControlsCoreMC14ConversionMathcad14TextStyleMechanicalDifferentialTests")]
    public Mathcad14TextStyle(Dictionary<string, Mathcad14TextStyle> stylesDictionary)
    {
        _stylesDictionary = stylesDictionary;
    }

    [MirrorVerified("PtcControlsCoreMC14ConversionMathcad14TextStyleMechanicalDifferentialTests")]
    private FontFamily _fontFamily;

    [MirrorVerified("PtcControlsCoreMC14ConversionMathcad14TextStyleMechanicalDifferentialTests")]
    public FontFamily FontFamily => ReadLocalOrInheritedValue(ref _fontFamily, (Mathcad14TextStyle style) => style.FontFamily);

    [MirrorVerified("PtcControlsCoreMC14ConversionMathcad14TextStyleMechanicalDifferentialTests")]
    private double? _fontSize;

    [MirrorVerified("PtcControlsCoreMC14ConversionMathcad14TextStyleMechanicalDifferentialTests")]
    public double? FontSize => ReadLocalOrInheritedValue(ref _fontSize, (Mathcad14TextStyle style) => style.FontSize);

    [MirrorVerified("PtcControlsCoreMC14ConversionMathcad14TextStyleMechanicalDifferentialTests")]
    private FontWeight? _fontWeight;

    [MirrorVerified("PtcControlsCoreMC14ConversionMathcad14TextStyleMechanicalDifferentialTests")]
    public FontWeight? FontWeight => ReadLocalOrInheritedValue(ref _fontWeight, (Mathcad14TextStyle style) => style.FontWeight);

    [MirrorVerified("PtcControlsCoreMC14ConversionMathcad14TextStyleMechanicalDifferentialTests")]
    private FontStyle? _fontStyle;

    [MirrorVerified("PtcControlsCoreMC14ConversionMathcad14TextStyleMechanicalDifferentialTests")]
    public FontStyle? FontStyle => ReadLocalOrInheritedValue(ref _fontStyle, (Mathcad14TextStyle style) => style.FontStyle);

    [MirrorVerified("PtcControlsCoreMC14ConversionMathcad14TextStyleMechanicalDifferentialTests")]
    private TextDecorationCollection _textDecorations;

    [MirrorVerified("PtcControlsCoreMC14ConversionMathcad14TextStyleMechanicalDifferentialTests")]
    public TextDecorationCollection TextDecorations
    {
        get
        {
            return ReadLocalOrInheritedValue(ref _textDecorations, (Mathcad14TextStyle style) => style.TextDecorations);
        }
        set
        {
            _textDecorations = value;
        }
    }

    [MirrorVerified("PtcControlsCoreMC14ConversionMathcad14TextStyleMechanicalDifferentialTests")]
    private Color? _color;

    [MirrorVerified("PtcControlsCoreMC14ConversionMathcad14TextStyleMechanicalDifferentialTests")]
    public Color? Color => ReadLocalOrInheritedValue(ref _color, (Mathcad14TextStyle style) => style.Color);

    [MirrorVerified("PtcControlsCoreMC14ConversionMathcad14TextStyleMechanicalDifferentialTests")]
    private FontVariants? _variants;

    [MirrorVerified("PtcControlsCoreMC14ConversionMathcad14TextStyleMechanicalDifferentialTests")]
    public FontVariants? Variants => ReadLocalOrInheritedValue(ref _variants, (Mathcad14TextStyle style) => style.Variants);

    [MirrorVerified("PtcControlsCoreMC14ConversionMathcad14TextStyleMechanicalDifferentialTests")]
    public void ResetVariantsToDeafault()
    {
        _variants = TextFormat.DefaultTextFormat.Variants;
    }

    [MirrorVerified("PtcControlsCoreMC14ConversionMathcad14TextStyleMechanicalDifferentialTests")]
    private Thickness? _margin;

    [MirrorVerified("PtcControlsCoreMC14ConversionMathcad14TextStyleMechanicalDifferentialTests")]
    public Thickness? Margin => ReadLocalOrInheritedValue(ref _margin, (Mathcad14TextStyle style) => style.Margin);

    [MirrorVerified("PtcControlsCoreMC14ConversionMathcad14TextStyleMechanicalDifferentialTests")]
    private double? _textIndent;

    [MirrorVerified("PtcControlsCoreMC14ConversionMathcad14TextStyleMechanicalDifferentialTests")]
    public double? TextIndent => ReadLocalOrInheritedValue(ref _textIndent, (Mathcad14TextStyle style) => style.TextIndent);

    [MirrorVerified("PtcControlsCoreMC14ConversionMathcad14TextStyleMechanicalDifferentialTests")]
    private TextAlignment? _textAlign;

    [MirrorVerified("PtcControlsCoreMC14ConversionMathcad14TextStyleMechanicalDifferentialTests")]
    public TextAlignment? TextAlign => ReadLocalOrInheritedValue(ref _textAlign, (Mathcad14TextStyle style) => style.TextAlign);

    [MirrorVerified("PtcControlsCoreMC14ConversionMathcad14TextStyleMechanicalDifferentialTests")]
    private TextMarkerStyle? _markerStyle;

    [MirrorVerified("PtcControlsCoreMC14ConversionMathcad14TextStyleMechanicalDifferentialTests")]
    public TextMarkerStyle? MarkerStyle => ReadLocalOrInheritedValue(ref _markerStyle, (Mathcad14TextStyle style) => style.MarkerStyle);

    [MirrorVerified("PtcControlsCoreMC14ConversionMathcad14TextStyleMechanicalDifferentialTests")]
    protected T ReadLocalOrInheritedValue<T>(ref T thisValue, DelegateFunction1<T, Mathcad14TextStyle> getValue)
    {
        T val = thisValue;
        if (val == null && BaseStyle != null)
        {
            val = getValue(BaseStyle);
            if (val != null)
            {
                thisValue = val;
            }
        }
        return val;
    }

    [MirrorVerified("PtcControlsCoreMC14ConversionMathcad14TextStyleMechanicalDifferentialTests")]
    public static string MC14TextStyleNameConverter(string MC14TextStyleName)
    {
        return MC14TextStyleName.Replace("'", "`").Replace("<", "[").Replace(">", "]")
            .Replace("\"", "``")
            .Replace("~", "^")
            .Replace(".", ",");
    }

    [MirrorVerified("PtcControlsCoreMC14ConversionMathcad14TextStyleMechanicalDifferentialTests")]
    public void SetInlineTextClassProperty(string elementName, XmlAttributeCollection attributes)
    {
        switch (elementName)
        {
            case "b":
                _fontWeight = FontWeights.Bold;
                break;
            case "i":
                _fontStyle = FontStyles.Italic;
                break;
            case "u":
                _textDecorations = System.Windows.TextDecorations.Underline;
                break;
            case "sub":
                _variants = FontVariants.Subscript;
                break;
            case "sup":
                _variants = FontVariants.Superscript;
                break;
            case "c":
                _color = WpfUtils.WpfColorFromName(attributes["val"].Value);
                break;
            case "f":
            {
                XmlAttribute xmlAttribute2 = attributes["family"];
                XmlAttribute xmlAttribute3 = attributes["size"];
                if (xmlAttribute2 != null)
                {
                    _fontFamily = (xmlAttribute2.Value.Equals("inherit") ? null : new FontFamily(xmlAttribute2.Value));
                }
                if (xmlAttribute3 != null)
                {
                    if (double.TryParse(xmlAttribute3.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out var result))
                    {
                        _fontSize = result;
                        break;
                    }
                    DebugTools.DevAssert(xmlAttribute3.Value.Equals("inherit"), null, Array.Empty<object>());
                    _fontSize = null;
                }
                break;
            }
            case "inlineAttr":
            {
                XAttribute[] array = new XAttribute[attributes.Count];
                for (int i = 0; i < attributes.Count; i++)
                {
                    XmlAttribute xmlAttribute = attributes[i];
                    array[i] = new XAttribute(xmlAttribute.Name, xmlAttribute.Value);
                }
                XElement xElement = new XElement((XName)elementName, (object[])array);
                ReadInlineAttr(xElement);
                break;
            }
        }
    }

    [MirrorVerified("PtcControlsCoreMC14ConversionMathcad14TextStyleMechanicalDifferentialTests")]
    private void SetLocalValueFromXElementValue(XElement xElement, string attributeName, Delegate1<string> setLocalValue)
    {
        XAttribute xAttribute = xElement.Attributes(attributeName).FirstOrDefault();
        if (xAttribute != null)
        {
            setLocalValue(xAttribute.Value);
        }
    }

    [MirrorVerified("PtcControlsCoreMC14ConversionMathcad14TextStyleMechanicalDifferentialTests")]
    private void ReadInlineAttr(XElement xElement)
    {
        SetLocalValueFromXElementValue(xElement, "font-family", (string value) =>
        {
            _fontFamily = (value.Equals("inherit") ? null : new FontFamily(value));
        });
        SetLocalValueFromXElementValue(xElement, "font-size", (string value) =>
        {
            try
            {
                _fontSize = double.Parse(value, CultureInfo.InvariantCulture);
            }
            catch (FormatException)
            {
                _fontSize = null;
            }
        });
        SetLocalValueFromXElementValue(xElement, "font-weight", (string value) =>
        {
            if (value.Equals("bold"))
            {
                _fontWeight = FontWeights.Bold;
            }
            else if (value.Equals("normal"))
            {
                _fontWeight = FontWeights.Normal;
            }
        });
        SetLocalValueFromXElementValue(xElement, "font-style", (string value) =>
        {
            if (value.Equals("italic"))
            {
                _fontStyle = FontStyles.Italic;
            }
            else if (value.Equals("normal"))
            {
                _fontStyle = FontStyles.Normal;
            }
        });
        SetLocalValueFromXElementValue(xElement, "underline", (string value) =>
        {
            try
            {
                if (bool.Parse(value))
                {
                    _textDecorations = System.Windows.TextDecorations.Underline;
                }
                else
                {
                    _textDecorations = new TextDecorationCollection();
                }
            }
            catch (FormatException)
            {
                _textDecorations = null;
            }
        });
        SetLocalValueFromXElementValue(xElement, "color", (string value) =>
        {
            _color = (value.Equals("inherit") ? ((Color?)null) : new Color?(WpfUtils.WpfColorFromName(value)));
        });
        SetLocalValueFromXElementValue(xElement, "vertical-align", (string value) =>
        {
            if (value.Equals("super"))
            {
                _variants = FontVariants.Superscript;
            }
            else if (value.Equals("sub"))
            {
                _variants = FontVariants.Subscript;
            }
            else if (value.Equals("baseline"))
            {
                _variants = FontVariants.Normal;
            }
        });
    }

    [MirrorVerified("PtcControlsCoreMC14ConversionMathcad14TextStyleMechanicalDifferentialTests")]
    public void ApplyInlineAttr(Run inline)
    {
        if (FontFamily != null)
        {
            if ((FontFamily.Source == "Mathcad UniMath Prime" || FontFamily.Source == "Mathcad UniMath") && !Fonts.SystemFontFamilies.Contains(FontFamily))
            {
                inline.FontFamily = new FontFamily(new Uri("pack://application:,,,"), FontConstants.EmbedFontFullPath + FontFamily.Source);
            }
            else
            {
                inline.FontFamily = FontFamily;
            }
        }
        if (FontSize.HasValue)
        {
            inline.FontSize = WpfUtils.PointsToDips(FontSize.Value);
        }
        if (FontWeight.HasValue)
        {
            inline.FontWeight = FontWeight.Value;
        }
        if (FontStyle.HasValue)
        {
            inline.FontStyle = FontStyle.Value;
        }
        if (TextDecorations != null)
        {
            inline.TextDecorations = TextDecorations;
        }
        if (Color.HasValue)
        {
            inline.Foreground = new SolidColorBrush(Color.Value);
        }
        if (Variants != TextFormat.DefaultTextFormat.Variants)
        {
            DebugTools.FailAssertion("Shouldn't contain Variants for Subscript or Superscript", Array.Empty<object>());
        }
        if (Name != null)
        {
            TextElementAttachedProperties.SetStyleKey(inline, Name);
        }
    }

    [MirrorVerified("PtcControlsCoreMC14ConversionMathcad14TextStyleMechanicalDifferentialTests")]
    public void ApplyBlockAttr(Paragraph paragraph)
    {
        if (Margin.HasValue && MarkerStyle != TextMarkerStyle.Decimal && MarkerStyle != TextMarkerStyle.Disc)
        {
            paragraph.Margin = Margin.Value;
        }
        if (TextIndent.HasValue)
        {
            paragraph.TextIndent = TextIndent.Value;
        }
        if (TextAlign.HasValue)
        {
            paragraph.TextAlignment = TextAlign.Value;
        }
    }

    [MirrorVerified("PtcControlsCoreMC14ConversionMathcad14TextStyleMechanicalDifferentialTests")]
    public void ReadBlockAttr(XElement xStyleSource)
    {
        SetLocalValueFromXElementValue(xStyleSource, "text-indent", (string value) =>
        {
            double result = 0.0;
            if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out result))
            {
                if (result < 0.0)
                {
                    result = 0.0;
                    _textIndent = result * 1.3333333730697632;
                }
            }
            else
            {
                _textIndent = null;
            }
        });
        SetLocalValueFromXElementValue(xStyleSource, "margin-left", (string value) =>
        {
            double result = 0.0;
            if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out result))
            {
                _margin = new Thickness(result * 1.3333333730697632, 0.0, 0.0, 0.0);
            }
            else
            {
                _margin = null;
            }
        });
        SetLocalValueFromXElementValue(xStyleSource, "text-align", (string value) =>
        {
            if (value.Equals("left"))
            {
                _textAlign = TextAlignment.Left;
            }
            else if (value.Equals("right"))
            {
                _textAlign = TextAlignment.Right;
            }
            else if (value.Equals("center"))
            {
                _textAlign = TextAlignment.Center;
            }
            else if (value.Equals("justify"))
            {
                _textAlign = TextAlignment.Justify;
            }
        });
        SetLocalValueFromXElementValue(xStyleSource, "list-style-type", (string value) =>
        {
            if (value.Equals("none"))
            {
                _markerStyle = TextMarkerStyle.None;
            }
            else if (value.Equals("disc"))
            {
                _markerStyle = TextMarkerStyle.Disc;
            }
            else if (value.Equals("decimal"))
            {
                _markerStyle = TextMarkerStyle.Decimal;
            }
        });
    }

    [MirrorVerified("PtcControlsCoreMC14ConversionMathcad14TextStyleMechanicalDifferentialTests")]
    public void ReadXml(XElement xElement)
    {
        Name = xElement.Attributes("name").First().Value;
        XAttribute xAttribute = xElement.Attributes("base-style").FirstOrDefault();
        if (xAttribute != null)
        {
            BaseStyleName = xAttribute.Value;
        }
        XElement xElement2 = xElement.Descendants(_namespace + "inlineAttr").FirstOrDefault();
        if (xElement2 != null)
        {
            ReadInlineAttr(xElement2);
        }
        else
        {
            DebugTools.DevAssert(false, null, Array.Empty<object>());
        }
        XElement xElement3 = xElement.Descendants(_namespace + "blockAttr").FirstOrDefault();
        if (xElement3 != null)
        {
            ReadBlockAttr(xElement3);
        }
        else
        {
            DebugTools.DevAssert(false, null, Array.Empty<object>());
        }
    }

    [MirrorVerified("PtcControlsCoreMC14ConversionMathcad14TextStyleMechanicalDifferentialTests")]
    public static void LoadTextStylesFromXml(XmlNode stylesNode, Dictionary<string, Mathcad14TextStyle> textStyles)
    {
        if (stylesNode == null)
        {
            DebugTools.LogError("empty text style xml");
            return;
        }
        using XmlNodeReader reader = new XmlNodeReader(stylesNode);
        foreach (XElement item in XDocument.Load(reader).Descendants(_namespace + "textStyle"))
        {
            Mathcad14TextStyle mathcad14TextStyle = new Mathcad14TextStyle(textStyles);
            mathcad14TextStyle.ReadXml(item);
            textStyles.Add(mathcad14TextStyle.Name, mathcad14TextStyle);
        }
    }
}
