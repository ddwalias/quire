using System.Reflection;
using System.Runtime.CompilerServices;
using Quire.Calculation.Core;

namespace Quire.Differential.Tests;

public class PtcCalculationCoreEngineErrorRelationsDifferentialTests
{
    private const string Contract = "Ptc.Calculation.Core.EngineError";
    private const BindingFlags InstancePrivate = BindingFlags.Instance | BindingFlags.NonPublic;
    private static readonly System.Type Original = Mathcad.Type("Ptc.Calculation.Core", Contract);
    private static readonly FieldInfo QuireCode = typeof(EngineError).GetField("_code", InstancePrivate)!;
    private static readonly FieldInfo OriginalCode = Original.GetField("_code", InstancePrivate)!;
    private static readonly FieldInfo QuireMessage = typeof(EngineError).GetField("_message", InstancePrivate)!;
    private static readonly FieldInfo OriginalMessage = Original.GetField("_message", InstancePrivate)!;

    [SkippableFact]
    public void Message_get_cases()
    {
        foreach (string message in new[] { "", "plain message", "code\targument" })
        {
            var qError = new EngineError();
            object mError = Activator.CreateInstance(Original)!;
            QuireMessage.SetValue(qError, message);
            OriginalMessage.SetValue(mError, message);
            MethodComparison.Compare($"Message getter: {message}", Contract,
                Method(typeof(EngineError), "get_Message"), qError, [],
                Method(Original, "get_Message"), mError, [],
                Reflect.Text, Reflect.Text, RenderError, RenderError);
        }
    }
    [SkippableFact]
    public void IsEquivalentTo_semantic_cases()
    {
        var qTarget = NewError("outer", 7, 11);
        var mTarget = NewOriginalError("outer", 7, 11);
        var qArgument = NewError("outer", 7, 11);
        var mArgument = NewOriginalError("outer", 7, 11);
        SetOriginal(qTarget, mTarget, NewError("inner", 2, 3), NewOriginalError("inner", 2, 3));
        SetOriginal(qArgument, mArgument, NewError("inner", 2, 3), NewOriginalError("inner", 2, 3));
        Compare("matching nested errors", "IsEquivalentTo", qTarget, mTarget, [qArgument, true], [mArgument, true]);

        var qDifferentNode = NewError("outer", 8, 11);
        var mDifferentNode = NewOriginalError("outer", 8, 11);
        SetOriginal(qDifferentNode, mDifferentNode, NewError("inner", 2, 3), NewOriginalError("inner", 2, 3));
        Compare("node handle checked", "IsEquivalentTo", qTarget, mTarget, [qDifferentNode, true], [mDifferentNode, true]);
        Compare("node handle ignored", "IsEquivalentTo", qTarget, mTarget, [qDifferentNode, false], [mDifferentNode, false]);

        var qDifferentArgument = NewError("outer", 7, 12);
        var mDifferentArgument = NewOriginalError("outer", 7, 12);
        Compare("argument differs", "IsEquivalentTo", qTarget, mTarget, [qDifferentArgument, false], [mDifferentArgument, false]);

        var qDifferentCode = NewError("other", 7, 11);
        var mDifferentCode = NewOriginalError("other", 7, 11);
        Compare("error code differs", "IsEquivalentTo", qTarget, mTarget, [qDifferentCode, false], [mDifferentCode, false]);

        var qDifferentNested = NewError("outer", 7, 11);
        var mDifferentNested = NewOriginalError("outer", 7, 11);
        SetOriginal(qDifferentNested, mDifferentNested, NewError("different", 2, 3), NewOriginalError("different", 2, 3));
        Compare("nested error differs", "IsEquivalentTo", qTarget, mTarget, [qDifferentNested, true], [mDifferentNested, true]);
        var qSharedSource = CreateCalculatableProxy(typeof(global::Quire.Calculation.Core.ICalculatable));
        var mSourceType = Original.GetProperty("ErrorSource")!.PropertyType;
        var mSharedSource = CreateCalculatableProxy(mSourceType);
        var qSameSource = NewError("source", 1, 2);
        var mSameSource = NewOriginalError("source", 1, 2);
        var qSameSourceArgument = NewError("source", 1, 2);
        var mSameSourceArgument = NewOriginalError("source", 1, 2);
        SetErrorSource(qSameSource, mSameSource, qSharedSource, mSharedSource);
        SetErrorSource(qSameSourceArgument, mSameSourceArgument, qSharedSource, mSharedSource);
        Compare("same error source", "IsEquivalentTo", qSameSource, mSameSource,
            [qSameSourceArgument, true], [mSameSourceArgument, true]);

        var qOtherSource = CreateCalculatableProxy(typeof(global::Quire.Calculation.Core.ICalculatable));
        var mOtherSource = CreateCalculatableProxy(mSourceType);
        var qDifferentSource = NewError("source", 1, 2);
        var mDifferentSource = NewOriginalError("source", 1, 2);
        SetErrorSource(qSameSource, mSameSource, qSharedSource, mSharedSource);
        SetErrorSource(qDifferentSource, mDifferentSource, qOtherSource, mOtherSource);
        Compare("different error sources", "IsEquivalentTo", qSameSource, mSameSource,
            [qDifferentSource, true], [mDifferentSource, true]);
        Compare("null error", qTarget, mTarget, [null, true], [null, true]);
    }

