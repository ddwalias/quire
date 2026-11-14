#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Quire.Mirror;
using Color = System.Drawing.Color;

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

    static RtfDocConstructor()
    {
        IsRtfCopyEnabled = true;
        ImprovedColorMatchingDictionary = new Dictionary<int, Color>();
        ImprovedColorMatchingDictionary.Add(Color.FromArgb(255, 109, 69, 10).ToArgb(), Color.FromArgb(255, 128, 0, 0));
        ImprovedColorMatchingDictionary.Add(Color.FromArgb(255, 42, 80, 114).ToArgb(), Color.FromArgb(255, 0, 128, 128));
        ImprovedColorMatchingDictionary.Add(Color.FromArgb(255, 237, 29, 47).ToArgb(), Color.FromArgb(255, 255, 0, 0));
        ImprovedColorMatchingDictionary.Add(Color.FromArgb(255, 246, 136, 31).ToArgb(), Color.FromArgb(255, 128, 128, 128));
    }

    [MirrorVerified("PtcControlsCoreRtfDocConstructorMechanicalDifferentialTests")]
    public static bool IsRtfCopyEnabled { get; set; }

    [MirrorVerified("PtcControlsCoreRtfDocConstructorMechanicalDifferentialTests")]
    private bool IsOnePageContent { get; set; }

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
}
