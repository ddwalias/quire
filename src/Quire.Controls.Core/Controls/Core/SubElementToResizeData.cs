#nullable disable
using System.Windows;
using Quire.Mirror;

namespace Quire.Controls.Core;

public partial class SubElementToResizeData
{
    public FrameworkElement Element { get; private set; }

    public bool SerializeDimensions { get; private set; }

    public bool IsFullSize { get; private set; }

    public SubElementToResizeData(FrameworkElement element, bool serializeDimensions, bool isFullSize)
    {
        Element = element;
        SerializeDimensions = serializeDimensions;
        IsFullSize = isFullSize;
    }
}
