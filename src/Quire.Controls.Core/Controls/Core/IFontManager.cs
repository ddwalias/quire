#nullable disable
using System;
using System.Windows.Media;

namespace Quire.Controls.Core;

public partial interface IFontManager
{
    ExtendedFont GetFont(string fontFamilyName, double fontPointSize, bool isItalic = false, bool isBold = false, bool isUnderline = false, Color? fontColor = default);
    ExtendedFont GetFont(IMathFormat mathFormat);
    ExtendedFont GetScaledFont(IMathFormat mathFormat, double scaleFactor);
    double GetScaledFontSize(IMathFormat mathFormat, double scaleFactor);
}
