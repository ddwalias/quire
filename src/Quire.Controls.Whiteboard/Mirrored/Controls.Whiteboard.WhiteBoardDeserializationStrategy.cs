#nullable disable
#pragma warning disable
using System;
using System.Collections.Generic;
using System.Windows;
using Quire.Controls.Core;
using Quire.Controls.Core.Serialization;
using Quire.Controls;
using Quire.Controls.Include;

namespace Quire.Controls.Whiteboard;

public abstract partial class WhiteBoardDeserializationStrategy : DeserializationStrategyWithMathFormat
{
    private readonly WhiteboardControl _whiteboard;

    protected WhiteBoardDeserializationStrategy(WhiteboardControl control)
    {
        _whiteboard = control;
    }

    public override void DeserializeRegionProlog(UIElement region)
    {
        if (region is IMultiLevelInclusionProvider multiLevelInclusionProvider)
        {
            multiLevelInclusionProvider.MultiLevelInclusionItem.CacheValid = true;
        }
        base.DeserializeRegionProlog(region);
    }

    public override void AddRegion(UIElement element, Point location)
    {
        _whiteboard.AddItemAtLocation((IWhiteboardItem)element, location);
    }

    public override IList<UIElement> Regions
    {
        get
        {
            List<UIElement> list = new List<UIElement>();
            foreach (IWhiteboardItem whiteboardItem in _whiteboard.WhiteboardItems)
            {
                list.Add((UIElement)whiteboardItem);
            }
            return list;
        }
    }

    public override void DeleteRegion(UIElement control)
    {
        if (control is IWhiteboardItem)
        {
            _whiteboard.RemoveItem(control as IWhiteboardItem);
            return;
        }
        throw new ArgumentException("should implement IWhiteboardItem", "control");
    }

    protected override bool IsRegionNotAffectingCalculation(UIElement element)
    {
        if (element is EquationControl equationControl)
        {
            return equationControl.EquationType == EquationTypes.Evaluation;
        }
        return false;
    }
}
