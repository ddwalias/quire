#nullable disable
#pragma warning disable

namespace Quire.Xml;

public static partial class XmlWorksheetData
{
    public static global::System.Xml.XmlNamespaceManager McdxNameSpaceManager = createMcdxNamespaceManager(null);
    public static global::System.Xml.XmlNamespaceManager Mathcad14NameSpaceManager = createMathcad14NamespaceManager(null);
    public const string worksheet30 = "http://schemas.mathsoft.com/worksheet30";
    public const string worksheet50 = "http://schemas.mathsoft.com/worksheet50";
    public const string xmlSchemaInstance = "http://www.w3.org/2001/XMLSchema-instance";
    public const string markupCompatibility = "http://schemas.openxmlformats.org/markup-compatibility/2006";
    public const string officeDocumentRelationships = "http://schemas.openxmlformats.org/officeDocument/2006/relationships";
    public const string packageRelationship = "http://schemas.openxmlformats.org/package/2006/relationships";
    public const string math30 = "http://schemas.mathsoft.com/math30";
    public const string math50 = "http://schemas.mathsoft.com/math50";
    public const string units10 = "http://schemas.mathsoft.com/units10";
    public const string provenance10 = "http://schemas.mathsoft.com/provenance10";
    public const string calculation10 = "http://schemas.ptc.com/mathcad/settings/calculation10";
    public const string presentation10 = "http://schemas.ptc.com/mathcad/settings/presentation10";
    public const string aliasPrefix = "http://www.w3.org/2000/xmlns/";
    public const string xmlns = "xmlns";
    public const string xamlPresentation = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";
    public const string xsi = "xsi";
    public const string ve = "ve";
    public const string r = "r";
    public const string ws = "ws";
    public const string ml = "ml";
    public const string u = "u";
    public const string p = "p";
    public const string cl = "cl";
    public static readonly global::System.Version CurrentAppVersion = AppVersionProvider.CurrentAppVersion;
    public static readonly global::System.Version CurrentFormatVersion = AppVersionProvider.CurrentFormatVersion;
    public static readonly global::System.Version CurrentEngNbVersion = AppVersionProvider.CurrentEngNbVersion;
    public const string MathcadPrime = "Quire";
    public const string ProductName = "Quire Quire";
    public const string EngNbProductName = "Creo Engineering Notebook";
    public const string EngNbPlusProductName = "Creo+ Engineering Notebook";
    public const string OptionsName = "Options";
    public static readonly string AppVersionName = string.Format("Prime {0}", CurrentAppVersion);
    private static string _stream;
    private static string _buildId;
    public static readonly global::Quire.PersistentDataObjects.propertiesTypeSerializationVersionArchitecture Architecture = global::System.Environment.Is64BitProcess ? global::Quire.PersistentDataObjects.propertiesTypeSerializationVersionArchitecture.x64 : global::Quire.PersistentDataObjects.propertiesTypeSerializationVersionArchitecture.x86;
}
