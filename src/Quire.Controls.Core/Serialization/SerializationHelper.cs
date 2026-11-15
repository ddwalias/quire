#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Quire.Controls.Core.MC14Conversion;
using Quire.Controls.Core.Whiteboard;
using Quire.Mirror;
using Quire.PersistentDataObjects;
using Quire.Serialization;
using Quire.Xml;

namespace Quire.Controls.Core.Serialization;

public partial class SerializationHelper
{
    public static readonly DependencyProperty ConvertedFromProperty = DependencyProperty.RegisterAttached("ConvertedFrom", typeof(applicationVersionType?), typeof(SerializationHelper));

    private Dictionary<SchemasManager.SchemaNameEnum, Version> _serializedSchemaVersions;

    public static readonly DependencyProperty RegionIdProperty = DependencyProperty.RegisterAttached("RegionId", typeof(long), typeof(SerializationHelper));

    private long _regionId;

    private readonly Dictionary<string, Dictionary<byte[], Pair<string, Uri>>> _imagesHashTable = new Dictionary<string, Dictionary<byte[], Pair<string, Uri>>>();

    private static readonly BytesComparer _byteArraysComparer = new BytesComparer();

    private readonly Dictionary<Pair<string, DateTime>, Pair<string, Uri>> _cachedWorksheetsTable = new Dictionary<Pair<string, DateTime>, Pair<string, Uri>>();

    [MirrorVerified("PtcControlsCoreSerializationSerializationHelperMechanicalDifferentialTests")]
    private readonly Dictionary<string, Mathcad14TextStyle> _textStyles = new Dictionary<string, Mathcad14TextStyle>();

    [MirrorVerified("PtcControlsCoreSerializationSerializationHelperMechanicalDifferentialTests")]
    public string ApplicationName { get; set; }

    [MirrorVerified("PtcControlsCoreSerializationSerializationHelperMechanicalDifferentialTests")]
    public Version AppVersion { get; set; }

    [MirrorVerified("PtcControlsCoreSerializationSerializationHelperMechanicalDifferentialTests")]
    public string Release { get; set; }

    [MirrorVerified("PtcControlsCoreSerializationSerializationHelperMechanicalDifferentialTests")]
    public Version FormatVersion { get; set; }

    [MirrorVerified("PtcControlsCoreSerializationSerializationHelperMechanicalDifferentialTests")]
    public string EngineVersion { get; set; }

    public IDictionary<SchemasManager.SchemaNameEnum, Version> SerializedSchemaVersions => _serializedSchemaVersions ?? (_serializedSchemaVersions = new Dictionary<SchemasManager.SchemaNameEnum, Version>());

    public long GetRegionId(UIElement region)
    {
        return (long)((DependencyObject)region).GetValue(RegionIdProperty);
    }

    public void AssignNextRegionId(UIElement region)
    {
        ((DependencyObject)region).SetValue(RegionIdProperty, (object)_regionId);
        _regionId++;
    }

    public long GetNextRegionId()
    {
        long regionId = _regionId;
        _regionId++;
        return regionId;
    }

    public void SetRegionId(UIElement region, long regionId)
    {
        ((DependencyObject)region).SetValue(RegionIdProperty, (object)regionId);
    }

    public UIElement GetRegionById(IEnumerable<UIElement> regions, long value)
    {
        foreach (UIElement region in regions)
        {
            // Release IL keeps the container and the nested result in loop-body locals, declared before the tests.
            IWhiteboardItemContainer whiteboardItemContainer;
            UIElement regionById;
            if (value == GetRegionId(region))
            {
                return region;
            }

            whiteboardItemContainer = region as IWhiteboardItemContainer;
            if (whiteboardItemContainer != null)
            {
                regionById = GetRegionById(whiteboardItemContainer.WhiteboardItems.OfType<UIElement>(), value);
                if (regionById != null)
                {
                    return regionById;
                }
            }
        }

        return null;
    }

    public void CleanSerializerTempData(IList<UIElement> regions)
    {
        if (regions == null)
        {
            return;
        }

        foreach (UIElement region in regions)
        {
            ((DependencyObject)region).ClearValue(RegionIdProperty);
            ((DependencyObject)region).ClearValue(ConvertedFromProperty);
        }
    }

    public void Reset()
    {
        _regionId = 0L;
        _imagesHashTable.Clear();
        _cachedWorksheetsTable.Clear();
        AppVersion = null;
        Release = null;
        FormatVersion = null;
        ApplicationName = null;
        EngineVersion = null;
        SerializedSchemaVersions.Clear();
        CleanupRegions();
    }

