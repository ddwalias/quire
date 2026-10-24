#nullable disable
using System.Collections;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Quire;

[global::Quire.Mirror.MirrorOf("Mathcad.WorksheetsClass, interop.mathcad")]
[ComImport]
[DefaultMember("Item")]
[ClassInterface(ClassInterfaceType.None)]
[ComSourceInterfaces("Mathcad._IWorksheetEvents\0\0")]
[Guid("7BDD57E3-0A1A-11E0-8520-11F8E2A8C36C")]
public class WorksheetsClass : IMathcadWorksheets, Worksheets, _IWorksheetEvents_Event, IEnumerable
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
	public virtual extern int Count
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
	[DispId(2)]
		get;
	}

	[DispId(3)]
	public virtual extern Application Parent
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
	[DispId(3)]
		[return: MarshalAs(UnmanagedType.Interface)]
		get;
	}

	[method: MethodImpl(MethodImplOptions.InternalCall)]
	public virtual extern event _IWorksheetEvents_ChangedEventHandler Changed;

	[method: MethodImpl(MethodImplOptions.InternalCall)]
	public virtual extern event _IWorksheetEvents_RecalculatedEventHandler Recalculated;

	[MethodImpl(MethodImplOptions.InternalCall)]
	[DispId(-4)]
	[TypeLibFunc(TypeLibFuncFlags.FRestricted | TypeLibFuncFlags.FHidden)]
	[return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "System.Runtime.InteropServices.CustomMarshalers.EnumeratorToEnumVariantMarshaler, CustomMarshalers, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	public virtual extern IEnumerator GetEnumerator();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[DispId(4)]
	[return: MarshalAs(UnmanagedType.Interface)]
	public virtual extern Worksheet Add();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[DispId(0)]
	[return: MarshalAs(UnmanagedType.Interface)]
	public virtual extern Worksheet Item([Optional][In][MarshalAs(UnmanagedType.Struct)] object index);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[DispId(5)]
	[return: MarshalAs(UnmanagedType.Interface)]
	public virtual extern Worksheet Open([In][MarshalAs(UnmanagedType.BStr)] string pFileName);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[DispId(6)]
	public virtual extern void Remove([In][MarshalAs(UnmanagedType.Struct)] object index);
}
