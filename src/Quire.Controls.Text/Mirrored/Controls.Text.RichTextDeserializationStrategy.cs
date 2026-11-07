#nullable disable
#pragma warning disable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Quire.Controls.Core;
using Quire.Controls;
using Quire.PersistentDataObjects;
using Quire.Serialization;

namespace Quire.Controls.Text;

internal partial class RichTextDeserializationStrategy
{
    private readonly IRichTextWithEmbeddedItem _richText;

    public RichTextDeserializationStrategy(IRichTextWithEmbeddedItem richTextBase)
    {
        _richText = richTextBase;
    }

    public override IList<UIElement> Regions => _richText.WhiteboardItems.Cast<UIElement>().ToList();

    public override IRegionFactory RegionFactory => _richText.RegionFactory;

    protected override void AddRegion(IRegionPersistentData regionPersistentData)
    {
        int offset = int.Parse(((IRegionInTextType)regionPersistentData.RegionData).textcaretposition);
        IWhiteboardItemInText item = (IWhiteboardItemInText)regionPersistentData.Control;
        _richText.ReplacePlaceholderWithEmbeddedItem(item, offset);
    }

    public override void AddRegion(UIElement element, Point location)
    {
        throw new NotImplementedException();
    }

    public override void DeleteRegion(UIElement control)
    {
        IWhiteboardItemInText item = control as IWhiteboardItemInText;
        if (item != null)
        {
            _richText.RemoveItem(item);
            return;
        }
        throw new ArgumentException("should implement IWhiteboardItemInText", "control");
    }

    protected override bool IsRegionNotAffectingCalculation(UIElement element)
    {
        EquationControl equationControl = element as EquationControl;
        return equationControl == null || equationControl.EquationType == EquationTypes.Evaluation
            ? true
            : equationControl.EquationType == EquationTypes.Expression;
    }
}