    [SkippableFact]
    public void FindNextError_semantic_cases()
    {
        var qRoot = NewError("root", 1, 2);
        var mRoot = NewOriginalError("root", 1, 2);
        var qNext = NewError("next", 3, 4);
        var mNext = NewOriginalError("next", 3, 4);
        SetOriginal(qRoot, mRoot, qNext, mNext);
        Compare("next error is found", "FindNextError", qRoot, mRoot, [qNext], [mNext]);

        var qMiddle = NewError("middle", 5, 6);
        var mMiddle = NewOriginalError("middle", 5, 6);
        var qLeaf = NewError("leaf", 7, 8);
        var mLeaf = NewOriginalError("leaf", 7, 8);
        SetOriginal(qNext, mNext, qMiddle, mMiddle);
        SetOriginal(qMiddle, mMiddle, qLeaf, mLeaf);
        Compare("next error is found deeper in chain", "FindNextError", qRoot, mRoot, [qLeaf], [mLeaf]);

        var qMissing = NewError("missing", 9, 10);
        var mMissing = NewOriginalError("missing", 9, 10);
        Compare("next error is absent", "FindNextError", qRoot, mRoot, [qMissing], [mMissing]);
        Compare("null next error", "FindNextError", qRoot, mRoot, [null], [null]);
    }

    [SkippableFact]
    public void IsAncestorOf_semantic_cases()
    {
        var qRoot = NewError("root", 1, 2);
        var mRoot = NewOriginalError("root", 1, 2);
        var qChild = NewError("child", 3, 4);
        var mChild = NewOriginalError("child", 3, 4);
        SetOriginal(qRoot, mRoot, qChild, mChild);

        Compare("self is an ancestor", "IsAncestorOf", qRoot, mRoot, [NewError("root", 1, 2)], [NewOriginalError("root", 1, 2)]);
        Compare("nested error is an ancestor", "IsAncestorOf", qRoot, mRoot, [NewError("child", 3, 4)], [NewOriginalError("child", 3, 4)]);
        Compare("nonmember is not an ancestor", "IsAncestorOf", qRoot, mRoot, [NewError("missing", 9, 10)], [NewOriginalError("missing", 9, 10)]);
        Compare("null is not an ancestor", "IsAncestorOf", qRoot, mRoot, [null], [null]);
    }

    private static EngineError NewError(string code, uint nodeHandle, uint argument)
    {
        var error = new EngineError { NodeHandle = nodeHandle, Argument = argument };
        QuireCode.SetValue(error, code);
        return error;
    }

    private static object NewOriginalError(string code, uint nodeHandle, uint argument)
    {
        object error = Activator.CreateInstance(Original)!;
        OriginalCode.SetValue(error, code);
        Original.GetProperty("NodeHandle")!.SetValue(error, nodeHandle);
        Original.GetProperty("Argument")!.SetValue(error, argument);
        return error;
    }

