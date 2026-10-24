#nullable disable
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Quire.VSINTERFACESLib;

[global::Quire.Mirror.MirrorOf("VSINTERFACESLib.IVSBuddyObject, interop.vsinterfaceslib")]
[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[TypeLibType(TypeLibTypeFlags.FOleAutomation)]
[Guid("7BF3ED24-0A1A-11E0-B69A-11F8E2A8C36C")]
public interface IVSBuddyObject
{
	void GetBuddyInfo(ref Guid lpclsid, [MarshalAs(UnmanagedType.LPWStr)] string lpszFileName, ref uint pdwFlags);

	void SetBuddyObject([MarshalAs(UnmanagedType.IUnknown)] object lpUnknown);

	void GetBuddyObject([MarshalAs(UnmanagedType.IUnknown)] ref object lppUnknown);
}
