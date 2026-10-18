#nullable disable
using System.Windows;
using Quire.Mirror;

namespace Quire.Controls.Core;

public partial class NumericUpDown
{
    [MirrorVerified("PtcControlsCoreNumericUpDownMechanicalDifferentialTests")]
    private void UpButton_Click(object sender, RoutedEventArgs e) => IncrementValue();

    [MirrorVerified("PtcControlsCoreNumericUpDownMechanicalDifferentialTests")]
    private void DownButton_Click(object sender, RoutedEventArgs e) => DecrementValue();

    [MirrorVerified("PtcControlsCoreNumericUpDownMechanicalDifferentialTests")]
    public Action<int> NumberChangedAction { get; set; }

    [MirrorVerified("PtcControlsCoreNumericUpDownMechanicalDifferentialTests")]
    public int Minimum { get; set; }

    [MirrorVerified("PtcControlsCoreNumericUpDownMechanicalDifferentialTests")]
    public int Maximum { get; set; }
}
