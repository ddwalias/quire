#nullable disable
using System.Collections.Generic;
using System.Xml;
using Quire.PersistentDataObjects;
using Quire.Serialization;

namespace Quire.Controls.Core.Serialization;

public partial interface ISerializationStrategy
{
    void SerializeRegionsEpilog(XmlDocument xmlDocument);
    IList<IRegionType> GetRegions(IS11NProvider s11NProvider);
}
