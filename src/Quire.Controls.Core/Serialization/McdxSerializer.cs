#nullable disable
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media.Imaging;
using System.Xml;
using System.Xml.Schema;
using Quire.IO;
using Quire.Mirror;
using Quire.PersistentData;
using Quire.PersistentDataObjects;
using Quire.Serialization;
using Quire.Wpf;
using Quire.Xml;

namespace Quire.Controls.Core.Serialization;

public partial class McdxSerializer
{
    private readonly Action<XmlSchemaException> _onWriteValidationFailed;

    private readonly ISerializationStrategy _serializationStrategy;

    private readonly ISerializationHelper _serializationHelper;

    private readonly IRegionCollectionSerializer _regionCollectionSerializer;

    [MirrorVerified("PtcControlsCoreSerializationMcdxSerializerMechanicalDifferentialTests")]
    private readonly PackagePart _packagePart;

    private readonly string _worksheetNodeName;

    [MirrorVerified("PtcControlsCoreSerializationMcdxSerializerMechanicalDifferentialTests")]
    private readonly ITextStyleChangedHandler _textStyleChangedHandler;

    public bool IsConverterApplication => Helper.ApplicationName == "WorksheetConverter";

    public McdxSerializer(PackagePart part, ISerializationStrategy serializationStrategy, ISerializationHelper helper, IRegionCollectionSerializer regionCollectionSerializer, Action<XmlSchemaException> onWriteValidationFailed, bool useOverrides, ITextStyleChangedHandler textStyleChangedHandler = null)
    {
        if (part == null)
        {
            throw new ArgumentNullException("part");
        }

        _serializationStrategy = serializationStrategy;
        _worksheetNodeName = regionCollectionSerializer.RootNodeName;
        _regionCollectionSerializer = regionCollectionSerializer;
        _serializationHelper = helper;
        _packagePart = part;
        _onWriteValidationFailed = onWriteValidationFailed;
        UseOverrides = useOverrides;
        _textStyleChangedHandler = textStyleChangedHandler;
    }

    [MirrorVerified("PtcControlsCoreSerializationMcdxSerializerMechanicalDifferentialTests")]
    public ITextStyleChangedHandler TextStyleChangedHandler => _textStyleChangedHandler;

    public void Serialize(PackagePart part, bool useTempFile = false)
    {
        XmlContentDocument = new XmlDocument
        {
            PreserveWhitespace = true,
        };
        InitializeDocument(XmlContentDocument);
        BuildDocumentXml(_regionCollectionSerializer);
        XmlContentDocument.Validate(OnWriteValidationFailedHandler);
        if (useTempFile)
        {
            SerializationHelperUtils.SaveToPartUsingTemporaryFile(tempFile => XmlContentDocument.Save(tempFile), part);
            return;
        }

        using Stream outStream = part.GetStream(FileMode.Create);
        XmlContentDocument.Save(outStream);
    }

    private void OnWriteValidationFailedHandler(object sender, ValidationEventArgs args)
    {
        if (_onWriteValidationFailed != null)
        {
            _onWriteValidationFailed(args.Exception);
            return;
        }

        throw args.Exception;
    }

