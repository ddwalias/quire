#nullable disable
using System.Collections.Generic;
using Quire.Mirror;

namespace Quire.PersistentDataObjects;

public static partial class IMC14AnnotationProviderExtensions
{
    [MirrorVerified("Release IL")]
    public static void AddMC14ConversionLossAnnotations(this IMC14AnnotationProvider annotationsProvider, MC14ConversionAnnotationType type)
    {
        annotationsProvider.MC14ConversionLossAnnotationsLocal = annotationsProvider.MC14ConversionLossAnnotationsLocal ?? new List<string>();
        if (!annotationsProvider.MC14ConversionLossAnnotationsLocal.Contains(type.ToString()))
        {
            annotationsProvider.MC14ConversionLossAnnotationsLocal.Add(type.ToString());
        }
    }

    [MirrorVerified("Release IL")]
    public static void AddMC14ConversionLossAnnotations(this IMC14AnnotationProvider annotationsProvider, IEnumerable<MC14ConversionAnnotationType> types)
    {
        foreach (MC14ConversionAnnotationType type in types)
        {
            annotationsProvider.AddMC14ConversionLossAnnotations(type);
        }
    }

    [MirrorVerified("Release IL")]
    public static string[] ConvertMC14ConversionLossAnnotationsToStringArray(IEnumerable<MC14ConversionAnnotationType> annotationsList)
    {
        if (annotationsList.IsNullOrEmpty())
        {
            return new string[0];
        }
        List<string> list = new List<string>();
        foreach (MC14ConversionAnnotationType annotations in annotationsList)
        {
            if (!list.Contains(annotations.ToString()))
            {
                list.Add(annotations.ToString());
            }
        }
        return list.ToArray();
    }
}
