#nullable disable
#pragma warning disable

using System;
using System.Windows.Input;
using Quire.FunctionalitiesLimitation;
using Quire.Wpf;

namespace Quire.Controls.Text;

public static partial class HyperlinkCommands
{
    private static readonly Type _typeOfRichTextWithEmbeddedItem;
    public static readonly RoutedUICommand ModifyHyperlink;
    public static readonly RoutedUICommand RemoveHyperlinks;
    public static readonly RoutedUICommand OpenHyperlink;
    public static readonly RoutedUICommand CopyHyperlink;

    static HyperlinkCommands()
    {
        _typeOfRichTextWithEmbeddedItem = typeof(RichTextWithEmbeddedItem);
        ModifyHyperlink = new RoutedUICommandWithFunctionalityKey("Modify Hyperlink", "ModifyHyperlink", _typeOfRichTextWithEmbeddedItem, EnumFunctionalityKey.ModifyHyperlinkCommand);
        RemoveHyperlinks = new RoutedUICommandWithFunctionalityKey("Remove Hyperlinks", "RemoveHyperlinks", _typeOfRichTextWithEmbeddedItem, EnumFunctionalityKey.RemoveHyperlinkCommand);
        OpenHyperlink = new RoutedUICommandWithFunctionalityKey("Open Hyperlink", "OpenHyperlink", _typeOfRichTextWithEmbeddedItem, EnumFunctionalityKey.FollowHyperlinkCommand);
        CopyHyperlink = new RoutedUICommandWithFunctionalityKey("Copy Hyperlink", "CopyHyperlink", _typeOfRichTextWithEmbeddedItem, EnumFunctionalityKey.CopyHyperlinkCommand);
        CommandManager.RegisterClassInputBinding(_typeOfRichTextWithEmbeddedItem,
            new InputBinding(new InputBindingFunctionalityCommandWrapper(ModifyHyperlink), Gestures.CtrlK));
    }
}
