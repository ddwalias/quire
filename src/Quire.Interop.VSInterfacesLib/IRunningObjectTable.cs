#nullable disable
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Quire.VSINTERFACESLib;

[global::Quire.Mirror.MirrorOf("VSINTERFACESLib.IRunningObjectTable, interop.vsinterfaceslib")]
[ComImport]
[Guid("00000010-0000-0000-C000-000000000046")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface IRunningObjectTable
{
	void Register([In] uint grfFlags, [In][MarshalAs(UnmanagedType.IUnknown)] object punkObject, [In][MarshalAs(UnmanagedType.Interface)] IMoniker pmkObjectName, out uint pdwRegister);

	void Revoke([In] uint dwRegister);

	void IsRunning([In][MarshalAs(UnmanagedType.Interface)] IMoniker pmkObjectName);

	void GetObject([In][MarshalAs(UnmanagedType.Interface)] IMoniker pmkObjectName, [MarshalAs(UnmanagedType.IUnknown)] out object ppunkObject);

	void NoteChangeTime([In] uint dwRegister, [In] ref _FILETIME pfiletime);

	void GetTimeOfLastChange([In][MarshalAs(UnmanagedType.Interface)] IMoniker pmkObjectName, out _FILETIME pfiletime);

	void EnumRunning([MarshalAs(UnmanagedType.Interface)] out IEnumMoniker ppenumMoniker);
}
