#nullable disable
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Quire.VSINTERFACESLib;

[global::Quire.Mirror.MirrorOf("VSINTERFACESLib.IEnumMoniker, interop.vsinterfaceslib")]
[ComImport]
[Guid("00000102-0000-0000-C000-000000000046")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface IEnumMoniker
{
	void RemoteNext([In] uint celt, [MarshalAs(UnmanagedType.Interface)] out IMoniker rgelt, out uint pceltFetched);

	void Skip([In] uint celt);

	void Reset();

	void Clone([MarshalAs(UnmanagedType.Interface)] out IEnumMoniker ppenum);
}
