#nullable disable
using System.Collections;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Quire;

[global::Quire.Mirror.MirrorOf("Mathcad.CustomMetadataCollectionClass, interop.mathcad")]
[ComImport]
[Guid("7BDD7EF5-0A1A-11E0-8797-11F8E2A8C36C")]
[ClassInterface(ClassInterfaceType.None)]
[DefaultMember("Item")]
public class CustomMetadataCollectionClass : ICustomMetadataCollection, CustomMetadataCollection, IEnumerable
{
	[DispId(1)]
	public virtual extern IMathcadApplication2 Application
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
	[DispId(1)]
		[return: MarshalAs(UnmanagedType.Interface)]
		get;
	}

	[DispId(2)]
	public virtual extern int Count
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
	[DispId(2)]
		get;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[DispId(0)]
	[return: MarshalAs(UnmanagedType.Interface)]
	public virtual extern CustomMetadataItem Item([Optional][In][MarshalAs(UnmanagedType.Struct)] object index);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[TypeLibFunc(TypeLibFuncFlags.FRestricted | TypeLibFuncFlags.FHidden)]
	[DispId(-4)]
	[return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "System.Runtime.InteropServices.CustomMarshalers.EnumeratorToEnumVariantMarshaler, CustomMarshalers, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	public virtual extern IEnumerator GetEnumerator();
}
