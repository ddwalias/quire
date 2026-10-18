#nullable disable
using System.Windows;
using System.Windows.Documents;
using Quire.Mirror;

namespace Quire.Controls.Text;

public partial class CustomScriptRunBase
{
    [MirrorVerified("PtcControlsTextCustomScriptRunBaseMechanicalDifferentialTests")]
    public static readonly DependencyProperty RevertCoerceAfterSerializationProcessProperty;

    [MirrorVerified("PtcControlsTextCustomScriptRunBaseMechanicalDifferentialTests")]
    public void ClearRevertCoerceAfterSerializationProcess() => base.ClearValue(RevertCoerceAfterSerializationProcessProperty);

    [MirrorVerified("PtcControlsTextCustomScriptRunBaseMechanicalDifferentialTests")]
    protected CustomScriptRunBase() { }

    [MirrorVerified("PtcControlsTextCustomScriptRunBaseMechanicalDifferentialTests")]
    protected CustomScriptRunBase(string text) : base(text) { }

    [MirrorVerified("PtcControlsTextCustomScriptRunBaseMechanicalDifferentialTests")]
    protected CustomScriptRunBase(string text, TextPointer insertionPosition) : base(text, insertionPosition) { }

    [MirrorVerified("PtcControlsTextCustomScriptRunBaseMechanicalDifferentialTests")]
    public Nullable<double> FontSizeToRestore { private get; set; }
}
