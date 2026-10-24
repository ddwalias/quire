#nullable disable
using System;
using System.Runtime.InteropServices;

namespace Quire.VSINTERFACESLib;

[global::Quire.Mirror.MirrorOf("VSINTERFACESLib._COAUTHIDENTITY, interop.vsinterfaceslib")]
[StructLayout(LayoutKind.Sequential, Pack = 8)]
[ComConversionLoss]
public struct _COAUTHIDENTITY
{
	[ComConversionLoss]
	public IntPtr User;

	public uint UserLength;

	[ComConversionLoss]
	public IntPtr Domain;

	public uint DomainLength;

	[ComConversionLoss]
	public IntPtr Password;

	public uint PasswordLength;

	public uint Flags;
}
