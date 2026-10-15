using System.Reflection;
using Quire.Mpl.Run;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

/// <summary>
/// Compares mplrun's <c>Mpl.Run.AppException</c>, <c>AbortRequestedException</c> and <c>NonCriticalException</c> with
/// Quire's: constructors, the packed handle, message parsing in Raise, and the stack probe; cases are listed in
/// docs/mirror/evidence/Mpl.Run.AppException.md.
/// </summary>
public class AppExceptionDifferentialTests
{
    private const string Contract = "Mpl.Run.AppException";
    private static System.Type M => Mathcad.Type("mplrun", "Mpl.Run.AppException");
    private static System.Type MRts => Mathcad.Type("mplrun", "Mpl.Run.Rts");
    private static System.Type MAbort => Mathcad.Type("mplrun", "Mpl.Run.AbortRequestedException");
    private static System.Type MNonCrit => Mathcad.Type("mplrun", "Mpl.Run.NonCriticalException");

    private static readonly string[] Messages =
    [
        "12>msg", "", "plain", "x@5", "a>b", "12>", ">x", "1>2>msg", "x>y>z", "12>34>msg@5", "msg@5", "msg@", "@5", "msg@x", "msg@-1",
        "msg@4294967295", "msg@4294967296", "a@1@2", "1>a@2", "a@1>2", " 12 >msg", "+12>msg", "007>msg", "12>@3", "\t", "bad_dimensions\tLength\tTime",
        "4294967296>m", "-1>m", "1,000>m", "١٢>arabic digits", "tag>rest", "0>m", "12>34>", ">12>m", "12>>m", "12@34", "@0", "12@",
    ];

    private static readonly uint[] Arguments = [0, 1, 3, 0xFFFF, 0x10000, uint.MaxValue - 1, uint.MaxValue];

    private static string Unmap(string s) => System.Text.RegularExpressions.Regex.Replace(s, @"\bQuire\.(?=(Mcd|Mpl)\.)", "");

    /// <summary>An exception and its inner chain: type, message, argument, handle, region, tag, EfiMessage.</summary>
    private static string Dump(object? e)
    {
        if (e == null) return "null";
        var ex = (Exception)e;
        string text = ex.GetType().Name + ": " + Text(Unmap(ex.Message));
        if (ex.GetType().GetProperty("Handle") != null)
            text += $" arg={Get(ex, "Argument")} handle={Get(ex, "Handle")} region={Get(ex, "Region")} tag={Get(ex, "Tag")} efi={Text(Unmap((string)Get(ex, "EfiMessage")!))}";
        return ex.InnerException == null ? text : text + " <- " + Dump(ex.InnerException);
    }

    private static string Show(object? o)
    {
        if (o == null) return "null";
        if (o is Exception ex) return Dump(ex);
        if (o is object[] arr) return "[" + string.Join(", ", arr.Select(Show)) + "]";
        return Reflect.Text(o);
    }

    private static string State(object target) => Dump(target);

