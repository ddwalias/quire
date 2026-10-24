#nullable disable
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Quire;

[global::Quire.Mirror.MirrorOf("Mathcad.IMathcadWindow, interop.mathcad")]
[ComImport]
[Guid("7BDE9064-0A1A-11E0-9E91-11F8E2A8C36C")]
[TypeLibType(TypeLibTypeFlags.FDual | TypeLibTypeFlags.FDispatchable)]
public interface IMathcadWindow
{
	[DispId(1)]
	Application Application
	{
		[DispId(1)]
		[return: MarshalAs(UnmanagedType.Interface)]
		get;
	}

	[DispId(2)]
	int Height
	{
		[DispId(2)]
		get;
		[DispId(2)]
		[param: In]
		set;
	}

	[DispId(3)]
	int Left
	{
		[DispId(3)]
		get;
		[DispId(3)]
		[param: In]
		set;
	}

	[DispId(4)]
	Application Parent
	{
		[DispId(4)]
		[return: MarshalAs(UnmanagedType.Interface)]
		get;
	}

	[DispId(5)]
	int Top
	{
		[DispId(5)]
		get;
		[DispId(5)]
		[param: In]
		set;
	}

	[DispId(6)]
	int Width
	{
		[DispId(6)]
		get;
		[DispId(6)]
		[param: In]
		set;
	}

	[ComAliasName("Mathcad.MCWindowState")]
	[DispId(7)]
	MCWindowState WindowState
	{
		[DispId(7)]
		[return: ComAliasName("Mathcad.MCWindowState")]
		get;
		[DispId(7)]
		[param: In]
		[param: ComAliasName("Mathcad.MCWindowState")]
		set;
	}

	[DispId(8)]
	Worksheet Worksheet
	{
		[DispId(8)]
		[return: MarshalAs(UnmanagedType.Interface)]
		get;
	}

	[DispId(9)]
	short Zoom
	{
		[DispId(9)]
		get;
		[DispId(9)]
		[param: In]
		set;
	}

	[DispId(10)]
	void Activate();

	[DispId(11)]
	void ScrollTo([In] int X, [In] int Y);

	[DispId(12)]
	void ScrollToRegion([In][MarshalAs(UnmanagedType.Interface)] Region pRegion);
}
