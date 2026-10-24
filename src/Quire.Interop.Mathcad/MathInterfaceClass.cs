#nullable disable
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Quire;

[global::Quire.Mirror.MirrorOf("Mathcad.MathInterfaceClass, interop.mathcad")]
[ComImport]
[Guid("7BDD7EF3-0A1A-11E0-A42F-11F8E2A8C36C")]
[ClassInterface(ClassInterfaceType.None)]
public class MathInterfaceClass : IMathInterface, MathInterface
{
	[DispId(1)]
	public virtual extern string XML
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
	[return: MarshalAs(UnmanagedType.BStr)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
	[param: In]
		[param: MarshalAs(UnmanagedType.BStr)]
		set;
	}

	[DispId(2)]
	public virtual extern string UnitsXML
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
	[return: MarshalAs(UnmanagedType.BStr)]
		get;
	}

	[DispId(3)]
	public virtual extern bool HasError
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
	get;
	}

	[DispId(4)]
	public virtual extern string ErrorMsg
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
	[return: MarshalAs(UnmanagedType.BStr)]
		get;
	}
}
