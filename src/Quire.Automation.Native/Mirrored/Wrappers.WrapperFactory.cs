namespace Quire.Automation.Wrappers;

public partial class WrapperFactory
{
    public new static WrapperFactory Instance => new WrapperFactory();

    public override object AutomationToWrapper(object inner)
    {
        if (inner is global::Quire.Automation.NumericValue numericValue)
        {
            return new NumericValue(numericValue);
        }
        if (inner is global::Quire.Automation.MatrixValue matrixValue)
        {
            return new MatrixValue(matrixValue);
        }
        if (inner is global::Quire.Automation.StringValue stringValue)
        {
            return new StringValue(stringValue);
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
