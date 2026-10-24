#nullable disable
using System.Collections;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Quire;

[global::Quire.Mirror.MirrorOf("Mathcad.IMathcadRegions, interop.mathcad")]
[ComImport]
[Guid("7BDE4246-0A1A-11E0-99B7-11F8E2A8C36C")]
[DefaultMember("Item")]
[TypeLibType(TypeLibTypeFlags.FDual | TypeLibTypeFlags.FDispatchable)]
public interface IMathcadRegions : IEnumerable
{
	[DispId(0)]
	[return: MarshalAs(UnmanagedType.Interface)]
	Region Item([Optional][In][MarshalAs(UnmanagedType.Struct)] object index);

	[DispId(1)]
	int Count
	{
		[DispId(1)]
		get;
	}

	[TypeLibFunc(TypeLibFuncFlags.FRestricted | TypeLibFuncFlags.FHidden)]
	[DispId(-4)]
	[return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "System.Runtime.InteropServices.CustomMarshalers.EnumeratorToEnumVariantMarshaler, CustomMarshalers, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	new IEnumerator GetEnumerator();
}
