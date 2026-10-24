#nullable disable
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Quire;

[global::Quire.Mirror.MirrorOf("Mathcad.IRegionMetadata, interop.mathcad")]
[ComImport]
[TypeLibType(TypeLibTypeFlags.FDual | TypeLibTypeFlags.FDispatchable)]
[Guid("7BDDF424-0A1A-11E0-AD90-11F8E2A8C36C")]
public interface IRegionMetadata : IMetadata
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
}
