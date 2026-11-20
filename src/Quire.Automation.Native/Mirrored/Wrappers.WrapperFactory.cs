namespace Quire.Automation.Wrappers;

public partial class WrapperFactory
{
    public new static WrapperFactory Instance => new WrapperFactory();

    public override object AutomationToWrapper(object inner)
    {
        if (inner is global::Quire.Automation.NumericValue inner2)
        {
            return new NumericValue(inner2);
        }
        if (inner is global::Quire.Automation.MatrixValue inner3)
        {
            return new MatrixValue(inner3);
        }
        if (inner is global::Quire.Automation.StringValue inner4)
        {
            return new StringValue(inner4);
        }
        throw new global::System.InvalidCastException();
    }

    public override object WrapperToAutomation(object outer)
    {
        if (outer is NumericValue numericValue)
        {
            return numericValue._innerObject;
        }
        if (outer is MatrixValue matrixValue)
        {
            return matrixValue._innerObject;
        }
        if (outer is StringValue stringValue)
        {
            return stringValue._innerObject;
        }
        return null;
    }

    public WrapperFactory()
    {
    }
}
