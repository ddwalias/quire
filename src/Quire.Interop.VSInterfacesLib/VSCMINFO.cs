#nullable disable
using System;
using System.Runtime.InteropServices;

namespace Quire.VSINTERFACESLib;

[global::Quire.Mirror.MirrorOf("VSINTERFACESLib.VSCMINFO, interop.vsinterfaceslib")]
[StructLayout(LayoutKind.Sequential, Pack = 8)]
public struct VSCMINFO
{
	public uint cbSize;

	public uint idMenuItem;

	[ComAliasName("VSINTERFACESLib.wireHWND")]
	public IntPtr hwnd;
}
