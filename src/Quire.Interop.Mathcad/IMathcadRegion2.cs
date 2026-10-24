#nullable disable
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Quire;

[global::Quire.Mirror.MirrorOf("Mathcad.IMathcadRegion2, interop.mathcad")]
[ComImport]
[Guid("7BDE1B37-0A1A-11E0-AC99-11F8E2A8C36C")]
[TypeLibType(TypeLibTypeFlags.FOleAutomation | TypeLibTypeFlags.FDispatchable)]
public interface IMathcadRegion2 : IMathcadRegion
{
	[DispId(1)]
	new Application Application
	{
		[return: MarshalAs(UnmanagedType.Interface)]
		get;
	}

	[DispId(2)]
	new Worksheet Parent
	{
		[return: MarshalAs(UnmanagedType.Interface)]
		get;
	}

	[DispId(3)]
	[ComAliasName("Mathcad.MCRegionType")]
	new MCRegionType Type
	{
		[return: ComAliasName("Mathcad.MCRegionType")]
		get;
	}

	[DispId(4)]
	new string Tag
	{
		[return: MarshalAs(UnmanagedType.BStr)]
		get;
	}

	[DispId(5)]
	new int X
	{
		get;
	}

	[DispId(6)]
	new int Y
	{
		get;
	}

	[DispId(7)]
	RegionMetadata Metadata
	{
		[return: MarshalAs(UnmanagedType.Interface)]
		get;
	}

	[DispId(8)]
	MathInterface MathInterface
	{
		[return: MarshalAs(UnmanagedType.Interface)]
		get;
	}
}
