#nullable disable
using System.IO;
using System.IO.Packaging;
using System.Windows.Documents;
using System.Xml;
using Quire.Mirror;
using Quire.Serialization;

namespace Quire.PersistentDataObjects;

public partial class pictureType
{
    [MirrorVerified("PtcPersistentDataObjectspictureTypeMechanicalDifferentialTests")]
    public pictureType() { }

    [MirrorVerified("PtcPersistentDataObjectspictureTypeMechanicalDifferentialTests")]
    public string PackExternalObject(string sourceFileName, string uniqueId, ExternalObjectType objectType) => throw new NotImplementedException();

    [MirrorVerified("PtcPersistentDataObjectspictureTypeMechanicalDifferentialTests")]
    public string PackIncludedWorksheet(string fullFilePath, DateTime lastSavedDate, string uniqueId, Func<Package, string, PackagePart> writePackagePart) => throw new NotImplementedException();

    [MirrorVerified("PtcPersistentDataObjectspictureTypeMechanicalDifferentialTests")]
    public string PackFlowDocument(FlowDocument flowDocument, string regionId) => throw new NotImplementedException();

    [MirrorVerified("PtcPersistentDataObjectspictureTypeMechanicalDifferentialTests")]
    public string PackProtectedPart(Stream content, string uniqueId, string regionType) => throw new NotImplementedException();

    [MirrorVerified("PtcPersistentDataObjectspictureTypeMechanicalDifferentialTests")]
    private basePictureType itemField;

    [MirrorVerified("PtcPersistentDataObjectspictureTypeMechanicalDifferentialTests")]
    public basePictureType Item { get => itemField; set => itemField = value; }

    [MirrorVerified("PtcPersistentDataObjectspictureTypeMechanicalDifferentialTests")]
    private ItemChoiceType itemElementNameField;

    [MirrorVerified("PtcPersistentDataObjectspictureTypeMechanicalDifferentialTests")]
    public ItemChoiceType ItemElementName { get => itemElementNameField; set => itemElementNameField = value; }

    [MirrorVerified("PtcPersistentDataObjectspictureTypeMechanicalDifferentialTests")]
    public ITextStyleChangedHandler TextStyleChangedHandler { get; }

    [MirrorVerified("PtcPersistentDataObjectspictureTypeMechanicalDifferentialTests")]
    private Uri PictureSourceUri { get; set; }

    [MirrorVerified("PtcPersistentDataObjectspictureTypeMechanicalDifferentialTests")]
    public IS11NProvider S11NProvider { get; set; }

    [MirrorVerified("PtcPersistentDataObjectspictureTypeMechanicalDifferentialTests")]
    public XmlNode[] InnerXml { get; set; }
}
