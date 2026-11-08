#nullable disable
#pragma warning disable
using System.Xml;
using Quire.Controls.Core;
using Quire.Controls.Whiteboard;

namespace Quire.Controls.ExcelComponent.View;

public partial class OleBaseComponentDeserializationStrategy : WhiteBoardDeserializationStrategy
{
    private readonly OleBaseComponentControl _control;

    public OleBaseComponentDeserializationStrategy(OleBaseComponentControl control)
        : base(control)
    {
        _control = control;
    }

    public override IRegionFactory RegionFactory => _control.RegionFactory;

    public override void DeserializeRegionsEpilog(XmlDocument xmlContentDocument)
    {
        _control.ReplaceCalculatablesWithWrappersInResultManager();
    }
}
