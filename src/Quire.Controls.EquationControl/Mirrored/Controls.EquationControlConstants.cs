namespace Quire.Controls;

public static partial class EquationControlConstants
{
    internal static string GetEquationIdentifierHelpContext(string helpContentPrefix = default)
    {
        return (helpContentPrefix ?? "EquationControl") + ".Identifier";
    }

    internal static string GetEquationOperatorHelpContext(string helpContentPrefix = default)
    {
        return (helpContentPrefix ?? "EquationControl") + ".Operator";
    }

    internal static string GetPlaceholderHelpContext(string helpContentPrefix)
    {
        return GetEquationOperatorHelpContext(helpContentPrefix) + ".PLACEHOLDER";
    }
}
