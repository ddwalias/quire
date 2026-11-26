#nullable disable
using System.Collections.Generic;
using System.IO;
using Quire;
using Quire.Controls.Core;
using Quire.FunctionalitiesLimitation;
using Quire.IO;
using Quire.Services;

namespace Quire.Controls.Worksheet;

public partial class SaveAsRtfWorksheetItemsVisitor
{
    public SaveAsRtfWorksheetItemsVisitor(string fileName, RtfItemWrapperHelper helper)
        : base(helper)
    {
        _fileName = fileName;
    }

    public override void VisitAll(IEnumerable<IWhiteboardItem> items)
    {
        DebugTools.DevAssert(FunctionalityService.Instance.IsFunctionalityEnabled(EnumFunctionalityKey.SaveEngineeringDocumentAsRtfCommand), null);
        base.VisitAll(items);
        try
        {
            File.WriteAllText(_fileName, Helper.PrepareRtf(RtfItems, isSavingWholeDocument: true).Rtf);
        }
        catch (IOException exception)
        {
            throw new FileIsUsedByAnotherProcessException(exception);
        }
    }
}
