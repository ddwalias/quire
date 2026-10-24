#nullable disable
using System;
using System.Runtime.InteropServices;

namespace Quire.VSINTERFACESLib;

[global::Quire.Mirror.MirrorOf("VSINTERFACESLib.tagFORMATETC, interop.vsinterfaceslib")]
[StructLayout(LayoutKind.Sequential, Pack = 8)]
[ComConversionLoss]
public struct tagFORMATETC
{
	[ComAliasName("VSINTERFACESLib.wireCLIPFORMAT")]
	public IntPtr cfFormat;

	[ComConversionLoss]
	public IntPtr ptd;

	public uint dwAspect;

	public int lindex;

	public uint tymed;
}
