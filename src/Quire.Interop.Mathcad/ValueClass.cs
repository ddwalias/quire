#nullable disable
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Quire;

[global::Quire.Mirror.MirrorOf("Mathcad.ValueClass, interop.mathcad")]
[ComImport]
[Guid("7BDD30D4-0A1A-11E0-BBB9-11F8E2A8C36C")]
[ClassInterface(ClassInterfaceType.None)]
public class ValueClass : IValue, Value
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
}
