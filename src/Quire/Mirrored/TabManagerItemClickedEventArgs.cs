#nullable disable

using System.Windows;
using System.Windows.Input;

namespace Quire;

public partial class TabManagerItemClickedEventArgs
{
    public MouseButtonEventArgs OriginalClickArgs { get; }

    public TabManagerItem SourceTabItem { get; }

    public TabManagerItemClickedEventArgs(RoutedEvent routedEvent, TabManagerItem tabItem, MouseButtonEventArgs originalClickArgs)
        : base(routedEvent)
    {
        OriginalClickArgs = originalClickArgs;
        SourceTabItem = tabItem;
    }
}
