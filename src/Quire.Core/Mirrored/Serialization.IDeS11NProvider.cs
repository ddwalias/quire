#nullable disable
#pragma warning disable

namespace Quire.Serialization;

partial interface IDeS11NProvider
{
    string FilePath { get; }
    global::Quire.ITextStyleChangedHandler TextStyleChangedHandler { get; }
    bool SkipRegionsNotAffectingCalculation { get; }
    string UnpackExternalObject(string itemidref) ;
    void UnpackFlowDocument(ref global::System.Windows.Documents.FlowDocument flowDocument, string itemidref) ;
    global::System.IO.Stream UnpackProtectedPart(string itemIdRef) ;
}
