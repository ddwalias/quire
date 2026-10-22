#nullable disable
#pragma warning disable

namespace Quire.PersistentDataObjects;

partial class plotType
{
    public global::System.Collections.Generic.IEnumerable<string> MC14ConversionLossAnnotations
    {
        get
        {
            global::System.Collections.Generic.List<string> annotations = MC14ConversionLossAnnotationsLocal == null
                ? new global::System.Collections.Generic.List<string>()
                : new global::System.Collections.Generic.List<string>(MC14ConversionLossAnnotationsLocal);
            global::Quire.PersistentDataObjects.IMC14AnnotationProvider itemProvider = Item as global::Quire.PersistentDataObjects.IMC14AnnotationProvider;
            if (itemProvider != null)
            {
                global::System.Collections.Generic.IEnumerable<string> itemAnnotations = itemProvider.MC14ConversionLossAnnotations;
                if (itemAnnotations != null)
                {
                    annotations.AddRange(itemAnnotations);
                }
            }
            if (annotations.Count > 0)
            {
                goto HasAnnotations;
            }
            return null;

        HasAnnotations:
            return annotations;
        }
    }
}
