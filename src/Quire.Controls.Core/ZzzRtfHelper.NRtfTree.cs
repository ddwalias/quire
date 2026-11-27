// The Zzz file prefix places these partial members after generated LocationFlags, matching Mathcad closure ordinals.
#nullable disable
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Media;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media.Imaging;
using Net.Sgoliver.NRtfTree.Core;
using Net.Sgoliver.NRtfTree.Util;
using Color = System.Drawing.Color;
using Quire.Wpf;
using Quire.IO;
using Quire.MindTouch.IO;

namespace Quire.Controls.Core;

public static partial class RtfHelper
{
    private static void AppendKeywordNode(this RtfTreeNode node, string keyword, int parameter)
    {
        RtfTreeNode keywordNode = new RtfTreeNode(RtfNodeType.Keyword, keyword, hasParameter: true, parameter);
        node.AppendChild(keywordNode);
    }

    private static void InsertKeywordNode(this RtfTreeNode node, string keyword, int parameter, int insertIndex)
    {
        RtfTreeNode keywordNode = new RtfTreeNode(RtfNodeType.Keyword, keyword, hasParameter: true, parameter);
        node.InsertChild(insertIndex, keywordNode);
    }

    private static void AppendKeywordNode(this RtfTreeNode node, string keyword)
    {
        RtfTreeNode keywordNode = new RtfTreeNode(RtfNodeType.Keyword, keyword, hasParameter: false, 0);
        node.AppendChild(keywordNode);
    }

    private static void InsertKeywordNode(this RtfTreeNode node, string keyword, int insertIndex)
    {
        RtfTreeNode keywordNode = new RtfTreeNode(RtfNodeType.Keyword, keyword, hasParameter: false, 0);
        node.InsertChild(insertIndex, keywordNode);
    }

    private static void AppendTextNode(this RtfTreeNode node, string content)
    {
        RtfTreeNode textNode = new RtfTreeNode(RtfNodeType.Text, content, hasParameter: false, 0);
        node.AppendChild(textNode);
    }

    private static RtfTreeNode AppendGroupNode(this RtfTreeNode node)
    {
        RtfTreeNode groupNode = new RtfTreeNode(RtfNodeType.Group);
        node.AppendChild(groupNode);
        return groupNode;
    }

    private static int ConvertPixelsToTwips(double pixels)
    {
        return (int)Math.Round(pixels * 15.0);
    }

    public static void InsertLocation(this RtfTreeNode node, Rect layout, LocationFlags flags, int insertIndex, bool hasBorder = false)
    {
        node.InsertKeywordNode("pard", insertIndex++);
        node.InsertKeywordNode("phmrg", insertIndex++);
        node.InsertKeywordNode(flags.HasFlag(LocationFlags.RelativeToCaret) ? "pvpara" : "pvmrg", insertIndex++);
        node.InsertKeywordNode("posx", NormalizeHorizontalPosition(layout.X), insertIndex++);
        node.InsertKeywordNode("posy", ConvertPixelsToTwips(layout.Y), insertIndex++);
        if (!flags.HasFlag(LocationFlags.AutoWidth))
        {
            node.InsertKeywordNode("absw", ConvertPixelsToTwips(layout.Width), insertIndex++);
        }
        if (!hasBorder)
        {
            node.InsertKeywordNode("absh", -ConvertPixelsToTwips(layout.Height), insertIndex);
        }
    }

    private static int NormalizeHorizontalPosition(double leftPosition)
    {
        return Math.Min(ConvertPixelsToTwips(leftPosition), 27360);
    }

    public static void AddImage(this RtfTreeNode node, BitmapSource bitmap)
    {
        RtfTreeNode imageGroup = node.AppendGroupNode();
        imageGroup.AppendKeywordNode("pict");
        imageGroup.AppendKeywordNode("pngblip");
        imageGroup.AppendKeywordNode("dn", -27);
        Size imageSize = ImageUtils.GetArchitectureIndependentSize(bitmap);
        imageGroup.AppendKeywordNode("picw", ConvertPixelsToTwips(imageSize.Width));
        imageGroup.AppendKeywordNode("pich", ConvertPixelsToTwips(imageSize.Height));
        imageGroup.AppendTextNode(ImageUtils.BitmapToB16String(bitmap));
    }

