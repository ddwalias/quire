#nullable disable
using System;
using System.Runtime.InteropServices;

namespace Quire.VSINTERFACESLib;

[global::Quire.Mirror.MirrorOf("VSINTERFACESLib._COAUTHINFO, interop.vsinterfaceslib")]
[StructLayout(LayoutKind.Sequential, Pack = 8)]
[ComConversionLoss]
public struct _COAUTHINFO
{
	public uint dwAuthnSvc;

	public uint dwAuthzSvc;

	[MarshalAs(UnmanagedType.LPWStr)]
	public string pwszServerPrincName;

	public uint dwAuthnLevel;

	public uint dwImpersonationLevel;

	[ComConversionLoss]
	public IntPtr pAuthIdentityData;

	public uint dwCapabilities;
}
