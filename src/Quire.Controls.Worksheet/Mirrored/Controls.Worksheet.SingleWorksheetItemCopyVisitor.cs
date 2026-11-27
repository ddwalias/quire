using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Quire;
using Quire.Controls.Core;
using Quire.Controls.ExcelComponent.View;

namespace Quire.Controls.Worksheet;

public partial class SingleWorksheetItemCopyVisitor
{
    private readonly DataObject _dataObject;

    private readonly RtfItemWrapperHelper _helper;

    private readonly bool _copyAsImage;

    public RegionImageSettings GenericImageSettings => RegionImageSettings.DefaultForImageCopy;

    public SingleWorksheetItemCopyVisitor(DataObject dataObject, RtfItemWrapperHelper helper, bool copyAsImage)
    {
        _dataObject = dataObject;
        _helper = helper;
        _copyAsImage = copyAsImage;
    }

    public void Visit(IEquationControl equationControl)
    {
        equationControl.AddToDataObject(_dataObject);
        VisitGeneric(equationControl);
    }

    public void Visit(IRichTextWithEmbeddedItem textBox)
    {
        WpfClipboard.CopyRtfStringToDataObject(_dataObject, _helper.CreateText(textBox).Rtf);
        textBox.CopyWholeRangeXamlToDataObject(_dataObject);
    }

    public void Visit(ICollapsibleAreaControl collapsibleArea)
    {
        VisitGeneric(collapsibleArea);
    }

    private void VisitGeneric(IWhiteboardItem item)
    {
        if (_copyAsImage)
        {
            DebugTools.DevAssert(_helper.CopyItemAsImageToDataObject(_dataObject, item, GenericImageSettings), "CopyItemAsImageToDataObject");
        }
    }

    public void VisitAll(IEnumerable<IWhiteboardItem> items)
    {
        IWhiteboardItem whiteboardItem = items.SingleOrDefault();
        if (DebugTools.AssertNotNull(whiteboardItem).Failed)
        {
            return;
        }
        using (ChartComponentUtils.CreateDisposablePrintHelper(whiteboardItem.Yield(), !GenericImageSettings.ApplyZoom))
        {
            if (whiteboardItem is ISpecialCopyType specialCopyType)
            {
                specialCopyType.Accept(this);
            }
            else
            {
                VisitGeneric(whiteboardItem);
            }
        }
        WpfClipboard.SetDataObjectToClipboard(_dataObject);
    }
}
