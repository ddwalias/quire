#nullable disable
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Quire;

[global::Quire.Mirror.MirrorOf("Mathcad.IMatrixValue, interop.mathcad")]
[ComImport]
[Guid("7BDE9063-0A1A-11E0-BA2A-11F8E2A8C36C")]
[TypeLibType(TypeLibTypeFlags.FDual | TypeLibTypeFlags.FDispatchable)]
public interface IMatrixValue : IValue
{
	[DispId(1)]
	new string Type
	{
		[DispId(1)]
		[return: MarshalAs(UnmanagedType.BStr)]
		get;
	}

	[DispId(2)]
	new string AsString
	{
		[DispId(2)]
		[return: MarshalAs(UnmanagedType.BStr)]
		get;
	}

	[DispId(3)]
	int Rows
	{
		[DispId(3)]
		get;
	}

	[DispId(4)]
	int Cols
	{
		[DispId(4)]
		get;
	}

	[DispId(5)]
	[return: MarshalAs(UnmanagedType.Struct)]
	object GetElement([In] int row, [In] int col);

	[DispId(6)]
	void SetElement([In] int row, [In] int col, [In][MarshalAs(UnmanagedType.Struct)] object val);
}
