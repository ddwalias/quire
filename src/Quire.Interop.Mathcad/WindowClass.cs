#nullable disable
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Quire;

[global::Quire.Mirror.MirrorOf("Mathcad.WindowClass, interop.mathcad")]
[ComImport]
[ClassInterface(ClassInterfaceType.None)]
[Guid("7BDD57E5-0A1A-11E0-8895-11F8E2A8C36C")]
public class WindowClass : IMathcadWindow, Window
{
	[DispId(1)]
	public virtual extern Application Application
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
	[DispId(1)]
		[return: MarshalAs(UnmanagedType.Interface)]
		get;
	}

	[DispId(2)]
	public virtual extern int Height
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
	[DispId(2)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
	[DispId(2)]
		[param: In]
		set;
	}

	[DispId(3)]
	public virtual extern int Left
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
	[DispId(3)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
	[DispId(3)]
		[param: In]
		set;
	}

	[DispId(4)]
	public virtual extern Application Parent
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
	[DispId(4)]
		[return: MarshalAs(UnmanagedType.Interface)]
		get;
	}

	[DispId(5)]
	public virtual extern int Top
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
	[DispId(5)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
	[DispId(5)]
		[param: In]
		set;
	}

	[DispId(6)]
	public virtual extern int Width
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
	[DispId(6)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
	[DispId(6)]
		[param: In]
		set;
	}

	[DispId(7)]
	[ComAliasName("Mathcad.MCWindowState")]
	public virtual extern MCWindowState WindowState
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
	[DispId(7)]
		[return: ComAliasName("Mathcad.MCWindowState")]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
	[DispId(7)]
		[param: In]
		[param: ComAliasName("Mathcad.MCWindowState")]
		set;
	}

	[DispId(8)]
	public virtual extern Worksheet Worksheet
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
	[DispId(8)]
		[return: MarshalAs(UnmanagedType.Interface)]
		get;
	}

	[DispId(9)]
	public virtual extern short Zoom
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
	[DispId(9)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
	[DispId(9)]
		[param: In]
		set;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[DispId(10)]
	public virtual extern void Activate();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[DispId(11)]
	public virtual extern void ScrollTo([In] int X, [In] int Y);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[DispId(12)]
	public virtual extern void ScrollToRegion([In][MarshalAs(UnmanagedType.Interface)] Region pRegion);
}
