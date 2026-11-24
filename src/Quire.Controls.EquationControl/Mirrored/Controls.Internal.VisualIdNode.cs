using System.Collections.Generic;
using Quire.Controls.Core;
using Quire.EquationEditor.Tree;
using Quire.EquationEditor.View;

namespace Quire.Controls.Internal;

internal partial class VisualIdNode
{
    public FormattedString Identifier => ((IdentifierNodeInfo)NodeInfo).Value;

    public IdLabels? IdLabel => ((IdentifierNodeInfo)NodeInfo).IdLabelForDisplay;

    public IList<double> CharacterWidths
    {
        get
        {
            if (Children.Count == 0)
            {
                return new List<double>();
            }

            IVisualText text = Children[0] as IVisualText;
            if (text == null)
            {
                return new List<double>();
            }

            return text.CharacterWidths;
        }
    }

    public override INodeInfo NodeInfo
    {
        get => base.NodeInfo;
        set
        {
            base.NodeInfo = value;
            MathFormat.IsItalic = true;
            CreateVisualChildren();
        }
    }

    public override double TextBaselineY
    {
        get
        {
            if (Children.Count == 0)
            {
                return 0.0;
            }

            return ((IVisualText)Children[0]).Baseline;
        }
    }

    public override double EffectiveBaselineY => ((VisualMathNode)this).TextBaselineY;

    public override double EffectiveScriptBaselineY
    {
        get
        {
            if (Children.Count == 0)
            {
                return 0.0;
            }

            return ((IVisualText)Children[0]).SuperscriptBaseline;
        }
    }

    public double CaretNormalHeight
    {
        get
        {
            if (Children.Count == 0)
            {
                return 0.0;
            }
            ExtendedFont font = ((VisualTextSpan)Children[0]).Font;
            return font.FontSize * font.GlyphTypeface.Height;
        }
    }

    public double CaretNormalOffset
    {
        get
        {
            if (Children.Count == 0)
            {
                return 0.0;
            }
            ExtendedFont font = ((VisualTextSpan)Children[0]).Font;
            double num = font.FontSize * font.GlyphTypeface.Baseline;
            return EffectiveBaselineY - num;
        }
    }

    public double CaretSubscriptHeight
    {
        get
        {
            if (Children.Count == 0)
            {
                return 0.0;
            }
            ExtendedFont subscriptFont = ((VisualTextSpan)Children[0]).GetSubscriptFont();
            return subscriptFont.FontSize * subscriptFont.GlyphTypeface.Height;
        }
    }

    public double CaretSubscriptOffset
    {
        get
        {
            if (Children.Count == 0)
            {
                return 0.0;
            }
            VisualTextSpan visualTextSpan = (VisualTextSpan)Children[0];
            ExtendedFont subscriptFont = visualTextSpan.GetSubscriptFont();
            double num = subscriptFont.FontSize * subscriptFont.GlyphTypeface.Baseline;
            return visualTextSpan.SubscriptBaseline - num;
        }
    }
}
