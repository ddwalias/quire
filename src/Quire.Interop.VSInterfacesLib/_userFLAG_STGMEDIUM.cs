#nullable disable
using System.Runtime.InteropServices;

namespace Quire.VSINTERFACESLib;

[global::Quire.Mirror.MirrorOf("VSINTERFACESLib._userFLAG_STGMEDIUM, interop.vsinterfaceslib")]
[StructLayout(LayoutKind.Sequential, Pack = 8)]
public struct _userFLAG_STGMEDIUM
{
	public int ContextFlags;

	public int fPassOwnership;

	public _userSTGMEDIUM Stgmed;
}
