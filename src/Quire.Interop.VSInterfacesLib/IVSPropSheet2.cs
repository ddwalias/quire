#nullable disable
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Quire.VSINTERFACESLib;

[global::Quire.Mirror.MirrorOf("VSINTERFACESLib.IVSPropSheet2, interop.vsinterfaceslib")]
[ComImport]
[TypeLibType(TypeLibTypeFlags.FOleAutomation)]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid("7BF3ED26-0A1A-11E0-B951-11F8E2A8C36C")]
public interface IVSPropSheet2 : IVSPropSheet
{
	new void AddPages(ref int lpfnAddPage, [ComAliasName("VSINTERFACESLib.LONG_PTR")] long lParam);

	new void OnApply();

	void ReleasePages();
}
