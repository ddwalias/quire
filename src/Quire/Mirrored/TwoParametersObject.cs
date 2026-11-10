#nullable disable

namespace Quire;

public partial class TwoParametersObject
{
    public object ParameterOne => _parameterOne;

    public object ParameterTwo => _parameterTwo;

    public TwoParametersObject(object parameterOneToInsert, object parameterTwoToInsert)
    {
        _parameterOne = parameterOneToInsert;
        _parameterTwo = parameterTwoToInsert;
    }

    public string GetParameterOneAsString()
    {
        return ParameterOne as string;
    }

    public string GetParameterTwoAsString()
    {
        return ParameterTwo as string;
    }
}
