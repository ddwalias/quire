#nullable disable
using System.Runtime.InteropServices;

namespace Quire;

[global::Quire.Mirror.MirrorOf("Mathcad._IWorksheetEvents_Event, interop.mathcad")]
[TypeLibType(TypeLibTypeFlags.FHidden)]
[ComVisible(false)]
[ComEventInterface(typeof(_IWorksheetEvents), typeof(_IWorksheetEvents_EventProvider))]
public interface _IWorksheetEvents_Event
{
	event _IWorksheetEvents_ChangedEventHandler Changed;

	event _IWorksheetEvents_RecalculatedEventHandler Recalculated;
}
