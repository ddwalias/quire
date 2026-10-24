#nullable disable
using System.Runtime.InteropServices;

namespace Quire.VSINTERFACESLib;

[global::Quire.Mirror.MirrorOf("VSINTERFACESLib.tagPALETTEENTRY, interop.vsinterfaceslib")]
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct tagPALETTEENTRY
{
	public byte peRed;

	public byte peGreen;

	public byte peBlue;

	public byte peFlags;
}
