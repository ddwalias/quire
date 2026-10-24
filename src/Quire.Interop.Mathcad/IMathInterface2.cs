#nullable disable
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Quire;

[global::Quire.Mirror.MirrorOf("Mathcad.IMathInterface2, interop.mathcad")]
[ComImport]
[Guid("7BDE1B36-0A1A-11E0-B16A-11F8E2A8C36C")]
[TypeLibType(TypeLibTypeFlags.FOleAutomation | TypeLibTypeFlags.FDispatchable)]
public interface IMathInterface2 : IMathInterface
{
	[DispId(1)]
	new string XML
	{
		[return: MarshalAs(UnmanagedType.BStr)]
		get;
		[param: In]
		[param: MarshalAs(UnmanagedType.BStr)]
		set;
	}

	[DispId(2)]
	new string UnitsXML
	{
		[return: MarshalAs(UnmanagedType.BStr)]
		get;
	}

	[DispId(3)]
	new bool HasError
	{
		get;
	}

	[DispId(4)]
	new string ErrorMsg
	{
		[return: MarshalAs(UnmanagedType.BStr)]
		get;
	}

	[DispId(5)]
	Value Result
	{
		[return: MarshalAs(UnmanagedType.Interface)]
		get;
	}
}
