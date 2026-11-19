using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Data;

namespace Quire;

public partial class MathFontSizeValidationRule
{
    protected override double MinFontSize => global::Quire.Controls.Core.FontConstants.MinimumFontSizeInPoints;
    public MathFontSizeValidationRule() { }
}
