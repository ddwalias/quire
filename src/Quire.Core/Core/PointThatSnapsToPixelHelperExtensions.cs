#nullable disable
using System.Windows;
using System.Windows.Media;
using Quire.Mirror;

namespace Quire.Core;

public static partial class PointThatSnapsToPixelHelperExtensions
{
    [MirrorVerified("Release IL")]
    public static GeneralTransform CreateTransformFromChildPoint(this ISnapToPixelHelper parentHelper, ISnapToPixelHelper childHelper) => GroupTransforms(parentHelper, childHelper.ItemElement, childHelper.TransformFromPoint);

    [MirrorVerified("Release IL")]
    public static GeneralTransform CreateTransformFromChildOrigin(this ISnapToPixelHelper parentHelper, UIElement childElement) => GroupTransforms(parentHelper, childElement, childElement.RenderTransform.Inverse);
}
