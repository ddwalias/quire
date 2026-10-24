#nullable disable
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Quire;

[global::Quire.Mirror.MirrorOf("Mathcad.ApplicationClass, interop.mathcad")]
[ComImport]
[TypeLibType(TypeLibTypeFlags.FCanCreate)]
[ComSourceInterfaces("Mathcad._IApplicationEvents\0\0")]
[ClassInterface(ClassInterfaceType.None)]
[Guid("766C1033-0A1A-11E0-BAF3-11F8E2A8C36C")]
[DefaultMember("Name")]
public class ApplicationClass : IMathcadApplication, Application, _IApplicationEvents_Event
{
	[DispId(0)]
	public virtual extern string Name
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
	[DispId(0)]
		[return: MarshalAs(UnmanagedType.BStr)]
		get;
	}

	[DispId(1)]
	public virtual extern Application Application
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
	[DispId(1)]
		[return: MarshalAs(UnmanagedType.Interface)]
		get;
	}

	[DispId(2)]
	public virtual extern Application Parent
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
	[DispId(2)]
		[return: MarshalAs(UnmanagedType.Interface)]
		get;
	}

	[DispId(3)]
	public virtual extern Worksheet ActiveWorksheet
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
	[DispId(3)]
		[return: MarshalAs(UnmanagedType.Interface)]
		get;
	}

	[DispId(4)]
	public virtual extern Window ActiveWindow
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
	[DispId(4)]
		[return: MarshalAs(UnmanagedType.Interface)]
		get;
	}

	[DispId(5)]
	public virtual extern Worksheets Worksheets
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
	[DispId(5)]
		[return: MarshalAs(UnmanagedType.Interface)]
		get;
	}

	[DispId(6)]
	public virtual extern string FullName
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
	[DispId(6)]
		[return: MarshalAs(UnmanagedType.BStr)]
		get;
	}

	[DispId(7)]
	public virtual extern string Path
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
	[DispId(7)]
		[return: MarshalAs(UnmanagedType.BStr)]
		get;
	}

	[DispId(8)]
	public virtual extern string DefaultFilePath
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
	[DispId(8)]
		[return: MarshalAs(UnmanagedType.BStr)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
	[DispId(8)]
		[param: In]
		[param: MarshalAs(UnmanagedType.BStr)]
		set;
	}

	[DispId(9)]
	public virtual extern int Height
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
	[DispId(9)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
	[DispId(9)]
		[param: In]
		set;
	}

	[DispId(10)]
	public virtual extern int Width
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
	[DispId(10)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
	[DispId(10)]
		[param: In]
		set;
	}

	[DispId(11)]
	public virtual extern int Left
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
	[DispId(11)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
	[DispId(11)]
		[param: In]
		set;
	}

	[DispId(12)]
	public virtual extern int Top
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
	[DispId(12)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
	[DispId(12)]
		[param: In]
		set;
	}

	[DispId(13)]
	public virtual extern string Version
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
	[DispId(13)]
		[return: MarshalAs(UnmanagedType.BStr)]
		get;
	}

	[DispId(14)]
	public virtual extern bool Visible
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
	[DispId(14)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
	[DispId(14)]
		[param: In]
		set;
	}

	[DispId(15)]
	public virtual extern Windows Windows
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
	[DispId(15)]
		[return: MarshalAs(UnmanagedType.Interface)]
		get;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	public virtual extern void add_Quit(_IApplicationEvents_QuitEventHandler handler);

	[MethodImpl(MethodImplOptions.InternalCall)]
	public virtual extern void remove_Quit(_IApplicationEvents_QuitEventHandler handler);

	[method: MethodImpl(MethodImplOptions.InternalCall)]
	public virtual extern event _IApplicationEvents_WorksheetClosingEventHandler WorksheetClosing;

	[method: MethodImpl(MethodImplOptions.InternalCall)]
	public virtual extern event _IApplicationEvents_WorksheetOpenedEventHandler WorksheetOpened;

	[method: MethodImpl(MethodImplOptions.InternalCall)]
	public virtual extern event _IApplicationEvents_WindowActivatedEventHandler WindowActivated;

	[method: MethodImpl(MethodImplOptions.InternalCall)]
	public virtual extern event _IApplicationEvents_WindowDeactivatedEventHandler WindowDeactivated;

	[method: MethodImpl(MethodImplOptions.InternalCall)]
	public virtual extern event _IApplicationEvents_WorksheetSavingEventHandler WorksheetSaving;

	[method: MethodImpl(MethodImplOptions.InternalCall)]
	public virtual extern event _IApplicationEvents_WorksheetSavedEventHandler WorksheetSaved;

	[method: MethodImpl(MethodImplOptions.InternalCall)]
	public virtual extern event _IApplicationEvents_WorksheetClosedEventHandler WorksheetClosed;

	[method: MethodImpl(MethodImplOptions.InternalCall)]
	public virtual extern event _IApplicationEvents_IdleEventHandler Idle;

	[MethodImpl(MethodImplOptions.InternalCall)]
	[DispId(16)]
	public virtual extern void Quit([In][ComAliasName("Mathcad.MCSaveOption")] MCSaveOption SaveOption = MCSaveOption.mcPromptToSaveChanges);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[DispId(17)]
	public virtual extern void CloseAll([In][ComAliasName("Mathcad.MCSaveOption")] MCSaveOption SaveOption = MCSaveOption.mcPromptToSaveChanges);
}
