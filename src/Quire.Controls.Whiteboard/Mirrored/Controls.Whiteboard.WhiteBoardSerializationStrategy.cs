#nullable disable
#pragma warning disable
using System.Collections.Generic;
using System.Windows;
using System.Xml;
using Quire.Controls.Core;
using Quire.Controls.Core.Serialization;
using Quire.PersistentDataObjects;
using Quire.Serialization;

namespace Quire.Controls.Whiteboard;

public abstract partial class WhiteBoardSerializationStrategy : ISerializationStrategy
{
    private readonly WhiteboardControl _whiteboard;

    protected WhiteBoardSerializationStrategy(WhiteboardControl control)
    {
        _whiteboard = control;
    }

    public abstract void SerializeRegionsEpilog(XmlDocument xmlDocument);

    protected abstract IRegionInWhiteboardType ConvertToRegionPersistantData(IS11NProvider s11NProvider, IWhiteboardItem item);

    public IList<IRegionType> GetRegions(IS11NProvider s11NProvider)
    {
        IList<IRegionType> list = new List<IRegionType>();
        foreach (IWhiteboardItem whiteboardItem in _whiteboard.WhiteboardItems)
        {
            IRegionInWhiteboardType regionInWhiteboardType = ConvertToRegionPersistantData(s11NProvider, whiteboardItem);
            if (regionInWhiteboardType != null)
            {
                list.Add(regionInWhiteboardType);
            }
        }
        return list;
    }

    public Point GetLocation(IWhiteboardItem element)
    {
        return _whiteboard.GetItemLocation(element);
    }
}
