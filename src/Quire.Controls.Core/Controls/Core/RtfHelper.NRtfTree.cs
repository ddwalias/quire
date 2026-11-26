#nullable disable
using System;
using System.Windows;
using System.Windows.Media.Imaging;
using Net.Sgoliver.NRtfTree.Core;
using Quire.Wpf;

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
}
