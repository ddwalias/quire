using System.Xml;
using Quire.Calculation.Core;
using Quire.EquationEditor.Tree;
using Quire.EquationEditor.Tree.Transform;
using Quire.Serialization;

namespace Quire.PersistentDataObjects;

public partial class MathTransformer
{
    private readonly XmlNode e;
    private readonly IResultManager f;
    private readonly XmlNamespaceManager g;
    private readonly IWorksheetMathPresentation h;
    private XmlNode i;
    private MathNode j;
    private string k;
    private IResultNode l;
    private int m = -1;
    private bool? n;
    private bool? o;

    public XmlNode Math50Xml
    {
        get
        {
            if (i != null)
            {
                return i;
            }

            switch (e.NamespaceURI)
            {
                // Braces keep the switch temporary in local 0, as Mathcad has it.
                case "http://schemas.mathsoft.com/math30":
                {
                    MathNode node = MathNode;
                    if (node != null)
                    {
                        i = b.Transform(node);
                    }

                    break;
                }

                case "http://schemas.mathsoft.com/math50":
                    i = e;
                    break;
                default:
                    DebugTools.DevAssert(false, null);
                    break;
            }

            return i;
        }
    }

    public string SExpression
    {
        get
        {
            if (k != null)
            {
                return k;
            }

            MathNode node = MathNode;
            if (node != null)
            {
                k = node.SExpression;
            }

            return k;
        }
    }

    public int ResultRefId
    {
        get
        {
            IResultNode result = ResultNode;
            if (result == null || IResultManagerHelper.IsResultRefIdSpecified(m))
            {
                return m;
            }

            if (f != null)
            {
                m = f.CreateOldResultRefId(result, null);
            }
            else
            {
                DebugTools.DevAssert(false, null);
            }

            return m;
        }
    }

    public bool ResultRefIdSpecified => IResultManagerHelper.IsResultRefIdSpecified(ResultRefId);

    private bool HasUnitOverride
    {
        get
        {
            if (n.HasValue)
            {
                return n.Value;
            }

            string xpath = SpiritResolver.SimpleXpathCreator(".//", "ml", "unitOverride");
            XmlNode unitOverride = e.SelectSingleNode(xpath, g);
            n = unitOverride != null;
            return n.Value;
        }
    }

    public MathTransformer(XmlNode xmlNode, XmlNamespaceManager nameSpaceManager, IResultManager resultManager, IWorksheetMathPresentation worksheetMathPresentation)
    {
        e = xmlNode;
        f = resultManager;
        g = nameSpaceManager;
        h = worksheetMathPresentation;
    }
}
