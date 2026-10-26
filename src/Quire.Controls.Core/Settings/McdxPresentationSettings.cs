#nullable disable
using System.Xml.Linq;
using System.Xml.Schema;
using Quire.Xml;

namespace Quire.Controls.Core.Settings;

public partial class McdxPresentationSettings : PresentationSettings
{
    private readonly TextRenderingSerializer _textRendering;

    public McdxPresentationSettings(IEngineeringDocument document)
        : base(document, "http://schemas.ptc.com/mathcad/settings/presentation10", "http://schemas.mathsoft.com/worksheet50")
    {
        _textRendering = new TextRenderingSerializer(document.WorksheetData);
        _textRendering.TextStyles.Add("areaLayoutStyle", document.GetContentTextFormat());
        _textRendering.TextStyles.Add("headerLayoutStyle", document.GetHeaderTextFormat());
        _textRendering.TextStyles.Add("footerLayoutStyle", document.GetFooterTextFormat());
    }

    public override XmlSchema GetSchema()
    {
        return SchemasManager.Presentation10.Schema;
    }

    public override void ReadXml(XElement xElement)
    {
        base.ReadXml(xElement);
        _textRendering.ReadXml(xElement);
        _textRendering.WorksheetData.TextStyles = _textRendering.TextStyles.ToTextFormats();
    }

    public override void WriteXml(XElement xElement)
    {
        base.WriteXml(xElement);
        _textRendering.WriteXml(xElement);
    }
}
