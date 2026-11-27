#nullable disable
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Xml;
using Quire.IO;
using Quire.MindTouch.IO;
using Quire.Mirror;
using Quire.PersistentData;
using Quire.PersistentDataObjects;
using Quire.Serialization;
using Quire.Xml;

namespace Quire.Controls.Core.Serialization;

public partial class McdxDeserializer
{
    [MirrorVerified("PtcControlsCoreSerializationMcdxDeserializerMechanicalDifferentialTests")]
    private readonly ITextStyleChangedHandler _textStyleChangedHandler;

    [MirrorVerified("PtcControlsCoreSerializationMcdxDeserializerMechanicalDifferentialTests")]
    public ITextStyleChangedHandler TextStyleChangedHandler => _textStyleChangedHandler;

    [MirrorVerified("PtcControlsCoreSerializationMcdxDeserializerMechanicalDifferentialTests")]
    public IList<IRegionPersistentData> DeserializedRegions { get; private set; }

    [MirrorVerified("PtcControlsCoreSerializationMcdxDeserializerMechanicalDifferentialTests")]
    public bool SkipRegionsNotAffectingCalculation { get; set; }

    [MirrorVerified("PtcControlsCoreSerializationMcdxDeserializerMechanicalDifferentialTests")]
    private readonly PackagePart _packagePart;

    [MirrorVerified("PtcControlsCoreSerializationMcdxDeserializerMechanicalDifferentialTests")]
    public PackagePart PackagePart => _packagePart;

    [MirrorVerified("PtcControlsCoreSerializationMcdxDeserializerMechanicalDifferentialTests")]
    private readonly ISerializationHelper _serializationHelper;

    [MirrorVerified("PtcControlsCoreSerializationMcdxDeserializerMechanicalDifferentialTests")]
    public ISerializationHelper Helper => _serializationHelper;

    [MirrorVerified("PtcControlsCoreSerializationMcdxDeserializerMechanicalDifferentialTests")]
    public XmlNamespaceManager NameSpaceManager => XmlWorksheetData.McdxNameSpaceManager;

    [MirrorVerified("PtcControlsCoreSerializationMcdxDeserializerMechanicalDifferentialTests")]
    private XmlDocument _xmlContentDocument;

    [MirrorVerified("PtcControlsCoreSerializationMcdxDeserializerMechanicalDifferentialTests")]
    public XmlDocument XmlContentDocument => _xmlContentDocument;

    [MirrorVerified("PtcControlsCoreSerializationMcdxDeserializerMechanicalDifferentialTests")]
    public bool UseOverrides { get; private set; }

    [MirrorVerified("PtcControlsCoreSerializationMcdxDeserializerMechanicalDifferentialTests")]
    private readonly string _filePath;

    [MirrorVerified("PtcControlsCoreSerializationMcdxDeserializerMechanicalDifferentialTests")]
    public string FilePath => _filePath;

    private readonly IDeserializationStrategy _deserializationStrategy;
    private readonly IRegionCollectionSerializer _regionCollectionSerializer;
    private readonly string _worksheetNodeName;

    public void Deserialize(Stream stream)
    {
        if (stream == null)
        {
            throw new ArgumentNullException("stream");
        }
        try
        {
            _xmlContentDocument = new XmlDocument();
            SpiritResolver.InitializeDocument(_xmlContentDocument, SchemasManager.Worksheet50);
            _xmlContentDocument.Load(stream);
            RepairXmlDocument(_xmlContentDocument);
            _xmlContentDocument.Validate(null);
            Deserialize(_xmlContentDocument);
        }
        catch (Exception ex)
        {
            ThrowIncorrectSerializationFormatException(ex);
        }
    }

    private void RepairXmlDocument(XmlDocument xmlContentDocument)
    {
        RepairPictureElements(xmlContentDocument);
        RepairNumberFormatPrecision(xmlContentDocument);
    }

    private void RepairNumberFormatPrecision(XmlDocument xmlContentDocument)
    {
        if (_serializationHelper.AppVersion >= new Version(7, 0, 0, 0))
        {
            return;
        }
        string value = "ws";
        string xpath = $"/{value}:{_worksheetNodeName}//{value}:{"resultFormat"}/*[@{"precision"} = \"\"]/@{"precision"}";
        XmlNodeList xmlNodeList = xmlContentDocument.SelectNodes(xpath, NameSpaceManager);
        if (xmlNodeList != null)
        {
            string formatVersion = 3.ToString();
            foreach (XmlNode item in xmlNodeList)
            {
                if (DebugTools.DevAssert(item is XmlAttribute xmlAttribute && xmlAttribute.Name == "precision" && xmlAttribute.Value == string.Empty, null).Passed)
                {
                    item.Value = formatVersion;
                }
            }
        }
    }

