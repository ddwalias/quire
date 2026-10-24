#nullable disable
using System.Runtime.InteropServices;

namespace Quire.VSINTERFACESLib;

[global::Quire.Mirror.MirrorOf("VSINTERFACESLib.tagRECT, interop.vsinterfaceslib")]
[StructLayout(LayoutKind.Sequential, Pack = 4)]
public struct tagRECT
{
	public int left;

	public int top;

	public int right;

	public int bottom;
}
