#nullable disable
using Quire.Mirror;

namespace Quire.EquationEditor;

public partial class VisualOperatorDescriptorTable
{
    [MirrorVerified("Release IL")]
    internal Delegate a(Type A_0, string A_1) => Delegate.CreateDelegate(A_0, this, A_1);
}
