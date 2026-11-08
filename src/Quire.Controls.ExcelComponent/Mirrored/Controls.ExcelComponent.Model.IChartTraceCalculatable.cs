#nullable disable
#pragma warning disable
using System.Collections.Generic;
using Quire.Calculation.Core;

namespace Quire.Controls.ExcelComponent.Model;

internal partial interface IChartTraceCalculatable
{
    IChartComponentCalculatableWrapper XCalculatable { get; }
    IChartComponentCalculatableWrapper YCalculatable { get; }
    SynchronizationStatus SyncStatus { get; }
    CalculatingStatus CalcStatus { get; }
    IEnumerable<IChartComponentCalculatableWrapper> ErrorOwners { get; }
    uint TraceId { get; }
}
