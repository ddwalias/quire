using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Quire.EquationEditor.View;

namespace Quire.Controls.Internal;

internal partial class VerticalFence
{
    protected double _glyphGap;

    public override IVisualMathNode this[int index]
    {
        get => null;
        set
        {
        }
    }

    public FormattedText Top => _components[0];

    public FormattedText Bottom => _components[2];

    public VerticalFence(char fenceChar)
        : this(new char[] { fenceChar })
    {
    }

    public VerticalFence(char[] componentChars)
        : base(componentChars)
    {
        UpdateGlyphGap();
    }

    private VerticalFence()
    {
    }

    private void UpdateGlyphGap()
    {
        if (!IsComposable)
        {
            _glyphGap = -_sgap.Top;
        }
        else
        {
            _glyphGap = -_fgap.Top;
        }
    }

    protected override void ReformatComponents()
    {
        base.ReformatComponents();
        UpdateGlyphGap();
    }

    public override object Clone()
    {
        VerticalFence clone = new VerticalFence();
        Clone(clone);
        clone._glyphGap = _glyphGap;
        return clone;
    }

    protected override void OnContextMenuOpening(ContextMenuEventArgs e)
    {
        e.Handled = true;
    }

    public static VerticalFence CreateLeftAngleBracket()
    {
        return new VerticalFence((char)10216);
    }

    public static VerticalFence CreateRightAngleBracket()
    {
        return new VerticalFence((char)10217);
    }
}
