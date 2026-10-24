#nullable disable
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Quire;

[global::Quire.Mirror.MirrorOf("Mathcad.IMathcad, interop.mathcad")]
[ComImport]
[Guid("7BDEB776-0A1A-11E0-882B-11F8E2A8C36C")]
[TypeLibType(TypeLibTypeFlags.FDual | TypeLibTypeFlags.FDispatchable)]
public interface IMathcad
{
	[DispId(1)]
	int Recalculate();

	[DispId(2)]
	int GetComplex([In][MarshalAs(UnmanagedType.BStr)] string Name, [MarshalAs(UnmanagedType.Struct)] out object RealPart, [MarshalAs(UnmanagedType.Struct)] out object ImagPart);

	[DispId(3)]
	int SetComplex([In][MarshalAs(UnmanagedType.BStr)] string Name, [In][MarshalAs(UnmanagedType.Struct)] object RealPart, [In][MarshalAs(UnmanagedType.Struct)] object ImagPart);

	[DispId(4)]
	int SaveAs([Optional][In][MarshalAs(UnmanagedType.Struct)] object Name);

	[DispId(5)]
	Worksheet Worksheet
	{
		[DispId(5)]
		[return: MarshalAs(UnmanagedType.Interface)]
		get;
	}
}
