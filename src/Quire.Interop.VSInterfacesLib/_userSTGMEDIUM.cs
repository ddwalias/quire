#nullable disable
using System.Runtime.InteropServices;

namespace Quire.VSINTERFACESLib;

[global::Quire.Mirror.MirrorOf("VSINTERFACESLib._userSTGMEDIUM, interop.vsinterfaceslib")]
[StructLayout(LayoutKind.Sequential, Pack = 8)]
public struct _userSTGMEDIUM
{
	public _STGMEDIUM_UNION DUMMYUNIONNAME;

	[MarshalAs(UnmanagedType.IUnknown)]
	public object pUnkForRelease;
}
