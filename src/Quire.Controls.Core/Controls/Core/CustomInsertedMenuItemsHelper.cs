#nullable disable
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Quire.FunctionalitiesLimitation;

namespace Quire.Controls.Core;

public static partial class CustomInsertedMenuItemsHelper
{
    public static ContextMenu ContextMenuOpened(object sender, RoutedEventArgs args)
    {
        ContextMenu contextMenu = sender as ContextMenu;
        if (DebugTools.AssertNotNull(contextMenu, null).Failed)
        {
            return null;
        }
        if (contextMenu is IPrimeContextMenu)
        {
            return null;
        }
        for (int num = contextMenu.Items.Count - 1; num >= 0; num--)
        {
            if (contextMenu.Items[num] is ICustomInsertedMenuPart)
            {
                contextMenu.Items.RemoveAt(num);
            }
        }
        return contextMenu;
    }

    public static int TryInsertMenuItemByKey(ContextMenu ctxMenu, ResourceDictionary dict, object itemKey)
    {
        object obj = (dict.Contains(itemKey) ? dict[itemKey] : null);
        int num = -1;
        if (obj is CustomInsertedContextMenuItem { Command: RoutedUICommand command } customInsertedContextMenuItem)
        {
            if (new InputBindingFunctionalityCommandWrapper(command).CanExecute(null, null) || itemKey.ToString() == "TextStyleCreateFrom")
            {
                num = IndexOfCommandMenuItem(ctxMenu.Items, customInsertedContextMenuItem.NextItemCommand, ctxMenu.Items.Count);
                try
                {
                    ctxMenu.Items.Insert(num, customInsertedContextMenuItem);
                }
                catch (Exception ex)
                {
                    DebugTools.FailAssertion(ex);
                }
            }
        }
        else
        {
            DebugTools.FailAssertion($"{obj?.GetType().Name}, {(obj as MenuItem)?.Command}");
        }
        return num;
    }

    public static MenuItem GetMenuItemWithCommand(ItemCollection ctxMenuItems, RoutedUICommand itemCommand)
    {
        if (itemCommand != null)
        {
            return ctxMenuItems.OfType<MenuItem>().FirstOrDefault((MenuItem item) => item.Command == itemCommand);
        }
        return null;
    }

    private static int IndexOfCommandMenuItem(ItemCollection ctxMenuItems, RoutedUICommand itemCommand, int fallbackInsertIndex)
    {
        MenuItem menuItemWithCommand = GetMenuItemWithCommand(ctxMenuItems, itemCommand);
        int num = ((menuItemWithCommand == null) ? (-1) : ctxMenuItems.IndexOf(menuItemWithCommand));
        if (num == -1)
        {
            return fallbackInsertIndex;
        }
        return num;
    }
}
