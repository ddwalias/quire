using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Windows;
using System.Windows.Documents;
using System.Xml;
using Quire.Controls.Core;
using Quire.Controls.Core.Serialization;
using Quire.Controls.Core.Settings;
using Quire.IO;
using Quire.PersistentData;
using Quire.PersistentDataObjects;
using Quire.Serialization;
using Quire.Wpf;
using Quire.Xml;

namespace Quire.Controls.Include;

public partial class IncludeWorksheetFileManager : IS11NProvider, IBaseS11NProvider, IHasS11NProvider
{
    private SerializationHelper _serializationHelper;
    private IncludeFileDeserializationStrategy _deserializationStrategy;
    private IncludeWorksheetSerializationStrategy _serializationStrategy;
    private IMetadataModel _metadataModel;
    private readonly IPackageOperationsProvider _packageOpsProvider;
    private CalculationOptions _calcOptions;
    private CalculationVariables _calcVariables;
    private bool _isInCacheSerialization;
    private const string IncludeUri = "/mathcad/include/include{0}.xml";
    private const string IncludeMimeType = "application/vnd.openxmlformats-officedocument.mathprocessingml.mathcad.include+xml";
    private McdxSerializer _serializer;

    public ITextStyleChangedHandler TextStyleChangedHandler { get; }

    public IncludeWorksheetFileManager()
    {
        Reset();
        _packageOpsProvider = PackageOperationsProvider.Instance;
    }

    internal IRegionFactory RegionFactory { get; set; }

    internal IList<UIElement> Regions { get; set; }

    public string PathIndexes { get; set; }

    internal void Reset()
    {
        InitializeSerialization();
        ResetCalcSettings();
        if (Regions != null)
        {
            Regions.Clear();
        }
    }

    internal CalculationVariables DocCalculationVariables
    {
        get => _calcVariables;
        set => _calcVariables = value;
    }

    internal CalculationOptions DocCalculationOptions
    {
        get => _calcOptions;
        set => _calcOptions = value;
    }

    private void InitializeSerialization()
    {
        _serializationHelper = new SerializationHelper();
        _deserializationStrategy = new IncludeFileDeserializationStrategy(this);
        _serializationStrategy = new IncludeWorksheetSerializationStrategy(this);
        _metadataModel = new MetadataModel();
    }

    private void ResetCalcSettings()
    {
        _calcOptions = null;
        _calcVariables = null;
    }

