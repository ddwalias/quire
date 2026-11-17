using System;
using System.Collections.Generic;
using System.Windows;
using Quire.Controls.Core;
using Quire.FunctionalitiesLimitation;

namespace Quire.Controls.Worksheet;

public static partial class WorksheetControlExtensions
{
    private static IWorksheetViewModel GetViewModel(this WorksheetControl control)
    {
        return control.DataContext as IWorksheetViewModel;
    }

    public static void SetItemWorksheetLocation(this WorksheetControl control, IWhiteboardItem item, Point location)
    {
        control.GetViewModel().MoveItem(item, location);
    }

    public static void SetItemWorksheetGridLocation(this WorksheetControl control, IWhiteboardItem item, Point location)
    {
        Point location2 = control.GridLocationToWorksheetLocation(location);
        control.SetItemWorksheetLocation(item, location2);
    }

    public static void MoveItemGridLocation(this WorksheetControl control, IWhiteboardItem item, Point location)
    {
        Point point = control.GridLocationToWorksheetLocation(location);
        control.MoveItem(item, point);
    }

    public static void MoveItemsByGridLocation(this WorksheetControl control, IEnumerable<IWhiteboardItem> items, Vector offset)
    {
        Size val = (control.GetViewModel().IsContentActive ? control.GetViewModel().PageLayout.GridCellSize : control.GetViewModel().PageLayout.HeaderFooterGridCellSize);
        control.GetViewModel().MoveItemsBy(items, offset * val.Width);
    }

    public static Point GetWorksheetSectionItemLocation(this WorksheetControl control, IWhiteboardItem item)
    {
        return control.GetViewModel().GetWorksheetSectionItemLocation(item);
    }

    public static Point GetWorksheetSectionItemLocation(this WorksheetControl control, int itemIndex)
    {
        return control.GetWorksheetSectionItemLocation(control.GetViewModel().ActiveSection.WhiteboardItems[itemIndex]);
    }

    public static Point GetItemWorksheetGridLocation(this WorksheetControl control, IWhiteboardItem item)
    {
        Point worksheetSectionItemLocation = control.GetWorksheetSectionItemLocation(item);
        return control.WorksheetLocationToGridLocation(worksheetSectionItemLocation);
    }

    public static Point GetItemWorksheetGridLocation(this WorksheetControl control, int index)
    {
        Point worksheetSectionItemLocation = control.GetWorksheetSectionItemLocation(index);
        return control.WorksheetLocationToGridLocation(worksheetSectionItemLocation);
    }

    public static int GetItemWorksheetPageIndex(this WorksheetControl control, IWhiteboardItem item)
    {
        return control.GetViewModel().GetItemWorksheetPageIndex(item);
    }

    public static void SetLayoutMode(this WorksheetControl control, PageLayoutMode layout)
    {
        control.GetViewModel().WorksheetPageLayoutMode = layout;
    }

    public static void SetInsertionPoint(this WorksheetControl control, Point insertionPoint)
    {
        control.GetViewModel().InsertionPoint = insertionPoint;
    }

    public static void DiscreteSelectRegion(this WorksheetControl control, IWhiteboardItem item)
    {
        control.GetViewModel().ToggleItemSelection(item, includeActiveItem: true);
    }

    public static void ContinuousSelectRegion(this WorksheetControl control, IWhiteboardItem item)
    {
        control.GetViewModel().SelectContiguous(item);
    }

    public static void SelectRegion(this WorksheetControl control, IWhiteboardItem item)
    {
        control.GetViewModel().SelectItem(item);
    }

    public static void ActivateRegion(this WorksheetControl control, int itemIndex)
    {
        control.ActiveSectionItems[itemIndex].Activate(isActive: true);
    }

    public static Point GetInsertionPointInGridCoordinates(this WorksheetControl control)
    {
        return control.WorksheetLocationToGridLocation(control.InsertionPoint);
    }

    public static void SetInsertionPointInGridCoordinates(this WorksheetControl control, Point point)
    {
        control.SetInsertionPoint(control.GridLocationToWorksheetLocation(point));
    }

    public static void ActivateHeader(this WorksheetControl control, Point gridPt)
    {
        control.GetViewModel().ActivateHeader(control.GridLocationToWorksheetLocation(gridPt, useContentGrid: false));
    }

    public static void ActivateFooter(this WorksheetControl control, Point gridPt)
    {
        control.GetViewModel().ActivateFooter(control.GridLocationToWorksheetLocation(gridPt, useContentGrid: false));
    }

    public static void ActivateContent(this WorksheetControl control, Point gridPt)
    {
        control.GetViewModel().ActivateContent(control.GridLocationToWorksheetLocation(gridPt, useContentGrid: true));
    }

    public static void InsertPageBreak(this WorksheetControl control)
    {
        new InputBindingFunctionalityCommandWrapper(WorksheetCommands.NewPageBreak).Execute(null, control);
    }

    public static IList<IWhiteboardItem> GetWorksheetItemParts(this WorksheetControl control, IWhiteboardItem item)
    {
        return control.GetViewModel().GetWorksheetItemParts(item);
    }

    private static Point WorksheetLocationToGridLocation(this WorksheetControl control, Point worksheetLocation)
    {
        return control.WorksheetLocationToGridLocation(worksheetLocation, control.IsContentActive);
    }

    private static Point GridLocationToWorksheetLocation(this WorksheetControl control, Point gridLocation)
    {
        return control.GridLocationToWorksheetLocation(gridLocation, control.IsContentActive);
    }

    private static Point WorksheetLocationToGridLocation(this WorksheetControl control, Point worksheetLocation, bool useContentGrid)
    {
        WorksheetPageLayout pageLayout = control.GetViewModel().PageLayout;
        Size val = (useContentGrid ? pageLayout.GridCellSize : pageLayout.HeaderFooterGridCellSize);
        return new Point(Math.Round(worksheetLocation.X / val.Width), Math.Round(worksheetLocation.Y / val.Height));
    }

    private static Point GridLocationToWorksheetLocation(this WorksheetControl control, Point gridLocation, bool useContentGrid)
    {
        WorksheetPageLayout pageLayout = control.GetViewModel().PageLayout;
        Size val = (useContentGrid ? pageLayout.GridCellSize : pageLayout.HeaderFooterGridCellSize);
        return new Point(gridLocation.X * val.Width, gridLocation.Y * val.Height);
    }

    private static int GetLinesPerPage(this WorksheetControl control)
    {
        return control.GetViewModel().PageLayout.LinesPerPage;
    }
}
