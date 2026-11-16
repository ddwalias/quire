#nullable disable
using System.Runtime.InteropServices;
using System.Text;
using Quire.Mpl.Run;

namespace Quire.Mpl;

// C# stand-in for the Debug build. Release ships the C++/CLI members in MixedMode/MsclrPort.cpp.
internal partial class Region
{
    // sizeof(Hex::Signature)
    private const int SignatureSize = 40;

    private readonly FV<object[], object[]> m_ldr;

    private Symbol[] m_imp;

    private Symbol[] m_exp;

    internal unsafe Region(FV<object[], object[]> loader, byte* importsBegin, byte* importsEnd, byte* exportsBegin, byte* exportsEnd)
    {
        m_ldr = loader;
        m_imp = NewSymbols(importsBegin, importsEnd);
        m_exp = NewSymbols(exportsBegin, exportsEnd);
    }

    public override Symbol[] GetImports()
    {
        return m_imp;
    }

    public override Symbol[] GetExports()
    {
        return m_exp;
    }

    public override void Evaluate(object[] A_0, object[] A_1)
    {
        m_ldr(A_0, A_1);
    }

    private static unsafe Symbol[] NewSymbols(byte* begin, byte* end)
    {
        Symbol[] symbols = new Symbol[(int)((end - begin) / SignatureSize)];
        IntPtr mshex = NativeLibrary.Load("mshex.dll");
        try
        {
            // Each accessor returns a Substring<const char>: a begin and an end pointer.
            var getName = (delegate* unmanaged[Cdecl]<byte*, sbyte**, sbyte**>)NativeLibrary.GetExport(mshex, "?GetName@Signature@Hex@@QEBA?AV?$Substring@$$CBD@@XZ");
            var getType = (delegate* unmanaged[Cdecl]<byte*, sbyte**, sbyte**>)NativeLibrary.GetExport(mshex, "?GetType@Signature@Hex@@QEBA?AV?$Substring@$$CBD@@XZ");
            sbyte** text = stackalloc sbyte*[2];
            for (int i = 0; i != symbols.Length; i++)
            {
                getName(begin + (long)i * SignatureSize, text);
                symbols[i].name = new string(text[0], 0, (int)(text[1] - text[0]), Encoding.UTF8);
                getType(begin + (long)i * SignatureSize, text);
                symbols[i].type = new string(text[0], 0, (int)(text[1] - text[0]), Encoding.UTF8);
                symbols[i].ordinal = (ulong)i;
            }
        }
        finally
        {
            NativeLibrary.Free(mshex);
        }

        return symbols;
    }
}
