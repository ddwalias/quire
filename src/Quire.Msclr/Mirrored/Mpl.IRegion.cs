#nullable disable
using System.Runtime.InteropServices;
using Quire.Mpl.Run;

namespace Quire.Mpl;

// C# stand-in for the Debug build. Release ships the C++/CLI members in MixedMode/MsclrPort.cpp.
public abstract partial class IRegion
{
    public partial struct Symbol
    {
        public string name;
        public string type;
        public ulong ordinal;
    }

    public abstract Symbol[] GetImports();

    public abstract Symbol[] GetExports();

    public abstract void Evaluate(object[] i, object[] e);

    public static unsafe IRegion GetRegion(Hex.Module* A_0)
    {
        IntPtr mshex = NativeLibrary.Load("mshex.dll");
        try
        {
            // Mpl::Loaded: the loader handle at 40, the import signatures at 48 and the export signatures at 64.
            byte* state = ((delegate* unmanaged[Cdecl]<Hex.Module*, byte*>)NativeLibrary.GetExport(mshex, "?GetState@Module@Hex@@QEBAPEAUState@12@XZ"))(A_0);
            var loader = GCHandle.FromIntPtr(*(IntPtr*)(state + 40)).Target as FV<object[], object[]>;
            return new Region(loader, *(byte**)(state + 48), *(byte**)(state + 56), *(byte**)(state + 64), *(byte**)(state + 72));
        }
        finally
        {
            NativeLibrary.Free(mshex);
        }
    }

    public IRegion()
    {
    }
}
