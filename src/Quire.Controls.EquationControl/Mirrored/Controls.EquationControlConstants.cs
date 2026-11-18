using System;
using System.Collections.Generic;

namespace Quire.Controls;

public static partial class EquationControlConstants
{
    internal const string EquationHelpContext = "EquationControl";
    internal const string ScaledEquationHelpContext = "ScaledEquationControl";
    internal const string Identifier = "Identifier";
    internal const string Operator = "Operator";
    internal const string Odesolve = "odesolve";
    internal const string Pdesolve = "pdesolve";
    internal const string Numol = "numol";
    internal const string Genfit = "genfit";
    internal const string Find = "find";
    internal const string Maximize = "maximize";
    internal const string Minerr = "minerr";
    internal const string Minimize = "minimize";
    internal const string polyroots = "polyroots";
    internal const string MultiStart = "multiStart";
    internal const string Optimize = "optimize";
    internal const string DefiniteIntegralContextMenu = "DefiniteIntegralContextMenu";
    internal const string OdeSolveContextMenu = "OdeSolveContextMenu";
    internal const string NumolAndPdesolveContextMenu = "NumolAndPdesolveContextMenu";
    internal const string GenfitContextMenu = "GenfitContextMenu";
    internal const string FindAndMinerrContextMenu = "FindAndMinerrContextMenu";
    internal const string MinimizeAndMaximizeContextMenu = "MinimizeAndMaximizeContextMenu";
    internal const string PolyrootsContextMenu = "PolyrootsContextMenu";
    internal const string SymbolicEvalContextMenu = "SymbolicEvalContextMenu";

    internal static string[] DefaultAlgorithms = new string[5] { "autoSelect", "adams", "recursive5PointDifferences", "optimizedLevenbergMarquardt", "laGuerre" };

    internal static readonly HashSet<string> EnabledAlgorithmsForOptions = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "nonlinearConjugateGradient", "nonlinearSQP", "nonlinearInteriorPoint", "nonlinearActiveSet" };

    internal static readonly string[] specialIdentifiers = new string[3] { "sec", "min", "R" };

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
