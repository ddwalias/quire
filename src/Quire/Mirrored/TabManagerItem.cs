#nullable disable

using System.Windows.Input;

namespace Quire;

public partial class TabManagerItem
{
    public WindowTabTupleData WrappedItem => (WindowTabTupleData)DataContext;

    protected override void OnMouseDown(MouseButtonEventArgs e)
    {
        base.OnMouseDown(e);
        TabManagerItemClickedEventArgs args = new TabManagerItemClickedEventArgs(TabManagerItemClickedEvent, this, e);
        RaiseEvent(args);
    }

    public TabManagerItem() { }
}
