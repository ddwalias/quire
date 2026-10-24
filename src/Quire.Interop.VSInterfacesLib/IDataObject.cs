#nullable disable
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Quire.VSINTERFACESLib;

[global::Quire.Mirror.MirrorOf("VSINTERFACESLib.IDataObject, interop.vsinterfaceslib")]
[ComImport]
[Guid("0000010E-0000-0000-C000-000000000046")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[ComConversionLoss]
public interface IDataObject
{
	void RemoteGetData([In] ref tagFORMATETC pformatetcIn, [Out][ComAliasName("VSINTERFACESLib.wireSTGMEDIUM")] IntPtr pRemoteMedium);

	void RemoteGetDataHere([In] ref tagFORMATETC pformatetc, [In][Out][ComAliasName("VSINTERFACESLib.wireSTGMEDIUM")] IntPtr pRemoteMedium);

	void QueryGetData([In] ref tagFORMATETC pformatetc);

	void GetCanonicalFormatEtc([In] ref tagFORMATETC pformatectIn, out tagFORMATETC pformatetcOut);

	void RemoteSetData([In] ref tagFORMATETC pformatetc, [In][ComAliasName("VSINTERFACESLib.wireFLAG_STGMEDIUM")] IntPtr pmedium, [In] int fRelease);

	void EnumFormatEtc([In] uint dwDirection, [MarshalAs(UnmanagedType.Interface)] out IEnumFORMATETC ppenumFormatEtc);

	void DAdvise([In] ref tagFORMATETC pformatetc, [In] uint advf, [In][MarshalAs(UnmanagedType.Interface)] IAdviseSink pAdvSink, out uint pdwConnection);

	void DUnadvise([In] uint dwConnection);

	void EnumDAdvise([MarshalAs(UnmanagedType.Interface)] out IEnumSTATDATA ppenumAdvise);
}
