#nullable disable
using System.IO.Packaging;
using System.Xml;
using Quire.Mirror;
using Quire.Mirror;
using Quire.Serialization;
using Quire.Serialization;

namespace Quire.PersistentDataObjects;

public partial class regionBase
{
    public abstract object Item { get; set; }
    public abstract string[] mc14conversionlossannotations { get; set; }

    [MirrorVerified("PtcPersistentDataObjectsregionBaseMechanicalDifferentialTests")]
    private string regionidField;

    [MirrorVerified("PtcPersistentDataObjectsregionBaseMechanicalDifferentialTests")]
    public string regionid { get => regionidField; set => regionidField = value; }

    [MirrorVerified("PtcPersistentDataObjectsregionBaseMechanicalDifferentialTests")]
    private string tagField;

    [MirrorVerified("PtcPersistentDataObjectsregionBaseMechanicalDifferentialTests")]
    public string Tag { get => tagField; set => tagField = value; }

    [MirrorVerified("PtcPersistentDataObjectsregionBaseMechanicalDifferentialTests")]
    private bool mc14conversionlossField;

    [MirrorVerified("PtcPersistentDataObjectsregionBaseMechanicalDifferentialTests")]
    public bool mc14conversionloss { get => mc14conversionlossField; set => mc14conversionlossField = value; }

    [MirrorVerified("PtcPersistentDataObjectsregionBaseMechanicalDifferentialTests")]
    private applicationVersionType convertedfromField;

    [MirrorVerified("PtcPersistentDataObjectsregionBaseMechanicalDifferentialTests")]
    public applicationVersionType convertedfrom { get => convertedfromField; set => convertedfromField = value; }

    [MirrorVerified("PtcPersistentDataObjectsregionBaseMechanicalDifferentialTests")]
    private bool convertedfromFieldSpecified;

    [MirrorVerified("PtcPersistentDataObjectsregionBaseMechanicalDifferentialTests")]
    public bool convertedfromSpecified { get => convertedfromFieldSpecified; set => convertedfromFieldSpecified = value; }

    [MirrorVerified("Release IL")]
    public bool ConvertedFromMC14
    {
        get
        {
            if (convertedfromSpecified)
            {
                return convertedfrom == 0;
            }
            return false;
        }
    }

    [MirrorVerified("Release IL")]
    public string PackExternalObject(string sourceFileName, string uniqueId, ExternalObjectType objectType) => S11NProvider.PackExternalObjectExt(sourceFileName, uniqueId ?? regionid, objectType);

    [MirrorVerified("Release IL")]
    public string PackIncludedWorksheet(string fullFilePath, DateTime lastSavedDate, string uniqueId, Func<Package, string, PackagePart> writePackagePart) => S11NProvider.PackIncludedWorksheet(fullFilePath, lastSavedDate, uniqueId ?? regionid, writePackagePart);

    [MirrorVerified("Release IL")]
    public string PackFlowDocument(System.Windows.Documents.FlowDocument flowDocument, string flowDocumentRegionId) => S11NProvider.PackFlowDocument(flowDocument, string.IsNullOrWhiteSpace(flowDocumentRegionId) ? regionid : flowDocumentRegionId);

    [MirrorVerified("Release IL")]
    public string PackProtectedPart(System.IO.Stream content, string uniqueId, string regionType) => S11NProvider.PackProtectedPart(content, string.IsNullOrWhiteSpace(uniqueId) ? regionid : uniqueId, regionType);

    [MirrorVerified("Release IL")]
    public string PackImage(Quire.Wpf.ImagePackagingInformation pictureInfo)
    {
        if (pictureInfo.ImageId == null)
        {
            pictureInfo.ImageId = regionid;
        }
        return S11NProvider.PackImageExt(pictureInfo);
    }

    [MirrorVerified("PtcPersistentDataObjectsregionBaseMechanicalDifferentialTests")]
    private double heightField;

    [MirrorVerified("PtcPersistentDataObjectsregionBaseMechanicalDifferentialTests")]
    public double height { get => heightField; set => heightField = value; }

    [MirrorVerified("PtcPersistentDataObjectsregionBaseMechanicalDifferentialTests")]
    private bool heightFieldSpecified;

    [MirrorVerified("PtcPersistentDataObjectsregionBaseMechanicalDifferentialTests")]
    public bool heightSpecified { get => heightFieldSpecified; set => heightFieldSpecified = value; }

    [MirrorVerified("PtcPersistentDataObjectsregionBaseMechanicalDifferentialTests")]
    private double widthField;

    [MirrorVerified("PtcPersistentDataObjectsregionBaseMechanicalDifferentialTests")]
    public double width { get => widthField; set => widthField = value; }

    [MirrorVerified("PtcPersistentDataObjectsregionBaseMechanicalDifferentialTests")]
    private bool widthFieldSpecified;

