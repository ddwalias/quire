#nullable disable
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Quire;

[global::Quire.Mirror.MirrorOf("Mathcad.IMetadata, interop.mathcad")]
[ComImport]
[Guid("7BDDCD16-0A1A-11E0-9F6B-11F8E2A8C36C")]
[TypeLibType(TypeLibTypeFlags.FDual | TypeLibTypeFlags.FDispatchable)]
public interface IMetadata
{
	[DispId(1)]
	IMathcadApplication2 Application
	{
		[DispId(1)]
		[return: MarshalAs(UnmanagedType.Interface)]
		get;
	}

	[DispId(2)]
	object Parent
	{
		[DispId(2)]
		[return: MarshalAs(UnmanagedType.IDispatch)]
		get;
	}

	[DispId(3)]
	CustomMetadataCollection CustomItems
	{
		[DispId(3)]
		[return: MarshalAs(UnmanagedType.Interface)]
		get;
	}

	[DispId(4)]
	void AddCustomItem([In][MarshalAs(UnmanagedType.Interface)] CustomMetadataItem pItem);

	[DispId(5)]
	void RemoveCustomItem([In][MarshalAs(UnmanagedType.Struct)] object vItem);
}
