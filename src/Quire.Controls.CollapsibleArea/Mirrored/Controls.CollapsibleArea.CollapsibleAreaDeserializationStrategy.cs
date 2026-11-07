#nullable disable
#pragma warning disable

using System.Xml;
using Quire.Controls.Core;

namespace Quire.Controls.CollapsibleArea;

public partial class CollapsibleAreaDeserializationStrategy
{
    private readonly CollapsibleAreaControl _control;

    public CollapsibleAreaDeserializationStrategy(CollapsibleAreaControl control)
        : base(control)
    {
        _control = control;
    }

    public override IRegionFactory RegionFactory => _control.RegionFactory;

    public override void DeserializeRegionsEpilog(XmlDocument xmlContentDocument)
    {
    }
}
