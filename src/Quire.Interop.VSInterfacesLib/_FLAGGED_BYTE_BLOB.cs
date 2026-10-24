#nullable disable
using System;
using System.Runtime.InteropServices;

namespace Quire.VSINTERFACESLib;

[global::Quire.Mirror.MirrorOf("VSINTERFACESLib._FLAGGED_BYTE_BLOB, interop.vsinterfaceslib")]
[StructLayout(LayoutKind.Sequential, Pack = 4)]
[ComConversionLoss]
public struct _FLAGGED_BYTE_BLOB
{
	public uint fFlags;

	public uint clSize;

	[ComConversionLoss]
	public IntPtr abData;
}
