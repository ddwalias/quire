#nullable disable
using System.Collections.Generic;
using System.Windows;
using System.Xml;
using Quire.PersistentDataObjects;
using Quire.Serialization;

namespace Quire.Controls.Core.Serialization;

public partial interface IDeserializationStrategy
{
    IList<UIElement> Regions { get; }
    IMathFormat DeserializeMathFormat { get; set; }
    IDictionary<IdLabels, ExtendedFont> DeserializeLabeledIdFormat { get; set; }
    void DeserializeRegionProlog(UIElement region);
    void DeserializeRegionsEpilog(XmlDocument xmlContentDocument);
    IList<IRegionPersistentData> SetRegions(IEnumerable<IRegionType> regions, IDeS11NProvider deS11NProvider);
}
