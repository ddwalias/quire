using System.Runtime.Serialization;

namespace Quire.Controls.Internal;

internal partial class VisualNode
{
    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        info.AddValue("RenderSize", RenderSize);
    }
}
