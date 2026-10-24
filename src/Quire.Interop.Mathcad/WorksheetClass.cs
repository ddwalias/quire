#nullable disable
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Quire;

[global::Quire.Mirror.MirrorOf("Mathcad.WorksheetClass, interop.mathcad")]
[ComImport]
[Guid("786EE514-0A1A-11E0-8337-11F8E2A8C36C")]
[ClassInterface(ClassInterfaceType.None)]
[TypeLibType(TypeLibTypeFlags.FCanCreate)]
[ComSourceInterfaces("Mathcad._IWorksheetEvents\0\0")]
public class WorksheetClass : IMathcadWorksheet, Worksheet, _IWorksheetEvents_Event
{
	[DispId(1)]
	public virtual extern Application Application
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
	[DispId(1)]
		[return: MarshalAs(UnmanagedType.Interface)]
		get;
	}

	[DispId(2)]
	public virtual extern string FullName
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
	[DispId(2)]
		[return: MarshalAs(UnmanagedType.BStr)]
		get;
	}

	[DispId(3)]
	public virtual extern string Name
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
	[DispId(3)]
		[return: MarshalAs(UnmanagedType.BStr)]
		get;
	}

	[DispId(4)]
	public virtual extern bool NeedsSave
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
	[DispId(4)]
		get;
	}

	[DispId(5)]
	public virtual extern Application Parent
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
	[DispId(5)]
		[return: MarshalAs(UnmanagedType.Interface)]
		get;
	}

	[DispId(6)]
	public virtual extern string Path
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
	[DispId(6)]
		[return: MarshalAs(UnmanagedType.BStr)]
		get;
	}

	[DispId(7)]
	public virtual extern Regions Regions
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
	[DispId(7)]
		[return: MarshalAs(UnmanagedType.Interface)]
		get;
	}

	[DispId(8)]
	public virtual extern Windows Windows
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
	[DispId(8)]
		[return: MarshalAs(UnmanagedType.Interface)]
		get;
	}

	[method: MethodImpl(MethodImplOptions.InternalCall)]
	public virtual extern event _IWorksheetEvents_ChangedEventHandler Changed;

	[method: MethodImpl(MethodImplOptions.InternalCall)]
	public virtual extern event _IWorksheetEvents_RecalculatedEventHandler Recalculated;

	[MethodImpl(MethodImplOptions.InternalCall)]
	[DispId(9)]
	public virtual extern void Close([In][ComAliasName("Mathcad.MCSaveOption")] MCSaveOption SaveOption = MCSaveOption.mcPromptToSaveChanges);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[DispId(10)]
	[return: MarshalAs(UnmanagedType.Struct)]
	public virtual extern object GetOption([In][ComAliasName("Mathcad.MCWorksheetOption")] MCWorksheetOption option);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[DispId(11)]
	[return: MarshalAs(UnmanagedType.IDispatch)]
	public virtual extern object GetValue([In][MarshalAs(UnmanagedType.BStr)] string bstrName);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[DispId(12)]
	public virtual extern void PrintAll();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[DispId(13)]
	public virtual extern void Recalculate();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[DispId(14)]
	public virtual extern void Save();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[DispId(15)]
	public virtual extern void SaveAs([In][MarshalAs(UnmanagedType.BStr)] string filespec, [In][ComAliasName("Mathcad.MCFileFormat")] MCFileFormat Format = MCFileFormat.mcMcadCurrentVersion);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[DispId(16)]
	public virtual extern void SetOption([In][ComAliasName("Mathcad.MCWorksheetOption")] MCWorksheetOption option, [In][MarshalAs(UnmanagedType.Struct)] object vValue);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[DispId(17)]
	public virtual extern void SetValue([In][MarshalAs(UnmanagedType.BStr)] string bstrName, [In][MarshalAs(UnmanagedType.Struct)] object vValue);
}
