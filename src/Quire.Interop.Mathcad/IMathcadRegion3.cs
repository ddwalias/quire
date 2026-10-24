#nullable disable
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Quire;

[global::Quire.Mirror.MirrorOf("Mathcad.IMathcadRegion3, interop.mathcad")]
[ComImport]
[Guid("7BDE4245-0A1A-11E0-96E8-11F8E2A8C36C")]
[TypeLibType(TypeLibTypeFlags.FOleAutomation | TypeLibTypeFlags.FDispatchable)]
public interface IMathcadRegion3 : IMathcadRegion2
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

	[ComAliasName("Mathcad.MCRegionType")]
	[DispId(3)]
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
	new RegionMetadata Metadata
	{
		[return: MarshalAs(UnmanagedType.Interface)]
		get;
	}

	[DispId(8)]
	new MathInterface MathInterface
	{
		[return: MarshalAs(UnmanagedType.Interface)]
		get;
	}

	[return: MarshalAs(UnmanagedType.BStr)]
	string ToString();
}
