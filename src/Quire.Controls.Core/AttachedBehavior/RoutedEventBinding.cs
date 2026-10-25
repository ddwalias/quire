#nullable disable
using System.Windows;
using Quire.Mirror;

namespace Quire.Controls.Core.AttachedBehavior;

public partial class RoutedEventBinding
{
    private readonly RoutedEvent _routedEvent;

    public RoutedEvent RoutedEvent => _routedEvent;

    private readonly RoutedEventHandler _handler;

    public RoutedEventHandler Handler => _handler;

    public RoutedEventBinding(RoutedEvent routedEvent, RoutedEventHandler handler)
    {
        _routedEvent = routedEvent;
        _handler = handler;
    }
}
