#nullable disable
#pragma warning disable

namespace Quire.Microsoft.Office.Interop.Excel;

public partial interface AppEvents_Event
{
    event AppEvents_WorkbookOpenEventHandler WorkbookOpen;

    event AppEvents_WorkbookActivateEventHandler WorkbookActivate;
}
