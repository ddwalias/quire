using Quire.Controls.Core;

namespace Quire.Controls.Worksheet;

public static partial class WorksheetPageLayoutExtensions
{
    public static void SetGridSize(this WorksheetPageLayout pageLayout, GridSize gridSize)
    {
        pageLayout.GridSize = gridSize;
    }
}
