#nullable disable
#pragma warning disable

using System.Collections.Generic;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Quire.Controls.Core;
using Quire.Wpf;

namespace Quire.Controls.Text;

public partial class CustomSplitRichTextWithEmbeddedItem
{
    private FlowDocumentSplitData _splitData;

    public event Action<ICustomSplitItem> SplitItemIfNeeded;

    public double GetValidSplitOffset(double originalOffset, double pageHeight, ref int lineNumberHint)
    {
        Func<Rect, double> selectOffset = rect =>
        {
            if (!(rect.Height <= pageHeight))
            {
                return originalOffset;
            }
            return rect.Top;
        };
        if (_splitData == null)
        {
            _splitData = new FlowDocumentSplitData(Document);
        }
        Rect firstLine = _splitData.FirstLineRect;
        if (originalOffset <= firstLine.Top || firstLine.Bottom == originalOffset)
        {
            return originalOffset;
        }
        if (firstLine.Bottom > originalOffset)
        {
            return selectOffset(firstLine);
        }
        Rect lastLine = _splitData.LastLineRect;
        if (originalOffset == lastLine.Top || lastLine.Bottom <= originalOffset)
        {
            return originalOffset;
        }
        if (lastLine.Top < originalOffset)
        {
            return selectOffset(lastLine);
        }
        int count = _splitData.EnsureLines();
        if (DebugTools.DevAssert(count > 1).Failed)
        {
            return originalOffset;
        }
        if (DebugTools.DevAssert(lineNumberHint > -1 && lineNumberHint < count).Failed)
        {
            lineNumberHint = 0;
        }
        int lo = 0;
        int hi = count - 1;
        int mid = lineNumberHint == 0 ? (count - 1) / 2 : lineNumberHint;
        double? result = null;
        int iterations = 0;
        while (iterations < count && !result.HasValue && lo < hi)
        {
            Rect line = _splitData.GetLineRect(mid);
            if (originalOffset < line.Top)
            {
                if (lo + 1 == mid)
                {
                    result = originalOffset;
                }
                else
                {
                    hi = mid;
                }
            }
            else
            {
                if (line.Top == originalOffset || line.Bottom == originalOffset)
                {
                    return originalOffset;
                }
                if (line.Top < originalOffset && originalOffset < line.Bottom)
                {
                    result = selectOffset(line);
                }
                else if (originalOffset > line.Bottom)
                {
                    if (hi - 1 == mid)
                    {
                        result = originalOffset;
                    }
                    else
                    {
                        lo = mid;
                    }
                }
            }
            lineNumberHint = mid;
            iterations++;
            mid = (lo + hi) / 2;
        }
        if (!DebugTools.AssertNotNull(result).Passed)
        {
            return originalOffset;
        }
        return result.Value;
    }

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

    protected TextPointer GetInvalidPointer()
    {
        TextPointer result;
        if (!Document.ContentStart.HasValidLayout)
        {
            result = Document.ContentStart;
        }
        else if (!Document.ContentEnd.HasValidLayout)
        {
            result = Document.ContentEnd;
        }
        else if (Selection.IsEmpty)
        {
            result = CaretPosition.HasValidLayout ? null : CaretPosition;
        }
        else if (!Selection.Start.HasValidLayout)
        {
            result = Selection.Start;
        }
        else if (!Selection.End.HasValidLayout)
        {
            result = Selection.End;
        }
        else
        {
            result = null;
        }
        return result;
    }

    public void ValidateLayout()
    {
        TextPointer invalid = GetInvalidPointer();
        if (invalid == null)
        {
            return;
        }
        try
        {
            _ = invalid.IsAtLineStartPosition;
        }
        catch (Exception ex)
        {
            DebugTools.FailAssertion(ex.Message);
        }
        DebugTools.DevAssert(GetInvalidPointer() == null, "Text pointer layout validation failed!");
    }

    protected void FireSplitItemIfNeeded()
    {
        SplitItemIfNeeded?.Invoke(this);
    }

    protected partial class FlowDocumentSplitData
    {
        private readonly TextPointer _firstLinePtr;
        private readonly TextPointer _lastLinePtr;
        private readonly Rect _firstLineRect;
        private readonly Rect _lastLineRect;
        private IList<LineData> _lines;

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

        public int EnsureLines()
        {
            if (EnumerableExtensions.IsNullOrEmpty(_lines))
            {
                _lines = new List<LineData> { new LineData { TextPtr = _firstLinePtr, LineRect = _firstLineRect } };
                TextPointer ptr = _firstLinePtr;
                while (ptr != null)
                {
                    TextPointer next = RichTextWithEmbeddedItem.GetNextLine(ptr);
                    TextPointer cursor = ptr;
                    while (next != null && ptr.CompareTo(next) == 0)
                    {
                        cursor = cursor.GetNextInsertionPosition(LogicalDirection.Forward);
                        next = RichTextWithEmbeddedItem.GetNextLine(cursor);
                    }
                    if (next != null)
                    {
                        _lines.Add(new LineData { TextPtr = next });
                    }
                    ptr = next;
                }
                LineData last = _lines[_lines.Count - 1];
                if (DebugTools.DevAssert(last.TextPtr.CompareTo(_lastLinePtr) == 0).Passed)
                {
                    last.LineRect = _lastLineRect;
                }
            }
            return _lines.Count;
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
