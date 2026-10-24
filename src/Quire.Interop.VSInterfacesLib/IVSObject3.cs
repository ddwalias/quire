#nullable disable
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Quire.VSINTERFACESLib;

[global::Quire.Mirror.MirrorOf("VSINTERFACESLib.IVSObject3, interop.vsinterfaceslib")]
[ComImport]
[TypeLibType(TypeLibTypeFlags.FOleAutomation)]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid("7BF3C616-0A1A-11E0-99EB-11F8E2A8C36C")]
public interface IVSObject3
{
	void SetDocumentPath([MarshalAs(UnmanagedType.BStr)] string pcszDocumentPath);
}
