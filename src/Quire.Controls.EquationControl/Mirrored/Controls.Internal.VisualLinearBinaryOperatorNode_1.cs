namespace Quire.Controls.Internal;

internal partial class VisualLinearBinaryOperatorNode<TOperator>
{
    protected new readonly TOperator _operator;

    public VisualLinearBinaryOperatorNode()
        : base(new TOperator())
    {
        _operator = (TOperator)base._operator;
    }
}
