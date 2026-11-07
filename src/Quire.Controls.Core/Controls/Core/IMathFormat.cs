#nullable disable
using System;
using System.Windows.Media;

namespace Quire.Controls.Core;

public partial interface IMathFormat
{
    string FontFamilyName { get; set; }
    double? FontSize { get; set; }
    bool? IsBold { get; set; }
    bool? IsItalic { get; set; }
    bool? IsUnderline { get; set; }
    Color? FontColor { get; set; }
    Color? HighlightColor { get; set; }
}
