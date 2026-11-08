#nullable disable
#pragma warning disable

using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Quire.Controls.Text;

public partial class CustomSplitRichTextWithEmbeddedItem
{
    internal CustomSplitRichTextWithEmbeddedItem()
    {
    }

    protected override void HandleTextChanged(object sender, TextChangedEventArgs args)
    {
        base.HandleTextChanged(sender, args);
        _splitData = null;
    }

    protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
    {
        base.OnRenderSizeChanged(sizeInfo);
        _splitData = null;
    }

    protected void FireSplitItemIfNeeded()
    {
        SplitItemIfNeeded?.Invoke(this);
    }

    protected partial class FlowDocumentSplitData
    {
        public partial class LineData
        {
            public TextPointer TextPtr { get; set; }

            public Rect? LineRect { get; set; }

            public LineData()
            {
            }
        }
    }
}
