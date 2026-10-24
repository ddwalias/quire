#nullable disable
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Quire.VSINTERFACESLib;

[global::Quire.Mirror.MirrorOf("VSINTERFACESLib.IVSUnitData, interop.vsinterfaceslib")]
[ComImport]
[Guid("7BF043A3-0A1A-11E0-B227-11F8E2A8C36C")]
[TypeLibType(TypeLibTypeFlags.FOleAutomation)]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface IVSUnitData
{
	void GetNumBaseUnits(ref int res);

	void GetUnitName(int unit_num, [MarshalAs(UnmanagedType.BStr)] ref string result);

	void GetUnitValue(int unit_num, ref double result);

	void SetUnitValue(int unit_num, double val);

	void SetUnitSystem(int unit_sys);

	void GetUnitSystem(ref int unit_sys);

	[DispId(1610678278)]
	bool IsDimensionless
	{
		get;
	}
}
