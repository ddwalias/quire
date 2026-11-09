#nullable disable
using System.Collections.Generic;
using System.Windows;
using System.Xml;
using Quire.Controls.Core.Serialization;
using Quire.Controls.Worksheet;
using Quire.Mirror;
using Quire.PersistentData;
using Quire.PersistentDataObjects;
using Quire.Serialization;

namespace Quire.Controls;

internal partial class WorksheetSectionSerializationStrategy : ISerializationStrategy
{
    private readonly IWorksheetSectionPersistentData _worksheetSectionData;
    private readonly DelegateFunction0<IRegionType> _worksheetSectionRegionDataCreator;

    public WorksheetSectionSerializationStrategy(IWorksheetSectionPersistentData sectionData, DelegateFunction0<IRegionType> worksheetSectionRegionDataCreator)
    {
        _worksheetSectionData = sectionData;
        _worksheetSectionRegionDataCreator = worksheetSectionRegionDataCreator;
    }

    [MirrorVerified("PtcControlsWorksheetSectionSerializationStrategyMechanicalDifferentialTests")]
    public void SerializeRegionsEpilog(XmlDocument xmlDocument) { }

    public IList<IRegionType> GetRegions(IS11NProvider s11NProvider)
    {
        IList<IRegionType> list = new List<IRegionType>();
        foreach (KeyValuePair<UIElement, Point> item in _worksheetSectionData.RegionsToSerialize)
        {
            IRegionInWhiteboardType regionInWhiteboardType = RegionFactory.Instance.ConvertToIRegionType(_worksheetSectionRegionDataCreator, s11NProvider, item.Key) as IRegionInWhiteboardType;
            regionInWhiteboardType.Location = item.Value;
            list.Add(regionInWhiteboardType);
        }
        return list;
    }
}
