#nullable disable
using System.Runtime.InteropServices;

namespace Quire.VSINTERFACESLib;

[global::Quire.Mirror.MirrorOf("VSINTERFACESLib._GDI_OBJECT, interop.vsinterfaceslib")]
[StructLayout(LayoutKind.Sequential, Pack = 8)]
public struct _GDI_OBJECT
{
	public uint ObjectType;

	public __MIDL_IAdviseSink_0002 u;
}
