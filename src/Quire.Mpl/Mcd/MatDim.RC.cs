#if QUIRE_MIRROR_STUB_FREE
// MirrorGen emits this whole nested enum for normal builds. The stub-free build needs its declaration
// to compile the dimension factories; retaining MirrorOf tells MirrorGen to continue emitting the enum.
using Quire.Mirror;
namespace Quire.Mcd;
public abstract partial class MatDim
{
    [MirrorOf("Mcd.MatDim+RC, mplrun")]
    public enum RC { ROW = 0, COL = 1 }
}
#endif
