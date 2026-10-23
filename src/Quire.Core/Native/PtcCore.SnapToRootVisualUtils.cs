#nullable disable
using System.Windows;
using System.Windows.Media;
using Quire.Wpf;

namespace Quire.Native.PtcCore;

public static partial class SnapToRootVisualUtils
{
    public static Visual GetSnappableRootVisual(Visual visual)
    {
        IAlignedRoot alignedRoot = visual.TryFindVisualAncestor<IAlignedRoot>();
        Visual result = null;
        if (alignedRoot != null)
        {
            result = alignedRoot.Visual;
        }
        else
        {
            PresentationSource presentationSource = PresentationSource.FromVisual(visual);
            if (presentationSource != null)
            {
                result = presentationSource.RootVisual;
            }
        }
        return result;
    }

    public static GeneralTransform GetTransformToRootVisual(Visual visual)
    {
        Visual snappableRootVisual = GetSnappableRootVisual(visual);
        if (!global::Quire.DebugTools.AssertNotNull(snappableRootVisual, null).Failed)
        {
            return visual.TransformToAncestor(snappableRootVisual);
        }
        return null;
    }

    public static GeneralTransform GetTransformToRootVisualConsideringDpi(Visual visual)
    {
        GeneralTransform transformToRootVisual = GetTransformToRootVisual(visual);
        if (transformToRootVisual != null)
        {
            return WpfUtils.GroupTransformsAsFrozen(transformToRootVisual, DpiScale);
        }
        return null;
    }
}