    // Parked: Roslyn counts four members per auto-property where Mathcad counts three, so the lambda's
    // closure ordinal here is 38 and Mathcad's is 33.
    public void InitializeForWrite(string appName) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public void AddImagePath(string imageType, byte[] imageHash, Pair<string, Uri> imagePath)
    {
        if (imageHash == null)
        {
            return;
        }

        // Release IL keeps one dictionary local across both branches.
        Dictionary<byte[], Pair<string, Uri>> dictionary;
        if (_imagesHashTable.ContainsKey(imageType))
        {
            dictionary = _imagesHashTable[imageType];
            dictionary.Add(imageHash, imagePath);
            dictionary.ContainsKey(imageHash);
        }
        else
        {
            dictionary = new Dictionary<byte[], Pair<string, Uri>>(_byteArraysComparer) { { imageHash, imagePath } };
            _imagesHashTable.Add(imageType, dictionary);
        }
    }

    public Pair<string, Uri> GetImagePath(string imageType, byte[] imageHash)
    {
        Pair<string, Uri> result = new Pair<string, Uri>(null, null);
        if (imageHash != null && _imagesHashTable.ContainsKey(imageType))
        {
            Dictionary<byte[], Pair<string, Uri>> dictionary = _imagesHashTable[imageType];
            if (dictionary.ContainsKey(imageHash))
            {
                result = dictionary[imageHash];
            }
        }

        return result;
    }

    public Pair<string, Uri> GetCachedWorksheetPath(Pair<string, DateTime> fullFilePathAndTimeStamp)
    {
        Pair<string, Uri> result = new Pair<string, Uri>(string.Empty, null);
        if (_cachedWorksheetsTable.ContainsKey(fullFilePathAndTimeStamp))
        {
            result = _cachedWorksheetsTable[fullFilePathAndTimeStamp];
        }

        return result;
    }

    public void AddCachedPartPath(Pair<string, DateTime> fullFilePathAndTimeStamp, Pair<string, Uri> includePath)
    {
        if (_cachedWorksheetsTable.ContainsKey(fullFilePathAndTimeStamp))
        {
            return;
        }

        _cachedWorksheetsTable.Add(fullFilePathAndTimeStamp, includePath);
    }

    [MirrorVerified("PtcControlsCoreSerializationSerializationHelperMechanicalDifferentialTests")]
    public Dictionary<string, Mathcad14TextStyle> MC14TextStyles => _textStyles;

    [MirrorVerified("PtcControlsCoreSerializationSerializationHelperMechanicalDifferentialTests")]
    public IList<IRegionPersistentData> MainRegions { get; set; }

    [MirrorVerified("PtcControlsCoreSerializationSerializationHelperMechanicalDifferentialTests")]
    public IList<IRegionPersistentData> HeaderRegions { get; set; }

    [MirrorVerified("PtcControlsCoreSerializationSerializationHelperMechanicalDifferentialTests")]
    public IList<IRegionPersistentData> FooterRegions { get; set; }

    [MirrorVerified("PtcControlsCoreSerializationSerializationHelperMechanicalDifferentialTests")]
    public IList<IRegionPersistentData> FirstPageHeaderRegions { get; set; }

    [MirrorVerified("PtcControlsCoreSerializationSerializationHelperMechanicalDifferentialTests")]
    public IList<IRegionPersistentData> FirstPageFooterRegions { get; set; }

    public bool ErrorsOccurredWhenLoadingRegions
    {
        get
        {
            DebugTools.DevAssert(MainRegions != null, null);
            if (!OptionalPartHasErrors(HeaderRegions) && !OptionalPartHasErrors(FooterRegions) && !OptionalPartHasErrors(FirstPageHeaderRegions) && !OptionalPartHasErrors(FirstPageFooterRegions))
            {
                return CompulsoryPartHasErrors(MainRegions);
            }

            return true;
        }
    }

    private static bool OptionalPartHasErrors(IList<IRegionPersistentData> elements)
    {
        if (elements != null)
        {
            return elements.HasDeserializationErrors();
        }

        return false;
    }

    private static bool CompulsoryPartHasErrors(IList<IRegionPersistentData> elements)
    {
        if (elements != null)
        {
            return elements.HasDeserializationErrors();
        }

        return true;
    }

    private void CleanupRegions()
    {
        MainRegions = null;
        HeaderRegions = null;
        FooterRegions = null;
        FirstPageHeaderRegions = null;
        FirstPageFooterRegions = null;
    }

    partial class BytesComparer
    {
        [MirrorVerified("PtcControlsCoreSerializationSerializationHelperMechanicalDifferentialTests")]
        public BytesComparer() { }

        public bool Equals(byte[] x, byte[] y)
        {
            if (x.Length != y.Length)
            {
                return false;
            }

            for (int i = 0; i < x.Length; i++)
            {
                if (x[i] != y[i])
                {
                    return false;
                }
            }

            return true;
        }

        public int GetHashCode(byte[] values)
        {
            int num = 0;
            foreach (byte b in values)
            {
                num += b;
            }

            return num;
        }
    }
}
