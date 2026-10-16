using System.Reflection;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

public class MatDimOnErrorDifferentialTests
{
    private const string Contract = "Mcd.MatDim";
    private const BindingFlags StaticDeclared = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;
    private static readonly int[] Tokens = [0x06000641, 0x06000642, 0x06000643, 0x06000644];

    private static MethodInfo Method(Side side, string[] parameterNames) =>
        side.T("Mcd.MatDim").GetMethod("OnError", StaticDeclared, null,
            parameterNames.Select(name => name switch
            {
                "String" => typeof(string),
                "Int32" => typeof(int),
                "UInt32" => typeof(uint),
                _ => side.T("Mcd." + name),
            }).ToArray(), null)
        ?? throw new HarnessException($"Missing {side.Name} MatDim.OnError({string.Join(",", parameterNames)})");

    private static string Show(Side side, object? value)
    {
        if (value == null) return "null";
        if (side.Dim.IsInstanceOfType(value)) return side.DimText(value);
        if (value is IFormattable scalar) return scalar.ToString(null, System.Globalization.CultureInfo.InvariantCulture);
        return side.Show(value);
    }

    private static void Compare(string caseId, string[] signature, object?[] qArgs, object?[] mArgs, int token)
    {
        var qMethod = Method(Side.Quire, signature); var mMethod = Method(Side.Mathcad, signature);
        Assert.Equal(token, mMethod.MetadataToken);
        MethodComparison.Compare(caseId, Contract, qMethod, null, qArgs, mMethod, null, mArgs,
            quireShow: value => Show(Side.Quire, value), originalShow: value => Show(Side.Mathcad, value));
    }

    [SkippableFact]
    public void OnError_overloads_format_and_raise_exact_original_diagnostics()
    {
        Runtime.Sync();
        Assert.Equal(Tokens[0], Method(Side.Mathcad, ["String", "Dim", "UInt32"]).MetadataToken);
        Assert.Equal(Tokens[1], Method(Side.Mathcad, ["Dim", "UInt32"]).MetadataToken);
        Assert.Equal(Tokens[2], Method(Side.Mathcad, ["String", "Dim", "Int32", "Int32", "UInt32"]).MetadataToken);
        Assert.Equal(Tokens[3], Method(Side.Mathcad, ["String", "Dim", "Int32", "UInt32"]).MetadataToken);

        Compare("OnError(string,Dim,uint) custom message and parsed unit", ["String", "Dim", "UInt32"],
            ["custom dimension error", Side.Quire.D("L"), 17u], ["custom dimension error", Side.Mathcad.D("L"), 17u], Tokens[0]);
        Compare("OnError(string,Dim,uint) normalizes null and preserves uint.MaxValue", ["String", "Dim", "UInt32"],
            ["null unit", null, uint.MaxValue], ["null unit", null, uint.MaxValue], Tokens[0]);

        Compare("OnError(Dim,uint) uses the default matrix message", ["Dim", "UInt32"],
            [Side.Quire.D("T"), 0u], [Side.Mathcad.D("T"), 0u], Tokens[1]);
        Compare("OnError(Dim,uint) normalizes null dimension at uint.MaxValue", ["Dim", "UInt32"],
            [null, uint.MaxValue], [null, uint.MaxValue], Tokens[1]);

        Compare("OnError(string,Dim,int,int,uint) keeps negative then positive index order", ["String", "Dim", "Int32", "Int32", "UInt32"],
            ["matrix index error", Side.Quire.D("N"), -3, 8, 23u], ["matrix index error", Side.Mathcad.D("N"), -3, 8, 23u], Tokens[2]);
        Compare("OnError(string,Dim,int,int,uint) keeps zero then negative index order at max uint", ["String", "Dim", "Int32", "Int32", "UInt32"],
            ["matrix index error", Side.Quire.D("identity"), 0, -5, uint.MaxValue], ["matrix index error", Side.Mathcad.D("identity"), 0, -5, uint.MaxValue], Tokens[2]);

        Compare("OnError(string,Dim,int,uint) formats one negative index", ["String", "Dim", "Int32", "UInt32"],
            ["vector index error", Side.Quire.D("v"), -1, 37u], ["vector index error", Side.Mathcad.D("v"), -1, 37u], Tokens[3]);
        Compare("OnError(string,Dim,int,uint) formats null as identity and zero index", ["String", "Dim", "Int32", "UInt32"],
            ["vector index error", null, 0, uint.MaxValue], ["vector index error", null, 0, uint.MaxValue], Tokens[3]);
    }
}
