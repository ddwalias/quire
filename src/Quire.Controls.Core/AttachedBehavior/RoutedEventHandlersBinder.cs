#nullable disable
using System.Windows;
using Quire.Mirror;

namespace Quire.Controls.Core.AttachedBehavior;

public partial class RoutedEventHandlersBinder
{
    [MirrorVerified("PtcControlsCoreAttachedBehaviorRoutedEventHandlersBinderMechanicalDifferentialTests")]
    public RoutedEventHandlersBinder() { }

    [MirrorVerified("PtcControlsCoreAttachedBehaviorRoutedEventHandlersBinderMechanicalDifferentialTests")]
    public RoutedEventBindingCollection OriginalValue { get; set; }

    public void Attach(FrameworkElement element)
    {
        if (OriginalValue != null)
        {
            foreach (RoutedEventBinding item in OriginalValue)
            {
                element.AddHandler(item.RoutedEvent, item.Handler);
            }
        }
    }

    public void Detach(FrameworkElement element)
    {
        if (OriginalValue != null)
        {
            foreach (RoutedEventBinding item in OriginalValue)
            {
                element.RemoveHandler(item.RoutedEvent, item.Handler);
            }
        }
    }
}
