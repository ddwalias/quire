#nullable disable
using System.Windows;
using Quire.Mirror;

namespace Quire.Controls.Core.Serialization;

public static partial class MC14ConversionAnnotation
{
    [MirrorVerified("PtcControlsCoreSerializationMC14ConversionAnnotationMechanicalDifferentialTests")]
    private static readonly DependencyPropertyKey TopLevelAnnotationAdornersPropertyKey;

    [MirrorVerified("PtcControlsCoreSerializationMC14ConversionAnnotationMechanicalDifferentialTests")]
    private static void SetTopLevelAnnotationAdorners(UIElement element, IList<VisualAdornerBase> value) => element.SetValue(TopLevelAnnotationAdornersPropertyKey, value);

    [MirrorVerified("PtcControlsCoreSerializationMC14ConversionAnnotationMechanicalDifferentialTests")]
    private static void ClearTopLevelAnnotationAdorners(UIElement element) => element.ClearValue(TopLevelAnnotationAdornersPropertyKey);
}
