#nullable disable
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Quire.VSINTERFACESLib;

[global::Quire.Mirror.MirrorOf("VSINTERFACESLib.IVSPropSheet, interop.vsinterfaceslib")]
[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid("7BF3ED25-0A1A-11E0-A49E-11F8E2A8C36C")]
[TypeLibType(TypeLibTypeFlags.FOleAutomation)]
public interface IVSPropSheet
{
	void AddPages(ref int lpfnAddPage, [ComAliasName("VSINTERFACESLib.LONG_PTR")] long lParam);

	void OnApply();
}