    private void RepairPictureElements(XmlDocument xmlContentDocument)
    {
        Version version = new Version("5.2.10");
        if (_serializationHelper.SerializedSchemaVersions != null &&
            _serializationHelper.SerializedSchemaVersions.ContainsKey(SchemasManager.SchemaNameEnum.Worksheet) &&
            _serializationHelper.SerializedSchemaVersions[SchemasManager.SchemaNameEnum.Worksheet] >= version)
        {
            return;
        }
        foreach (XmlNode childNode in SpiritResolver.WorksheetRootNode(xmlContentDocument, NameSpaceManager, _worksheetNodeName).FirstChild.ChildNodes)
        {
            if (childNode.FirstChild != null && childNode.FirstChild.Name.Equals("picture"))
            {
                RepairPictureElementIfNeed(childNode);
            }
        }
    }

    private static void RepairPictureElementIfNeed(XmlNode region)
    {
        XmlNode firstChild = region.FirstChild;
        if (firstChild.FirstChild != null && !Enum.TryParse<ItemChoiceType>(firstChild.FirstChild.Name, out ItemChoiceType result))
        {
            XmlAttributeCollection attributes = firstChild.FirstChild.Attributes;
            if (DebugTools.AssertNotNull(firstChild.OwnerDocument, null).Passed)
            {
                XmlElement xmlElement = firstChild.OwnerDocument.CreateElement("ws", ItemChoiceType.unknown.ToString(), "http://schemas.mathsoft.com/worksheet50");
                if (DebugTools.AssertNotNull(attributes, null).Passed)
                {
                    foreach (XmlAttribute item in attributes)
                    {
                        xmlElement.SetAttribute(item.Name, item.Value);
                    }
                }
                firstChild.ReplaceChild(xmlElement, firstChild.FirstChild);
            }
        }
    }

    protected void ThrowIncorrectSerializationFormatException(Exception ex)
    {
        throw new FormatException("Incorrect serialization format", ex);
    }

    public McdxDeserializer(string filePath, PackagePart part, IDeserializationStrategy deserializationStrategy, ISerializationHelper serializationHelper, IRegionCollectionSerializer regionCollectionSerializer, bool useOverrides, ITextStyleChangedHandler textStyleChangedHandler = null)
    {
        if (part == null)
        {
            throw new ArgumentNullException("part");
        }
        _packagePart = part;
        _deserializationStrategy = deserializationStrategy;
        _serializationHelper = serializationHelper;
        _filePath = filePath;
        _regionCollectionSerializer = regionCollectionSerializer;
        _textStyleChangedHandler = textStyleChangedHandler;
        _worksheetNodeName = regionCollectionSerializer.RootNodeName;
        UseOverrides = useOverrides;
    }

    public void Deserialize(XmlDocument xmlContentDocument)
    {
        XmlNode xmlNode = SpiritResolver.WorksheetRootNode(xmlContentDocument, NameSpaceManager, _worksheetNodeName);
        _regionCollectionSerializer.Deserialize(xmlNode as XmlElement, UseOverrides);
        DeserializedRegions = _deserializationStrategy.SetRegions(_regionCollectionSerializer.Regions, this);
        _deserializationStrategy.DeserializeRegionsEpilog(xmlContentDocument);
    }

    public string UnpackExternalObject(string itemIdRef)
    {
        string text;
        Stream sourceStream = PackageOperationsProvider.GetSourceStream(PackagePart, itemIdRef);
        if (sourceStream == null)
        {
            text = null;
        }
        else
        {
            using (sourceStream)
            {
                text = Path.GetTempFileName();
                using FileStream destination = File.Open(text, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
                sourceStream.CopyTo(destination);
            }
        }
        return text;
    }

    public Stream UnpackProtectedPart(string itemIdRef)
    {
        ChunkedMemoryStream chunkedMemoryStream;
        using (Stream source = PackageOperationsProvider.GetSourceStream(PackagePart, itemIdRef))
        {
            chunkedMemoryStream = new ChunkedMemoryStream();
            SerializationHelperUtils.CopyObfuscatedStream(source, chunkedMemoryStream);
        }
        return chunkedMemoryStream;
    }

    public void UnpackFlowDocument(ref FlowDocument flowDocument, string itemIdRef)
    {
        Stream sourceStream = PackageOperationsProvider.GetSourceStream(PackagePart, itemIdRef);
        if (sourceStream != null)
        {
            using (sourceStream)
            {
                if (flowDocument == null)
                {
                    flowDocument = new FlowDocument();
                }
                TextRegionSerializationHelper.GetTextRangeFromStartToEnd(flowDocument).Load(sourceStream, DataFormats.XamlPackage);
            }
        }
    }

    public void ThrowFirstRegionDeserializationExceptionIfExists()
    {
        Exception ex = DeserializedRegions.AllDeserializationExceptions().FirstOrDefault();
        if (ex != null)
        {
            ThrowIncorrectSerializationFormatException(ex);
        }
    }

    public void Dispose()
    {
        _xmlContentDocument = null;
    }
}
