#nullable disable
using System.Windows.Media;
using Quire.Mirror;

namespace Quire.Controls.Core;

public partial class FontManager
{
    [MirrorVerified("PtcControlsCoreFontManagerMechanicalDifferentialTests")]
    public bool IsConnected { get; set; }

    partial class FontKey
    {
        [MirrorVerified("PtcControlsCoreFontManagerMechanicalDifferentialTests")]
        public FontKey(string fontFamilyName, double fontPointSize, bool isItalic, bool isBold, bool isUnderline, Color fontColor) : base(fontFamilyName, fontPointSize, isItalic, isBold, isUnderline, fontColor) { }

        [MirrorVerified("PtcControlsCoreFontManagerMechanicalDifferentialTests")]
        public string FontFamilyName => base.Item1;

        [MirrorVerified("PtcControlsCoreFontManagerMechanicalDifferentialTests")]
        public double FontPointSize => base.Item2;

        [MirrorVerified("PtcControlsCoreFontManagerMechanicalDifferentialTests")]
        public bool IsItalic => base.Item3;

        [MirrorVerified("PtcControlsCoreFontManagerMechanicalDifferentialTests")]
        public bool IsBold => base.Item4;

        [MirrorVerified("PtcControlsCoreFontManagerMechanicalDifferentialTests")]
        public bool IsUnderline => base.Item5;

        [MirrorVerified("PtcControlsCoreFontManagerMechanicalDifferentialTests")]
        public Color FontColor => base.Item6;
    }
}
