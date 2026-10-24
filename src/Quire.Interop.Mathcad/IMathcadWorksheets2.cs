#nullable disable
using System.Collections;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Quire;

[global::Quire.Mirror.MirrorOf("Mathcad.IMathcadWorksheets2, interop.mathcad")]
[ComImport]
[TypeLibType(TypeLibTypeFlags.FDual | TypeLibTypeFlags.FDispatchable)]
[Guid("7BDE1B33-0A1A-11E0-BEEA-11F8E2A8C36C")]
[DefaultMember("Item")]
public interface IMathcadWorksheets2 : IMathcadWorksheets
{
	[DispId(1)]
	new Application Application
	{
		[DispId(1)]
		[return: MarshalAs(UnmanagedType.Interface)]
		get;
	}

	[DispId(2)]
	new int Count
	{
		[DispId(2)]
		get;
	}

	[TypeLibFunc(TypeLibFuncFlags.FRestricted | TypeLibFuncFlags.FHidden)]
	[DispId(-4)]
	[return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "System.Runtime.InteropServices.CustomMarshalers.EnumeratorToEnumVariantMarshaler, CustomMarshalers, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	new IEnumerator GetEnumerator();

	[DispId(3)]
	new Application Parent
	{
		[DispId(3)]
		[return: MarshalAs(UnmanagedType.Interface)]
		get;
	}

	[DispId(4)]
	[return: MarshalAs(UnmanagedType.Interface)]
	new Worksheet Add();

	[DispId(0)]
	[return: MarshalAs(UnmanagedType.Interface)]
	new Worksheet Item([Optional][In][MarshalAs(UnmanagedType.Struct)] object index);

	[DispId(5)]
	[return: MarshalAs(UnmanagedType.Interface)]
	new Worksheet Open([In][MarshalAs(UnmanagedType.BStr)] string pFileName);

	[DispId(6)]
	new void Remove([In][MarshalAs(UnmanagedType.Struct)] object index);

	[DispId(7)]
	[return: MarshalAs(UnmanagedType.Interface)]
	Worksheet AddFromTemplate([In][MarshalAs(UnmanagedType.BStr)] string pTemplateFileName);
}
