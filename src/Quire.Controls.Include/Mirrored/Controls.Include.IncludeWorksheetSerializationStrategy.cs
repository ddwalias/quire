using System.Collections.Generic;
using System.Windows;
using System.Xml;
using Quire.Controls.Core;
using Quire.Controls.Core.Serialization;
using Quire.PersistentData;
using Quire.PersistentDataObjects;
using Quire.Serialization;

namespace Quire.Controls.Include;

public partial class IncludeWorksheetSerializationStrategy : ISerializationStrategy
{
    private IList<UIElement> _regionsToSerialize;
    private readonly IncludeWorksheetFileManager _fileManager;

    public IncludeWorksheetSerializationStrategy(IncludeWorksheetFileManager fileManager)
    {
        _fileManager = fileManager;
    }

    public void SerializeRegionsEpilog(XmlDocument xmlDocument)
    {
        _fileManager.SerializationEpilog(xmlDocument);
    }

    public IList<IRegionType> GetRegions(IS11NProvider s11NProvider)
    {
        IList<IRegionType> list = new List<IRegionType>();
        DebugTools.AssertNotNull(_regionsToSerialize, null);
        DebugTools.DevAssert(s11NProvider == _fileManager.S11NProvider, null);
        foreach (UIElement item in _regionsToSerialize)
        {
            if (!(item is ImageControl) && _fileManager.RegionFactory.ConvertToIRegionType(() => new cachedWorksheetRegionType(), _fileManager, item) is IRegionInWhiteboardType regionInWhiteboardType)
            {
                regionInWhiteboardType.Location = DocumentAttachedProperties.GetLocation(item);
                list.Add(regionInWhiteboardType);
            }
        }
        return list;
    }

    public IList<UIElement> RegionsToSerialize
    {
        set => _regionsToSerialize = value;
    }
}