    [SkippableFact]
    public void Constructors_and_handle_packing()
    {
        var qCtorStr = typeof(AppException).GetConstructor([typeof(string)])!;
        var mCtorStr = M.GetConstructor([typeof(string)])!;
        var qCtorStrEx = typeof(AppException).GetConstructor([typeof(string), typeof(Exception)])!;
        var mCtorStrEx = M.GetConstructor([typeof(string), typeof(Exception)])!;
        var qCtorStrArg = typeof(AppException).GetConstructor([typeof(string), typeof(uint)])!;
        var mCtorStrArg = M.GetConstructor([typeof(string), typeof(uint)])!;
        var qCtorExArg = typeof(AppException).GetConstructor([typeof(Exception), typeof(uint)])!;
        var mCtorExArg = M.GetConstructor([typeof(Exception), typeof(uint)])!;
        var qCtorStrExArg = typeof(AppException).GetConstructor([typeof(string), typeof(Exception), typeof(uint)])!;
        var mCtorStrExArg = M.GetConstructor([typeof(string), typeof(Exception), typeof(uint)])!;
        var qCtorStrArgHdl = typeof(AppException).GetConstructor([typeof(string), typeof(uint), typeof(uint)])!;
        var mCtorStrArgHdl = M.GetConstructor([typeof(string), typeof(uint), typeof(uint)])!;
        var qCtorStrExArgTagReg = typeof(AppException).GetConstructor([typeof(string), typeof(Exception), typeof(uint), typeof(uint), typeof(uint)])!;
        var mCtorStrExArgTagReg = M.GetConstructor([typeof(string), typeof(Exception), typeof(uint), typeof(uint), typeof(uint)])!;
        var qCtorStrExArgHdl = typeof(AppException).GetConstructor([typeof(string), typeof(Exception), typeof(uint), typeof(uint)])!;
        var mCtorStrExArgHdl = M.GetConstructor([typeof(string), typeof(Exception), typeof(uint), typeof(uint)])!;

        var qGetRegion = typeof(AppException).GetMethod("get_Region")!;
        var mGetRegion = M.GetMethod("get_Region")!;
        var qSetRegion = typeof(AppException).GetMethod("set_Region", [typeof(uint)])!;
        var mSetRegion = M.GetMethod("set_Region", [typeof(uint)])!;
        var qGetTag = typeof(AppException).GetMethod("get_Tag")!;
        var mGetTag = M.GetMethod("get_Tag")!;
        var qSetTag = typeof(AppException).GetMethod("set_Tag", [typeof(uint)])!;
        var mSetTag = M.GetMethod("set_Tag", [typeof(uint)])!;
        var qGetHandle = typeof(AppException).GetMethod("get_Handle")!;
        var mGetHandle = M.GetMethod("get_Handle")!;
        var qGetArg = typeof(AppException).GetMethod("get_Argument")!;
        var mGetArg = M.GetMethod("get_Argument")!;
        var qGetEfi = typeof(AppException).GetMethod("get_EfiMessage")!;
        var mGetEfi = M.GetMethod("get_EfiMessage")!;

        var qInner = new InvalidOperationException("inner");
        var mInner = new InvalidOperationException("inner");

        foreach (var msg in new[] { "m", "", null })
        {
            MethodComparison.Compare($"new({Text(msg)})", Contract, qCtorStr, null, [msg], mCtorStr, null, [msg], Show, Show);
            MethodComparison.Compare($"new({Text(msg)}, inner)", Contract, qCtorStrEx, null, [msg, qInner], mCtorStrEx, null, [msg, mInner], Show, Show);
            MethodComparison.Compare($"new({Text(msg)}, null inner)", Contract, qCtorStrEx, null, [msg, null], mCtorStrEx, null, [msg, null], Show, Show);

            foreach (uint a in Arguments)
            {
                MethodComparison.Compare($"new({Text(msg)}, {a})", Contract, qCtorStrArg, null, [msg, a], mCtorStrArg, null, [msg, a], Show, Show);
                MethodComparison.Compare($"new(inner, {a})", Contract, qCtorExArg, null, [qInner, a], mCtorExArg, null, [mInner, a], Show, Show);
                MethodComparison.Compare($"new({Text(msg)}, inner, {a})", Contract, qCtorStrExArg, null, [msg, qInner, a], mCtorStrExArg, null, [msg, mInner, a], Show, Show);

                foreach (uint h in Arguments)
                {
                    MethodComparison.Compare($"new({Text(msg)}, {a}, {h})", Contract, qCtorStrArgHdl, null, [msg, a, h], mCtorStrArgHdl, null, [msg, a, h], Show, Show);
                    MethodComparison.Compare($"new({Text(msg)}, inner, {a}, {h})", Contract, qCtorStrExArgHdl, null, [msg, qInner, a, h], mCtorStrExArgHdl, null, [msg, mInner, a, h], Show, Show);
                    MethodComparison.Compare($"new({Text(msg)}, inner, {a}, tag {h}, region 3)", Contract, qCtorStrExArgTagReg, null, [msg, qInner, a, h, 3u], mCtorStrExArgTagReg, null, [msg, mInner, a, h, 3u], Show, Show);
                    MethodComparison.Compare($"new({Text(msg)}, inner, {a}, tag 3, region {h})", Contract, qCtorStrExArgTagReg, null, [msg, qInner, a, 3u, h], mCtorStrExArgTagReg, null, [msg, mInner, a, 3u, h], Show, Show);
                }
            }
        }

        MethodComparison.Compare("new(null inner, 1)", Contract, qCtorExArg, null, [null, 1u], mCtorExArg, null, [null, 1u], Show, Show);

        // Properties and setters
        foreach (uint h in Arguments)
        {
            var qTarget = new AppException("m", 0u, h);
            var mTarget = New(M, "m", 0u, h);

            MethodComparison.Compare($"handle {h}: get_Region", Contract, qGetRegion, qTarget, [], mGetRegion, mTarget, [], Reflect.Text, Reflect.Text, State, State);
            MethodComparison.Compare($"handle {h}: get_Tag", Contract, qGetTag, qTarget, [], mGetTag, mTarget, [], Reflect.Text, Reflect.Text, State, State);
            MethodComparison.Compare($"handle {h}: get_Handle", Contract, qGetHandle, qTarget, [], mGetHandle, mTarget, [], Reflect.Text, Reflect.Text, State, State);
            MethodComparison.Compare($"handle {h}: get_Argument", Contract, qGetArg, qTarget, [], mGetArg, mTarget, [], Reflect.Text, Reflect.Text, State, State);
            MethodComparison.Compare($"handle {h}: get_EfiMessage", Contract, qGetEfi, qTarget, [], mGetEfi, mTarget, [], Reflect.Text, Reflect.Text, State, State);

            foreach (uint v in new uint[] { 0, 1, 0xFFFF, 0x10000, 0x12345 })
            {
                var qSetR = new AppException("m", 0u, h);
                var mSetR = New(M, "m", 0u, h);
                MethodComparison.Compare($"handle {h}: set_Region({v})", Contract, qSetRegion, qSetR, [v], mSetRegion, mSetR, [v], Reflect.Text, Reflect.Text, State, State);

                var qSetT = new AppException("m", 0u, h);
                var mSetT = New(M, "m", 0u, h);
                MethodComparison.Compare($"handle {h}: set_Tag({v})", Contract, qSetTag, qSetT, [v], mSetTag, mSetT, [v], Reflect.Text, Reflect.Text, State, State);
            }
        }

        // Also test get_EfiMessage when argument != uint.MaxValue
        var qWithArg = new AppException("m", 5u, 0u);
        var mWithArg = New(M, "m", 5u, 0u);
        MethodComparison.Compare("get_EfiMessage with arg 5", Contract, qGetEfi, qWithArg, [], mGetEfi, mWithArg, [], Reflect.Text, Reflect.Text, State, State);

        // AbortRequestedException constructor
        var qAbortCtor = typeof(AbortRequestedException).GetConstructor([])!;
        var mAbortCtor = MAbort.GetConstructor([])!;
        MethodComparison.Compare("AbortRequestedException()", Contract, qAbortCtor, null, [], mAbortCtor, null, [], Show, Show);

        // NonCriticalException constructors
        var qNonCritCtor0 = typeof(NonCriticalException).GetConstructor([])!;
        var mNonCritCtor0 = MNonCrit.GetConstructor([])!;
        MethodComparison.Compare("NonCriticalException()", Contract, qNonCritCtor0, null, [], mNonCritCtor0, null, [], Show, Show);

        var qNonCritCtor1 = typeof(NonCriticalException).GetConstructor([typeof(string)])!;
        var mNonCritCtor1 = MNonCrit.GetConstructor([typeof(string)])!;
        MethodComparison.Compare("NonCriticalException(x)", Contract, qNonCritCtor1, null, ["x"], mNonCritCtor1, null, ["x"], Show, Show);
        MethodComparison.Compare("NonCriticalException(null)", Contract, qNonCritCtor1, null, [null], mNonCritCtor1, null, [null], Show, Show);
    }

