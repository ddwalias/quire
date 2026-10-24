#nullable disable
using System;
using System.Runtime.InteropServices;

namespace Quire.VSINTERFACESLib;

[global::Quire.Mirror.MirrorOf("VSINTERFACESLib.tagRemSNB, interop.vsinterfaceslib")]
[StructLayout(LayoutKind.Sequential, Pack = 4)]
[ComConversionLoss]
public struct tagRemSNB
{
	public uint ulCntStr;

	public uint ulCntChar;

	[ComConversionLoss]
	public IntPtr rgString;
}
