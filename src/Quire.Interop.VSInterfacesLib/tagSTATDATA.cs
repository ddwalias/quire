#nullable disable
using System.Runtime.InteropServices;

namespace Quire.VSINTERFACESLib;

[global::Quire.Mirror.MirrorOf("VSINTERFACESLib.tagSTATDATA, interop.vsinterfaceslib")]
[StructLayout(LayoutKind.Sequential, Pack = 8)]
public struct tagSTATDATA
{
	public tagFORMATETC formatetc;

	public uint advf;

	[MarshalAs(UnmanagedType.Interface)]
	public IAdviseSink pAdvSink;

	public uint dwConnection;
}
