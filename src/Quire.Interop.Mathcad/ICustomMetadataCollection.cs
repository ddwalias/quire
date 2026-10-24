#nullable disable
using System.Collections;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Quire;

[global::Quire.Mirror.MirrorOf("Mathcad.ICustomMetadataCollection, interop.mathcad")]
[ComImport]
[TypeLibType(TypeLibTypeFlags.FDual | TypeLibTypeFlags.FDispatchable)]
[Guid("7BDDCD15-0A1A-11E0-9195-11F8E2A8C36C")]
[DefaultMember("Item")]
public interface ICustomMetadataCollection : IEnumerable
{
	[DispId(1)]
	IMathcadApplication2 Application
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

	[DispId(0)]
	[return: MarshalAs(UnmanagedType.Interface)]
	CustomMetadataItem Item([Optional][In][MarshalAs(UnmanagedType.Struct)] object index);

	[TypeLibFunc(TypeLibFuncFlags.FRestricted | TypeLibFuncFlags.FHidden)]
	[DispId(-4)]
	[return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "System.Runtime.InteropServices.CustomMarshalers.EnumeratorToEnumVariantMarshaler, CustomMarshalers, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	new IEnumerator GetEnumerator();
}