    internal void OpenFile(string filePath, bool cacheValid)
    {
        if (string.IsNullOrEmpty(filePath))
        {
            return;
        }
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException(filePath);
        }
        string extension = Path.GetExtension(filePath);
        if (FileManager.IsPackage(filePath, extension, true))
        {
            OpenPackage(filePath, cacheValid);
        }
        else
        {
            throw new FormatException(filePath);
        }
    }

    private void OpenPackage(string filePath, bool cacheValid)
    {
        using (FileStream fileStream = PackageOperationsProvider.OpenFileStreamForReadWithFullShare(filePath))
        {
            if (fileStream.Length > 0L)
            {
                using (Package package = Package.Open(fileStream))
                {
                    Open(package, cacheValid);
                }
            }
        }
    }

    private void Open(Package package, bool cacheValid)
    {
        try
        {
            PackageSerializable.GetApplicationPropertiesSerializer(_packageOpsProvider, _serializationHelper).Read(package);
            PackagePart singlePartByContentType = _packageOpsProvider.GetSinglePartByContentType(package, "application/vnd.openxmlformats-officedocument.mathprocessingml.mathcad.settings.calculation+xml");
            if (singlePartByContentType != null)
            {
                _calcOptions = new CalculationOptions();
                _calcVariables = new CalculationVariables();
                // Release IL requires callvirt on newobj without a local slot.
                (new SerializableXmlPackagePart(new CalculationSettings(_calcVariables, _calcOptions, "http://schemas.ptc.com/mathcad/settings/calculation10"), SerializableXmlPackagePart.OnWriteValidationFailedDevAssert) switch { var p => p }).Read(singlePartByContentType);
            }
            PackagePart mainPartFromPackage = _packageOpsProvider.GetMainPartFromPackage(package);
            using (Stream stream = mainPartFromPackage.GetStream(FileMode.Open))
            {
                _deserializationStrategy.CacheValid = cacheValid;
                using (McdxDeserializer mcdxDeserializer = new McdxDeserializer(string.Empty, mainPartFromPackage, _deserializationStrategy, _serializationHelper, new worksheetRegionCollectionSerializer(), true)
                {
                    SkipRegionsNotAffectingCalculation = true
                })
                {
                    mcdxDeserializer.Deserialize(stream);
                    mcdxDeserializer.ThrowFirstRegionDeserializationExceptionIfExists();
                }
                Regions = _deserializationStrategy.Regions;
            }
            using (Stream stream2 = _packageOpsProvider.GetCoreMetadataPartFromPackage(package).GetStream(FileMode.Open))
            {
                using (StreamReader reader = new StreamReader(stream2))
                {
                    new SpiritMetadataSerializer().Deserialize(reader, _metadataModel);
                }
            }
        }
        finally
        {
            _serializationHelper.CleanSerializerTempData(Regions);
            _serializationHelper.Reset();
        }
    }

    private void SerializeCalculationSettings(XmlDocument xmlContentDocument)
    {
        if (_calcVariables != null && _calcOptions != null)
        {
            using (XmlNodeWriter xmlWriter = new XmlNodeWriter(xmlContentDocument.SelectSingleNode(SpiritResolver.SimpleXpathCreator("/", "ws", "cachedWorksheet"), XmlWorksheetData.McdxNameSpaceManager)))
            {
                calculation.XmlSerializer.Serialize(xmlWriter, SetCalculationData(_calcVariables, _calcOptions));
            }
        }
    }

    private void DeSerializeCalculationSettings(XmlDocument xmlContentDocument)
    {
        XmlNode xmlNode = xmlContentDocument.SelectSingleNode(SpiritResolver.SimpleXpathCreator("/", "ws", "cachedWorksheet"), XmlWorksheetData.McdxNameSpaceManager).SelectSingleNode(SpiritResolver.SimpleXpathCreator(".//", "cl", "calculation"), XmlWorksheetData.McdxNameSpaceManager);
        if (xmlNode != null)
        {
            using (XmlNodeReader xmlReader = new XmlNodeReader(xmlNode))
            {
                GetCalculationData((calculation)calculation.XmlSerializer.Deserialize(xmlReader), out _calcVariables, out _calcOptions);
            }
        }
    }

    internal PackagePart WriteIncludedWorksheetToPackage(Package package)
    {
        string text = string.Format(IncludeUri, PathIndexes);
        _serializationStrategy.RegionsToSerialize = Regions;
        _isInCacheSerialization = true;
        PackagePart packagePart = _packageOpsProvider.AddPartToPackage(package, text, IncludeMimeType);
        _serializer = new McdxSerializer(packagePart, _serializationStrategy, _serializationHelper, new cachedWorksheetRegionCollectionSerializer(), SerializableXmlPackagePart.OnWriteValidationFailedDevAssert, true);
        _serializer.Serialize(packagePart);
        _serializer = null;
        _isInCacheSerialization = false;
        return packagePart;
    }

    internal void ReadIncludedWorksheetFromPackage(PackagePart cachedPart)
    {
        try
        {
            _isInCacheSerialization = true;
            using (Stream stream = cachedPart.GetStream(FileMode.Open))
            {
                using (McdxDeserializer mcdxDeserializer = new McdxDeserializer(string.Empty, cachedPart, _deserializationStrategy, _serializationHelper, new cachedWorksheetRegionCollectionSerializer(), true))
                {
                    mcdxDeserializer.Deserialize(stream);
                    Regions = _deserializationStrategy.Regions;
                }
            }
        }
        finally
        {
            _serializationHelper.CleanSerializerTempData(Regions);
            _serializationHelper.Reset();
            _isInCacheSerialization = false;
        }
    }

    private static calculation SetCalculationData(CalculationVariables variables, CalculationOptions options)
    {
        return new calculation
        {
            builtInVariables = new calculationBuiltInVariables
            {
                arrayorigin = variables.IndexOrigin.ToString(),
                constrainttolerance = variables.CTol,
                convergencetolerance = variables.Tol,
                prncolwidth = variables.PRNColWidth.ToString(),
                prnprecision = variables.PRNPrecision.ToString(),
                randomseed = variables.RandSeed.ToString()
            },
            calculationBehavior = new calculationCalculationBehavior
            {
                automaticrecalculation = options.IsAutoCalcOn,
                exactboolean = options.StrictComparisions,
                matrixstrictsingularitycheck = options.StrictSingularityChecking,
                stringsuseorigin = options.StringsIndexedFromOrigin,
                unitsinsymbolics = options.UnitsAndConstantsInSymbolics,
                constantsinsymbolics = options.UnitsAndConstantsInSymbolics,
                symbolicengine = options.SymbolicEngineType,
                binocthexmode = options.BinOctHexMode
            }
        };
    }

    private static void GetCalculationData(calculation calculationData, out CalculationVariables variables, out CalculationOptions options)
    {
        variables = null;
        options = null;
        if (calculationData != null)
        {
            if (calculationData.builtInVariables != null)
            {
                variables = new CalculationVariables
                {
                    IndexOrigin = int.Parse(calculationData.builtInVariables.arrayorigin),
                    CTol = calculationData.builtInVariables.constrainttolerance,
                    Tol = calculationData.builtInVariables.convergencetolerance,
                    PRNColWidth = int.Parse(calculationData.builtInVariables.prncolwidth),
                    PRNPrecision = int.Parse(calculationData.builtInVariables.prnprecision),
                    RandSeed = int.Parse(calculationData.builtInVariables.randomseed)
                };
            }
            if (calculationData.calculationBehavior != null)
            {
                options = new CalculationOptions
                {
                    IsAutoCalcOn = calculationData.calculationBehavior.automaticrecalculation,
                    StrictComparisions = calculationData.calculationBehavior.exactboolean,
                    StrictSingularityChecking = calculationData.calculationBehavior.matrixstrictsingularitycheck,
                    StringsIndexedFromOrigin = calculationData.calculationBehavior.stringsuseorigin,
                    UnitsAndConstantsInSymbolics = calculationData.calculationBehavior.unitsinsymbolics,
                    SymbolicEngineType = calculationData.calculationBehavior.symbolicengine,
                    BinOctHexMode = calculationData.calculationBehavior.binocthexmode
                };
            }
        }
    }

    public void SerializationEpilog(XmlDocument xmlDocument)
    {
        if (_isInCacheSerialization)
        {
            SerializeCalculationSettings(xmlDocument);
        }
    }

    public void DeserializationEpilog(XmlDocument xmlDocument)
    {
        if (_isInCacheSerialization)
        {
            DeSerializeCalculationSettings(xmlDocument);
        }
    }

    public PackagePart PackagePart => S11NProvider.PackagePart;

    public ISerializationHelper Helper => S11NProvider.Helper;

    public XmlNamespaceManager NameSpaceManager => S11NProvider.NameSpaceManager;

    public XmlDocument XmlContentDocument => S11NProvider.XmlContentDocument;

    private string ConstructUniqueIdForIncluded(string includedId)
    {
        return PathIndexes + "_" + includedId;
    }

    public string PackImage(ImagePackagingInformation pictureInfo)
    {
        throw new NotSupportedException("include region shouldn't contain image");
    }

    public string PackExternalObject(string sourceFileName, string objectUniqueId, ExternalObjectType objectType)
    {
        return S11NProvider.PackExternalObject(sourceFileName, ConstructUniqueIdForIncluded(objectUniqueId), objectType);
    }

    public string PackIncludedWorksheet(string fullFilePath, DateTime lastSavedDate, string includedUniqueId, Func<Package, string, PackagePart> writePackagePart)
    {
        return S11NProvider.PackIncludedWorksheet(fullFilePath, lastSavedDate, ConstructUniqueIdForIncluded(includedUniqueId), writePackagePart);
    }

    public string PackFlowDocument(FlowDocument flowDocument, string flowUniqueId)
    {
        return S11NProvider.PackFlowDocument(flowDocument, ConstructUniqueIdForIncluded(flowUniqueId));
    }

    public string PackProtectedPart(Stream content, string uniqueId, string regionType)
    {
        return S11NProvider.PackProtectedPart(content, ConstructUniqueIdForIncluded(uniqueId), regionType);
    }

    public bool UseOverrides => S11NProvider.UseOverrides;

    public IS11NProvider S11NProvider
    {
        get
        {
            DebugTools.AssertNotNull(_serializer, null);
            return _serializer;
        }
    }
}
