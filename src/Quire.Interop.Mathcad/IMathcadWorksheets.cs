#nullable disable
using System.Collections;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Quire;

[global::Quire.Mirror.MirrorOf("Mathcad.IMathcadWorksheets, interop.mathcad")]
[ComImport]
[Guid("7BDDF427-0A1A-11E0-8724-11F8E2A8C36C")]
[DefaultMember("Item")]
[TypeLibType(TypeLibTypeFlags.FDual | TypeLibTypeFlags.FDispatchable)]
public interface IMathcadWorksheets : IEnumerable
{
	[DispId(1)]
	Application Application
	{
		[DispId(1)]
		[return: MarshalAs(UnmanagedType.Interface)]
		get;
	}

	[DispId(2)]
	int Count
	{
		[DispId(2)]
		get;
	}

	[DispId(-4)]
	[TypeLibFunc(TypeLibFuncFlags.FRestricted | TypeLibFuncFlags.FHidden)]
	[return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "System.Runtime.InteropServices.CustomMarshalers.EnumeratorToEnumVariantMarshaler, CustomMarshalers, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	new IEnumerator GetEnumerator();

	[DispId(3)]
	Application Parent
	{
		[DispId(3)]
		[return: MarshalAs(UnmanagedType.Interface)]
		get;
	}

	[DispId(4)]
	[return: MarshalAs(UnmanagedType.Interface)]
	Worksheet Add();

	[DispId(0)]
	[return: MarshalAs(UnmanagedType.Interface)]
	Worksheet Item([Optional][In][MarshalAs(UnmanagedType.Struct)] object index);

	[DispId(5)]
	[return: MarshalAs(UnmanagedType.Interface)]
	Worksheet Open([In][MarshalAs(UnmanagedType.BStr)] string pFileName);

	[DispId(6)]
	void Remove([In][MarshalAs(UnmanagedType.Struct)] object index);
}
