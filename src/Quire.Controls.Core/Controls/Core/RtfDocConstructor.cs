#nullable disable
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows;
using Net.Sgoliver.NRtfTree.Core;
using Net.Sgoliver.NRtfTree.Util;
using Quire.Mirror;
using Point = System.Windows.Point;
using Size = System.Windows.Size;

namespace Quire.Controls.Core;

public partial class RtfDocConstructor
{
    private const string DefaultFont = "Tahoma";

    internal const int ImageOffsetForDefaultFont = -27;

    private readonly List<IRtfItemBase> _orderedRtfItems;

    private readonly bool _isSavingWholeDocument;

    private readonly int _startingPageIndex;

    private readonly double _leftTrim;

    private readonly double _contentWidth;

    private readonly Size _pageSize;

    private readonly Size _pageBodySize;

    public static Dictionary<int, Color> ImprovedColorMatchingDictionary;

    public static bool IsRtfCopyEnabled { get; set; }

    static RtfDocConstructor()
    {
        IsRtfCopyEnabled = true;
        ImprovedColorMatchingDictionary = new Dictionary<int, Color>();
        ImprovedColorMatchingDictionary.Add(Color.FromArgb(255, 109, 69, 10).ToArgb(), Color.FromArgb(255, 128, 0, 0));
        ImprovedColorMatchingDictionary.Add(Color.FromArgb(255, 42, 80, 114).ToArgb(), Color.FromArgb(255, 0, 128, 128));
        ImprovedColorMatchingDictionary.Add(Color.FromArgb(255, 237, 29, 47).ToArgb(), Color.FromArgb(255, 255, 0, 0));
        ImprovedColorMatchingDictionary.Add(Color.FromArgb(255, 246, 136, 31).ToArgb(), Color.FromArgb(255, 128, 128, 128));
    }

    [MirrorStub]
    public static RtfTree PrepareRtf(IEnumerable<IRtfItemBase> rtfItems, Size pageSize, Size pageBodySize, bool isSavingWholeDocument)
        => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private static RtfTree InitTree()
    {
        RtfTree tree = new RtfTree();
        tree.LoadRtfText("{\\rtf1\\ansi\\deff0 {\\colortbl }{\\fonttbl {\\f0\\fnil Tahoma;}}{\\stylesheet}}");
        return tree;
    }

    private RtfDocConstructor(IEnumerable<IRtfItemBase> rtfItems, Size pageSize, Size pageBodySize, bool isSavingWholeDocument)
    {
        _orderedRtfItems = rtfItems.OrderBy(item => item.TopLeft.Y).ThenBy(item => item.TopLeft.X).ToList();
        _pageBodySize = pageBodySize;
        _isSavingWholeDocument = isSavingWholeDocument;
        _startingPageIndex = isSavingWholeDocument ? 0 : _orderedRtfItems.First().PageIndex;
        ComputeIsOnePageContent();
        _pageSize = pageSize;
        if (isSavingWholeDocument)
        {
            _leftTrim = 0.0;
        }
        else
        {
            IEnumerable<double> leftPositions = _orderedRtfItems.Where(item => !item.NoLeftOffset).Select(item => item.TopLeft.X);
            _leftTrim = leftPositions.IsNullOrEmpty() ? 0.0 : leftPositions.Min();
        }
        _contentWidth = rtfItems.Max(item => item.TopLeft.X + item.FullSize.Width) - _leftTrim;
    }

    private void ComputeIsOnePageContent()
    {
        double firstTop = _orderedRtfItems.First().TopLeft.Y;
        double contentBottom = 0;
        foreach (var item in _orderedRtfItems)
        {
            contentBottom = Math.Max(contentBottom, item.TopLeft.Y + item.FullSize.Height);
        }

        IsOnePageContent = contentBottom - firstTop < _pageBodySize.Height;
    }

    private bool IsOnePageContent { get; set; }

    private RtfTree PopulateRtfTree(RtfTree tree)
    {
        RtfMerger merger = new RtfMerger(tree);
        RtfTreeNode root = tree.RootNode.FirstChild;
        root.SetPageSize(_contentWidth, _pageSize, _isSavingWholeDocument);
        if (_isSavingWholeDocument || !IsOnePageContent)
        {
            if (!_isSavingWholeDocument)
            {
                root.AddPageBreak();
            }
            int pageIndex = _startingPageIndex;
            foreach (IRtfItemBase item in _orderedRtfItems)
            {
                for (; pageIndex < item.PageIndex; pageIndex++)
                {
                    root.AddPageBreak();
                }
                Point offset;
                offset = new Point(item.NoLeftOffset ? 0.0 : _leftTrim, _pageBodySize.Height * (double)item.PageIndex);
                item.AddItemToDocument(root, offset, onePageContent: false, merger);
            }
            if (!_isSavingWholeDocument)
            {
                root.AddPageBreak();
            }
        }
        else
        {
            Point offset = new Point(_leftTrim, _orderedRtfItems.First().TopLeft.Y);
            root.AddParagraph();
            _orderedRtfItems.Each(item => item.AddItemToDocument(root, offset, onePageContent: true, merger));
        }
        merger.Merge(removeLastPar: true);
        NormalizeHighlightColor(tree);
        return tree;
    }

    public void NormalizeHighlightColor(RtfTree tree)
    {
        RtfColorTable colorTable = tree.GetColorTable();
        Dictionary<int, int> colorIndexes = new Dictionary<int, int>();
        RtfTreeNode node;
        for (int i = 0; i < colorTable.Count; i++)
        {
            int key = colorTable[i].ToArgb();
            if (ImprovedColorMatchingDictionary.ContainsKey(key))
            {
                colorIndexes.Add(i, tree.GetColorID(colorTable, ImprovedColorMatchingDictionary[key]));
            }
        }

        if (colorIndexes.Count == 0)
        {
            return;
        }

        for (node = tree.RootNode.FirstChild; node != null; node = node.NextNode)
        {
            if (node.NodeType == RtfNodeType.Keyword && node.NodeKey == "highlight" && colorIndexes.ContainsKey(node.Parameter))
            {
                node.Parameter = colorIndexes[node.Parameter];
            }
        }
    }
}
