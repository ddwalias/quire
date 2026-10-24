#nullable disable
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Quire;

[global::Quire.Mirror.MirrorOf("Mathcad.IMathInterface, interop.mathcad")]
[ComImport]
[Guid("7BDE1B35-0A1A-11E0-9EAB-11F8E2A8C36C")]
[TypeLibType(TypeLibTypeFlags.FOleAutomation | TypeLibTypeFlags.FDispatchable)]
public interface IMathInterface
{
	[DispId(1)]
	string XML
	{
		[return: MarshalAs(UnmanagedType.BStr)]
		get;
		[param: In]
		[param: MarshalAs(UnmanagedType.BStr)]
		set;
	}

	[DispId(2)]
	string UnitsXML
	{
		[return: MarshalAs(UnmanagedType.BStr)]
		get;
	}

	[DispId(3)]
	bool HasError
	{
		get;
	}

	[DispId(4)]
	string ErrorMsg
	{
		[return: MarshalAs(UnmanagedType.BStr)]
		get;
	}
}
