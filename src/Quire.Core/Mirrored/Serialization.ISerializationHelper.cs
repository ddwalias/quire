#nullable disable
#pragma warning disable

namespace Quire.Serialization;

partial interface ISerializationHelper
{
    string ApplicationName { get; set; }
    global::System.Version AppVersion { get; set; }
    string Release { get; set; }
    global::System.Version FormatVersion { get; set; }
    string EngineVersion { get; set; }
    global::System.Collections.Generic.IDictionary<global::Quire.Xml.SchemasManager.SchemaNameEnum, global::System.Version> SerializedSchemaVersions { get; }
    bool ErrorsOccurredWhenLoadingRegions { get; }
    global::System.Collections.Generic.IList<global::Quire.Serialization.IRegionPersistentData> MainRegions { get; set; }
    global::System.Collections.Generic.IList<global::Quire.Serialization.IRegionPersistentData> HeaderRegions { get; set; }
    global::System.Collections.Generic.IList<global::Quire.Serialization.IRegionPersistentData> FooterRegions { get; set; }
    global::System.Collections.Generic.IList<global::Quire.Serialization.IRegionPersistentData> FirstPageHeaderRegions { get; set; }
    global::System.Collections.Generic.IList<global::Quire.Serialization.IRegionPersistentData> FirstPageFooterRegions { get; set; }
    long GetRegionId(global::System.Windows.UIElement region) ;
    void AssignNextRegionId(global::System.Windows.UIElement region) ;
    long GetNextRegionId() ;
    void SetRegionId(global::System.Windows.UIElement region, long regionId) ;
    void CleanSerializerTempData(global::System.Collections.Generic.IList<global::System.Windows.UIElement> regions) ;
    global::System.Windows.UIElement GetRegionById(global::System.Collections.Generic.IEnumerable<global::System.Windows.UIElement> regions, long value) ;
    void Reset() ;
    void AddImagePath(string imageType, byte[] imageHash, global::Quire.Pair<string, global::System.Uri> imagePath) ;
    global::Quire.Pair<string, global::System.Uri> GetImagePath(string imageType, byte[] imageHash) ;
    global::Quire.Pair<string, global::System.Uri> GetCachedWorksheetPath(global::Quire.Pair<string, global::System.DateTime> fullFilePathAndTimeStamp) ;
    void AddCachedPartPath(global::Quire.Pair<string, global::System.DateTime> fullFilePathAndTimeStamp, global::Quire.Pair<string, global::System.Uri> includePath) ;
    void InitializeForWrite(string appName) ;
}