    public static void AddPageBreak(this RtfTreeNode node)
    {
        node.AppendKeywordNode("pard");
        node.AppendKeywordNode("page");
        node.AppendKeywordNode("par");
    }

    public static void AddParagraph(this RtfTreeNode node)
    {
        node.AppendKeywordNode("par");
    }

    public static void SetPageSize(this RtfTreeNode node, double contentWidth, Size pageSize, bool isSavingWholeDocument)
    {
        node.AppendKeywordNode("paperw", ConvertPixelsToTwips(Math.Min(2112.0, isSavingWholeDocument ? pageSize.Width : Math.Max(pageSize.Width, contentWidth + 192.0))));
        node.AppendKeywordNode("paperh", ConvertPixelsToTwips(pageSize.Height));
    }

    public static void AddRegion(this RtfTreeNode node, BitmapSource image, Rect clippingRect, LocationFlags flags)
    {
        try
        {
            if (DebugTools.AssertNotNull(image, null).Failed)
            {
                return;
            }
            RtfTreeNode regionGroup = node.AppendGroupNode();
            regionGroup.InsertLocation(clippingRect, flags, 0);
            regionGroup.AddImage(image);
            regionGroup.AddParagraph();
        }
        catch (Exception exception)
        {
            DebugTools.FailAssertion(exception.Message);
        }
    }

    public static void AddRichTextbox(this RtfTreeNode node, RtfTree rtfModel, Brush background, Rect clippingRect, LocationFlags flags, RtfMerger merger, bool hasBorder, Brush borderColor)
    {
        if (DebugTools.AssertNotNull(rtfModel, null).Failed)
        {
            return;
        }

        RtfTreeNode firstChild = rtfModel.RootNode.FirstChild;
        RtfTreeNode plainNode = firstChild.FirstChild;
        string placeholder;
        RtfTreeNode currentNode;
        RtfTreeNode lastNode;
        while (plainNode != null)
        {
            if (plainNode.NodeType == RtfNodeType.Keyword && plainNode.NodeKey == "plain")
            {
                break;
            }
            plainNode = plainNode.NextSibling;
        }

        if (DebugTools.AssertNotNull(plainNode, null).Passed)
        {
            for (currentNode = plainNode; currentNode != null; currentNode = currentNode.NextNode)
            {
                DebugTools.DevAssert(currentNode.NodeType != RtfNodeType.Keyword || currentNode.NodeKey != "pard", null);
            }

            firstChild.ApplyBackgroundColor(background, plainNode.Index + 1, hasBorder, borderColor);
            firstChild.InsertLocation(clippingRect, flags, plainNode.Index + 1, hasBorder);

            lastNode = plainNode.ParentNode;
            while (lastNode.LastChild != null)
            {
                lastNode = lastNode.LastChild;
            }

            if (DebugTools.DevAssert(lastNode.NodeType == RtfNodeType.Keyword && lastNode.NodeKey == "par", null).Failed)
            {
                lastNode.ParentNode.InsertKeywordNode("par", lastNode.Index + 1);
            }
        }

        placeholder = "anchor" + Guid.NewGuid();
        merger.AddPlaceHolder(placeholder, rtfModel);
        node.AppendTextNode(placeholder);
    }

    [global::Quire.Mirror.MirrorStub]
    private static void ApplyBackgroundColor(this RtfTreeNode node, Brush background, int insertIndex, bool hasBorder, Brush borderColor)
        => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private static void InsertFrameBorder(this RtfTreeNode node, string orientationTag, int width, int color, int index)
    {
        node.InsertKeywordNode(orientationTag, index++);
        node.InsertKeywordNode("brdrs", index++);
        node.InsertKeywordNode("brdrw", width, index++);
        node.InsertKeywordNode("brdrcf", color, index);
    }

    private static void InsertTextBoxFrameBorder(this RtfTreeNode node, string orientationTag, int borderThickness, int borderColor, int index)
    {
        node.InsertKeywordNode(orientationTag, index++);
        node.InsertKeywordNode("brdrs", index++);
        node.InsertKeywordNode("brdrw", borderThickness, index++);
        node.InsertKeywordNode("brdrcf", borderColor, index);
    }

