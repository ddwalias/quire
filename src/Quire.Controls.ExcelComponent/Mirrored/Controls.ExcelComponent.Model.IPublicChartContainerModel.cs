#nullable disable
#pragma warning disable
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using Quire.Calculation.Core;

namespace Quire.Controls.ExcelComponent.Model;

public partial interface IPublicChartContainerModel
{
    IEnumerable<uint> CurrentTraceIds { get; set; }
    double[,] DataLimits { get; }
    bool IsImageReady { get; }
    bool IsImageFrozen { get; }
    event Func<IResultNode, double[,], StringBuilder, double[,]> OnProcessMinMax;
    Exception RemoveTrace(uint traceId);
    void ClearTracesAndLimits();
    void SetRect(Int32Rect rectInPixels);
    void AddImageFreezeSource(ImageFreezeSources source, bool forceRun = false);
    void RemoveImageFreezeSource(ImageFreezeSources source);
    void ForceUnfreezeImage(bool force);
}
