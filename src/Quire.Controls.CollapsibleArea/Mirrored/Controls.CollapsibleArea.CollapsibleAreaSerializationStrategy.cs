#nullable disable
#pragma warning disable

using System.Windows;
using System.Xml;
using Quire.Controls.Core;
using Quire.PersistentDataObjects;
using Quire.Serialization;

namespace Quire.Controls.CollapsibleArea;

public partial class CollapsibleAreaSerializationStrategy
{
    protected readonly CollapsibleAreaControl _control;

    private readonly DelegateFunction0<IRegionType> _regionCreator;

    public CollapsibleAreaSerializationStrategy(CollapsibleAreaControl control, DelegateFunction0<IRegionType> regionCreator)
        : base(control)
    {
        _control = control;
        _regionCreator = regionCreator;
    }

    protected override IRegionInWhiteboardType ConvertToRegionPersistantData(IS11NProvider s11NProvider, IWhiteboardItem item)
    {
        worksheetRegionType region = _control.RegionFactory.ConvertToIRegionType(_regionCreator, s11NProvider, item as UIElement) as worksheetRegionType;
        if (region == null)
        {
            DebugTools.FailAssertion((string)null);
            return null;
        }
        region.Location = GetLocation(item);
        return region;
    }

    public override void SerializeRegionsEpilog(XmlDocument xmlDocument)
    {
    }
}
