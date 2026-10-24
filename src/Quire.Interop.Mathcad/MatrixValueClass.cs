#nullable disable
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Quire;

[global::Quire.Mirror.MirrorOf("Mathcad.MatrixValueClass, interop.mathcad")]
[ComImport]
[TypeLibType(TypeLibTypeFlags.FCanCreate)]
[ClassInterface(ClassInterfaceType.None)]
[Guid("7673B155-0A1A-11E0-9116-11F8E2A8C36C")]
public class MatrixValueClass : IMatrixValue, MatrixValue
{
	[DispId(1)]
	public virtual extern string Type
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
	[DispId(1)]
		[return: MarshalAs(UnmanagedType.BStr)]
		get;
	}

	[DispId(2)]
	public virtual extern string AsString
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
	[DispId(2)]
		[return: MarshalAs(UnmanagedType.BStr)]
		get;
	}

	[DispId(3)]
	public virtual extern int Rows
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
	[DispId(3)]
		get;
	}

	[DispId(4)]
	public virtual extern int Cols
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
	[DispId(4)]
		get;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[DispId(5)]
	[return: MarshalAs(UnmanagedType.Struct)]
	public virtual extern object GetElement([In] int row, [In] int col);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[DispId(6)]
	public virtual extern void SetElement([In] int row, [In] int col, [In][MarshalAs(UnmanagedType.Struct)] object val);
}
