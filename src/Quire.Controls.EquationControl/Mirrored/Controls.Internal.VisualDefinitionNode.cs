using System.Windows;

namespace Quire.Controls.Internal;

internal partial class VisualDefinitionNode
{
    public Visibility CustomOperatorVisibility { get; private set; }

    public VisualDefinitionNode(VisualSymbolNode opSymbol)
        : base(opSymbol)
    {
    }

    internal void UpdateVisibility(Visibility lhsOfDefinitionVisibility)
    {
        CustomOperatorVisibility = lhsOfDefinitionVisibility;
        _operator.Visibility = CustomOperatorVisibility;
        FirstMathNode.Visibility = CustomOperatorVisibility;
    }
}
