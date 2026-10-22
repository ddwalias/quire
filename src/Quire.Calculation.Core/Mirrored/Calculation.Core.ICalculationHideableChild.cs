#nullable disable
#pragma warning disable

namespace Quire.Calculation.Core;

partial interface ICalculationHideableChild
{
    bool HideFromCalculationContext { get; set; }
    bool CanHideFromCalculationContext { get; }
}
