#nullable disable
using System.Collections.Generic;
using System.Windows;
using Quire.Serialization;

namespace Quire.PersistentData;

public partial interface IWorksheetSectionPersistentData
{
    IList<IRegionPersistentData> SerializedRegions { get; }
    IDictionary<UIElement, Point> RegionsToSerialize { get; }
    bool ConvertedFromMC14 { get; set; }
    PersistentDataObjects.worksheetMessageIdType MsgId { get; set; }
}
