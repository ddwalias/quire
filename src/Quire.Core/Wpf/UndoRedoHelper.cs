#nullable disable
using System.Windows.Input;
using System.Windows;
using Quire.Mirror;
using Quire.Undo;

namespace Quire.Wpf;

public static partial class UndoRedoHelper
{
    private static ICanBlockUndoRedo TryFindICanBlockUndoRedoAncestorUntilAncestorIsReached(object topParent, RoutedEventArgs args)
    {
        return (args.OriginalSource as DependencyObject)?.TryFindAncestorIncludingSelfUntilGivenAncestorIsReached<ICanBlockUndoRedo>(topParent as DependencyObject);
    }

    [MirrorVerified("Release IL")]
    public static void CanExecuteUndoCommand(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = CanUndo(sender, e);

    [MirrorVerified("Release IL")]
    public static void CanExecuteUndoCommandPreview(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = CanUndo(sender, e);
        e.Handled = true;
    }

    [MirrorVerified("Release IL")]
    public static void HandleUndoCommand(object sender, ExecutedRoutedEventArgs e)
    {
        CommandBuffer.Undo();
        e.Handled = true;
    }

    [MirrorVerified("Release IL")]
    public static void CanExecuteRedoCommand(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = CanRedo(sender, e);

    [MirrorVerified("Release IL")]
    public static void CanExecuteRedoCommandPreview(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = CanRedo(sender, e);
        e.Handled = true;
    }

    [MirrorVerified("Release IL")]
    public static void HandleRedoCommand(object sender, ExecutedRoutedEventArgs e)
    {
        CommandBuffer.Redo();
        e.Handled = true;
    }
}