    [SkippableFact]
    public void Raise_parses_tags_and_arguments()
    {
        var qRaise0 = typeof(AppException).GetMethod("Raise", BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly, [])!;
        var mRaise0 = M.GetMethod("Raise", BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly, [])!;
        MethodComparison.Compare("Raise()", Contract, qRaise0, null, [], mRaise0, null, [], Show, Show);

        var qRaiseObj = typeof(AppException).GetMethod("Raise", BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly, [typeof(object)])!;
        var mRaiseObj = M.GetMethod("Raise", BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly, [typeof(object)])!;

        var qRaiseStr = typeof(AppException).GetMethod("Raise", BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly, [typeof(string)])!;
        var mRaiseStr = M.GetMethod("Raise", BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly, [typeof(string)])!;

        var qRaiseStrArg = typeof(AppException).GetMethod("Raise", BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly, [typeof(string), typeof(uint)])!;
        var mRaiseStrArg = M.GetMethod("Raise", BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly, [typeof(string), typeof(uint)])!;

        var qRaiseStrArgHdl = typeof(AppException).GetMethod("Raise", BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly, [typeof(string), typeof(uint), typeof(uint)])!;
        var mRaiseStrArgHdl = M.GetMethod("Raise", BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly, [typeof(string), typeof(uint), typeof(uint)])!;

        var qRaiseStrEx = typeof(AppException).GetMethod("Raise", BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly, [typeof(string), typeof(Exception)])!;
        var mRaiseStrEx = M.GetMethod("Raise", BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly, [typeof(string), typeof(Exception)])!;

        var qRaiseExArg = typeof(AppException).GetMethod("Raise", BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly, [typeof(Exception), typeof(uint)])!;
        var mRaiseExArg = M.GetMethod("Raise", BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly, [typeof(Exception), typeof(uint)])!;

        var qRaiseArr = typeof(AppException).GetMethod("Raise", BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly, [typeof(object[]), typeof(uint)])!;
        var mRaiseArr = M.GetMethod("Raise", BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly, [typeof(object[]), typeof(uint)])!;

        var qChainTagArg = typeof(AppException).GetMethod("ChainTagAndArgument", BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly, [typeof(string)])!;
        var mChainTagArg = M.GetMethod("ChainTagAndArgument", BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly, [typeof(string)])!;

        var qChainTagged = typeof(AppException).GetMethod("ChainTaggedError", BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly, [typeof(string), typeof(uint)])!;
        var mChainTagged = M.GetMethod("ChainTaggedError", BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly, [typeof(string), typeof(uint)])!;

        var qFatal = typeof(AppException).GetMethod("FatalException", BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly, [typeof(string)])!;
        var mFatal = M.GetMethod("FatalException", BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly, [typeof(string)])!;

        var qStackOver = typeof(AppException).GetMethod("StackOverflowException", BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly, [])!;
        var mStackOver = M.GetMethod("StackOverflowException", BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly, [])!;

        var qAsApp = typeof(AppException).GetMethod("asAppException", BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly, [typeof(Exception)])!;
        var mAsApp = M.GetMethod("asAppException", BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly, [typeof(Exception)])!;

        foreach (var msg in Messages)
        {
            foreach (uint a in Arguments)
            {
                MethodComparison.Compare($"ChainTaggedError({Text(msg)}, {a})", Contract, qChainTagged, null, [msg, a], mChainTagged, null, [msg, a], Show, Show);
            }

            MethodComparison.Compare($"ChainTagAndArgument({Text(msg)})", Contract, qChainTagArg, null, [msg], mChainTagArg, null, [msg], Show, Show);
            MethodComparison.Compare($"Raise({Text(msg)})", Contract, qRaiseStr, null, [msg], mRaiseStr, null, [msg], Show, Show);
            MethodComparison.Compare($"Raise(object {Text(msg)})", Contract, qRaiseObj, null, [(object)msg], mRaiseObj, null, [(object)msg], Show, Show);

            foreach (uint a in Arguments)
            {
                MethodComparison.Compare($"Raise({Text(msg)}, {a})", Contract, qRaiseStrArg, null, [msg, a], mRaiseStrArg, null, [msg, a], Show, Show);
                MethodComparison.Compare($"Raise({Text(msg)}, {a}, 9)", Contract, qRaiseStrArgHdl, null, [msg, a, 9u], mRaiseStrArgHdl, null, [msg, a, 9u], Show, Show);
            }
        }

        MethodComparison.Compare("Raise(object 12.5)", Contract, qRaiseObj, null, [12.5], mRaiseObj, null, [12.5], Show, Show);
        MethodComparison.Compare("Raise(object null)", Contract, qRaiseObj, null, [null], mRaiseObj, null, [null], Show, Show);
        MethodComparison.Compare("Raise(string null)", Contract, qRaiseStr, null, [null], mRaiseStr, null, [null], Show, Show);

        object?[][] arrays = [[], ["a"], ["a", 1, null, 2.5], ["1>x", "y@3"], [null]];
        foreach (var arr in arrays)
            foreach (uint a in new uint[] { 0, 4, uint.MaxValue })
                MethodComparison.Compare($"Raise([{string.Join(",", arr)}], {a})", Contract, qRaiseArr, null, [arr, a], mRaiseArr, null, [arr, a], Show, Show);

        MethodComparison.Compare("Raise(null array, 2)", Contract, qRaiseArr, null, [null, 2u], mRaiseArr, null, [null, 2u], Show, Show);

        var qInnerEx = new FormatException("f");
        var mInnerEx = new FormatException("f");
        MethodComparison.Compare("Raise(m, inner)", Contract, qRaiseStrEx, null, ["m", qInnerEx], mRaiseStrEx, null, ["m", mInnerEx], Show, Show);
        MethodComparison.Compare("Raise(inner, 6)", Contract, qRaiseExArg, null, [qInnerEx, 6u], mRaiseExArg, null, [mInnerEx, 6u], Show, Show);
        MethodComparison.Compare("FatalException", Contract, qFatal, null, ["fatal"], mFatal, null, ["fatal"], Show, Show);
        MethodComparison.Compare("StackOverflowException", Contract, qStackOver, null, [], mStackOver, null, [], Show, Show);

        // asAppException
        Exception[] qChains = [new FormatException("x"), new FormatException("x", new AppException("deep", 4u)), new AppException("top", 1u)];
        Exception[] mChains = [new FormatException("x"), new FormatException("x", (Exception)New(M, "deep", 4u)), (Exception)New(M, "top", 1u)];
        for (int i = 0; i < qChains.Length; i++)
            MethodComparison.Compare($"asAppException({i})", Contract, qAsApp, null, [qChains[i]], mAsApp, null, [mChains[i]], Show, Show);
        MethodComparison.Compare("asAppException(null)", Contract, qAsApp, null, [null], mAsApp, null, [null], Show, Show);
    }

