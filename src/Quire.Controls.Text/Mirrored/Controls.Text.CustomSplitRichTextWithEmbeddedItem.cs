#nullable disable
#pragma warning disable

using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Quire.Wpf;

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
        public FlowDocumentSplitData(FlowDocument doc)
        {
            _firstLinePtr = RichTextWithEmbeddedItem.GetFirstLine(doc);
            TextPointer nextLine = RichTextWithEmbeddedItem.GetNextLine(_firstLinePtr);
            _firstLineRect = GetLineRect(_firstLinePtr, nextLine);
            if (_firstLineRect.Top < 0.0)
            {
                DebugTools.DevAssert(_firstLineRect.Top > -0.1);
                _firstLineRect.Y = 0.0;
            }
            if (nextLine == null)
            {
                _lastLinePtr = _firstLinePtr;
                _lastLineRect = _firstLineRect;
                return;
            }
            _lastLinePtr = RichTextWithEmbeddedItem.GetLastLine(doc);
            _lastLineRect = GetLineRect(_lastLinePtr, null);
        }

        public Rect FirstLineRect => _firstLineRect;

        public Rect LastLineRect => _lastLineRect;

        public Rect GetLineRect(int iLine)
        {
            if (DebugTools.DevAssert(EnumerableExtensions.IsNotEmpty(_lines) && iLine > -1 && iLine < _lines.Count).Failed)
            {
                return Rect.Empty;
            }
            LineData line = _lines[iLine];
            Rect? lineRect = line.LineRect;
            if (lineRect.HasValue)
            {
                // Release IL reads LineRect into the same local again before taking Value (no extra temporary).
                lineRect = line.LineRect;
                return lineRect.Value;
            }
            return GetLineRect(line.TextPtr, iLine + 1 < _lines.Count ? _lines[iLine + 1].TextPtr : null);
        }

        private static Rect GetLineRect(TextPointer line, TextPointer nextLine)
        {
            if (DebugTools.AssertNotNull(line).Failed)
            {
                return Rect.Empty;
            }
            Rect rect = line.GetCharacterRect(LogicalDirection.Forward);
            TextPointer end = (nextLine ?? line.DocumentEnd).GetInsertionPosition(LogicalDirection.Backward);
            TextPointer position = line.GetNextInsertionPosition(LogicalDirection.Forward);
            while (position != null && position.CompareTo(end) < 0)
            {
                rect.Union(position.GetCharacterRect(LogicalDirection.Forward));
                position = position.GetNextInsertionPosition(LogicalDirection.Forward);
            }
            return rect;
        }

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
