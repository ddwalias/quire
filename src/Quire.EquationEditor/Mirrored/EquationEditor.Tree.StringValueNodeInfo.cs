namespace Quire.EquationEditor.Tree;

public partial class StringValueNodeInfo
{
    public new string Value
    {
        get => (string)base.Value;
        set => base.Value = value;
    }

    public override string ValueSExpression => Transform.SExpressionUtility.EscapeStringValue(Value);

    public StringValueNodeInfo()
    {
        Value = string.Empty;
    }

    public StringValueNodeInfo(StringValueNodeInfo other)
    {
        Value = other.Value;
    }

    public StringValueNodeInfo(IdentifierNodeInfo other)
    {
        if (!other.Value.IsSimpleString)
        {
            throw new System.InvalidOperationException("Logic Error: Cannot convert an IdentifierNodeInfo with formatting to a StringValueNode");
        }

        Value = other.Value.ToString();
    }

    protected override object bk() => Clone();

    public StringValueNodeInfo Clone()
    {
        return new StringValueNodeInfo { Value = Value };
    }
}
