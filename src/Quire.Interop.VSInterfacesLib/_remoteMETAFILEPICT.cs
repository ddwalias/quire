#nullable disable
using System;
using System.Runtime.InteropServices;

namespace Quire.VSINTERFACESLib;

[global::Quire.Mirror.MirrorOf("VSINTERFACESLib._remoteMETAFILEPICT, interop.vsinterfaceslib")]
[StructLayout(LayoutKind.Sequential, Pack = 8)]
[ComConversionLoss]
public struct _remoteMETAFILEPICT
{
	public int mm;

	public int xExt;

	public int yExt;

	[ComConversionLoss]
	public IntPtr hMF;
}
