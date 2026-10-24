#nullable disable
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Quire.VSINTERFACESLib;

[global::Quire.Mirror.MirrorOf("VSINTERFACESLib.IVSItemContainer, interop.vsinterfaceslib")]
[ComImport]
[Guid("7BF39F03-0A1A-11E0-B928-11F8E2A8C36C")]
[TypeLibType(TypeLibTypeFlags.FOleAutomation)]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface IVSItemContainer
{
	[return: MarshalAs(UnmanagedType.Interface)]
	IVSObject CreateObject([MarshalAs(UnmanagedType.BStr)] string pcszObjID, ref tagPOINT ppoint, [MarshalAs(UnmanagedType.BStr)] string pcszFileName, VSOLEFLAGS vsFlags);

	[return: MarshalAs(UnmanagedType.Interface)]
	IVSObject CreateObjectFromData([MarshalAs(UnmanagedType.Interface)] IDataObject lpDataObject, [ComAliasName("VSINTERFACESLib.wireCLIPFORMAT")] ref _userCLIPFORMAT cfFormat, ref tagPOINT ppoint, VSOLEFLAGS vsFlags);

	void ReleaseObjects();

	VSSUBOBJINFO GetInfo([MarshalAs(UnmanagedType.Interface)] IVSObject lpVSObject);

	void SetInfo([MarshalAs(UnmanagedType.Interface)] IVSObject lpVSObject, ref VSSUBOBJINFO lpVSInfo);

	void Changed([MarshalAs(UnmanagedType.Interface)] IVSObject lpVSObject, VSSUBOBJCHANGE vsChange);

	tagSIZE GetExtent([MarshalAs(UnmanagedType.Interface)] IVSObject lpVSObject);

	void Activate([MarshalAs(UnmanagedType.Interface)] IVSObject lpVSObject);

	void Properties([MarshalAs(UnmanagedType.Interface)] IVSObject lpVSObject);

	void Delete([MarshalAs(UnmanagedType.Interface)] IVSObject lpVSObject);

	void Select([MarshalAs(UnmanagedType.Interface)] IVSObject lpVSObject);

	tagRECT GetPosition([MarshalAs(UnmanagedType.Interface)] IVSObject lpVSObject);
}
