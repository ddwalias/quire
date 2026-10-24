#nullable disable
using System;
using System.Runtime.InteropServices;

namespace Quire.VSINTERFACESLib;

[global::Quire.Mirror.MirrorOf("VSINTERFACESLib.tagDVTARGETDEVICE, interop.vsinterfaceslib")]
[StructLayout(LayoutKind.Sequential, Pack = 4)]
[ComConversionLoss]
public struct tagDVTARGETDEVICE
{
	public uint tdSize;

	public ushort tdDriverNameOffset;

	public ushort tdDeviceNameOffset;

	public ushort tdPortNameOffset;

	public ushort tdExtDevmodeOffset;

	[ComConversionLoss]
	public IntPtr tdData;
}
