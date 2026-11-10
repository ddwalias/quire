#nullable disable

using System.ComponentModel;
using System.Runtime.CompilerServices;
using AvalonDock.Layout;

namespace Quire;

public partial class WindowTabTupleData : INotifyPropertyChanged
{
    public LayoutAnchorable Wnd { get; private set; }

    public int Order
    {
        get
        {
            return _order;
        }
        set
        {
            if (_order == value)
            {
                return;
            }

            _order = value;
            OnPropertyChanged("Order");
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public WindowTabTupleData(LayoutAnchorable wnd, int order)
    {
        Wnd = wnd;
        Order = order;
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
