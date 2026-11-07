#nullable disable
namespace Quire.Controls.Core;

public partial interface IFormatterHelper
{
    string Mult(string left, string right);
    string Exponent(string left, string right);
    string Neg(string arg);
    string Percent(string arg);
}
