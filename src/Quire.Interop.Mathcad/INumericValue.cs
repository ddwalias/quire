#nullable disable
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Quire;

[global::Quire.Mirror.MirrorOf("Mathcad.INumericValue, interop.mathcad")]
[ComImport]
[Guid("7BDE6956-0A1A-11E0-8F53-11F8E2A8C36C")]
[DefaultMember("Real")]
[TypeLibType(TypeLibTypeFlags.FDual | TypeLibTypeFlags.FDispatchable)]
public interface INumericValue : IValue
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

	[DispId(0)]
	double Real
	{
		[DispId(0)]
		get;
		[DispId(0)]
		[param: In]
		set;
	}

	[DispId(3)]
	double Imag
	{
		[DispId(3)]
		get;
		[DispId(3)]
		[param: In]
		set;
	}

	[DispId(4)]
	int Integer
	{
		[DispId(4)]
		get;
		[DispId(4)]
		[param: In]
		set;
	}
}
