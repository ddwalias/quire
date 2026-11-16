using System.Text;
using Quire.EquationEditor.Tree;

namespace Quire.Controls.Plots;

public static partial class SExpressionUtils
{
    public static readonly string PlusOperator = DefaultDescriptorTable.Default["ADDITION"].OperatorSExpName;

    public static readonly string MinusOperator = DefaultDescriptorTable.Default["SUBTRACTION"].OperatorSExpName;

    public static readonly string DivOperator = DefaultDescriptorTable.Default["DIVISION"].OperatorSExpName;

    public static readonly string MultOperator = DefaultDescriptorTable.Default["MULTIPLICATION"].OperatorSExpName;

    public static readonly string EvaluationOperator = DefaultDescriptorTable.Default["EVALUATION"].OperatorSExpName;

    public static readonly string DefinitionOperator = DefaultDescriptorTable.Default["DEFINITION"].OperatorSExpName;

    public static readonly string GroupingOperator = DefaultDescriptorTable.Default["GROUPING"].OperatorSExpName;

    public static readonly string NegationOperator = DefaultDescriptorTable.Default["NEGATION"].OperatorSExpName;

    public static readonly string PlaceholderNode = DefaultDescriptorTable.Default["PLACEHOLDER"].OperatorSExpName;

    public static readonly string ResultNode = DefaultDescriptorTable.Default["RESULT"].OperatorSExpName;

    public static readonly string ExponentOperator = DefaultDescriptorTable.Default["EXPONENTIATION"].OperatorSExpName;

    public static readonly string EmptyIdentifier = DefaultDescriptorTable.Default["EMPTY_FUNCTION_NAME"].OperatorSExpName;

    public static readonly string Factorial = DefaultDescriptorTable.Default["FACTORIAL"].OperatorSExpName;

    public static readonly string PercentOperator = DefaultDescriptorTable.Default["PERCENT"].OperatorSExpName;

    public static string Plus(string left, string right)
    {
        return Build(PlusOperator, left, right);
    }

    public static string Minus(string left, string right)
    {
        return Build(MinusOperator, left, right);
    }

    public static string Div(string left, string right)
    {
        return Build(DivOperator, left, right);
    }

    public static string Exponent(string left, string right)
    {
        return Build(ExponentOperator, left, right);
    }

    public static string Mult(string left, string right)
    {
        return Build(MultOperator, left, right);
    }

    public static string Grouping(string arg)
    {
        return Build(GroupingOperator, arg);
    }

    public static string Neg(string arg)
    {
        return Build(NegationOperator, arg);
    }

    public static string Percent(string arg)
    {
        return Build(PercentOperator, arg);
    }

    public static string Build(params string[] terms)
    {
        if (terms.Length == 0)
        {
            return string.Empty;
        }
        if (terms.Length == 1)
        {
            return terms[0];
        }
        StringBuilder stringBuilder = new StringBuilder("(");
        stringBuilder.Append(terms[0]);
        for (int i = 1; i < terms.Length; i++)
        {
            stringBuilder.Append(" ").Append(terms[i]);
        }
        stringBuilder.Append(")");
        return stringBuilder.ToString();
    }
}