    // Parked: Mathcad keeps the method-entry display class in a local; Roslyn emits newobj and dup for the same captures.
    public static string PackImage(PackagePart sourcePackagePart, ISerializationHelper serializationHelper, Uri imageSourceUri, byte[] imageSourceStreamHash, BitmapSource image, string uniqueImageName, string imageExtension) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public static string PackFile(PackagePart sourcePackagePart, string sourceFileName, string pathInPackage, string relationshipUri, string mimeType)
    {
        if (sourcePackagePart == null)
        {
            throw new NullReferenceException("package part doesn't exist");
        }

        PackagePart packagePart = PackageOperationsProvider.Instance.AddPartToPackage(sourcePackagePart.Package, pathInPackage, mimeType);
        using (Stream destination = packagePart.GetStream(FileMode.Create))
        {
            using FileStream fileStream = File.Open(sourceFileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            fileStream.CopyTo(destination);
        }

        return sourcePackagePart.CreateRelationship(packagePart.Uri, TargetMode.Internal, relationshipUri).Id;
    }

    public string PackImage(ImagePackagingInformation pictureInfo)
    {
        return PackImage(_packagePart, _serializationHelper, pictureInfo.ImageSourceUri, pictureInfo.ImageOriginalStreamHash, pictureInfo.Image, pictureInfo.ImageName, pictureInfo.ImageExtension);
    }

    public string PackExternalObject(string sourceFileName, string objectUniqueId, ExternalObjectType objectType)
    {
        string format;
        string mimeType;
        string relationshipUri;
        switch (objectType)
        {
            case ExternalObjectType.Excel:
                format = "/mathcad/excel/excel{0}.ole";
                mimeType = "application/vnd.openxmlformats-officedocument.oleObject";
                relationshipUri = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/excel";
                break;
            case ExternalObjectType.Ole:
                format = "/mathcad/ole/object{0}.ole";
                mimeType = "application/vnd.openxmlformats-officedocument.oleObject";
                relationshipUri = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/oleObject";
                break;
            case ExternalObjectType.Chart:
                format = "/mathcad/chart/chart{0}.ole";
                mimeType = "application/vnd.openxmlformats-officedocument.oleObject";
                relationshipUri = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/chart";
                break;
            default:
                throw new NotImplementedException("Unsupported external object type: " + objectType);
        }

        return PackFile(_packagePart, sourceFileName, string.Format(format, objectUniqueId), relationshipUri, mimeType);
    }

    public string PackIncludedWorksheet(string fullFilePath, DateTime lastSavedDate, string uniqueId, Func<Package, string, PackagePart> writePartToPackage)
    {
        DebugTools.DevAssert(!string.IsNullOrEmpty(fullFilePath), null);
        Pair<string, DateTime> fullFilePathAndTime = new Pair<string, DateTime>(fullFilePath, lastSavedDate);
        Func<Pair<string, Uri>> getExistingPartRelationshipId = () => Helper.GetCachedWorksheetPath(fullFilePathAndTime);
        Action<string, PackagePart> cacheRelationshipIdForNewPart = (string relationshipId, PackagePart cachedPart) =>
        {
            Helper.AddCachedPartPath(fullFilePathAndTime, new Pair<string, Uri>(relationshipId, cachedPart.Uri));
        };
        Func<PackagePart> writeNewPartToPackage = () => writePartToPackage(PackagePart.Package, uniqueId);
        return PackageOperationsProvider.PackPartAndGetId(PackagePart, getExistingPartRelationshipId, cacheRelationshipIdForNewPart, writeNewPartToPackage, "http://schemas.openxmlformats.org/officeDocument/2006/relationships/include");
    }

    public XmlDocument XmlContentDocument { get; private set; }

    public bool UseOverrides { get; private set; }

    public PackagePart PackagePart => _packagePart;

    public ISerializationHelper Helper => _serializationHelper;

    public XmlNamespaceManager NameSpaceManager => XmlWorksheetData.McdxNameSpaceManager;

    public string PackFlowDocument(FlowDocument flowDocument, string regionId)
    {
        PackagePart packagePart = PackagePart;
        PackagePart flowDocumentPart = PackageOperationsProvider.Instance.AddPartToPackage(packagePart.Package, string.Format("/mathcad/xaml/FlowDocument{0}.{1}", regionId, DataFormats.XamlPackage), "application/zip");
        PackageRelationship packageRelationship = packagePart.CreateRelationship(flowDocumentPart.Uri, TargetMode.Internal, "http://schemas.openxmlformats.org/officeDocument/2006/relationships/flowDocument");
        using (Stream stream = flowDocumentPart.GetStream(FileMode.Create, FileAccess.ReadWrite))
        {
            TextRegionSerializationHelper.SaveTextRangeAsXamlPackage(TextRegionSerializationHelper.GetTextRangeFromStartToEnd(flowDocument), stream, IsConverterApplication);
            stream.Flush();
        }

        return packageRelationship.Id;
    }

    public string PackProtectedPart(Stream content, string uniqueId, string regionType)
    {
        PackagePart packagePart = PackagePart;
        PackagePart protectedPart = PackageOperationsProvider.Instance.AddPartToPackage(packagePart.Package, string.Format("/mathcad/ProtectedData/{0}{1}.dat", regionType, uniqueId), "application/octet-stream");
        PackageRelationship packageRelationship = packagePart.CreateRelationship(protectedPart.Uri, TargetMode.Internal, "http://schemas.openxmlformats.org/officeDocument/2006/relationships/ProtectedData");
        SerializationHelperUtils.SaveToPartUsingTemporaryFile(tempFilePath =>
        {
            using FileStream fileStream = new FileStream(tempFilePath, FileMode.Create, FileAccess.Write);
            SerializationHelperUtils.CopyObfuscatedStream(content, fileStream);
            fileStream.Flush();
        }, protectedPart);
        return packageRelationship.Id;
    }

    private static void InitializeDocument(XmlDocument xmlContentDocument)
    {
        SpiritResolver.InitializeDocument(xmlContentDocument, SchemasManager.Worksheet50);
    }

    private void BuildDocumentXml(IRegionCollectionSerializer regionCollectionSerializer)
    {
        IList<IRegionType> regions = _serializationStrategy.GetRegions(this);
        regions.Each(item =>
        {
            if (item.S11NProvider == null)
            {
                item.S11NProvider = this;
            }
            else
            {
                IS11NProvider s11NProvider = item.S11NProvider;
                while (s11NProvider is IHasS11NProvider)
                {
                    s11NProvider = (s11NProvider as IHasS11NProvider).S11NProvider;
                }

                DebugTools.DevAssert(s11NProvider == this, null);
            }
        });
        regionCollectionSerializer.Regions = regions;
        regionCollectionSerializer.Serialize(XmlContentDocument, UseOverrides);
        _serializationStrategy.SerializeRegionsEpilog(XmlContentDocument);
        if (SpiritResolver.WorksheetRootNode(XmlContentDocument, NameSpaceManager, _worksheetNodeName) is XmlElement xmlElement)
        {
            xmlElement.SetAttribute("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");
            xmlElement.SetAttribute("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");
            xmlElement.SetAttribute("xmlns:ve", "http://schemas.openxmlformats.org/markup-compatibility/2006");
            xmlElement.SetAttribute("xmlns:r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
            xmlElement.SetAttribute("xmlns:ws", "http://schemas.mathsoft.com/worksheet50");
            xmlElement.SetAttribute("xmlns:ml", "http://schemas.mathsoft.com/math50");
            xmlElement.SetAttribute("xmlns:u", "http://schemas.mathsoft.com/units10");
            xmlElement.SetAttribute("xmlns:p", "http://schemas.mathsoft.com/provenance10");
        }
    }
}
