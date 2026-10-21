#nullable disable
using System.Windows;
using System.Windows.Media;
using Quire.Mirror;
using Quire.Wpf;

namespace Quire.Core;

public static partial class PointThatSnapsToPixelHelperExtensions
{
    [MirrorVerified("Release IL")]
    public static GeneralTransform CreateTransformFromChildPoint(this ISnapToPixelHelper parentHelper, ISnapToPixelHelper childHelper) => GroupTransforms(parentHelper, childHelper.ItemElement, childHelper.TransformFromPoint);

    [MirrorVerified("Release IL")]
    public static GeneralTransform CreateTransformFromChildOrigin(this ISnapToPixelHelper parentHelper, UIElement childElement) => GroupTransforms(parentHelper, childElement, childElement.RenderTransform.Inverse);

    [MirrorVerified("Release IL")]
    private static GeneralTransform GroupTransforms(this ISnapToPixelHelper parentHelper, UIElement childElement, GeneralTransform transform)
    {
        UIElement itemElement = parentHelper.ItemElement;
        if (itemElement != childElement)
        {
            return WpfUtils.GroupTransformsAsFrozen(transform, childElement.TransformToAncestor(itemElement));
        }
        return transform;
    }
}