    private static void SetOriginal(EngineError qError, object mError, EngineError? qOriginal, object? mOriginal)
    {
        qError.OriginalError = qOriginal!;
        Original.GetProperty("OriginalError")!.SetValue(mError, mOriginal);
    }

    private static void SetErrorSource(EngineError qError, object mError, object qSource, object mSource)
    {
        qError.ErrorSource = (global::Quire.Calculation.Core.ICalculatable)qSource;
        Original.GetProperty("ErrorSource")!.SetValue(mError, mSource);
    }

    private static CalculatableDispatchProxy CreateCalculatableProxy(System.Type interfaceType)
    {
        MethodInfo create = typeof(DispatchProxy).GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Single(method => method.Name == "Create" && method.IsGenericMethodDefinition
                && method.GetGenericArguments().Length == 2 && method.GetParameters().Length == 0)
            .MakeGenericMethod(interfaceType, typeof(CalculatableDispatchProxy));
        return (CalculatableDispatchProxy)create.Invoke(null, null)!;
    }

    public class CalculatableDispatchProxy : DispatchProxy
    {
        protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
        {
            if (targetMethod!.Name == nameof(object.Equals)) return ReferenceEquals(this, args?[0]);
            if (targetMethod.Name == nameof(object.GetHashCode)) return RuntimeHelpers.GetHashCode(this);
            return targetMethod.ReturnType == typeof(void) ? null
                : targetMethod.ReturnType.IsValueType ? Activator.CreateInstance(targetMethod.ReturnType)
                : null;
        }
    }
    private static void Compare(string caseId, string methodName, object qTarget, object mTarget,
        object?[] qArgs, object?[] mArgs)
    {
        System.Type[] qParameters = qArgs.Length == 2 ? [typeof(EngineError), typeof(bool)] : [typeof(EngineError)];
        System.Type[] mParameters = mArgs.Length == 2 ? [Original, typeof(bool)] : [Original];
        MethodInfo qMethod = Method(typeof(EngineError), methodName, qParameters);
        MethodInfo mMethod = Method(Original, methodName, mParameters);
        Compare(caseId, qMethod, qTarget, mMethod, mTarget, qArgs, mArgs);
    }

    private static void Compare(string caseId, object qTarget, object mTarget, object?[] qArgs, object?[] mArgs)
    {
        MethodInfo qMethod = Method(typeof(EngineError), "IsEquivalentTo", typeof(EngineError), typeof(bool));
        MethodInfo mMethod = Method(Original, "IsEquivalentTo", Original, typeof(bool));
        Compare(caseId, qMethod, qTarget, mMethod, mTarget, qArgs, mArgs);
    }

    private static MethodInfo Method(System.Type type, string name, params System.Type[] parameters) =>
        type.GetMethod(name, BindingFlags.Public | BindingFlags.Instance, null, parameters, null)!;

    private static void Compare(string caseId, MethodInfo qMethod, object qTarget, MethodInfo mMethod, object mTarget,
        object?[] qArgs, object?[] mArgs) =>
        MethodComparison.Compare(caseId, Contract, qMethod, qTarget, qArgs, mMethod, mTarget, mArgs,
            RenderError, RenderError, RenderError, RenderError);

    private static string RenderError(object? value)
    {
        if (value == null) return "null";
        if (value is string text) return "\"" + text + "\"";
        if (value is bool boolean) return boolean ? "true" : "false";
        System.Type type = value.GetType();
        FieldInfo? codeField = type.GetField("_code", InstancePrivate);
        if (codeField == null) return value.ToString() ?? type.Name;
        string code = (string?)codeField.GetValue(value) ?? "null";
        string message = (string?)type.GetField("_message", InstancePrivate)?.GetValue(value) ?? "null";
        string source = type.GetProperty("ErrorSource")?.GetValue(value) == null ? "null" : "set";
        uint node = (uint)type.GetProperty("NodeHandle")!.GetValue(value)!;
        uint argument = (uint)type.GetProperty("Argument")!.GetValue(value)!;
        object? original = type.GetProperty("OriginalError")!.GetValue(value);
        return $"EngineError({code},{message},{node},{argument},source={source}; original={RenderError(original)})";
    }
}
