using System.Collections.Generic;
using System.Linq;
using Quire.Core;

namespace Quire.Controls.Core;

public partial class RegionCollectionWithProperty
{
    public bool ShowAxisExpressions
    {
        get => (bool)GetValue(ShowAxisExpressionsProperty);
        set => SetValue(ShowAxisExpressionsProperty, value);
    }

    public bool ShowTicks
    {
        get => (bool)GetValue(ShowTicksProperty);
        set => SetValue(ShowTicksProperty, value);
    }

    public bool ShowNumbers
    {
        get => (bool)GetValue(ShowNumbersProperty);
        set => SetValue(ShowNumbersProperty, value);
    }

    public TextFormat TextFormat
    {
        get => (TextFormat)GetValue(TextFormatProperty);
        set => SetValue(TextFormatProperty, value);
    }

    public bool CalcIsDisabled
    {
        get => (bool)GetValue(CalcIsDisabledProperty);
        set => SetValue(CalcIsDisabledProperty, value);
    }

    public IEnumerable<IWhiteboardItem> Regions => _regions;

    public IEnumerable<IRichTextBase> TextBlocks => Regions.OfType<IRichTextBase>();

    public IEnumerable<ICurrentMathFormatProperty> MathFormatElements => Regions.OfType<ICurrentMathFormatProperty>();
}
