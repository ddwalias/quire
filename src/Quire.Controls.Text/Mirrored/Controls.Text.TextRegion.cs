#nullable disable
#pragma warning disable

using System.Windows;
using System.Windows.Documents;
using System;
using System.Windows.Markup;

namespace Quire.Controls.Text;

public partial class TextRegion
{
    private bool _contentLoaded;

    public override object Clone() => CloneTextRegion();

    public void InitializeComponent()
    {
        if (_contentLoaded)
        {
            return;
        }

        _contentLoaded = true;
        Uri resourceLocator = new Uri("/Quire.Controls.Text;component/textregion.xaml", UriKind.Relative);
        Application.LoadComponent(this, resourceLocator);
    }

    internal Delegate _CreateDelegate(Type delegateType, string handler) => Delegate.CreateDelegate(delegateType, this, handler);

    void IComponentConnector.Connect(int connectionId, object target)
    {
        _contentLoaded = true;
    }

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
