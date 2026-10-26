#nullable disable
using System.Xml.Linq;
using Quire.Controls.Core.Serialization;
using Quire.Mirror;

namespace Quire.Controls.Core.Settings;

public partial class MathStyle : XElementSerializable
{
    private ExtendedFont _font;

    public const string localName = "mathStyle";

    [MirrorVerified("PtcControlsCoreSettingsMathStyleMechanicalDifferentialTests")]
    public IdLabels Label { get; set; }

    public ExtendedFont Font
    {
        get => _font;
        set
        {
            _font = value;
            TextDecorationSerializer = new TextDecorationsToAttributesSerializer(Font.TextDecorations, Namespace);
        }
    }

    [MirrorVerified("PtcControlsCoreSettingsMathStyleMechanicalDifferentialTests")]
    public TextDecorationsToAttributesSerializer TextDecorationSerializer { get; set; }

    public MathStyle()
        : this(FontConstants.GetDefaultFontForMath().Clone())
    {
    }

    public MathStyle(ExtendedFont font, XNamespace nameSpace)
        : base(XName.Get("mathStyle", nameSpace.NamespaceName))
    {
        Font = font;
    }

    public MathStyle(IdLabels label, ExtendedFont font)
        : this(label, font, XNamespace.None)
    {
    }

    public MathStyle(ExtendedFont font)
        : this(font, XNamespace.None)
    {
    }

    public MathStyle(XNamespace nameSpace)
        : this(FontConstants.GetDefaultFontForMath().Clone(), nameSpace)
    {
    }

    public MathStyle(IdLabels label, ExtendedFont font, XNamespace nameSpace)
        : this(font, nameSpace)
    {
        Label = label;
    }

    protected override void CreateChildren(XElement thisElement = null)
    {
        Children = new object[8]
        {
            new PropertySerializableToXmlAttribute(this, "Label", "name"),
            new PropertySerializableToXmlAttribute(Font, "FontFamily", "font-family"),
            new PropertySerializableToXmlAttribute(Font, "FontSize", "font-size"),
            new PropertySerializableToXmlAttribute(Font, "FontWeight", "font-weight"),
            new PropertySerializableToXmlAttribute(Font, "FontStyle", "font-style"),
            new SerializableAsElementContents(TextDecorationSerializer),
            new PropertySerializableToXmlAttribute(Font, "Foreground", "color"),
            new PropertySerializableToXmlAttribute(Font, "FontStretch", "font-stretch")
        };
    }

    public override void ReadXml(XElement xElement)
    {
        base.ReadXml(xElement);
        Font.TextDecorations = TextDecorationSerializer.TextDecorationsCollection;
    }
}
