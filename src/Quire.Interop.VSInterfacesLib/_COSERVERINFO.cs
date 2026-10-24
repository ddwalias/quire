#nullable disable
using System;
using System.Runtime.InteropServices;

namespace Quire.VSINTERFACESLib;

[global::Quire.Mirror.MirrorOf("VSINTERFACESLib._COSERVERINFO, interop.vsinterfaceslib")]
[StructLayout(LayoutKind.Sequential, Pack = 8)]
[ComConversionLoss]
public struct _COSERVERINFO
{
	public uint dwReserved1;

	[MarshalAs(UnmanagedType.LPWStr)]
	public string pwszName;

	[ComConversionLoss]
	public IntPtr pAuthInfo;

	public uint dwReserved2;
}
