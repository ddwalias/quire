using System.Windows;

namespace Quire.Controls.Internal;

internal partial class VisualOperatorNode
{
    public VisualMathNode[] OperatorSymbols => _operatorSymbols;

    public VisualMathNode[] Operands => _operands;

    public void SetVisibilityOfOperatorSymbols(Visibility visibility)
    {
        VisualMathNode[] operatorSymbols = _operatorSymbols;
        for (int i = 0; i < operatorSymbols.Length; i++)
        {
            operatorSymbols[i].Visibility = visibility;
        }
    }
}
