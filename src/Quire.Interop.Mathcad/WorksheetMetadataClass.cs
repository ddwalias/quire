#nullable disable
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Quire;

[global::Quire.Mirror.MirrorOf("Mathcad.WorksheetMetadataClass, interop.mathcad")]
[ComImport]
[Guid("7BDD7EF6-0A1A-11E0-84A4-11F8E2A8C36C")]
[ClassInterface(ClassInterfaceType.None)]
public class WorksheetMetadataClass : IWorksheetMetadata, WorksheetMetadata
{
	[DispId(1)]
	public virtual extern IMathcadApplication2 Application
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
	[DispId(1)]
		[return: MarshalAs(UnmanagedType.Interface)]
		get;
	}

	[DispId(2)]
	public virtual extern object Parent
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
	[DispId(2)]
		[return: MarshalAs(UnmanagedType.IDispatch)]
		get;
	}

	[DispId(3)]
	public virtual extern CustomMetadataCollection CustomItems
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
	[DispId(3)]
		[return: MarshalAs(UnmanagedType.Interface)]
		get;
	}

	[DispId(10)]
	public virtual extern string Author
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
	[DispId(10)]
		[return: MarshalAs(UnmanagedType.BStr)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
	[DispId(10)]
		[param: In]
		[param: MarshalAs(UnmanagedType.BStr)]
		set;
	}

	[DispId(11)]
	public virtual extern string Company
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
	[DispId(11)]
		[return: MarshalAs(UnmanagedType.BStr)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
	[DispId(11)]
		[param: In]
		[param: MarshalAs(UnmanagedType.BStr)]
		set;
	}

	[DispId(12)]
	public virtual extern string Description
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
	[DispId(12)]
		[return: MarshalAs(UnmanagedType.BStr)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
	[DispId(12)]
		[param: In]
		[param: MarshalAs(UnmanagedType.BStr)]
		set;
	}

	[DispId(13)]
	public virtual extern string DocumentID
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
	[DispId(13)]
		[return: MarshalAs(UnmanagedType.BStr)]
		get;
	}

	[DispId(14)]
	public virtual extern string Keywords
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
	[DispId(14)]
		[return: MarshalAs(UnmanagedType.BStr)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
	[DispId(14)]
		[param: In]
		[param: MarshalAs(UnmanagedType.BStr)]
		set;
	}

	[DispId(15)]
	public virtual extern string RevisedBy
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
	[DispId(15)]
		[return: MarshalAs(UnmanagedType.BStr)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
	[DispId(15)]
		[param: In]
		[param: MarshalAs(UnmanagedType.BStr)]
		set;
	}

	[DispId(16)]
	public virtual extern int Revision
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
	[DispId(16)]
		get;
	}

	[DispId(17)]
	public virtual extern string Title
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
	[DispId(17)]
		[return: MarshalAs(UnmanagedType.BStr)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
	[DispId(17)]
		[param: In]
		[param: MarshalAs(UnmanagedType.BStr)]
		set;
	}

	[DispId(18)]
	public virtual extern string VersionID
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
	[DispId(18)]
		[return: MarshalAs(UnmanagedType.BStr)]
		get;
	}

	[DispId(19)]
	public virtual extern string ParentVersionID
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
	[DispId(19)]
		[return: MarshalAs(UnmanagedType.BStr)]
		get;
	}

	[DispId(20)]
	public virtual extern string BranchID
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
	[DispId(20)]
		[return: MarshalAs(UnmanagedType.BStr)]
		get;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[DispId(4)]
	public virtual extern void AddCustomItem([In][MarshalAs(UnmanagedType.Interface)] CustomMetadataItem pItem);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[DispId(5)]
	public virtual extern void RemoveCustomItem([In][MarshalAs(UnmanagedType.Struct)] object vItem);
}
