using System.Collections.Generic;
using Quire.Controls.Core;

namespace Quire.Controls.Worksheet;

public partial interface IWorksheetItemCopyVisitor
{
    RegionImageSettings GenericImageSettings { get; }

    void VisitAll(IEnumerable<IWhiteboardItem> items);
}
