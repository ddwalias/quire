#nullable disable
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Quire.VSINTERFACESLib;

[global::Quire.Mirror.MirrorOf("VSINTERFACESLib.IEnumSTATSTG, interop.vsinterfaceslib")]
[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid("0000000D-0000-0000-C000-000000000046")]
public interface IEnumSTATSTG
{
	void RemoteNext([In] uint celt, out tagSTATSTG rgelt, out uint pceltFetched);

	void Skip([In] uint celt);

	void Reset();

	void Clone([MarshalAs(UnmanagedType.Interface)] out IEnumSTATSTG ppenum);
}
