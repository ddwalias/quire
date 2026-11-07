#nullable disable
using System.Collections.Generic;

namespace Quire.Controls.Core.IORegions;

public partial interface IValuesSetter
{
    IEnumerable<string> Aliases { get; }
    void AddScalarValue(string alias, double value, string units);
    void AddMatrixValue(string alias, object[,] value, string units);
    void AddStringValue(string aliasArg, string valueArg);
    void AddSExprValue(string aliasArg, string sexpressionArg);
    ISetValueResults SetValues(int seconds);
}
