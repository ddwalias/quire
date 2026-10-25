#nullable disable
using System.Xml.Linq;
using Quire.Controls.Core.Serialization;

namespace Quire.Controls.Core.Settings;

public partial class MathRendering : XElementSerializable
{
    public const string LocalName = "mathRendering";

    private readonly IEngineeringDocument _document;

    private readonly XNamespace _namespace;

    private readonly XNamespace _resultsNS;

    public MathRendering(IEngineeringDocument document, XName name, string resultsNS)
        : base(name)
    {
        _document = document;
        _namespace = name.Namespace;
        _resultsNS = resultsNS;
    }

    protected override void CreateChildren(XElement thisElement = null)
    {
        Children = new object[3]
        {
            new MathStyles(_document, _namespace),
            new MathFormatSerializer("formatting", _namespace.NamespaceName, _document.MathFormat),
            new ResultsSerializer(_document, _resultsNS)
        };
    }
}
