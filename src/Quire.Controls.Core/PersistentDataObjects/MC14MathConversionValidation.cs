#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Quire;
using Quire.Controls.Core.MC14Conversion;
using Quire.Controls.Core.Serialization;
using Quire.Core;
using Quire.Xml;

namespace Quire.PersistentDataObjects;

public static partial class MC14MathConversionValidation
{
    private const string PercentString = "%";
    private const string WhitespaceString = " ";
    private const string ElementOfString = "∈";

    private static readonly string[] _numberSetStrings = new string[3]
    {
        'ℂ'.ToString(),
        'ℝ'.ToString(),
        'ℤ'.ToString()
    };

    private const string GradientString = "∇";
    private const int GradientNumChildren = 2;

    private static AnnotationTypeList _types;

    public static AnnotationTypeList MathValidate(XmlNode xmlElement, string sExpression)
    {
        if ((AssertionE)DebugTools.DevAssert(_types == null, null) == AssertionE.Failed)
        {
            return null;
        }
        AnnotationTypeList annotationTypeList = new AnnotationTypeList();
        try
        {
            _types = annotationTypeList;
            XDocument xDocument = XDocument.Parse(xmlElement.OuterXml);
            ContainsMixedNumber(xDocument);
            ContainsInOrTreeFixity(xDocument);
            ContainsOtherwiseOperator(xDocument);
            ContainsFontStyleAttribute(xDocument);
            ContainsNameSpace(xDocument);
            ContainsOperatorWhichDisplayWasChanged(xDocument);
            ContainsPercentOperatorInResult(xDocument);
            ContainsExponentialNotation(xDocument);
            AnnotateSymbolicIfNeeded(xDocument);
            ContainsUnsupportedNames(xDocument);
            ContainsSymbolicEvaluationOfSolver(xDocument);
        }
        finally
        {
            _types = null;
        }
        return annotationTypeList;
    }

    private static void ContainsSymbolicEvaluationOfSolver(XDocument xDocument)
    {
        if (xDocument.Descendants(GetMc14Math30XName("symEval")).FirstOrDefault(element => SolveBlockXmlElements.ContainsSolverFunctionWithFilter(XmlHelper.XElementToXmlNode(element))) != null)
        {
            _types.AddOnce(MC14ConversionAnnotationType.SymbolicEvaluatingSolverFunctions);
        }
    }

    private static void ContainsUnsupportedNames(XDocument xDocument)
    {
        IEnumerable<XElement> enumerable = xDocument.Descendants(GetMc14Math30XName("id"));
        if (enumerable.Any())
        {
            Action<XElement> action = (XElement element) =>
            {
                ContainsOldMc15Functions(element);
                ContainsWhitespaces(element);
            };
            enumerable.Each(action);
        }
    }

    private static void ContainsOldMc15Functions(XElement element)
    {
        switch (element.Value)
        {
            case "svd":
                _types.AddOnce(MC14ConversionAnnotationType.RenamedFunctionSvdToSvdold);
                break;
            case "svd2":
                _types.AddOnce(MC14ConversionAnnotationType.RenamedFunctionSvd2ToSvd);
                break;
            case "fullfact":
                _types.AddOnce(MC14ConversionAnnotationType.RenamedFunctionFullfractToFullfract2);
                break;
            case "fractfact":
                _types.AddOnce(MC14ConversionAnnotationType.RenamedFunctionFractfactToFractfact2);
                break;
            case "boxwilson":
                _types.AddOnce(MC14ConversionAnnotationType.RenamedFunctionBoxwilsonToBoxwilson2);
                break;
        }
    }

    private static void ContainsWhitespaces(XElement element)
    {
        if (element.Value.Contains(" "))
        {
            _types.AddOnce(MC14ConversionAnnotationType.WhitespacesReplacedWithUnderscores);
        }
        else if (element.Attributes("subscript").Any(item => item.Value.Contains(" ")))
        {
            _types.AddOnce(MC14ConversionAnnotationType.WhitespacesReplacedWithUnderscores);
        }
    }

