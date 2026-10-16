using System.Reflection;
using Quire.Mpl.Run;
using Xunit;
using static Quire.Differential.Tests.MplRunSides;

namespace Quire.Differential.Tests;

public class CodeArgDifferentialTests
{
    private const BindingFlags DeclaredInstancePublic = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;

    [SkippableFact]
    public void Constructors_preserve_the_code_references()
    {
        Runtime.Sync();
        var qCode = typeof(Code);
        var mCode = M("Mpl.Run.Code");
        object qNullCode = New(qCode, [])!;
        object mNullCode = Get(mCode, "NullCode")!;
        qCode.GetField("NullCode", BindingFlags.Public | BindingFlags.Static)!.SetValue(null, qNullCode);
        var qCodeArg = Q("Mpl.Run.CodeArg");
        var mCodeArg = M("Mpl.Run.CodeArg");
        var qPairCtor = qCodeArg.GetConstructor(DeclaredInstancePublic, null, [qCode, qCode], null)!;
        var mPairCtor = mCodeArg.GetConstructor(DeclaredInstancePublic, null, [mCode, mCode], null)!;
        var qSingleCtor = qCodeArg.GetConstructor(DeclaredInstancePublic, null, [qCode], null)!;
        var mSingleCtor = mCodeArg.GetConstructor(DeclaredInstancePublic, null, [mCode], null)!;
        object qFirst = New(qCode, [])!;
        object qSecond = New(qCode, [])!;
        object mFirst = New(mCode, [])!;
        object mSecond = New(mCode, [])!;
        var qShow = (object? value) => value == null || value.GetType().Name != "CodeArg"
            ? Describe(value)
            : $"beg={ReferenceEquals(Get(value, "m_beg"), qFirst)};end={ReferenceEquals(Get(value, "m_end"), qSecond)}";
        var mShow = (object? value) => value == null || value.GetType().Name != "CodeArg"
            ? Describe(value)
            : $"beg={ReferenceEquals(Get(value, "m_beg"), mFirst)};end={ReferenceEquals(Get(value, "m_end"), mSecond)}";
        MethodComparison.Compare("CodeArg(Code, Code) retains both references", "Mpl.Run.CodeArg",
            qPairCtor, null, [qFirst, qSecond], mPairCtor, null, [mFirst, mSecond], qShow, mShow);
        qSecond = qNullCode;
        mSecond = mNullCode;
        MethodComparison.Compare("CodeArg(Code) uses Code.NullCode", "Mpl.Run.CodeArg",
            qSingleCtor, null, [qFirst], mSingleCtor, null, [mFirst], qShow, mShow);
        qFirst = null!;
        mFirst = null!;
        MethodComparison.Compare("CodeArg(null) retains null and Code.NullCode", "Mpl.Run.CodeArg",
            qSingleCtor, null, [qFirst], mSingleCtor, null, [mFirst], qShow, mShow);
    }
}
