using System.Reflection;
using Quire.Mpl.Run;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

public class SimpleKeywordDifferentialTests
{
    private static System.Type MathcadType(string name) => Mathcad.Type("mplrun", "Mpl.Run." + name);

    private static string Show(object? value)
    {
        if (value == null) return "null";
        if (value is string or bool or byte or sbyte or short or ushort or int or uint or long or ulong or float or double or decimal)
            return Text(value);
        if (value is Keyword keyword) return keyword.name + "/" + keyword.KeyNum;
        if (value.GetType().Name.EndsWith("_keyword", StringComparison.Ordinal))
            return Convert.ToString(Get(value, "name"), System.Globalization.CultureInfo.InvariantCulture) + "/" + Convert.ToString(Get(value, "KeyNum"), System.Globalization.CultureInfo.InvariantCulture);
        if (value is not Tok && value.GetType().Name != "Tok") return Text(value);
        var atom = value is Tok tok ? tok.atom : Get(value, "Field1");
        var handle = value is Tok qt ? qt.handle : Get(value, "Field2");
        return Text(atom) + "@" + Text(handle);
    }

    private static string SingletonState(object? target, string field)
    {
        var singleton = target == null ? null : Get(target, field);
        return singleton == null ? "null" : Show(singleton);
    }

    private static void CompareKeyword(string name, System.Type quireType, int keynum)
    {
        var originalType = MathcadType(name);
        var qCctor = quireType.TypeInitializer!;
        var mCctor = originalType.TypeInitializer!;
        MethodComparison.Compare(name + ".cctor", "Mpl.Run." + name, qCctor, null, [], mCctor, null, [],
            _ => SingletonState(quireType, "singleton"), _ => SingletonState(originalType, "singleton"));

        var qCtor = quireType.GetConstructor(Type.EmptyTypes)!;
        var mCtor = originalType.GetConstructor(Type.EmptyTypes)!;
        MethodComparison.Compare(name + ".ctor", "Mpl.Run." + name, qCtor, null, [], mCtor, null, [], Show, Show);

        var qSingleton = Get(quireType, "singleton")!;
        var mSingleton = Get(originalType, "singleton")!;
        Assert.Equal(keynum, Get(qSingleton, "KeyNum"));
        Assert.Equal(keynum, Get(mSingleton, "KeyNum"));

        var qNewTok = quireType.GetMethod("NewTok", BindingFlags.Public | BindingFlags.Static)!;
        var mNewTok = originalType.GetMethod("NewTok", BindingFlags.Public | BindingFlags.Static)!;
        foreach (uint handle in new[] { 0u, 1u, 37u, uint.MaxValue })
        {
            MethodComparison.Compare(name + ".NewTok(" + handle + ")", "Mpl.Run." + name,
                qNewTok, null, [handle], mNewTok, null, [handle], Show, Show);
        }
    }

    [SkippableFact]
    public void Singleton_constructors_and_tokens()
    {
        CompareKeyword(nameof(List_keyword), typeof(List_keyword), 13);
        CompareKeyword(nameof(Tuple_keyword), typeof(Tuple_keyword), 14);
    }
}
