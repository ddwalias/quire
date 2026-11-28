#nullable disable
using Quire.Mirror;

namespace Quire.DevComponents.WpfRibbon;

public partial class TitlePanel
{
    [MirrorVerified("Release IL")]
    private double m_TitleChromeHeight;

    [MirrorVerified("Release IL")]
    private double GetContextGroupHeight() => m_TitleChromeHeight + 20.0;

    [MirrorVerified("Release IL")]
    public TitlePanel() { }

    [MirrorVerified("Release IL")]
    private double m_TabXPosition;

    [MirrorVerified("Release IL")]
    internal double TabXPosition { get => m_TabXPosition; set => m_TabXPosition = value; }

    [MirrorVerified("Release IL")]
    private double m_QatRightPosition;

    [MirrorVerified("Release IL")]
    internal double QatRightPosition { get => m_QatRightPosition; set => m_QatRightPosition = value; }

    [MirrorVerified("Release IL")]
    internal double TitleChromeHeight { get => m_TitleChromeHeight; set => m_TitleChromeHeight = value; }

    partial class RectWidthComparer
    {
        [MirrorVerified("Release IL")]
        public RectWidthComparer() { }
    }
}
