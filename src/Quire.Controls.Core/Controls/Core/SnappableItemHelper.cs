#nullable disable
using System;
using System.Windows;
using Quire;
using Quire.Mirror;

namespace Quire.Controls.Core;

public partial class SnappableItemHelper : SnappableItemBaseHelper
{
    private EventHandler _onLayoutUpdated;

    [MirrorVerified("PtcControlsCoreSnappableItemHelperMechanicalDifferentialTests")]
    public static SnappableItemHelper Create(UIElement itemElement, bool isPageDescendant) => new SnappableItemHelper(itemElement, isPageDescendant);

    private SnappableItemHelper(UIElement itemElement, bool isPageDescendant)
        : base(itemElement, isPageDescendant)
    {
        _onLayoutUpdated = delegate
        {
            RecalculateRenderTransform();
        };
        ItemElement.LayoutUpdated += _onLayoutUpdated;
    }

    public override void Dispose()
    {
        if (_onLayoutUpdated != null)
        {
            ItemElement.LayoutUpdated -= _onLayoutUpdated;
            _onLayoutUpdated = null;
        }
        base.Dispose();
    }

    protected override Point? RecalculateOffset()
    {
        if (!DebugTools.AssertNotNull(RenderTarget, null).Passed)
        {
            return null;
        }
        return SnappableItemBaseHelper.CalculateOffset(ItemElement, RenderTarget.RenderTransform.Inverse);
    }
}
