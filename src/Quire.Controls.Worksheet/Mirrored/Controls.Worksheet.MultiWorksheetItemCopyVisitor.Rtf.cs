#nullable disable
using System.Collections.Generic;
using System.Windows;
using Quire;
using Quire.Controls.Core;
using Quire.FunctionalitiesLimitation;
using Quire.Services;
using Quire.Wpf;

namespace Quire.Controls.Worksheet;

public partial class MultiWorksheetItemCopyVisitor
{
    public MultiWorksheetItemCopyVisitor(DataObject dataObject, RtfItemWrapperHelper helper, bool copyAsImage)
        : base(helper)
    {
        _dataObject = dataObject;
        DebugTools.DevAssert(copyAsImage, null);
    }

    public override void VisitAll(IEnumerable<IWhiteboardItem> items)
    {
        if (RtfDocConstructor.IsRtfCopyEnabled && FunctionalityService.Instance.IsFunctionalityEnabled(EnumFunctionalityKey.CopyMultipleRegionsAsRtf))
        {
            base.VisitAll(items);
            WpfClipboard.CopyRtfStringToDataObject(_dataObject, Helper.PrepareRtf(RtfItems, isSavingWholeDocument: false).Rtf);
        }
        WpfClipboard.SetDataObjectToClipboard(_dataObject);
    }
}
