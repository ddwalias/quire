using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Quire.Controls.Core;
using Quire.Controls.Core.Whiteboard;
using Quire.Controls.ExcelComponent.Model;

namespace Quire.Controls.ExcelComponent.View;

public static partial class ChartComponentUtils
{
    public static void WaitForChartImagesReady(IEnumerable<IWhiteboardItem> items)
    {
        WaitForChartImagesReady(EnumerateChartComponents(items), null);
    }

    public static IDisposable CreateDisposablePrintHelper(IEnumerable<IWhiteboardItem> items, bool resetZoom)
    {
        return new PrintHelper(items, resetZoom);
    }

    private static void WaitForChartImagesReady(IEnumerable<IChartComponentControl> charts, Stopwatch sw)
    {
        ChartContainerModelUtils.WaitForChartImagesReady(charts.Select(chart => chart.ChartContainer.Model).ToList(), sw);
    }

    private static IEnumerable<IChartComponentControl> EnumerateChartComponents(IEnumerable<IWhiteboardItem> items)
    {
        foreach (IWhiteboardItem item in items)
        {
            if (item is IChartComponentControl chartComponentControl)
            {
                yield return chartComponentControl;
            }

            if (item is IWhiteboardItemContainer whiteboardItemContainer)
            {
                foreach (IChartComponentControl chart in EnumerateChartComponents(whiteboardItemContainer.WhiteboardItems))
                {
                    yield return chart;
                }
            }
        }
    }

    // Declare this nested type after the outer methods to preserve Mathcad's generated method ordinals.
    [global::Quire.Mirror.MirrorOf("Ptc.Controls.ExcelComponent.View.ChartComponentUtils+PrintHelper, Ptc.Controls.ExcelComponent")]
    private partial class PrintHelper : IDisposable
    {
        private readonly IList<IChartComponentControl> _chartsList;

        public PrintHelper(IEnumerable<IWhiteboardItem> items, bool changeZoom)
        {
            _chartsList = EnumerateChartComponents(items).ToList();
            DebugTools.DevAssert(_chartsList.All(chart => !chart.IsPrintingZoomSet && chart.ChartContainer.IsStorageEmpty), null);
            try
            {
                Stopwatch sw = null;
                _chartsList.Each(chart =>
                {
                    chart.SetPrintingZoom(true, changeZoom ? 1.0 : (double?)null);
                });
                WaitForChartImagesReady(_chartsList, sw);
                _chartsList.Each(chart =>
                {
                    chart.SetPrintingZoom(false, null);
                });
            }
            catch (Exception)
            {
                Dispose();
                throw;
            }
        }

        public void Dispose()
        {
            _chartsList.Each(chart => chart.ChartContainer.CleanStorage(deleteFile: true));
        }
    }
}
