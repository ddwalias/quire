#nullable disable
using System.Runtime.InteropServices;

namespace Quire.VSINTERFACESLib;

[global::Quire.Mirror.MirrorOf("VSINTERFACESLib.VSDATABOUNDS, interop.vsinterfaceslib")]
[StructLayout(LayoutKind.Sequential, Pack = 4)]
public struct VSDATABOUNDS
{
	public int Rows;

	public int Cols;
}
