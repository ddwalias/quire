#nullable disable
using Quire.Mirror;

namespace Quire.EquationEditor;

public partial class VisualOperatorDescriptorTable
{
    [MirrorVerified("Release IL")]
    internal Delegate a(Type type, string text) => Delegate.CreateDelegate(type, this, text);
}
