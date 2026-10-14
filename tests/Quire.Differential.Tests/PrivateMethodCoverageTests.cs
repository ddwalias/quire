using System.Reflection;

namespace Quire.Differential.Tests;

public class PrivateMethodCoverageTests
{
    [SkippableFact]
    public void Private_dimension_intern_is_compared_directly()
    {
        Runtime.Sync();
        const BindingFlags flags = BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;
        var q = typeof(Quire.Mcd.Dim).GetMethod("intern", flags, [typeof(int[])])!;
        var m = Mathcad.Type("mplrun", "Mcd.Dim").GetMethod("intern", flags, [typeof(int[])])!;
        var vector = new[] { 1234567, 0, 0, 0, 0, 0, 0, 0 };
        var vectors = new[] { new int[8], new int[8], vector, (int[])vector.Clone(), new int[7] };
        var previousQ = new Dictionary<string, object>();
        var previousM = new Dictionary<string, object>();
        string Show(Side side, object? value, object input, Dictionary<string, object> previous)
        {
            if (value is int[]) return Reflect.Text(value);
            string coordinates = side.DimText(value);
            bool repeated = previous.TryGetValue(coordinates, out var earlier) && ReferenceEquals(earlier, value);
            previous[coordinates] = value!;
            bool keptInput = ReferenceEquals(Reflect.Get(value!, "Coordinates"), input);
            bool identity = ReferenceEquals(value, side.Marker("identity"));
            return $"{coordinates}; repeated {repeated}; kept input {keptInput}; identity {identity}";
        }
        for (int i = 0; i < vectors.Length; i++)
        {
            object qInput = vectors[i].Clone(), mInput = vectors[i].Clone();
            MethodComparison.Compare($"intern case {i}: {Reflect.Text(vectors[i])}", "Mcd.Dim", q, null, [qInput],
                m, null, [mInput],
                value => Show(Side.Quire, value, qInput, previousQ),
                value => Show(Side.Mathcad, value, mInput, previousM));
        }
    }
}
