#nullable disable
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Quire.VSINTERFACESLib;

[global::Quire.Mirror.MirrorOf("VSINTERFACESLib.IVSPersistStream, interop.vsinterfaceslib")]
[ComImport]
[TypeLibType(TypeLibTypeFlags.FOleAutomation)]
[Guid("768C4265-0A1A-11E0-BB3B-11F8E2A8C36C")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface IVSPersistStream
{
	void Load([In][MarshalAs(UnmanagedType.Interface)] IStream pstm, [In] bool old);

	void Save([In][MarshalAs(UnmanagedType.Interface)] IStream pstm, [In] bool fClearDirty, [In] bool old);
}
