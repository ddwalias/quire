using System.Xml;
using Quire.Controls.Core;
using Quire.Controls.Whiteboard;

namespace Quire.Controls;

public partial class SolveBlockDeserializationStrategy
{
    private readonly SolveBlockControl _control;

    public override IRegionFactory RegionFactory => _control.RegionFactory;

    public SolveBlockDeserializationStrategy(SolveBlockControl control)
        : base(control)
    {
        _control = control;
    }

    public override void DeserializeRegionsEpilog(XmlDocument xmlContentDocument)
    {
        _control.ReplaceCalculatablesWithWrappersInResultManager();
    }
}
