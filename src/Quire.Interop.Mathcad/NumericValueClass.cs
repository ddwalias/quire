#nullable disable
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Quire;

[global::Quire.Mirror.MirrorOf("Mathcad.NumericValueClass, interop.mathcad")]
[ComImport]
[Guid("7673D863-0A1A-11E0-A87E-11F8E2A8C36C")]
[TypeLibType(TypeLibTypeFlags.FCanCreate)]
[ClassInterface(ClassInterfaceType.None)]
[DefaultMember("Real")]
public class NumericValueClass : INumericValue, NumericValue
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

	[DispId(0)]
	public virtual extern double Real
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
	[DispId(0)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
	[DispId(0)]
		[param: In]
		set;
	}

	[DispId(3)]
	public virtual extern double Imag
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
	[DispId(3)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
	[DispId(3)]
		[param: In]
		set;
	}

	[DispId(4)]
	public virtual extern int Integer
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
	[DispId(4)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
	[DispId(4)]
		[param: In]
		set;
	}
}