    [MirrorVerified("PtcPersistentDataObjectsregionBaseMechanicalDifferentialTests")]
    public bool widthSpecified { get => widthFieldSpecified; set => widthFieldSpecified = value; }

    [MirrorVerified("PtcPersistentDataObjectsregionBaseMechanicalDifferentialTests")]
    private double actualWidthField;

    [MirrorVerified("PtcPersistentDataObjectsregionBaseMechanicalDifferentialTests")]
    public double actualWidth { get => actualWidthField; set => actualWidthField = value; }

    [MirrorVerified("PtcPersistentDataObjectsregionBaseMechanicalDifferentialTests")]
    private bool actualWidthFieldSpecified;

    [MirrorVerified("PtcPersistentDataObjectsregionBaseMechanicalDifferentialTests")]
    public bool actualWidthSpecified { get => actualWidthFieldSpecified; set => actualWidthFieldSpecified = value; }

    [MirrorVerified("PtcPersistentDataObjectsregionBaseMechanicalDifferentialTests")]
    private double actualHeightField;

    [MirrorVerified("PtcPersistentDataObjectsregionBaseMechanicalDifferentialTests")]
    public double actualHeight { get => actualHeightField; set => actualHeightField = value; }

    [MirrorVerified("PtcPersistentDataObjectsregionBaseMechanicalDifferentialTests")]
    private bool actualHeightFieldSpecified;

    [MirrorVerified("PtcPersistentDataObjectsregionBaseMechanicalDifferentialTests")]
    public bool actualHeightSpecified { get => actualHeightFieldSpecified; set => actualHeightFieldSpecified = value; }

    [MirrorVerified("PtcPersistentDataObjectsregionBaseMechanicalDifferentialTests")]
    private double alignxField;

    [MirrorVerified("PtcPersistentDataObjectsregionBaseMechanicalDifferentialTests")]
    public double alignx { get => alignxField; set => alignxField = value; }

    [MirrorVerified("PtcPersistentDataObjectsregionBaseMechanicalDifferentialTests")]
    private bool alignxFieldSpecified;

    [MirrorVerified("PtcPersistentDataObjectsregionBaseMechanicalDifferentialTests")]
    public bool alignxSpecified { get => alignxFieldSpecified; set => alignxFieldSpecified = value; }

    [MirrorVerified("PtcPersistentDataObjectsregionBaseMechanicalDifferentialTests")]
    private double alignyField;

    [MirrorVerified("PtcPersistentDataObjectsregionBaseMechanicalDifferentialTests")]
    public double aligny { get => alignyField; set => alignyField = value; }

    [MirrorVerified("PtcPersistentDataObjectsregionBaseMechanicalDifferentialTests")]
    private bool alignyFieldSpecified;

    [MirrorVerified("PtcPersistentDataObjectsregionBaseMechanicalDifferentialTests")]
    public bool alignySpecified { get => alignyFieldSpecified; set => alignyFieldSpecified = value; }

    [MirrorVerified("PtcPersistentDataObjectsregionBaseMechanicalDifferentialTests")]
    private bool showBorderField;

    [MirrorVerified("PtcPersistentDataObjectsregionBaseMechanicalDifferentialTests")]
    public bool ShowBorder { get => showBorderField; set => showBorderField = value; }

    [MirrorVerified("PtcPersistentDataObjectsregionBaseMechanicalDifferentialTests")]
    public ITextStyleChangedHandler TextStyleChangedHandler { get; }

    [MirrorVerified("PtcPersistentDataObjectsregionBaseMechanicalDifferentialTests")]
    public IS11NProvider S11NProvider { get; set; }

    [MirrorVerified("PtcPersistentDataObjectsregionBaseMechanicalDifferentialTests")]
    public SolveBlockMemberType SolveBlockMemberType { get; set; }

    [MirrorVerified("Release IL")]
    public regionBase()
    {
        mc14conversionlossField = false;
        showBorderField = false;
    }

    [MirrorVerified("Release IL")]
    public void SetHeight(double newHeight)
    {
        height = newHeight;
        heightSpecified = true;
    }

    [MirrorVerified("Release IL")]
    public void SetWidth(double newWidth)
    {
        width = newWidth;
        widthSpecified = true;
    }

    [MirrorVerified("Release IL")]
    public bool UseOverrides => S11NProvider.UseOverrides;

    [MirrorVerified("Release IL")]
    public XmlDocument XmlContentDocument => S11NProvider.XmlContentDocument;

    [MirrorVerified("Release IL")]
    public PackagePart PackagePart => S11NProvider.PackagePart;

    [MirrorVerified("Release IL")]
    public ISerializationHelper Helper => S11NProvider.Helper;

    [MirrorVerified("Release IL")]
    public XmlNamespaceManager NameSpaceManager => S11NProvider.NameSpaceManager;
}
