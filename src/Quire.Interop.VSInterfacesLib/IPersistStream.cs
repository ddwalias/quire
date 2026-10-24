#nullable disable
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Quire.VSINTERFACESLib;

[global::Quire.Mirror.MirrorOf("VSINTERFACESLib.IPersistStream, interop.vsinterfaceslib")]
[ComImport]
[Guid("00000109-0000-0000-C000-000000000046")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface IPersistStream : IPersist
{
	new void GetClassID(out Guid pClassID);

	void IsDirty();

	void Load([In][MarshalAs(UnmanagedType.Interface)] IStream pstm);

	void Save([In][MarshalAs(UnmanagedType.Interface)] IStream pstm, [In] int fClearDirty);

	void GetSizeMax(out _ULARGE_INTEGER pcbSize);
}