    private static void ContainsExponentialNotation(XDocument document)
    {
        foreach (XElement realAndImaginaryElement in GetRealAndImaginaryElements(document))
        {
            if (realAndImaginaryElement.Value.ToUpper().Contains("E"))
            {
                _types.AddOnce(MC14ConversionAnnotationType.ExponentialNotation);
                break;
            }
        }
    }

    private static bool ContainsSymbolicEvaluationAsEquals(XContainer document)
    {
        return ContainsAttributeWithValue("symEval", "style", "equals", document);
    }

    private static bool ContainsBooleanEqualityAsEquals(XContainer document)
    {
        return ContainsAttributeWithValue("equal", "style", "equal", document);
    }

    private static bool ContainsLocalDefinitionAsEquals(XContainer document)
    {
        return ContainsAttributeWithValue("localDefine", "style", "equal", document);
    }

    private static void ContainsPercentOperatorInResult(XDocument document)
    {
        if (document.Descendants(GetMc14Math30XName("unitOverride")).Any(elem => elem.Value.Equals("%")))
        {
            _types.AddOnce(MC14ConversionAnnotationType.PercentOperatorInResult);
        }
    }

    private static void ContainsOperatorWhichDisplayWasChanged(XContainer document)
    {
        if (ContainsAttributeWithValue("define", "style", "equal", document) || ContainsAttributeWithValueNotEqualTo("mult", "style", "default", document) || ContainsAttributeWithValue("id", "subscript-size", "small", document) || ContainsBooleanEqualityAsEquals(document) || ContainsLocalDefinitionAsEquals(document) || ContainsSymbolicEvaluationAsEquals(document))
        {
            _types.AddOnce(MC14ConversionAnnotationType.DisplayOperator);
        }
    }

    private static bool IsNotAutoSelectedOrNotDefaultAdvancedOptionsAlgorithm(string nodeName, XContainer doc)
    {
        if (!ContainsAttributeWithValueNotEqualTo(nodeName, "auto-method", XmlConvert.ToString(true), doc) && !ContainsAttributeWithValueNotEqualTo(nodeName, "derivative-estimation", "central", doc) && !ContainsAttributeWithValueNotEqualTo(nodeName, "variable-estimation", "tangent", doc))
        {
            return ContainsAttributeWithValueNotEqualTo(nodeName, "linear-check", XmlConvert.ToString(false), doc);
        }
        return true;
    }

    private static void ContainsMixedNumber(XContainer xDocument)
    {
        if (IsXNameInXDocument(xDocument, GetMc14Math30XName("mixed")))
        {
            _types.AddOnce(MC14ConversionAnnotationType.Mixed);
        }
    }

    private static void ContainsNameSpace(XContainer document)
    {
        if (ContainsAttribute("id", "module", document))
        {
            _types.AddOnce(MC14ConversionAnnotationType.NamespaceToLabel);
        }
    }

    private static void ContainsFontStyleAttribute(XContainer document)
    {
        if (ContainsAttribute("id", "font", document) || ContainsAttribute("real", "font", document) || ContainsAttribute("imag", "font", document))
        {
            _types.AddOnce(MC14ConversionAnnotationType.FontStyle);
        }
    }

    public static bool IsValidGradientExpression(XElement elem)
    {
        XElement xElement = elem.Elements().First();
        if (elem.Attributes("fixity").Any() && elem.Attributes("fixity").First().Value.Equals("prefix") && elem.Elements().Count() == 2 && xElement.Name.LocalName == "apply" && xElement.HasElements && xElement.Elements().Count() == 3 && xElement.Elements().ElementAt(1).Value == "∇")
        {
            return xElement.Elements().First().Name.LocalName == "indexer";
        }
        return false;
    }

