namespace Quire.Calculation;

public partial interface IValueToIResultNodeTransform
{
    bool SimplifyUnits { get; set; }
    global::Quire.Calculation.Core.IResultNode Transform(global::Quire.Automation.IAutomationValue v);
}
