#nullable disable
#pragma warning disable
using System.Xml;
using Quire.Core;
using Quire.Controls.Whiteboard;
using Quire.PersistentDataObjects;

namespace Quire.Controls.ExcelComponent.View;

public abstract partial class OleBaseComponentSerializationStrategy : WhiteBoardSerializationStrategy
{
    protected readonly OleBaseComponentControl _control;
    protected readonly DelegateFunction0<IRegionType> _regionCreator;

    protected OleBaseComponentSerializationStrategy(OleBaseComponentControl control, DelegateFunction0<IRegionType> regionCreator)
        : base(control)
    {
        _control = control;
        _regionCreator = regionCreator;
    }

    public override void SerializeRegionsEpilog(XmlDocument xmlDocument)
    {
        _control.ReplaceCalculatablesAndResultWithWrappersInResultManager();
    }
}