    private static bool IsValidElementOfExpression(XElement elem)
    {
        XElement xElement = elem.Elements().First();
        XElement lastChild = elem.Elements().Last();
        if (elem.Attributes("fixity").First().Value.Equals("infix") && elem.Elements().Count() == 3 && xElement.Name.LocalName == "id" && xElement.Value == "∈")
        {
            return _numberSetStrings.Any(numberSet => numberSet == lastChild.Value);
        }
        return false;
    }

    private static void ContainsInOrTreeFixity(XContainer document)
    {
        if (ContainsAttribute("apply", "fixity", document, elem => !IsValidElementOfExpression(elem) && !IsValidGradientExpression(elem)))
        {
            _types.AddOnce(MC14ConversionAnnotationType.Fixity);
        }
    }

    private static void ContainsOtherwiseOperator(XContainer xDocument)
    {
        if (IsXNameInXDocument(xDocument, GetMc14Math30XName("otherwise")))
        {
            _types.AddOnce(MC14ConversionAnnotationType.Otherwise);
        }
    }

    private static void AnnotateSymbolicIfNeeded(XContainer xDocument)
    {
        ContainsHideKeyword(xDocument);
        ContainsHideLHS(xDocument);
        ContainsStyleAsRightShaft(xDocument);
    }

    private static void ContainsStyleAsRightShaft(XContainer xDocument)
    {
        if (ContainsAttributeWithValue("symEval", "style", "right-arrow", xDocument))
        {
            _types.AddOnce(MC14ConversionAnnotationType.SymbolicRightShaft);
        }
    }

    private static void ContainsHideLHS(XContainer xDocument)
    {
        if (ContainsAttributeWithValue("symEval", "hide-lhs", "true", xDocument))
        {
            _types.AddOnce(MC14ConversionAnnotationType.SymbolicLHS);
        }
    }

    private static void ContainsHideKeyword(XContainer xDocument)
    {
        if (ContainsAttributeWithValue("symEval", "hide-keywords", "true", xDocument))
        {
            _types.AddOnce(MC14ConversionAnnotationType.SymbolicHiddenKeyword);
        }
    }

    public static bool IsXNameInXDocument(XContainer xDocument, XName xmlMathNodeName)
    {
        return xDocument.Descendants(xmlMathNodeName).Any();
    }

    private static bool ContainsAttribute(string nodeName, string attributeName, XContainer doc, Func<XElement, bool> func)
    {
        return doc.Descendants(XNamespace.Get("http://schemas.mathsoft.com/math30") + nodeName).Any(elem => elem.Attributes(attributeName).Count() > 0 && func(elem));
    }

    private static bool ContainsAttribute(string nodeName, string attributeName, XContainer doc)
    {
        return ContainsAttribute(nodeName, attributeName, doc, delegate
        {
            return true;
        });
    }

    public static bool ContainsAttributeWithValue(string nodeName, string attributeName, string attributeValue, XContainer doc)
    {
        return ContainsAttribute(nodeName, attributeName, doc, elem => elem.Attributes(attributeName).First().Value.Equals(attributeValue));
    }

    private static bool ContainsAttributeWithValueNotEqualTo(string nodeName, string attributeName, string attributeValue, XContainer doc)
    {
        return ContainsAttribute(nodeName, attributeName, doc, elem => !elem.Attributes(attributeName).First().Value.Equals(attributeValue));
    }

    public static XName GetMc14Math30XName(string localName)
    {
        return XName.Get(localName, "http://schemas.mathsoft.com/math30");
    }

    private static IEnumerable<XElement> GetRealAndImaginaryElements(XContainer document)
    {
        XNamespace xNamespace = XNamespace.Get("http://schemas.mathsoft.com/math30");
        IEnumerable<XElement> first = document.Descendants(xNamespace + "real");
        IEnumerable<XElement> second = document.Descendants(xNamespace + "imag");
        return first.Union(second);
    }
}
