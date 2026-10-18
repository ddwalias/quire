using System.Reflection;
using Quire.Mpl.Run;
using Xunit;
using static Quire.Differential.Tests.MplRunSides;

namespace Quire.Differential.Tests;

public class BqDifferentialTests
{
    private const BindingFlags InstancePublic = BindingFlags.Public | BindingFlags.Instance;

    private static void SetMacro(object? quireMacro, object? originalMacro)
    {
        typeof(MplBackquoteMacro).GetField("m_macro", BindingFlags.Static | BindingFlags.NonPublic)!
            .SetValue(null, quireMacro);
        M("Mpl.Run.MplBackquoteMacro").GetField("m_macro", BindingFlags.Static | BindingFlags.NonPublic)!
            .SetValue(null, originalMacro);
    }

    [SkippableFact]
    public void Constructors_ToString_and_walk_expression()
    {
        Runtime.Sync();
        var qType = typeof(Bq);
        var mType = M("Mpl.Run.Bq");
        var qObjectCtor = qType.GetConstructor(InstancePublic, null, [typeof(object)], null)!;
        var mObjectCtor = mType.GetConstructor(InstancePublic, null, [typeof(object)], null)!;
        var qSexprCtor = qType.GetConstructor(InstancePublic, null, [typeof(Sexpr)], null)!;
        var mSexprCtor = mType.GetConstructor(InstancePublic, null, [M("Mpl.Run.Sexpr")], null)!;
        var qToString = qType.GetMethod(nameof(ToString), InstancePublic, Type.EmptyTypes)!;
        var mToString = mType.GetMethod(nameof(ToString), InstancePublic, Type.EmptyTypes)!;
        var qWalk = qType.GetMethod("walk_expression", InstancePublic, [typeof(MplEnv)])!;
        var mWalk = mType.GetMethod("walk_expression", InstancePublic, [M("Mpl.Run.MplEnv")])!;

        Sexpr qValue = int_constant.NewTok(17, 5u);
        object mValue = ToMathcad(qValue)!;
        Func<object?, string> show = value => Describe(value);
        Func<object, string> state = value => Describe(value);

        MethodComparison.Compare("Bq object constructor with expression", "Mpl.Run.Bq",
            qObjectCtor, null, [qValue], mObjectCtor, null, [mValue], show, show);
        MethodComparison.Compare("Bq object constructor with unrelated object", "Mpl.Run.Bq",
            qObjectCtor, null, [new object()], mObjectCtor, null, [new object()], show, show);
        MethodComparison.Compare("Bq Sexpr constructor", "Mpl.Run.Bq",
            qSexprCtor, null, [qValue], mSexprCtor, null, [mValue], show, show);
        MethodComparison.Compare("Bq Sexpr constructor with null", "Mpl.Run.Bq",
            qSexprCtor, null, [null], mSexprCtor, null, [null], show, show);

        var qBq = new Bq(qValue);
        object mBq = ToMathcad(qBq)!;
        MethodComparison.Compare("Bq.ToString", "Mpl.Run.Bq",
            qToString, qBq, [], mToString, mBq, [], show, show, state, state);

        // mplrun's base MplEnv cannot resolve the foreign backquote macro without its host integration.
        // Keep the singleton caches empty so both artifacts exercise this observable failure path.
        SetMacro(null, null);
        try
        {
            MethodComparison.Compare("Bq.walk_expression without an environment", "Mpl.Run.Bq",
                qWalk, qBq, [null], mWalk, mBq, [null], show, show, state, state);
        }
        finally
        {
            SetMacro(null, null);
        }
    }
}
