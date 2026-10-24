#nullable disable
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Quire.VSINTERFACESLib;

[global::Quire.Mirror.MirrorOf("VSINTERFACESLib.IEnumFORMATETC, interop.vsinterfaceslib")]
[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid("00000103-0000-0000-C000-000000000046")]
public interface IEnumFORMATETC
{
	void RemoteNext([In] uint celt, out tagFORMATETC rgelt, out uint pceltFetched);

	void Skip([In] uint celt);

	void Reset();

	void Clone([MarshalAs(UnmanagedType.Interface)] out IEnumFORMATETC ppenum);
}
