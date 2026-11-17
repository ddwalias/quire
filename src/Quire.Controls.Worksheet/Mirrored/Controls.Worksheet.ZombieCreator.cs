using System;
using System.Windows;
using Quire.Controls.Core;
using Quire.Controls.Core.Whiteboard;

namespace Quire.Controls.Worksheet;

public partial class ZombieCreator
{
    private static IWorksheetItemManager ItemManager => WorksheetViewModel.ItemManager;

    private int PageIndex { get; set; }

    private IWhiteboardItem Item { get; set; }

    private double PageBodyHeight => _pageLayout.PageBodyHeight;

    public static double RoundLocation(double value)
    {
        return Math.Round(value, 5);
    }

    public static int CompareLocation(double value1, double value2)
    {
        double num = RoundLocation(value1);
        double num2 = RoundLocation(value2);
        if (num != num2)
        {
            if (!(num > num2))
            {
                return -1;
            }
            return 1;
        }
        return 0;
    }
}
