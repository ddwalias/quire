#nullable disable
#pragma warning disable

using System.Windows;
using Quire.Controls.Core;
using Quire.PersistentData;
using Quire.PersistentDataObjects;
using Quire.Serialization;

namespace Quire.Controls.ExcelComponent.View;

public partial class ChartComponentSerializationStrategy
{
    public ChartComponentSerializationStrategy(ChartComponentControl control, DelegateFunction0<IRegionType> regionCreator)
        : base(control, regionCreator)
    {
    }

    protected override IRegionInWhiteboardType ConvertToRegionPersistantData(IS11NProvider s11NProvider, IWhiteboardItem item)
    {
        if (!(_control.RegionFactory.ConvertToIRegionType(_regionCreator, s11NProvider, item as UIElement) is chartComponentRegionType chartComponentRegionType2))
        {
            DebugTools.DevAssert(false, null);
            return null;
        }
        chartComponentRegionType2.Location = GetLocation(item);
        return chartComponentRegionType2;
    }
}
