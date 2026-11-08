#nullable disable
#pragma warning disable

using System;
using System.Windows.Input;
using Quire.Controls.Core;
using Quire.FunctionalitiesLimitation;
using Quire.Wpf;

namespace Quire.Controls.Whiteboard;

public static partial class WhiteboardCommands
{
    private static readonly Type _ownerType;

    public static readonly RoutedUICommand DeleteActiveItem;

    public static readonly RoutedUICommand DeleteSelectedItems;

    public static readonly RoutedUICommand DeleteActiveOrSelectedItems;

    public static readonly RoutedUICommand InsertVerticalSpace;

    public static readonly RoutedUICommand DeleteVerticalSpaceAbove;

    public static readonly RoutedUICommand DeleteVerticalSpaceBelow;

    public static readonly RoutedUICommand SeparateOverlappedItemsVertically;

    public static readonly RoutedUICommand SeparateOverlappedItemsHorizontally;

    public static readonly RoutedUICommand SelectAllRegions;

    static WhiteboardCommands()
    {
        _ownerType = typeof(WhiteboardControl);
        DeleteActiveItem = new RoutedUICommand("Delete the active item, if any, from the WhiteboardControl", "DeleteActiveItem", _ownerType);
        DeleteSelectedItems = new RoutedUICommand("Deletes the selected items, if any, from the WhiteboardControl", "DeleteSelectedItems", _ownerType);
        DeleteActiveOrSelectedItems = RegionCommands.DeleteRegion;
        InsertVerticalSpace = new RoutedUICommand("Insert a line of space below the caret", "InsertVerticalSpace", _ownerType);
        DeleteVerticalSpaceAbove = new RoutedUICommand("Delete a line of space before the caret", "DeleteVerticalSpaceAbove", _ownerType);
        DeleteVerticalSpaceBelow = new RoutedUICommand("Delete a line of space after the caret", "DeleteVerticalSpaceBelow", _ownerType);
        SeparateOverlappedItemsVertically = new RoutedUICommandWithFunctionalityKey("Separate the overlapped regions vertically.", "SeparateOverlappedRegionsVertically", _ownerType, EnumFunctionalityKey.SeparateRegionsSeparateVerticallyCommand);
        SeparateOverlappedItemsHorizontally = new RoutedUICommandWithFunctionalityKey("Separate the overlapped regions horizontally.", "SeparateOverlappedRegionsHorizontally", _ownerType, EnumFunctionalityKey.SeparateRegionsSeparateHorizontallyCommand);
        SelectAllRegions = new RoutedUICommand("SelectAllRegions", "Select All Regions", _ownerType);
        SeparateOverlappedItemsVertically.InputGestures.Add(Gestures.CtrlF3);
        SeparateOverlappedItemsHorizontally.InputGestures.Add(Gestures.CtrlF8);
        SelectAllRegions.InputGestures.Add(Gestures.CtrlA);
    }
}
