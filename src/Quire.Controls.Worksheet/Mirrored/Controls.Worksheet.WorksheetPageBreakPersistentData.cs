using System;
using System.Windows;
using Quire;
using Quire.Controls.Core;

namespace Quire.Controls.Worksheet;

public partial class WorksheetPageBreakPersistentData : RegionPersistentData
{
    public WorksheetPageBreakPersistentData(IWorksheetPageBreak pageBreak)
        : base(pageBreak as FrameworkElement)
    {
    }
}
