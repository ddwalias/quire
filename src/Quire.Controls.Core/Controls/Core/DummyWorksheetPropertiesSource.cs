#nullable disable
using System.ComponentModel;
using System.Windows.Media;
using Quire.Mirror;

namespace Quire.Controls.Core;

internal partial class DummyWorksheetPropertiesSource
{
    [MirrorVerified("PtcControlsCoreDummyWorksheetPropertiesSourceMechanicalDifferentialTests")]
    private DummyWorksheetPropertiesSource() { }

    [MirrorVerified("PtcControlsCoreDummyWorksheetPropertiesSourceMechanicalDifferentialTests")]
    public Color PageBackgroundColor { get; }

    [MirrorVerified("PtcControlsCoreDummyWorksheetPropertiesSourceMechanicalDifferentialTests")]
    public bool HasScriptableControls { get; }

    [MirrorVerified("PtcControlsCoreDummyWorksheetPropertiesSourceMechanicalDifferentialTests")]
    public event PropertyChangedEventHandler PropertyChanged;
}
