using System;
using System.Xml;
using Quire.Core;

namespace Quire.EquationEditor.Tree.Transform;

public partial class MathNodeToXmlTransform
{
    public XmlNode Transform(MathNode node)
    {
        var document = new XmlDocument();
        return b(document, node);
    }

    public void TransformChildNodes(XmlNode parent, MathNode node)
    {
        var childNodes = ConvertChildNodes(parent.OwnerDocument, node);
        AppendChildNodes(parent, childNodes);
    }

    public void AppendChildNodes(XmlNode parent, XmlNode[] childNodes)
    {
        foreach (var childNode in childNodes)
        {
            if (childNode != null)
            {
                parent.AppendChild(childNode);
            }
        }
    }

    public static XmlNode TransformDefaultNode(MathNodeToXmlTransform transformer, XmlDocument document, MathNode node)
    {
        return a(node.Descriptor.XMLFormat, transformer, document, node);
    }

    public static XmlNode TransformLineBreakGeneral(MathNodeToXmlTransform transformed, XmlDocument document, MathNode node)
    {
        var element = TransformOperatorNode(transformed, document, node) as XmlElement;
        DebugTools.DevAssert(a(node, element), null);
        return element;
    }

    public static XmlNode TransformLineBreakAddition(MathNodeToXmlTransform transformed, XmlDocument document, MathNode node)
    {
        node.Descriptor.XMLFormat = DefaultDescriptorTable.Default["ADDITION"].XMLFormat;
        return TransformLineBreakGeneral(transformed, document, node);
    }

    public static XmlNode TransformLineBreakSubtraction(MathNodeToXmlTransform transformed, XmlDocument document, MathNode node)
    {
        node.Descriptor.XMLFormat = DefaultDescriptorTable.Default["SUBTRACTION"].XMLFormat;
        return TransformLineBreakGeneral(transformed, document, node);
    }

    public static XmlNode TransformLineBreakMultiplication(MathNodeToXmlTransform transformed, XmlDocument document, MathNode node)
    {
        node.Descriptor.XMLFormat = DefaultDescriptorTable.Default["MULTIPLICATION"].XMLFormat;
        return TransformLineBreakGeneral(transformed, document, node);
    }

    public static XmlNode TransformLineBreakInlineDivision(MathNodeToXmlTransform transformed, XmlDocument document, MathNode node)
    {
        var element = TransformInlineDivision(transformed, document, node) as XmlElement;
        DebugTools.DevAssert(a(node, element), null);
        return element;
    }

    public static XmlNode TransformRScaleNode(MathNodeToXmlTransform transformer, XmlDocument document, MathNode node)
    {
        var result = a(document, "result");
        foreach (var child in node.Children)
        {
            transformer.a(document, result, child);
        }

        return result;
    }

    public static void TransformArgumentSeparator(MathNodeToXmlTransform transformer, XmlNode parent, MathNode node)
    {
        foreach (var child in node.Children)
        {
            if (child.DescriptorName == "ARGUMENT_SEPARATOR")
            {
                TransformArgumentSeparator(transformer, parent, node.Children[0]);
            }
            else
            {
                var xmlNode = transformer.b(parent.OwnerDocument, child);
                parent.AppendChild(xmlNode);
            }
        }
    }

    public static XmlNode TransformNegationNode(MathNodeToXmlTransform transformer, XmlDocument document, MathNode node)
    {
        XmlNode result;
        // The IL compares the constant first (ldc.i4.1 before ChildCount), so the literal is written on the left.
        if (1 == node.ChildCount)
        {
            var child = node.Children[0];
            if ("NUMERIC" == child.DescriptorName)
            {
                result = e(transformer, document, child);
                result.InnerText = string.Format("-{0}", result.InnerText);
            }
            else
            {
                result = TransformOperatorNode(transformer, document, node);
            }
        }
        else
        {
            result = TransformOperatorNode(transformer, document, node);
        }

        return result;
    }
}
