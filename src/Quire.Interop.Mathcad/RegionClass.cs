#nullable disable
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Quire;

[global::Quire.Mirror.MirrorOf("Mathcad.RegionClass, interop.mathcad")]
[ComImport]
[ClassInterface(ClassInterfaceType.None)]
[Guid("7BDD57E7-0A1A-11E0-AA93-11F8E2A8C36C")]
public class RegionClass : IMathcadRegion, Region
{
	[DispId(1)]
	public virtual extern Application Application
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
	[return: MarshalAs(UnmanagedType.Interface)]
		get;
	}

	[DispId(2)]
	public virtual extern Worksheet Parent
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
	[return: MarshalAs(UnmanagedType.Interface)]
		get;
	}

	[DispId(3)]
	[ComAliasName("Mathcad.MCRegionType")]
	public virtual extern MCRegionType Type
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
	[return: ComAliasName("Mathcad.MCRegionType")]
		get;
	}

	[DispId(4)]
	public virtual extern string Tag
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
	[return: MarshalAs(UnmanagedType.BStr)]
		get;
	}

	[DispId(5)]
	public virtual extern int X
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
	get;
	}

	[DispId(6)]
	public virtual extern int Y
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
	get;
	}
}
