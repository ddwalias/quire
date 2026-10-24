#nullable disable
using System.Runtime.InteropServices;

namespace Quire.VSINTERFACESLib;

[global::Quire.Mirror.MirrorOf("VSINTERFACESLib._STGMEDIUM_UNION, interop.vsinterfaceslib")]
[StructLayout(LayoutKind.Sequential, Pack = 8)]
public struct _STGMEDIUM_UNION
{
	public uint tymed;

	public __MIDL_IAdviseSink_0003 u;
}
