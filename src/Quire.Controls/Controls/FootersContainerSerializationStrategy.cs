#nullable disable
using System.Collections.Generic;
using System.Xml;
using Quire.Controls.Core.Serialization;
using Quire.Mirror;
using Quire.PersistentData;
using Quire.PersistentDataObjects;
using Quire.Serialization;

namespace Quire.Controls;

internal partial class FootersContainerSerializationStrategy : ISerializationStrategy
{
    private readonly WorksheetSectionSerializationStrategy _footerStrategy;
    private readonly WorksheetSectionSerializationStrategy _firstPageFooterStrategy;
    private readonly footersContainerRegionCollectionSerializer _containerSerializer;

    public FootersContainerSerializationStrategy(WorksheetSectionSerializationStrategy footerStrategy, WorksheetSectionSerializationStrategy firstPageFooterStrategy, footersContainerRegionCollectionSerializer containerSerializer)
    {
        _footerStrategy = footerStrategy;
        _firstPageFooterStrategy = firstPageFooterStrategy;
        _containerSerializer = containerSerializer;
    }

    [MirrorVerified("PtcControlsFootersContainerSerializationStrategyMechanicalDifferentialTests")]
    public void SerializeRegionsEpilog(XmlDocument xmlDocument) { }

    public IList<IRegionType> GetRegions(IS11NProvider s11NProvider)
    {
        footer footer = _containerSerializer.ContainerWrapper.Footer;
        footer.Regions = _footerStrategy.GetRegions(s11NProvider);
        _containerSerializer.ContainerWrapper.Footer = footer;
        footer firstPageFooter = _containerSerializer.ContainerWrapper.FirstPageFooter;
        firstPageFooter.Regions = _firstPageFooterStrategy.GetRegions(s11NProvider);
        _containerSerializer.ContainerWrapper.FirstPageFooter = firstPageFooter;
        return new List<IRegionType>();
    }
}
