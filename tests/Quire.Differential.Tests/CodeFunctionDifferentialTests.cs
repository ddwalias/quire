using System.Reflection;
using Quire.Mpl.Run;
using Xunit;
using static Quire.Differential.Tests.MplRunSides;

namespace Quire.Differential.Tests;

public class CodeFunctionDifferentialTests
{
    private const BindingFlags DeclaredInstancePublic = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;

    private static string State(object value)
    {
        object? call = Get(value, "m_call");
        var args = (Array?)Get(value, "m_args");
        return $"call={Describe(call)};args={(args == null ? "null" : string.Join(",", args.Cast<object?>().Select(Describe)))}";
    }

    private static object MathcadCode() => New(M("Mpl.Run.Code"), [])!;

    private static object MathcadFunction(object call, Array args)
    {
        var codeType = M("Mpl.Run.Code");
        return M("Mpl.Run.CodeFunction").GetConstructor(DeclaredInstancePublic, null,
            [codeType, codeType.MakeArrayType()], null)!.Invoke([call, args]);
    }

    [SkippableFact]
    public void Constructor_preserves_call_and_arguments()
    {
        Runtime.Sync();
        var qCtor = typeof(CodeFunction).GetConstructor(DeclaredInstancePublic, null,
            [typeof(Code), typeof(Code[])], null)!;
        var mCodeType = M("Mpl.Run.Code");
        var mCtor = M("Mpl.Run.CodeFunction").GetConstructor(DeclaredInstancePublic, null,
            [mCodeType, mCodeType.MakeArrayType()], null)!;
        foreach (Code[]? args in new Code[]?[] { [], [new Code(), null] })
        {
            var call = new Code();
            object? mCall = MathcadCode();
            Array mArgs = Array.CreateInstance(mCodeType, args.Length);
            for (int i = 0; i < args.Length; i++)
                mArgs.SetValue(args[i] == null ? null : New(mCodeType, []), i);
            Func<object?, string> qShow = value => RenderConstructor(value, call, args, typeof(CodeFunction));
            Func<object?, string> mShow = value => RenderConstructor(value, mCall, mArgs, mCtor.DeclaringType!);
            MethodComparison.Compare("CodeFunction constructor", "Mpl.Run.CodeFunction",
                qCtor, null, [call, args], mCtor, null, [mCall, mArgs], qShow, mShow);
        }
    }

    private static string RenderConstructor(object? value, object? expectedCall, object? expectedArgs, System.Type resultType)
    {
        if (value != null && resultType.IsInstanceOfType(value))
            return $"call={ReferenceEquals(Get(value, "m_call"), expectedCall)};args={ReferenceEquals(Get(value, "m_args"), expectedArgs)}";
        return Describe(value);
    }

    [SkippableFact]
    public void BegArg_and_EndArg_forward_the_index()
    {
        Runtime.Sync();
        var qBeg = typeof(CodeFunction).GetMethod(nameof(CodeFunction.BegArg), DeclaredInstancePublic)!;
        var qEnd = typeof(CodeFunction).GetMethod(nameof(CodeFunction.EndArg), DeclaredInstancePublic)!;
        var mType = M("Mpl.Run.CodeFunction");
        var mBeg = mType.GetMethod(nameof(CodeFunction.BegArg), DeclaredInstancePublic)!;
        var mEnd = mType.GetMethod(nameof(CodeFunction.EndArg), DeclaredInstancePublic)!;
        Func<object?, string> show = value => Describe(value);
        Func<object, string> state = State;

        var qInner = new CodeFunction(new Code(), [null]);
        var qArgs = new Code[] { new Code(), qInner };
        var qTarget = new CodeFunction(new Code(), qArgs);
        var mArgs = Array.CreateInstance(M("Mpl.Run.Code"), 2);
        mArgs.SetValue(MathcadCode(), 0);
        mArgs.SetValue(MathcadFunction(MathcadCode(), Array.CreateInstance(M("Mpl.Run.Code"), 1)), 1);
        object mTarget = MathcadFunction(MathcadCode(), mArgs);

        foreach (int index in new[] { 0, 1 })
        {
            MethodComparison.Compare($"CodeFunction.BegArg({index})", "Mpl.Run.CodeFunction",
                qBeg, qTarget, [null, index], mBeg, mTarget, [null, index], show, show, state, state);
            MethodComparison.Compare($"CodeFunction.EndArg({index})", "Mpl.Run.CodeFunction",
                qEnd, qTarget, [null, index], mEnd, mTarget, [null, index], show, show, state, state);
        }
    }
}
