#nullable disable
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Quire.VSINTERFACESLib;

[global::Quire.Mirror.MirrorOf("VSINTERFACESLib.IVSUCHelper, interop.vsinterfaceslib")]
[ComImport]
[Guid("7BF41434-0A1A-11E0-ABCF-11F8E2A8C36C")]
[TypeLibType(TypeLibTypeFlags.FOleAutomation)]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[ComConversionLoss]
public interface IVSUCHelper
{
	void InitObject([MarshalAs(UnmanagedType.Interface)] IVSObject lpVSObject, ref Guid clsid);

	void PostInitObject([MarshalAs(UnmanagedType.Interface)] IVSObject lpVSObject);

	void PostInitUCObject([MarshalAs(UnmanagedType.Interface)] IVSObject lpVSObject, [MarshalAs(UnmanagedType.Interface)] IStorage __MIDL__IVSUCHelper0000);

	void GetBuddyMem([ComAliasName("VSINTERFACESLib.wireHGLOBAL")] IntPtr __MIDL__IVSUCHelper0001);
}
