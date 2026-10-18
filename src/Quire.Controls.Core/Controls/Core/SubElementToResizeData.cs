#nullable disable
using System.Windows;
using Quire.Mirror;

namespace Quire.Controls.Core;

public partial class SubElementToResizeData
{
    [MirrorVerified("PtcControlsCoreSubElementToResizeDataMechanicalDifferentialTests")]
    public FrameworkElement Element { get; private set; }

    [MirrorVerified("PtcControlsCoreSubElementToResizeDataMechanicalDifferentialTests")]
    public bool SerializeDimensions { get; private set; }

    [MirrorVerified("PtcControlsCoreSubElementToResizeDataMechanicalDifferentialTests")]
    public bool IsFullSize { get; private set; }
}
