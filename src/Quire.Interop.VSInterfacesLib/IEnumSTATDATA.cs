#nullable disable
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Quire.VSINTERFACESLib;

[global::Quire.Mirror.MirrorOf("VSINTERFACESLib.IEnumSTATDATA, interop.vsinterfaceslib")]
[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid("00000105-0000-0000-C000-000000000046")]
public interface IEnumSTATDATA
{
	void RemoteNext([In] uint celt, out tagSTATDATA rgelt, out uint pceltFetched);

	void Skip([In] uint celt);

	void Reset();

	void Clone([MarshalAs(UnmanagedType.Interface)] out IEnumSTATDATA ppenum);
}
