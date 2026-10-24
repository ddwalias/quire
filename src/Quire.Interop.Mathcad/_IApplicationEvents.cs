#nullable disable
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Quire;

[global::Quire.Mirror.MirrorOf("Mathcad._IApplicationEvents, interop.mathcad")]
[ComImport]
[Guid("7BDD09C3-0A1A-11E0-A0AE-11F8E2A8C36C")]
[TypeLibType(TypeLibTypeFlags.FDispatchable)]
[InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
public interface _IApplicationEvents
{
	[DispId(1)]
	void Quit();

	[DispId(2)]
	void WorksheetClosing([MarshalAs(UnmanagedType.Interface)] Worksheet pWorksheet);

	[DispId(3)]
	void WorksheetOpened([MarshalAs(UnmanagedType.Interface)] Worksheet pWorksheet);

	[DispId(4)]
	void WindowActivated([MarshalAs(UnmanagedType.Interface)] Window pWindow);

	[DispId(5)]
	void WindowDeactivated([MarshalAs(UnmanagedType.Interface)] Window pWindow);

	[DispId(6)]
	void WorksheetSaving([MarshalAs(UnmanagedType.Interface)] Worksheet pWorksheet);

	[DispId(7)]
	void WorksheetSaved([MarshalAs(UnmanagedType.Interface)] Worksheet pWorksheet);

	[DispId(8)]
	void WorksheetClosed([MarshalAs(UnmanagedType.Interface)] Worksheet pWorksheet);

	[DispId(9)]
	void Idle();
}
