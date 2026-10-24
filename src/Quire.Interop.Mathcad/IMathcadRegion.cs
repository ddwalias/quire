#nullable disable
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Quire;

[global::Quire.Mirror.MirrorOf("Mathcad.IMathcadRegion, interop.mathcad")]
[ComImport]
[TypeLibType(TypeLibTypeFlags.FOleAutomation | TypeLibTypeFlags.FDispatchable)]
[Guid("7BDE1B34-0A1A-11E0-A16D-11F8E2A8C36C")]
public interface IMathcadRegion
{
	[DispId(1)]
	Application Application
	{
		[return: MarshalAs(UnmanagedType.Interface)]
		get;
	}

	[DispId(2)]
	Worksheet Parent
	{
		[return: MarshalAs(UnmanagedType.Interface)]
		get;
	}

	[DispId(3)]
	[ComAliasName("Mathcad.MCRegionType")]
	MCRegionType Type
	{
		[return: ComAliasName("Mathcad.MCRegionType")]
		get;
	}

	[DispId(4)]
	string Tag
	{
		[return: MarshalAs(UnmanagedType.BStr)]
		get;
	}

	[DispId(5)]
	int X
	{
		get;
	}

	[DispId(6)]
	int Y
	{
		get;
	}
}
