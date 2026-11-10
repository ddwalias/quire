using System;
using System.Windows;
using Quire.EquationEditor.Tree;
using Quire.EquationEditor.View;

namespace Quire.Controls.Internal;

internal partial class VisualOperatorNode
{
    private readonly MathNodeDescriptor _descriptor;
    private readonly VisualMathNode[] _operatorSymbols;
    private readonly VisualMathNode[] _operands;

    public VisualMathNode[] OperatorSymbols => _operatorSymbols;

    public VisualMathNode[] Operands => _operands;

    public override IVisualMathNode[] NavigatableParts => OperatorSymbols;

    public override IVisualMathNode this[int index]
    {
        get => _operands[index];
        set
        {
            if (index >= _operands.Length)
            {
                throw new IndexOutOfRangeException();
            }

            VisualMathNode visualMathNode = value as VisualMathNode;
            _operands[index] = visualMathNode;
            if (Children.Count - _operatorSymbols.Length > index)
            {
                Children[index + _operatorSymbols.Length] = visualMathNode;
            }
            else
            {
                Children.Add(visualMathNode);
            }
        }
    }

    protected VisualOperatorNode(string name, params VisualMathNode[] operatorSymbols)
    {
        _descriptor = DefaultDescriptorTable.Default[name];
        _operands = new VisualMathNode[_descriptor.ChildCount];
        _operatorSymbols = new VisualMathNode[operatorSymbols.Length];
        for (int i = 0; i < operatorSymbols.Length; i++)
        {
            _operatorSymbols[i] = operatorSymbols[i];
            Children.Add(operatorSymbols[i]);
        }
    }

    public void SetVisibilityOfOperatorSymbols(Visibility visibility)
    {
        VisualMathNode[] operatorSymbols = _operatorSymbols;
        for (int i = 0; i < operatorSymbols.Length; i++)
        {
            operatorSymbols[i].Visibility = visibility;
        }
    }
}
