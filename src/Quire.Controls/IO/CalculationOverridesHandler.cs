#nullable disable
using System.Windows;
using Quire.Mirror;

namespace Quire.Controls.IO;

internal static partial class CalculationOverridesHandler
{
    [MirrorVerified("PtcControlsIOCalculationOverridesHandlerMechanicalDifferentialTests")]
    public static DependencyProperty CalculationOverridesProperty;

    [MirrorVerified("PtcControlsIOCalculationOverridesHandlerMechanicalDifferentialTests")]
    public static void SetCalculationOverrides(DependencyObject region, ICalculationOverrideContainer container) => region.SetValue(CalculationOverridesProperty, container);

    [MirrorVerified("PtcControlsIOCalculationOverridesHandlerMechanicalDifferentialTests")]
    public static void ClearCalculationOverrides(DependencyObject region) => region.ClearValue(CalculationOverridesProperty);
}
