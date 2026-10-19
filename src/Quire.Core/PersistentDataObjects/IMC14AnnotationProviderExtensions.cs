#nullable disable
using Quire.Mirror;

namespace Quire.PersistentDataObjects;

public static partial class IMC14AnnotationProviderExtensions
{
    [MirrorVerified("PtcPersistentDataObjectsIMC14AnnotationProviderExtensionsDifferentialTests")]
    public static void AddMC14ConversionLossAnnotations(this IMC14AnnotationProvider annotationsProvider, MC14ConversionAnnotationType type)
    {
        annotationsProvider.MC14ConversionLossAnnotationsLocal = annotationsProvider.MC14ConversionLossAnnotationsLocal ?? new List<string>();
        if (!annotationsProvider.MC14ConversionLossAnnotationsLocal.Contains(type.ToString()))
        {
            annotationsProvider.MC14ConversionLossAnnotationsLocal.Add(type.ToString());
        }
    }

    [MirrorVerified("PtcPersistentDataObjectsIMC14AnnotationProviderExtensionsDifferentialTests")]
    public static void AddMC14ConversionLossAnnotations(this IMC14AnnotationProvider annotationsProvider, IEnumerable<MC14ConversionAnnotationType> types)
    {
        foreach (MC14ConversionAnnotationType type in types)
        {
            annotationsProvider.AddMC14ConversionLossAnnotations(type);
        }
    }
}
