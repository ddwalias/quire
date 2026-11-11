#nullable disable
using System.Windows;
using Quire.EquationEditor.View;

namespace Quire.Controls.Internal;

internal abstract partial class VisualBinaryOperatorNode : VisualMathNode, IVisualBinaryOperatorNode, IVisualMathNode, IVisualNode
{
    public static DependencyProperty FirstOperandProperty = DependencyProperty.Register(
        "FirstOperand",
        typeof(VisualMathNode),
        typeof(VisualBinaryOperatorNode),
        new FrameworkPropertyMetadata(
            null,
            FrameworkPropertyMetadataOptions.AffectsMeasure |
            FrameworkPropertyMetadataOptions.AffectsArrange |
            FrameworkPropertyMetadataOptions.AffectsParentMeasure |
            FrameworkPropertyMetadataOptions.AffectsParentArrange |
            FrameworkPropertyMetadataOptions.AffectsRender,
            OnFirstOperandPropertyChanged));

    public static DependencyProperty SecondOperandProperty = DependencyProperty.Register(
        "SecondOperand",
        typeof(VisualMathNode),
        typeof(VisualBinaryOperatorNode),
        new FrameworkPropertyMetadata(
            null,
            FrameworkPropertyMetadataOptions.AffectsMeasure |
            FrameworkPropertyMetadataOptions.AffectsArrange |
            FrameworkPropertyMetadataOptions.AffectsParentMeasure |
            FrameworkPropertyMetadataOptions.AffectsParentArrange |
            FrameworkPropertyMetadataOptions.AffectsRender,
            OnSecondOperandPropertyChanged));

    public override IVisualMathNode this[int index]
    {
        get
        {
            switch (index)
            {
                case 0:
                    return FirstOperand;
                case 1:
                    return SecondOperand;
                default:
                    return null;
            }
        }
        set
        {
            switch (index)
            {
                case 0:
                    FirstOperand = value;
                    break;
                case 1:
                    SecondOperand = value;
                    break;
                default:
                    base[index] = value;
                    break;
            }
        }
    }

    public virtual IVisualMathNode FirstOperand
    {
        get => FirstMathNode;
        set => SetValue(FirstOperandProperty, value);
    }

    public virtual IVisualMathNode SecondOperand
    {
        get => SecondMathNode;
        set => SetValue(SecondOperandProperty, value);
    }

    public abstract IVisualMathNode Operator { get; }

    public abstract VisualMathNode OperatorElement { get; }

    protected VisualMathNode FirstMathNode { get; private set; }

    protected VisualMathNode SecondMathNode { get; private set; }

    public override IVisualMathNode[] NavigatableParts => new IVisualMathNode[1] { Operator };

    private static void OnFirstOperandPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((VisualBinaryOperatorNode)d).OnFirstOperandChanged(e);
    }

    private static void OnSecondOperandPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((VisualBinaryOperatorNode)d).OnSecondOperandChanged(e);
    }

    protected virtual void OnFirstOperandChanged(DependencyPropertyChangedEventArgs e)
    {
        FirstMathNode = (VisualMathNode)GetValue(FirstOperandProperty);
        OperandChanged(e);
    }

    protected virtual void OnSecondOperandChanged(DependencyPropertyChangedEventArgs e)
    {
        SecondMathNode = (VisualMathNode)GetValue(SecondOperandProperty);
        OperandChanged(e);
    }

    protected VisualBinaryOperatorNode()
    {
    }
}
