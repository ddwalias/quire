#nullable disable
using System.Collections.Generic;
using System.Xml;
using Quire.Controls.Core.Serialization;
using Quire.Mirror;
using Quire.PersistentData;
using Quire.PersistentDataObjects;
using Quire.Serialization;

namespace Quire.Controls;

internal partial class HeadersContainerSerializationStrategy : ISerializationStrategy
{
    private readonly WorksheetSectionSerializationStrategy _headerStrategy;
    private readonly WorksheetSectionSerializationStrategy _firstPageHeaderStrategy;
    private readonly headersContainerRegionCollectionSerializer _containerSerializer;

    public HeadersContainerSerializationStrategy(WorksheetSectionSerializationStrategy headerStrategy, WorksheetSectionSerializationStrategy firstPageHeaderStrategy, headersContainerRegionCollectionSerializer containerSerializer)
    {
        _headerStrategy = headerStrategy;
        _firstPageHeaderStrategy = firstPageHeaderStrategy;
        _containerSerializer = containerSerializer;
    }

    [MirrorVerified("PtcControlsHeadersContainerSerializationStrategyMechanicalDifferentialTests")]
    public void SerializeRegionsEpilog(XmlDocument xmlDocument) { }

    public IList<IRegionType> GetRegions(IS11NProvider s11NProvider)
    {
        header header = _containerSerializer.ContainerWrapper.Header;
        header.Regions = _headerStrategy.GetRegions(s11NProvider);
        _containerSerializer.ContainerWrapper.Header = header;
        header firstPageHeader = _containerSerializer.ContainerWrapper.FirstPageHeader;
        firstPageHeader.Regions = _firstPageHeaderStrategy.GetRegions(s11NProvider);
        _containerSerializer.ContainerWrapper.FirstPageHeader = firstPageHeader;
        return new List<IRegionType>();
    }
}
