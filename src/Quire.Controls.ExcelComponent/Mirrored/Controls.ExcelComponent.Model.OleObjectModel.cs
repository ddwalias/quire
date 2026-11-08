using System;
using System.Collections.Generic;
using Quire.FunctionalitiesLimitation;
using Quire.PersistentData;
using Quire.Serialization;

namespace Quire.Controls.ExcelComponent.Model;

public partial class OleObjectModel
{
    protected override EnumFunctionalityKey ToggleActivateStateFunctionalityKey => EnumFunctionalityKey.OleToggleActiveStateCommand;

    public override IList<IRegionPersistentData> SetRegionPersistentData(IDeS11NProvider d11NProvider, oleObjectRegionType ole)
    {
        Unpack(d11NProvider, ole.itemidref);
        return null;
    }

    public override oleObjectRegionType GetRegionPersistentData(IS11NProvider s11NProvider, string uniqueId, Action onServerBusy)
    {
        return new oleObjectRegionType(SizeData)
        {
            itemidref = Pack(s11NProvider, uniqueId, ExternalObjectType.Ole, onServerBusy)
        };
    }
}
