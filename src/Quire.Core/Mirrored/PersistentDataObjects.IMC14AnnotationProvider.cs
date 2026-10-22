#nullable disable
#pragma warning disable

namespace Quire.PersistentDataObjects;

partial interface IMC14AnnotationProvider
{
    global::System.Collections.Generic.IEnumerable<string> MC14ConversionLossAnnotations { get; }
    global::System.Collections.Generic.List<string> MC14ConversionLossAnnotationsLocal { get; set; }
}
