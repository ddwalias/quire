#nullable disable
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Quire;

[global::Quire.Mirror.MirrorOf("Mathcad.IValue, interop.mathcad")]
[ComImport]
[TypeLibType(TypeLibTypeFlags.FDual | TypeLibTypeFlags.FDispatchable)]
[Guid("7BDE6955-0A1A-11E0-91D4-11F8E2A8C36C")]
public interface IValue
{
	[DispId(1)]
	string Type
	{
		[DispId(1)]
		[return: MarshalAs(UnmanagedType.BStr)]
		get;
	}

	[DispId(2)]
	string AsString
	{
		[DispId(2)]
		[return: MarshalAs(UnmanagedType.BStr)]
		get;
	}
}
