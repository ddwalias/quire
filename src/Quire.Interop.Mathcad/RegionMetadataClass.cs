#nullable disable
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Quire;

[global::Quire.Mirror.MirrorOf("Mathcad.RegionMetadataClass, interop.mathcad")]
[ComImport]
[Guid("7BDD7EF7-0A1A-11E0-A1DF-11F8E2A8C36C")]
[ClassInterface(ClassInterfaceType.None)]
public class RegionMetadataClass : IRegionMetadata, RegionMetadata
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

	[MethodImpl(MethodImplOptions.InternalCall)]
	[DispId(4)]
	public virtual extern void AddCustomItem([In][MarshalAs(UnmanagedType.Interface)] CustomMetadataItem pItem);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[DispId(5)]
	public virtual extern void RemoveCustomItem([In][MarshalAs(UnmanagedType.Struct)] object vItem);
}
