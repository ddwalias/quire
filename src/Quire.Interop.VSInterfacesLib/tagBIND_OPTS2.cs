#nullable disable
using System;
using System.Runtime.InteropServices;

namespace Quire.VSINTERFACESLib;

[global::Quire.Mirror.MirrorOf("VSINTERFACESLib.tagBIND_OPTS2, interop.vsinterfaceslib")]
[StructLayout(LayoutKind.Sequential, Pack = 8)]
[ComConversionLoss]
public struct tagBIND_OPTS2
{
	public uint cbStruct;

	public uint grfFlags;

	public uint grfMode;

	public uint dwTickCountDeadline;

	public uint dwTrackFlags;

	public uint dwClassContext;

	public uint locale;

	[ComConversionLoss]
	public IntPtr pServerInfo;
}
