#nullable disable
using System;
using System.Windows;
using System.Windows.Media;
using Quire.Core;
using Quire.Mirror;
using Quire.Native.PtcCore;
using Quire.Wpf;

namespace Quire.Controls.Core;

public abstract partial class SnappableItemBaseHelper : ISnappableItemHelper, ISnapToPixelHelper, IDisposable
{
    private readonly TranslateTransform _transform = new TranslateTransform();

    private DependencyPropertyChangedEventHandler _isOnVisiblePagePropertyChanged;

    [MirrorVerified("PtcControlsCoreSnappableItemBaseHelperMechanicalDifferentialTests")]
    public static bool IsClearImageEnabled { get; set; }

    [MirrorVerified("PtcControlsCoreSnappableItemBaseHelperMechanicalDifferentialTests")]
    public bool IsPageDescendant { get; private set; }

    static SnappableItemBaseHelper()
    {
        IsClearImageEnabled = true;
    }

    protected static Point? CalculateOffset(Visual visual, GeneralTransform transformFromSourceToVisual)
    {
        DebugTools.DevAssert(IsClearImageEnabled, null);
        GeneralTransform transformToRoot = SnapToRootVisualUtils.GetTransformToRootVisualConsideringDpi(visual);
        if (transformToRoot == null)
        {
            return null;
        }
        GeneralTransform generalTransform = WpfUtils.GroupTransformsAsFrozen(transformFromSourceToVisual, transformToRoot);
        GeneralTransform inverse = generalTransform.Inverse;
        if (DebugTools.AssertNotNull(inverse, null).Failed)
        {
            return null;
        }
        Point point = generalTransform.Transform(new Point(0.0, 0.0));
        Point point2 = new Point(DoubleExtension.Floor(point.X, 1E-05), DoubleExtension.Floor(point.Y, 1E-05));
        return inverse.Transform(point2);
    }

    protected SnappableItemBaseHelper(UIElement itemElement, bool isPageDescendant)
    {
        ItemElement = itemElement;
        IsPageDescendant = isPageDescendant;
        if (IsPageDescendant)
        {
            _isOnVisiblePagePropertyChanged = delegate
            {
                RecalculateRenderTransform();
            };
            DependencyPropertyChangeManager.AddPropertyChangedHandler(AttachedProperties.IsOnVisiblePageProperty, ItemElement, _isOnVisiblePagePropertyChanged);
        }
    }

    private UIElement _renderTarget;

    public virtual void Dispose()
    {
        if (_isOnVisiblePagePropertyChanged != null)
        {
            DependencyPropertyChangeManager.RemovePropertyChangedHandler(AttachedProperties.IsOnVisiblePageProperty, ItemElement, _isOnVisiblePagePropertyChanged);
            _isOnVisiblePagePropertyChanged = null;
        }
    }

    public UIElement RenderTarget
    {
        get
        {
            return _renderTarget;
        }
        set
        {
            DebugTools.DevAssert(_renderTarget == null, null);
            _renderTarget = value;
            if (DebugTools.AssertNotNull(_renderTarget, null).Passed)
            {
                _renderTarget.RenderTransform = _transform;
            }
            RecalculateRenderTransform();
        }
    }

    [MirrorVerified("PtcControlsCoreSnappableItemBaseHelperMechanicalDifferentialTests")]
    public UIElement ItemElement { get; private set; }

    protected abstract Point? RecalculateOffset();

    protected void RecalculateRenderTransform()
    {
        if (!IsClearImageEnabled)
        {
            return;
        }
        if (RenderTarget == null)
        {
            return;
        }
        if (!((!IsPageDescendant || AttachedProperties.GetIsOnVisiblePage(ItemElement)) && ItemElement.IsVisible && ItemElement.IsMeasureValid && ItemElement.IsArrangeValid))
        {
            return;
        }

        Point? point = RecalculateOffset();
        if (!point.HasValue)
        {
            _transform.X = _transform.Y = 0.0;
        }
        else if (!point.Value.IsEqualTo(new Point(_transform.X, _transform.Y)))
        {
            _transform.X = point.Value.X;
            _transform.Y = point.Value.Y;
        }
        DebugTools.DevAssert(RenderTarget.IsMeasureValid && RenderTarget.IsArrangeValid, null);
    }

    public GeneralTransform TransformFromPoint
    {
        get
        {
            if (!DebugTools.AssertNotNull(RenderTarget, null).Passed)
            {
                return null;
            }
            return this.CreateTransformFromChildOrigin(RenderTarget);
        }
    }

    public bool HasPointThatSnapsToPixel => RenderTarget != null;
}
