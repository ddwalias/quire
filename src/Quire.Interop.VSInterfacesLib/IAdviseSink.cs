#nullable disable
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Quire.VSINTERFACESLib;

[global::Quire.Mirror.MirrorOf("VSINTERFACESLib.IAdviseSink, interop.vsinterfaceslib")]
[ComImport]
[ComConversionLoss]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid("0000010F-0000-0000-C000-000000000046")]
public interface IAdviseSink
{
	void RemoteOnDataChange([In] ref tagFORMATETC pformatetc, [In][ComAliasName("VSINTERFACESLib.wireASYNC_STGMEDIUM")] IntPtr pStgmed);

	void RemoteOnViewChange([In] uint dwAspect, [In] int lindex);

	void RemoteOnRename([In][MarshalAs(UnmanagedType.Interface)] IMoniker pmk);

	void RemoteOnSave();

	void RemoteOnClose();
}
