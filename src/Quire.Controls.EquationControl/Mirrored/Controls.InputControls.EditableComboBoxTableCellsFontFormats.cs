using System.Windows;
using System.Windows.Media;
using Quire.Controls.Core;
using Quire.Core;

namespace Quire.Controls.InputControls;

public partial class EditableComboBoxTableCellsFontFormats
{
    private readonly Brush _unitCellForeground;
    private readonly Brush _normalCellForeground;
    private readonly MathFormatValues _mathFontFormat = new MathFormatValues();
    private readonly MathFormatValues _unitFontFormat = new MathFormatValues();

    public EditableComboBoxTableCellsFontFormats(IMathFormat mathFormat, IMathFormat unitLabelFont)
    {
        _mathFontFormat.SetPropertiesFrom(mathFormat);
        _unitFontFormat.SetPropertiesFrom(unitLabelFont);
        UnitCellTextDecorations = (_unitFontFormat.IsUnderline.GetValueOrDefault() ? TextDecorations.Underline : new TextDecorationCollection());
        if (!((Freezable)UnitCellTextDecorations).IsFrozen)
        {
            ((Freezable)UnitCellTextDecorations).Freeze();
        }
        string fontFamilyName = (DebugTools.AssertNotNull(_mathFontFormat.FontFamilyName, null).Passed ? _mathFontFormat.FontFamilyName : "Mathcad UniMath Prime");
        CellFontFamily = new FontFamily(fontFamilyName);
        _unitCellForeground = (DebugTools.AssertNotNull(_unitFontFormat.FontColor, null).Passed ? new SolidColorBrush(_unitFontFormat.FontColor.Value) : FontConstants.DefaultMathFontColor);
        if (!((Freezable)_unitCellForeground).IsFrozen)
        {
            ((Freezable)_unitCellForeground).Freeze();
        }
        _normalCellForeground = (DebugTools.AssertNotNull(_mathFontFormat.FontColor, null).Passed ? new SolidColorBrush(_mathFontFormat.FontColor.Value) : FontConstants.DefaultMathFontColor);
        if (!((Freezable)_normalCellForeground).IsFrozen)
        {
            ((Freezable)_normalCellForeground).Freeze();
        }
    }

    public FontFamily CellFontFamily { get; }

    public TextDecorationCollection UnitCellTextDecorations { get; }

    public FontWeight UnitCellFontWeight
    {
        get
        {
            if (!_unitFontFormat.IsBold.GetValueOrDefault())
            {
                return FontWeights.Normal;
            }
            return FontWeights.Bold;
        }
    }

    public FontStyle UnitCellFontStyle
    {
        get
        {
            if (!_unitFontFormat.IsItalic.GetValueOrDefault())
            {
                return FontStyles.Normal;
            }
            return FontStyles.Italic;
        }
    }

    public Brush GetCellForeground(bool isUnitCell)
    {
        if (!isUnitCell)
        {
            return _normalCellForeground;
        }

        return _unitCellForeground;
    }
}
