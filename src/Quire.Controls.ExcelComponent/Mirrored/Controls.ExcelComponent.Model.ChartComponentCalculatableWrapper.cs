namespace Quire.Controls.ExcelComponent.Model;

internal partial class ChartComponentCalculatableWrapper
{
    private BaseComponentEquationType ComponentEquationType { get; set; }

    public global::System.Nullable<uint> TraceId { get; private set; }

    public ChartWrapperType PreviousChartType { get; private set; }

    public override bool IsInputEquation => ComponentEquationType == BaseComponentEquationType.Input;

    public override bool IsUnknownEquation => ComponentEquationType == BaseComponentEquationType.Unknown;

    public override bool IsOutputEquation => ComponentEquationType == BaseComponentEquationType.Output;

    public override bool IsInvalidEquation => ComponentEquationType == BaseComponentEquationType.Invalid;
}
