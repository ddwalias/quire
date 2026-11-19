using System.Windows.Controls;

namespace Quire.Controls.Internal;

internal partial class VisualInputControlNode
{
    public bool IsEditorFrameVisible { get; internal set; }

    public override double TextBaselineY => _baseline;

    public override double EffectiveBaselineY => _baseline;

    public override double EffectiveScriptBaselineY => _exponentBaseline;

    public virtual bool BlockFocus => true;

    public Control InputControl => _inputControl;
}
