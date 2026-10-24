#nullable disable
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Quire;

[global::Quire.Mirror.MirrorOf("Mathcad._IWorksheetEvents, interop.mathcad")]
[ComImport]
[Guid("7BDD09C2-0A1A-11E0-83DD-11F8E2A8C36C")]
[TypeLibType(TypeLibTypeFlags.FDispatchable)]
[InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
public interface _IWorksheetEvents
{
	[DispId(1)]
	void Changed();

	[DispId(2)]
	void Recalculated();
}
