using System.Windows;
using System.Windows.Input;
using Quire.Wpf;

namespace Quire.EquationEditor.View;

public static partial class CanBlockFocusUtils
{
    public static ICanBlockFocus TryFindICanBlockFocusAncestorUntilGivenAncestorIsReached(RoutedEventArgs args, DependencyObject topParent)
    {
        return (args.OriginalSource as DependencyObject)?.TryFindAncestorIncludingSelfUntilGivenAncestorIsReached<ICanBlockFocus>(topParent, includeLogical: true);
    }
}
