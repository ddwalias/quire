#nullable disable
using System;
using System.Runtime.InteropServices;

namespace Quire.VSINTERFACESLib;

[global::Quire.Mirror.MirrorOf("VSINTERFACESLib.tagLOGPALETTE, interop.vsinterfaceslib")]
[StructLayout(LayoutKind.Sequential, Pack = 2)]
[ComConversionLoss]
public struct tagLOGPALETTE
{
	public ushort palVersion;

	public ushort palNumEntries;

	[ComConversionLoss]
	public IntPtr palPalEntry;
}
