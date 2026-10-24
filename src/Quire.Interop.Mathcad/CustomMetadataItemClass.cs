#nullable disable
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Quire;

[global::Quire.Mirror.MirrorOf("Mathcad.CustomMetadataItemClass, interop.mathcad")]
[ComImport]
[ClassInterface(ClassInterfaceType.None)]
[Guid("784C9002-0A1A-11E0-B397-11F8E2A8C36C")]
[DefaultMember("Value")]
[TypeLibType(TypeLibTypeFlags.FCanCreate)]
public class CustomMetadataItemClass : ICustomMetadataItem, CustomMetadataItem
{
	[DispId(1)]
	public virtual extern string Name
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
	[DispId(1)]
		[return: MarshalAs(UnmanagedType.BStr)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
	[DispId(1)]
		[param: In]
		[param: MarshalAs(UnmanagedType.BStr)]
		set;
	}

	[DispId(2)]
	[ComAliasName("Mathcad.MCCustomMetadataType")]
	public virtual extern MCCustomMetadataType Type
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
	[DispId(2)]
		[return: ComAliasName("Mathcad.MCCustomMetadataType")]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
	[DispId(2)]
		[param: In]
		[param: ComAliasName("Mathcad.MCCustomMetadataType")]
		set;
	}

	[DispId(0)]
	public virtual extern object Value
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
	[DispId(0)]
		[return: MarshalAs(UnmanagedType.Struct)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
	[DispId(0)]
		[param: In]
		[param: MarshalAs(UnmanagedType.Struct)]
		set;
	}
}
