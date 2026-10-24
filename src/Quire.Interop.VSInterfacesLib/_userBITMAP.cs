#nullable disable
using System;
using System.Runtime.InteropServices;

namespace Quire.VSINTERFACESLib;

[global::Quire.Mirror.MirrorOf("VSINTERFACESLib._userBITMAP, interop.vsinterfaceslib")]
[StructLayout(LayoutKind.Sequential, Pack = 4)]
[ComConversionLoss]
public struct _userBITMAP
{
	public int bmType;

	public int bmWidth;

	public int bmHeight;

	public int bmWidthBytes;

	public ushort bmPlanes;

	public ushort bmBitsPixel;

	public uint cbSize;

	[ComConversionLoss]
	public IntPtr pBuffer;
}
