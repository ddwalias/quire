#nullable disable
#pragma warning disable

using System.Windows;
using System.Windows.Documents;

namespace Quire.Controls.Text;

public partial class TextRegion
{
    protected override bool ShouldLeaveTextRegionOnCaretMove(
        Quire.Controls.Core.CommonRoutedEvents.CaretLeavesTextRegionEventArgs.TextAreaLeavingDirection direction)
    {
        TextPointer leftPosition;
        TextPointer rightPosition;
        switch (direction)
        {
            case Quire.Controls.Core.CommonRoutedEvents.CaretLeavesTextRegionEventArgs.TextAreaLeavingDirection.Left:
                leftPosition = Document.ContentStart.GetNextInsertionPosition(LogicalDirection.Forward) ?? Document.ContentEnd;
                return CaretPosition.CompareTo(leftPosition) == 0;
            case Quire.Controls.Core.CommonRoutedEvents.CaretLeavesTextRegionEventArgs.TextAreaLeavingDirection.Right:
                rightPosition = Document.ContentEnd.GetNextInsertionPosition(LogicalDirection.Backward) ?? Document.ContentStart;
                return CaretPosition.CompareTo(rightPosition) == 0;
            default:
                return base.ShouldLeaveTextRegionOnCaretMove(direction);
        }
    }

    public TextRegion()
    {
        InitializeComponent();
    }

    protected override bool ShouldSerializeProperty(DependencyProperty dp)
    {
        if ("Style" == dp.Name || "Template" == dp.Name)
        {
            return false;
        }

        return base.ShouldSerializeProperty(dp);
    }

    public TextRegion CloneTextRegion()
    {
        TextRegion textRegion = new TextRegion();
        textRegion.CloneContent(this, null);
        textRegion.Width = Width;
        return textRegion;
    }
}
