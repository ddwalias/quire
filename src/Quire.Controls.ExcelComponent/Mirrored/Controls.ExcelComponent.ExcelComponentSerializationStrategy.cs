#nullable disable
#pragma warning disable

using System.Windows;
using Quire.Controls.Core;
using Quire.PersistentData;
using Quire.PersistentDataObjects;
using Quire.Serialization;

namespace Quire.Controls.ExcelComponent;

public partial class ExcelComponentSerializationStrategy
{
    public ExcelComponentSerializationStrategy(ExcelComponentControl control, DelegateFunction0<IRegionType> regionCreator)
        : base(control, regionCreator)
    {
    }

    protected override IRegionInWhiteboardType ConvertToRegionPersistantData(IS11NProvider s11NProvider, IWhiteboardItem item)
    {
        if (!(_control.RegionFactory.ConvertToIRegionType(_regionCreator, s11NProvider, item as UIElement) is excelComponentRegionType excelComponentRegionType2))
        {
            DebugTools.DevAssert(false, null);
            return null;
        }
        excelComponentRegionType2.Location = GetLocation(item);
        return excelComponentRegionType2;
    }
}
