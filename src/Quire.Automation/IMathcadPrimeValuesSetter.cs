#nullable disable

namespace Quire.Automation;

public partial interface IMathcadPrimeValuesSetter
{
    void AddScalarValue(string aliasArg, double valueArg, string unitsArg);
    void AddMatrixValue(string aliasArg, object[,] valueArg, string unitsArg);
    void AddStringValue(string aliasArg, string valueArg);
    void AddSExprValue(string aliasArg, string sexpressionArg);
    IMathcadPrimeSetValueResults SetValues(int secondsArg);
}