    public static RtfTree TextboxToRtf(IRichTextWithEmbeddedItem richText, int embeddedItems)
    {
        RtfTree rtfTree = TextToRtfTree(richText.SaveWholeTextToRtf);
        if (DebugTools.AssertNotNull(rtfTree, null).Passed)
        {
            DebugTools.DevAssert(rtfTree.RootNode.ReplaceBaselineTextTagForRtfTag() == embeddedItems, null);
            List<Hyperlink> hyperlinks = richText.TextRange.EnumerateHyperlinksInRange().ToList();
            if (hyperlinks.IsNotEmpty())
            {
                FixHyperlinksRtfSerialization(rtfTree, hyperlinks);
            }
        }
        return rtfTree;
    }

    private static RtfTree TextToRtfTree(Action<Stream> saveRtfAction)
    {
        RtfTree rtfTree;
        try
        {
            rtfTree = new RtfTree();
            using (ChunkedMemoryStream stream = new ChunkedMemoryStream())
            {
                saveRtfAction(stream);
                stream.Seek(0L, SeekOrigin.Begin);
                StreamReader streamReader = new StreamReader(stream);
                if (rtfTree.LoadRtfStream(streamReader) != 0)
                {
                    rtfTree = null;
                }
            }
        }
        catch (Exception exception)
        {
            rtfTree = null;
            DebugTools.FailAssertion(exception);
        }

        return rtfTree;
    }

    private static int ReplaceBaselineTextTagForRtfTag(this RtfTreeNode node)
    {
        int replacements = 0;
        string nodeKey = node.NodeKey;
        int fontNameLength = nodeKey.Length - 1;
        if (node.NodeType == RtfNodeType.Text && fontNameLength > 0 &&
            (nodeKey.Remove(fontNameLength) == FontConstants.DefaultMathFontFamily.Source ||
             nodeKey.Remove(fontNameLength) == FontConstants.DefaultConverterMathFontFamily.Source))
        {
            node.NodeKey = "Calibri;";
        }

        if (node.NodeType == RtfNodeType.Text && node.Text.StartsWith("baselineOffsetTag"))
        {
            RtfTreeNode parent = node.ParentNode;
            if (DebugTools.AssertNotNull(parent, null).Failed)
            {
                return -1;
            }
            RtfTreeNode grandparent = parent.ParentNode;
            if (DebugTools.AssertNotNull(grandparent, null).Failed)
            {
                return -1;
            }
            if (DebugTools.DevAssert(double.TryParse(node.Text.Substring("baselineOffsetTag".Length), out double baselineOffset), null).Failed)
            {
                return -1;
            }
            if (DebugTools.DevAssert(grandparent.ChildNodes != null && grandparent.ChildNodes.Count >= parent.Index + 2, null).Failed)
            {
                return -1;
            }
            RtfTreeNode pictureNode = grandparent[parent.Index + 1];
            if (DebugTools.DevAssert(pictureNode?.ChildNodes != null && pictureNode.ChildNodes.Count >= 2, null).Failed)
            {
                return -1;
            }
            RtfTreeNode shapePictureNode = pictureNode.SelectSingleChildGroup("shppict", ignoreSpecial: true);
            int shapePictureIndex = 1;
            if (DebugTools.DevAssert(
                shapePictureNode?.ChildNodes != null &&
                shapePictureNode.ChildNodes.Count > shapePictureIndex &&
                shapePictureNode[shapePictureIndex] is { } nestedShapePictureNode &&
                nestedShapePictureNode.NodeKey == "shppict", null).Failed)
            {
                return -1;
            }
            shapePictureNode.InsertKeywordNode("dn", (int)Math.Round(baselineOffset * DnOffsetCoefficient, 0), shapePictureIndex + 1);
            grandparent.RemoveChild(parent.Index);
            replacements = 1;
        }
        else if (node.ChildNodes != null)
        {
            for (int i = 0; i < node.ChildNodes.Count; i++)
            {
                replacements += node[i].ReplaceBaselineTextTagForRtfTag();
            }
        }
        return replacements;
    }

