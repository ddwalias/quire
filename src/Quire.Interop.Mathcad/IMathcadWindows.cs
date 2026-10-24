#nullable disable
using System.Collections;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Quire;

[global::Quire.Mirror.MirrorOf("Mathcad.IMathcadWindows, interop.mathcad")]
[ComImport]
[Guid("7BDEB775-0A1A-11E0-AC76-11F8E2A8C36C")]
[TypeLibType(TypeLibTypeFlags.FDual | TypeLibTypeFlags.FDispatchable)]
public interface IMathcadWindows : IEnumerable
{
	[DispId(1)]
	Application Application
	{
		[DispId(1)]
		[return: MarshalAs(UnmanagedType.Interface)]
		get;
	}

	[DispId(2)]
	object Parent
	{
		[DispId(2)]
		[return: MarshalAs(UnmanagedType.IDispatch)]
		get;
	}

	// C# has no syntax for parameterized property 'Item'.
	[DispId(3)]
	[return: MarshalAs(UnmanagedType.Interface)]
	Window get_Item([MarshalAs(UnmanagedType.Struct)] object index);

	[DispId(4)]
	int Count
	{
		[DispId(4)]
		get;
	}

	[DispId(-4)]
	[TypeLibFunc(TypeLibFuncFlags.FRestricted | TypeLibFuncFlags.FHidden)]
	[return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "System.Runtime.InteropServices.CustomMarshalers.EnumeratorToEnumVariantMarshaler, CustomMarshalers, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	new IEnumerator GetEnumerator();
}
