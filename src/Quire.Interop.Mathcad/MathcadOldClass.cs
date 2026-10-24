#nullable disable
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Quire;

[global::Quire.Mirror.MirrorOf("Mathcad.MathcadOldClass, interop.mathcad")]
[ComImport]
[TypeLibType(TypeLibTypeFlags.FCanCreate)]
[Guid("7BDD7EF4-0A1A-11E0-9AE1-11F8E2A8C36C")]
[ClassInterface(ClassInterfaceType.None)]
public class MathcadOldClass : IMathcad, MathcadOld
{
	[DispId(5)]
	public virtual extern Worksheet Worksheet
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
	[DispId(5)]
		[return: MarshalAs(UnmanagedType.Interface)]
		get;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[DispId(1)]
	public virtual extern int Recalculate();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[DispId(2)]
	public virtual extern int GetComplex([In][MarshalAs(UnmanagedType.BStr)] string Name, [MarshalAs(UnmanagedType.Struct)] out object RealPart, [MarshalAs(UnmanagedType.Struct)] out object ImagPart);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[DispId(3)]
	public virtual extern int SetComplex([In][MarshalAs(UnmanagedType.BStr)] string Name, [In][MarshalAs(UnmanagedType.Struct)] object RealPart, [In][MarshalAs(UnmanagedType.Struct)] object ImagPart);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[DispId(4)]
	public virtual extern int SaveAs([Optional][In][MarshalAs(UnmanagedType.Struct)] object Name);
}
