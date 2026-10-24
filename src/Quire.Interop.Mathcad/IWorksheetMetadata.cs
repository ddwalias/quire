#nullable disable
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Quire;

[global::Quire.Mirror.MirrorOf("Mathcad.IWorksheetMetadata, interop.mathcad")]
[ComImport]
[Guid("7BDDF423-0A1A-11E0-9E84-11F8E2A8C36C")]
[TypeLibType(TypeLibTypeFlags.FDual | TypeLibTypeFlags.FDispatchable)]
public interface IWorksheetMetadata : IMetadata
{
	[DispId(1)]
	new IMathcadApplication2 Application
	{
		[DispId(1)]
		[return: MarshalAs(UnmanagedType.Interface)]
		get;
	}

	[DispId(2)]
	new object Parent
	{
		[DispId(2)]
		[return: MarshalAs(UnmanagedType.IDispatch)]
		get;
	}

	[DispId(3)]
	new CustomMetadataCollection CustomItems
	{
		[DispId(3)]
		[return: MarshalAs(UnmanagedType.Interface)]
		get;
	}

	[DispId(4)]
	new void AddCustomItem([In][MarshalAs(UnmanagedType.Interface)] CustomMetadataItem pItem);

	[DispId(5)]
	new void RemoveCustomItem([In][MarshalAs(UnmanagedType.Struct)] object vItem);

	[DispId(10)]
	string Author
	{
		[DispId(10)]
		[return: MarshalAs(UnmanagedType.BStr)]
		get;
		[DispId(10)]
		[param: In]
		[param: MarshalAs(UnmanagedType.BStr)]
		set;
	}

	[DispId(11)]
	string Company
	{
		[DispId(11)]
		[return: MarshalAs(UnmanagedType.BStr)]
		get;
		[DispId(11)]
		[param: In]
		[param: MarshalAs(UnmanagedType.BStr)]
		set;
	}

	[DispId(12)]
	string Description
	{
		[DispId(12)]
		[return: MarshalAs(UnmanagedType.BStr)]
		get;
		[DispId(12)]
		[param: In]
		[param: MarshalAs(UnmanagedType.BStr)]
		set;
	}

	[DispId(13)]
	string DocumentID
	{
		[DispId(13)]
		[return: MarshalAs(UnmanagedType.BStr)]
		get;
	}

	[DispId(14)]
	string Keywords
	{
		[DispId(14)]
		[return: MarshalAs(UnmanagedType.BStr)]
		get;
		[DispId(14)]
		[param: In]
		[param: MarshalAs(UnmanagedType.BStr)]
		set;
	}

	[DispId(15)]
	string RevisedBy
	{
		[DispId(15)]
		[return: MarshalAs(UnmanagedType.BStr)]
		get;
		[DispId(15)]
		[param: In]
		[param: MarshalAs(UnmanagedType.BStr)]
		set;
	}

	[DispId(16)]
	int Revision
	{
		[DispId(16)]
		get;
	}

	[DispId(17)]
	string Title
	{
		[DispId(17)]
		[return: MarshalAs(UnmanagedType.BStr)]
		get;
		[DispId(17)]
		[param: In]
		[param: MarshalAs(UnmanagedType.BStr)]
		set;
	}

	[DispId(18)]
	string VersionID
	{
		[DispId(18)]
		[return: MarshalAs(UnmanagedType.BStr)]
		get;
	}

	[DispId(19)]
	string ParentVersionID
	{
		[DispId(19)]
		[return: MarshalAs(UnmanagedType.BStr)]
		get;
	}

	[DispId(20)]
	string BranchID
	{
		[DispId(20)]
		[return: MarshalAs(UnmanagedType.BStr)]
		get;
	}
}
