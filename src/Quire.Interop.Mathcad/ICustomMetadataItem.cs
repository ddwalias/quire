#nullable disable
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Quire;

[global::Quire.Mirror.MirrorOf("Mathcad.ICustomMetadataItem, interop.mathcad")]
[ComImport]
[TypeLibType(TypeLibTypeFlags.FDual | TypeLibTypeFlags.FDispatchable)]
[DefaultMember("Value")]
[Guid("7BDDCD14-0A1A-11E0-B5AA-11F8E2A8C36C")]
public interface ICustomMetadataItem
{
	[DispId(1)]
	string Name
	{
		[DispId(1)]
		[return: MarshalAs(UnmanagedType.BStr)]
		get;
		[DispId(1)]
		[param: In]
		[param: MarshalAs(UnmanagedType.BStr)]
		set;
	}

	[ComAliasName("Mathcad.MCCustomMetadataType")]
	[DispId(2)]
	MCCustomMetadataType Type
	{
		[DispId(2)]
		[return: ComAliasName("Mathcad.MCCustomMetadataType")]
		get;
		[DispId(2)]
		[param: In]
		[param: ComAliasName("Mathcad.MCCustomMetadataType")]
		set;
	}

	[DispId(0)]
	object Value
	{
		[DispId(0)]
		[return: MarshalAs(UnmanagedType.Struct)]
		get;
		[DispId(0)]
		[param: In]
		[param: MarshalAs(UnmanagedType.Struct)]
		set;
	}
}
