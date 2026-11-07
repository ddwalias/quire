#nullable disable
using System.IO;

namespace Quire.Controls.Core.Serialization;

public partial interface IMetadataSerializer
{
    void Serialize(IMetadataModel model, TextWriter writer);
    void Deserialize(TextReader reader, IMetadataModel model);
}
