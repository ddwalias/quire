#nullable disable
using System.Collections.Generic;
using Quire;
using Quire.Controls.Core;
using Quire.Controls.ExcelComponent.View;

namespace Quire.Controls.Worksheet;

public partial class MultiWorksheetItemRtfCopyVisitorBase
{
    protected readonly List<RtfItemBase> RtfItems = new List<RtfItemBase>();

    protected readonly RtfItemWrapperHelper Helper;

    public RegionImageSettings GenericImageSettings => RegionImageSettings.DefaultForRtfCopy;

    protected MultiWorksheetItemRtfCopyVisitorBase(RtfItemWrapperHelper helper)
    {
        Helper = helper;
    }

    public virtual void Visit(IRichTextWithEmbeddedItem textBox)
    {
        RtfItems.Add(Helper.CreateRtfItemModel(textBox));
    }

    protected virtual void VisitGeneric(IWhiteboardItem item)
    {
        RtfItems.Add(Helper.CreateRtfItemImage(item, GenericImageSettings));
    }

    public virtual void Visit(IEquationControl equation)
    {
        VisitGeneric(equation);
    }

    public virtual void Visit(ICollapsibleAreaControl area)
    {
        bool hasErrorLine = !area.IsBorderVisible && area.AreaHasErrors;
        bool hasBorderOrErrorLine = area.IsBorderVisible | hasErrorLine;
        if (hasBorderOrErrorLine)
        {
            RtfItems.Add(Helper.CreateAreaSeparateLine(area, onTop: true));
        }
        if (area.IsCollapsed)
        {
            return;
        }
        Helper.PreAreaInWorksheetProcessing(area);
        ConvertItemsToRtfFormat(area.WhiteboardItems);
        Helper.PostAreaInWorksheetProcessing();
        if (hasBorderOrErrorLine)
        {
            RtfItems.Add(Helper.CreateAreaSeparateLine(area, onTop: false));
        }
    }

    private void ConvertItemsToRtfFormat(IEnumerable<IWhiteboardItem> items)
    {
        items.Each(item =>
        {
            if (item is ISpecialCopyType)
            {
                (item as ISpecialCopyType).Accept(this);
            }
            else
            {
                VisitGeneric(item);
            }
        });
    }

    public virtual void VisitAll(IEnumerable<IWhiteboardItem> items)
    {
        if (items.IsNullOrEmpty())
        {
            return;
        }
        Helper.PrePrintUpdateWorksheetLayout(items);
        try
        {
            using (ChartComponentUtils.CreateDisposablePrintHelper(items, !GenericImageSettings.ApplyZoom))
            {
                ConvertItemsToRtfFormat(items);
            }
        }
        finally
        {
            Helper.PostPrintUpdateWorksheetLayout();
        }
    }
}