    public static bool FixHyperlinksRtfSerialization(ref string rtfStr, System.Windows.Documents.TextRange range)
    {
        bool fixedHyperlinks = false;
        RtfTree rtfTree = new RtfTree();
        if (rtfTree.LoadRtfText(rtfStr) == 0)
        {
            List<Hyperlink> hyperlinks = range.EnumerateHyperlinksInRange().ToList();
            int index = 0;
            if (hyperlinks.IsNotEmpty() && hyperlinks[index].ElementStart.CompareTo(range.Start) < 0)
            {
                hyperlinks.RemoveAt(index);
            }
            int lastIndex = hyperlinks.Count - 1;
            if (hyperlinks.IsNotEmpty() && hyperlinks[lastIndex].ElementEnd.CompareTo(range.End) > 0)
            {
                hyperlinks.RemoveAt(lastIndex);
            }
            if (hyperlinks.IsNotEmpty())
            {
                fixedHyperlinks = FixHyperlinksRtfSerialization(rtfTree, hyperlinks);
            }
        }
        if (fixedHyperlinks)
        {
            rtfStr = rtfTree.Rtf;
        }
        return fixedHyperlinks;
    }

    private static bool FixHyperlinksRtfSerialization(RtfTree tree, List<Hyperlink> links)
    {
        bool result = false;
        if (DebugTools.DevAssert(links.Count > 0, null).Failed)
        {
            return false;
        }

        IList<RtfTreeNode> nodes = tree.MainGroup.SelectGroups("fldinst", ignoreSpecial: true).ToEnumerable().SelectMany((RtfTreeNode sourceGroup) => from node in sourceGroup.ChildNodes.ToEnumerable()
            where node.Text.Contains("HYPERLINK")
            select node).ToList();
        int count = DebugTools.DevAssert(links.Count == nodes.Count, null).Passed ? nodes.Count : Math.Min(nodes.Count, links.Count);
        for (int index = 0; index < count; index++)
        {
            RtfTreeNode node = nodes[index];
            Hyperlink hyperlink = links[index];
            if (UrlUtils.GetStringAddressFromUri(hyperlink.NavigateUri).ContainsUnicodeCharOrSlash())
            {
                (string oldAddress, RtfNodeCollection newAddress) = GenerateRtfKeyValueAddressFixPair(hyperlink);
                if (DebugTools.DevAssert(oldAddress != null && newAddress != null && oldAddress.Equals(node.RawText), null).Passed)
                {
                    RtfTreeNode group = new RtfTreeNode(RtfNodeType.Group);
                    group.AppendTextNode(" HYPERLINK \"");
                    group.ChildNodes.AddRange(newAddress);
                    group.AppendTextNode("\" ");
                    node.ParentNode.ChildNodes[node.Index] = group;
                    result = true;
                }
            }
        }

        return result;
    }

    private static (string oldAddress, RtfNodeCollection newAddress) GenerateRtfKeyValueAddressFixPair(Hyperlink source)
    {
        string oldAddress = null;
        RtfNodeCollection newAddress = null;
        if (source == null)
        {
            return (oldAddress: null, newAddress: null);
        }

        FlowDocument doc = new FlowDocument();
        Paragraph paragraph = new Paragraph();
        doc.Blocks.Add(paragraph);
        Hyperlink hyperlink = new Hyperlink(new Run(UrlUtils.GetStringAddressFromUri(source.NavigateUri).Replace("\\", "\\\\")))
        {
            NavigateUri = source.NavigateUri
        };
        paragraph.Inlines.Add(hyperlink);
        RtfTree tree = TextToRtfTree(SaveRTfAction);
        if (DebugTools.AssertNotNull(tree, null).Passed)
        {
            try
            {
                RtfTreeNode field = tree.MainGroup.SelectSingleGroup("field");
                oldAddress = field.SelectSingleChildGroup("fldinst", ignoreSpecial: true).RawText;
                newAddress = field.SelectSingleChildGroup("fldrslt").SelectSingleChildNode(RtfNodeType.Group).ChildNodes;
            }
            catch (Exception exception)
            {
                DebugTools.FailAssertion(exception.Message);
            }
        }

        return (oldAddress: oldAddress, newAddress: newAddress);

        void SaveRTfAction(Stream rtfStream)
        {
            new System.Windows.Documents.TextRange(doc.ContentStart, doc.ContentEnd).Save(rtfStream, DataFormats.Rtf);
        }
    }

    public static IEnumerable<RtfTreeNode> ToEnumerable(this RtfNodeCollection collection)
    {
        foreach (RtfTreeNode item in collection)
        {
            yield return item;
        }
    }

}
