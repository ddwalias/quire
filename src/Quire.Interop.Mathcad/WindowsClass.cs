#nullable disable
using System.Collections;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Quire;

[global::Quire.Mirror.MirrorOf("Mathcad.WindowsClass, interop.mathcad")]
[ComImport]
[ClassInterface(ClassInterfaceType.None)]
[Guid("7BDD57E4-0A1A-11E0-AA11-11F8E2A8C36C")]
public class WindowsClass : IMathcadWindows, Windows, IEnumerable
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
	public virtual extern object Parent
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
	[DispId(2)]
		[return: MarshalAs(UnmanagedType.IDispatch)]
		get;
	}

	// C# has no syntax for parameterized property 'Item'.
	[MethodImpl(MethodImplOptions.InternalCall)]
	[DispId(3)]
	[return: MarshalAs(UnmanagedType.Interface)]
	public virtual extern Window get_Item([MarshalAs(UnmanagedType.Struct)] object index);

	[DispId(4)]
	public virtual extern int Count
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
	[DispId(4)]
		get;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[DispId(-4)]
	[TypeLibFunc(TypeLibFuncFlags.FRestricted | TypeLibFuncFlags.FHidden)]
	[return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "System.Runtime.InteropServices.CustomMarshalers.EnumeratorToEnumVariantMarshaler, CustomMarshalers, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	public virtual extern IEnumerator GetEnumerator();
}
