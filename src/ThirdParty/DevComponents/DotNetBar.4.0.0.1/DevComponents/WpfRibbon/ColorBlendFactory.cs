#nullable disable
using System.Windows.Media;
using Quire.Mirror;

namespace Quire.DevComponents.WpfRibbon;

internal partial class ColorBlendFactory
{
    [MirrorVerified("Release IL")]
    private Color m_BlendColor;

    [MirrorVerified("Release IL")]
    public ColorBlendFactory(Color blendColor)
    {
        m_BlendColor = blendColor;
    }
}