    [SkippableFact]
    public void Stack_probe_and_restore()
    {
        var qProbe = typeof(AppException).GetMethod("ProbeStack", BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly, [])!;
        var mProbe = M.GetMethod("ProbeStack", BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly, [])!;
        var qRestore = typeof(AppException).GetMethod("RestoreStack", BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly, [])!;
        var mRestore = M.GetMethod("RestoreStack", BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly, [])!;

        // Reset budget first by creating an AppException
        _ = new AppException("reset");
        _ = New(M, "reset");

        // Normal probe and restore with state tracking
        MethodComparison.Compare("ProbeStack normal", Contract, qProbe, null, [], mProbe, null, [], Show, Show);
        MethodComparison.Compare("RestoreStack normal", Contract, qRestore, null, [], mRestore, null, [], _ => $"depth={Rts.m_stack_depth}", _ => $"depth={Get(MRts, "m_stack_depth")}");

        // Probe stack at budget boundary (depth 1 -> decrements to 0, which is >= 0 so no overflow)
        Rts.m_stack_depth = 1;
        Set(MRts, "m_stack_depth", 1);
        MethodComparison.Compare("ProbeStack depth 1", Contract, qProbe, null, [], mProbe, null, [], Show, Show);
        MethodComparison.Compare("RestoreStack depth 1", Contract, qRestore, null, [], mRestore, null, [], _ => $"depth={Rts.m_stack_depth}", _ => $"depth={Get(MRts, "m_stack_depth")}");

        // Probe under cancellation
        using var cts = new CancellationTokenSource();
        Rts.RegisterThreadForCancellationChecking(cts);
        Call(MRts, "RegisterThreadForCancellationChecking", cts);
        MethodComparison.Compare("ProbeStack registered uncanceled", Contract, qProbe, null, [], mProbe, null, [], Show, Show);

        cts.Cancel();
        MethodComparison.Compare("ProbeStack canceled", Contract, qProbe, null, [], mProbe, null, [], Show, Show);
        MethodComparison.Compare("ProbeStack canceled second", Contract, qProbe, null, [], mProbe, null, [], Show, Show);

        // Probe stack overflow: set budget to 0
        Rts.m_stack_depth = 0;
        Set(MRts, "m_stack_depth", 0);
        MethodComparison.Compare("ProbeStack overflow", Contract, qProbe, null, [], mProbe, null, [], Show, Show);

        // Clean up thread state
        _ = new AppException("cleanup");
        _ = New(M, "cleanup");
    }
}
