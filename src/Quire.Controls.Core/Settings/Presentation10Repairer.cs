#nullable disable
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Quire.Xml;

namespace Quire.Controls.Core.Settings;

internal static partial class Presentation10Repairer
{
    private const string _sourceNamespaceURI = "http://schemas.mathsoft.com/worksheet50";

    private const string _targetNamespaceURI = "http://schemas.ptc.com/mathcad/settings/presentation10";

    public static void RepairXDocument(XDocument xDocument)
    {
        if (AssertionE.Failed == (AssertionE)DebugTools.DevAssert(xDocument != null && xDocument.Root != null && xDocument.Root.Name.LocalName == "presentation", null))
        {
            return;
        }

        if (string.IsNullOrEmpty(xDocument.Root.Name.Namespace.NamespaceName))
        {
            UpdateNameSpace(xDocument);
            UpdateNodeStruct(xDocument);
            UpdateNodeValue(xDocument);
        }
    }

    private static void UpdateNodeValue(XDocument document)
    {
        XmlHelper.UpdateToLowerCase(document.Descendants(XName.Get("pageModel", "http://schemas.ptc.com/mathcad/settings/presentation10")), new string[3]
        {
            "show-grid",
            "show-hf-grid",
            "plot-background-type"
        });
        XmlHelper.UpdateToLowerCase(document.Descendants(XName.Get("mathStyle", "http://schemas.ptc.com/mathcad/settings/presentation10")), new string[4]
        {
            "underline",
            "overline",
            "strikethrough",
            "baseline"
        });
    }

    private static void UpdateNameSpace(IEnumerable<XElement> elements, ICollection<string> exceptions, string oldNameSpaceName, string newNameSpaceName)
    {
        foreach (XElement element in elements)
        {
            if (!exceptions.Contains(element.Name.LocalName) && (string.IsNullOrEmpty(element.Name.Namespace.NamespaceName) || element.Name.Namespace.NamespaceName.Equals(oldNameSpaceName)))
            {
                IEnumerable<XAttribute> source = element.Attributes("xmlns");
                if (source.Count() > 0)
                {
                    source.First().Remove();
                }

                if (element.Name.LocalName == "presentation")
                {
                    element.Add(new XAttribute(XName.Get("ws", "http://www.w3.org/2000/xmlns/"), "http://schemas.mathsoft.com/worksheet50"));
                }

                element.Name = XName.Get(element.Name.LocalName, newNameSpaceName);
                if (element.HasElements)
                {
                    UpdateNameSpace(element.Elements(), exceptions, oldNameSpaceName, newNameSpaceName);
                }
            }
        }
    }

    private static void UpdateNameSpace(XDocument document)
    {
        List<string> exceptions = new List<string>
        {
            "results",
            "general",
            "decimal",
            "scientific",
            "engineering",
            "fraction",
            "percent",
            "matrix",
            "unit"
        };
        UpdateNameSpace(document.Elements(), exceptions, "http://schemas.mathsoft.com/worksheet50", "http://schemas.ptc.com/mathcad/settings/presentation10");
    }

    private static XElement CreateNewParent(XElement oldParent, XElement newAddAfter, string newParentLocalName, string newParentNS, IEnumerable<string> childrenLocalNames)
    {
        XElement xElement = oldParent.Element(XName.Get(newParentLocalName, newParentNS));
        if (xElement == null)
        {
            XElement[] array = null;
            foreach (string childrenLocalName in childrenLocalNames)
            {
                XElement[] array2 = oldParent.Elements(XName.Get(childrenLocalName, newParentNS)).ToArray();
                if (array2 != null)
                {
                    array = ((array == null) ? array2 : array.Union(array2).ToArray());
                    array2.Each((XElement elem) =>
                    {
                        elem.Remove();
                    });
                }
            }

            xElement = new XElement(XName.Get(newParentLocalName, newParentNS), array);
            if (newAddAfter == null)
            {
                oldParent.AddFirst(xElement);
            }
            else
            {
                newAddAfter.AddAfterSelf(xElement);
            }
        }

        return xElement;
    }

    private static void UpdateNodeStruct(XDocument xDocument)
    {
        if (xDocument.Root.Name.LocalName == "presentation" && xDocument.Root.Name.NamespaceName == "http://schemas.ptc.com/mathcad/settings/presentation10")
        {
            XElement oldParent = xDocument.Descendants(XName.Get("mathRendering", "http://schemas.ptc.com/mathcad/settings/presentation10")).Single();
            XElement newAddAfter = CreateNewParent(oldParent, null, "mathStyles", "http://schemas.ptc.com/mathcad/settings/presentation10", new string[1] { "mathStyle" });
            CreateNewParent(oldParent, newAddAfter, "results", "http://schemas.mathsoft.com/worksheet50", new string[8]
            {
                "general",
                "decimal",
                "scientific",
                "engineering",
                "fraction",
                "percent",
                "matrix",
                "unit"
            });
        }
    }
}
