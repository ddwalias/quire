using System;
using System.Runtime.Serialization;
using System.Windows;

namespace Quire.Controls.Internal;

internal partial class VisualNode
{
    public VisualNode()
    {
    }

    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        info.AddValue("RenderSize", RenderSize);
    }

    public VisualNode(SerializationInfo info, StreamingContext context)
    {
        RenderSize = (Size)info.GetValue("RenderSize", ((object)RenderSize).GetType());
    }
}
