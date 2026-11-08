#nullable disable
#pragma warning disable

using System.Windows;

namespace Quire.Controls.Mathboard;

public partial interface IAggregateMathboardViewModel
{
    ISectionIndicatorInfo[] SectionIndicators { get; }

    bool HideFromCalculationContext { get; set; }

    Size MeasureOverrideHelper(Size constraint);
}
