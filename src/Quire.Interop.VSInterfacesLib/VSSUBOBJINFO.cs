#nullable disable
using System.Runtime.InteropServices;

namespace Quire.VSINTERFACESLib;

[global::Quire.Mirror.MirrorOf("VSINTERFACESLib.VSSUBOBJINFO, interop.vsinterfaceslib")]
[StructLayout(LayoutKind.Sequential, Pack = 4)]
public struct VSSUBOBJINFO
{
	public ushort cbSize;

	public VSSUBOBJFLAGS lFlags;

	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 255)]
	public ushort[] szLabel;

	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 255)]
	public ushort[] szDescription;
}
