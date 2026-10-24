#nullable disable
using System.Runtime.InteropServices;

namespace Quire;

[global::Quire.Mirror.MirrorOf("Mathcad._IApplicationEvents_Event, interop.mathcad")]
[ComVisible(false)]
[TypeLibType(TypeLibTypeFlags.FHidden)]
[ComEventInterface(typeof(_IApplicationEvents), typeof(_IApplicationEvents_EventProvider))]
public interface _IApplicationEvents_Event
{
	void add_Quit(_IApplicationEvents_QuitEventHandler handler);

	void remove_Quit(_IApplicationEvents_QuitEventHandler handler);

	event _IApplicationEvents_WorksheetClosingEventHandler WorksheetClosing;

	event _IApplicationEvents_WorksheetOpenedEventHandler WorksheetOpened;

	event _IApplicationEvents_WindowActivatedEventHandler WindowActivated;

	event _IApplicationEvents_WindowDeactivatedEventHandler WindowDeactivated;

	event _IApplicationEvents_WorksheetSavingEventHandler WorksheetSaving;

	event _IApplicationEvents_WorksheetSavedEventHandler WorksheetSaved;

	event _IApplicationEvents_WorksheetClosedEventHandler WorksheetClosed;

	event _IApplicationEvents_IdleEventHandler Idle;
}
