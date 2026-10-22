#nullable disable
#pragma warning disable

namespace Quire.Serialization;

partial interface IS11NProvider
{
    global::Quire.ITextStyleChangedHandler TextStyleChangedHandler { get; }
    string PackImage(global::Quire.Wpf.ImagePackagingInformation pictureInfo) ;
    string PackExternalObject(string sourceFileName, string objectUniqueId, global::Quire.Serialization.ExternalObjectType objectType) ;
    string PackIncludedWorksheet(string fullFilePath, global::System.DateTime lastSavedDate, string uniqueId, global::System.Func<global::System.IO.Packaging.Package, string, global::System.IO.Packaging.PackagePart> writePackagePart) ;
    string PackFlowDocument(global::System.Windows.Documents.FlowDocument flowDocument, string regionId) ;
    string PackProtectedPart(global::System.IO.Stream content, string uniqueId, string regionType) ;
}
