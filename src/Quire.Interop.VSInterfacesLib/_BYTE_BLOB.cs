#nullable disable
using System;
using System.Runtime.InteropServices;

namespace Quire.VSINTERFACESLib;

[global::Quire.Mirror.MirrorOf("VSINTERFACESLib._BYTE_BLOB, interop.vsinterfaceslib")]
[StructLayout(LayoutKind.Sequential, Pack = 4)]
[ComConversionLoss]
public struct _BYTE_BLOB
{
	public uint clSize;

	[ComConversionLoss]
	public IntPtr abData;
}
