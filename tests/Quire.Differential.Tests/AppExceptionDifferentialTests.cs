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
    private static System.Type M => Mathcad.Type("mplrun", "Mpl.Run.AppException");
    private static System.Type MRts => Mathcad.Type("mplrun", "Mpl.Run.Rts");

    private static readonly string[] Messages =
    [
        "", "plain", "a>b", "12>msg", "12>", ">x", "1>2>msg", "x>y>z", "12>34>msg@5", "msg@5", "msg@", "@5", "msg@x", "msg@-1",
        "msg@4294967295", "msg@4294967296", "a@1@2", "1>a@2", "a@1>2", " 12 >msg", "+12>msg", "007>msg", "12>@3", "\t", "bad_dimensions\tLength\tTime",
        "4294967296>m", "-1>m", "1,000>m", "١٢>arabic digits",
    ];

    private static readonly uint[] Arguments = [0, 1, 3, 0xFFFF, 0x10000, uint.MaxValue - 1, uint.MaxValue];

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

    [SkippableFact]
    public void Constructors_and_handle_packing()
    {
        Runtime.Sync();
        var diffs = new List<string>();
        var qInner = new InvalidOperationException("inner");
        var mInner = new InvalidOperationException("inner");
        foreach (var msg in new[] { "m", "", null })
        {
            Same(diffs, $"new({Text(msg)})", () => new AppException(msg!), () => CallCtor(M, ["String"], msg), Dump);
            Same(diffs, $"new({Text(msg)}, inner)", () => new AppException(msg!, qInner), () => CallCtor(M, ["String", "Exception"], msg, mInner), Dump);
            Same(diffs, $"new({Text(msg)}, null inner)", () => new AppException(msg!, (Exception)null!), () => CallCtor(M, ["String", "Exception"], msg, null), Dump);
            foreach (uint a in Arguments)
            {
                Same(diffs, $"new({Text(msg)}, {a})", () => new AppException(msg!, a), () => CallCtor(M, ["String", "UInt32"], msg, a), Dump);
                Same(diffs, $"new(inner, {a})", () => new AppException(qInner, a), () => CallCtor(M, ["Exception", "UInt32"], mInner, a), Dump);
                Same(diffs, $"new({Text(msg)}, inner, {a})", () => new AppException(msg!, qInner, a), () => CallCtor(M, ["String", "Exception", "UInt32"], msg, mInner, a), Dump);
                foreach (uint h in Arguments)
                {
                    Same(diffs, $"new({Text(msg)}, {a}, {h})", () => new AppException(msg!, a, h), () => CallCtor(M, ["String", "UInt32", "UInt32"], msg, a, h), Dump);
                    Same(diffs, $"new({Text(msg)}, inner, {a}, {h})", () => new AppException(msg!, qInner, a, h), () => CallCtor(M, ["String", "Exception", "UInt32", "UInt32"], msg, mInner, a, h), Dump);
                    Same(diffs, $"new({Text(msg)}, inner, {a}, tag {h}, region 3)", () => new AppException(msg!, qInner, a, h, 3u), () => CallCtor(M, ["String", "Exception", "UInt32", "UInt32", "UInt32"], msg, mInner, a, h, 3u), Dump);
                    Same(diffs, $"new({Text(msg)}, inner, {a}, tag 3, region {h})", () => new AppException(msg!, qInner, a, 3u, h), () => CallCtor(M, ["String", "Exception", "UInt32", "UInt32", "UInt32"], msg, mInner, a, 3u, h), Dump);
                }
            }
        }
        Same(diffs, "new(null inner, 1)", () => new AppException((Exception)null!, 1u), () => CallCtor(M, ["Exception", "UInt32"], null, 1u), Dump);
        // setters accumulate rather than replace
        foreach (uint h in Arguments)
            foreach (uint v in new uint[] { 0, 1, 0xFFFF, 0x10000, 0x12345 })
            {
                Same(diffs, $"handle {h}: Region = {v}", () => { var e = new AppException("m", 0u, h); e.Region = v; return e; },
                    () => { var e = New(M, "m", 0u, h); Set(e, "Region", v); return e; }, Dump);
                Same(diffs, $"handle {h}: Tag = {v}", () => { var e = new AppException("m", 0u, h); e.Tag = v; return e; },
                    () => { var e = New(M, "m", 0u, h); Set(e, "Tag", v); return e; }, Dump);
            }
        Same(diffs, "AbortRequestedException", () => new AbortRequestedException(), () => Activator.CreateInstance(Mathcad.Type("mplrun", "Mpl.Run.AbortRequestedException")), Dump);
        var nc = Mathcad.Type("mplrun", "Mpl.Run.NonCriticalException");
        Same(diffs, "NonCriticalException()", () => new NonCriticalException(), () => Activator.CreateInstance(nc), Dump);
        Same(diffs, "NonCriticalException(x)", () => new NonCriticalException("x"), () => New(nc, "x"), Dump);
        Same(diffs, "NonCriticalException(null)", () => new NonCriticalException(null!), () => New(nc, [null]), Dump);
        foreach (var f in new[] { "Default", "Unexpected", "ArgumentIsNull" })
            Same(diffs, $"NonCriticalException.{f}", () => Get(typeof(NonCriticalException), f), () => Get(nc, f));
        foreach (var f in new[] { "errorTagDelimiter", "errorArgDelimiter" })
            Same(diffs, $"AppException.{f}", () => Get(typeof(AppException), f), () => Get(M, f));
        NoDiffs(diffs);
    }

    private static string Unmap(string s) => System.Text.RegularExpressions.Regex.Replace(s, @"\bQuire\.(?=(Mcd|Mpl)\.)", "");

    private static object CallCtor(System.Type type, string[] parameterTypes, params object?[] args) =>
        Unwrapped(() => type.GetConstructors().Single(c => c.GetParameters().Select(p => p.ParameterType.Name).SequenceEqual(parameterTypes)).Invoke(args));

    private static object Unwrapped(Func<object> f)
    {
        try { return f(); }
        catch (System.Reflection.TargetInvocationException e) when (e.InnerException != null)
        {
            System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(e.InnerException).Throw();
            throw;
        }
    }

    /// <summary>Runs <paramref name="f"/>, which must throw, and dumps the exception.</summary>
    private static string Throws(Func<object?> f)
    {
        try { f(); return "no exception"; }
        catch (Exception e)
        {
            while (e is System.Reflection.TargetInvocationException { InnerException: { } i }) e = i;
            return Dump(e);
        }
    }

    [SkippableFact]
    public void Raise_parses_tags_and_arguments()
    {
        Runtime.Sync();
        var diffs = new List<string>();
        Same(diffs, "Raise()", Throws(() => AppException.Raise()), Throws(() => CallSig(M, "Raise", [])));
        foreach (var msg in Messages)
        {
            Same(diffs, $"Raise({Text(msg)})", Throws(() => AppException.Raise(msg)), Throws(() => CallSig(M, "Raise", ["String"], msg)));
            Same(diffs, $"Raise(object {Text(msg)})", Throws(() => AppException.Raise((object)msg)), Throws(() => CallSig(M, "Raise", ["Object"], msg)));
            Same(diffs, $"ChainTagAndArgument({Text(msg)})", Dump(AppException.ChainTagAndArgument(msg)), Dump(Call(M, "ChainTagAndArgument", msg)));
            foreach (uint a in Arguments)
            {
                Same(diffs, $"Raise({Text(msg)}, {a})", Throws(() => AppException.Raise(msg, a)), Throws(() => CallSig(M, "Raise", ["String", "UInt32"], msg, a)));
                Same(diffs, $"Raise({Text(msg)}, {a}, 9)", Throws(() => AppException.Raise(msg, a, 9u)), Throws(() => CallSig(M, "Raise", ["String", "UInt32", "UInt32"], msg, a, 9u)));
                Same(diffs, $"ChainTaggedError({Text(msg)}, {a})", Dump(AppException.ChainTaggedError(msg, a)), Dump(Call(M, "ChainTaggedError", msg, a)));
            }
        }
        Same(diffs, "Raise(object 12.5)", Throws(() => AppException.Raise((object)12.5)), Throws(() => CallSig(M, "Raise", ["Object"], 12.5)));
        Same(diffs, "Raise(object null)", Throws(() => AppException.Raise((object)null!)), Throws(() => CallSig(M, "Raise", ["Object"], [null])));
        Same(diffs, "Raise(string null)", Throws(() => AppException.Raise((string)null!)), Throws(() => CallSig(M, "Raise", ["String"], [null])));
        object?[][] arrays = [[], ["a"], ["a", 1, null, 2.5], ["1>x", "y@3"], [null]];
        foreach (var arr in arrays)
            foreach (uint a in new uint[] { 0, 4, uint.MaxValue })
                Same(diffs, $"Raise([{string.Join(",", arr)}], {a})", Throws(() => AppException.Raise(arr!, a)), Throws(() => CallSig(M, "Raise", ["Object[]", "UInt32"], arr, a)));
        Same(diffs, "Raise(null array, 2)", Throws(() => AppException.Raise((object[])null!, 2u)), Throws(() => CallSig(M, "Raise", ["Object[]", "UInt32"], null, 2u)));
        var inner = new FormatException("f");
        Same(diffs, "Raise(m, inner)", Throws(() => AppException.Raise("m", inner)), Throws(() => CallSig(M, "Raise", ["String", "Exception"], "m", inner)));
        Same(diffs, "Raise(inner, 6)", Throws(() => AppException.Raise(inner, 6u)), Throws(() => CallSig(M, "Raise", ["Exception", "UInt32"], inner, 6u)));
        Same(diffs, "FatalException", Throws(() => { AppException.FatalException("fatal"); return null; }), Throws(() => Call(M, "FatalException", "fatal")));
        Same(diffs, "StackOverflowException", Throws(() => { AppException.StackOverflowException(); return null; }), Throws(() => Call(M, "StackOverflowException")));
        // asAppException finds the first AppException in the chain, or wraps
        Exception[] qChains = [new FormatException("x"), new FormatException("x", new AppException("deep", 4u)), new AppException("top", 1u)];
        var mApp = M;
        Exception[] mChains = [new FormatException("x"), new FormatException("x", (Exception)New(mApp, "deep", 4u)), (Exception)New(mApp, "top", 1u)];
        for (int i = 0; i < qChains.Length; i++)
            Same(diffs, $"asAppException({i})", () => AppException.asAppException(qChains[i]), () => Call(M, "asAppException", mChains[i]), Dump);
        Same(diffs, "asAppException(null)", () => AppException.asAppException(null!), () => Call(M, "asAppException", [null]), Dump);
        NoDiffs(diffs);
    }

    /// <summary>The stack probe on a fresh thread: 260 probes, restores, cancellation, and the reset by a new exception.</summary>
    private static string ProbeScenario(System.Type app, System.Type rts)
    {
        var log = new List<string>();
        var t = new Thread(() =>
        {
            int ok = 0;
            string first = "none";
            for (int i = 0; i < 260 && first == "none"; i++)
            {
                try { Call(app, "ProbeStack"); ok++; }
                catch (Exception e) { first = $"probe {i}: {Thrown(e)}"; }
            }
            log.Add($"probes before failure {ok}; {first}; depth {Get(rts, "m_stack_depth")}");
            Call(app, "RestoreStack");
            Call(app, "RestoreStack");
            log.Add($"after 2 restores depth {Get(rts, "m_stack_depth")}");
            Activator.CreateInstance(app, "resets the budget");
            log.Add($"after new exception depth {Get(rts, "m_stack_depth")}");
            var cts = new CancellationTokenSource();
            Call(rts, "RegisterThreadForCancellationChecking", cts);
            log.Add("probe registered " + Outcome(() => Call(app, "ProbeStack"), _ => "ok"));
            cts.Cancel();
            log.Add("probe cancelled " + Outcome(() => Call(app, "ProbeStack"), _ => "ok"));
            log.Add("probe again " + Outcome(() => Call(app, "ProbeStack"), _ => "ok"));
            log.Add($"depth {Get(rts, "m_stack_depth")}");
        });
        t.Start();
        t.Join();
        return string.Join("\n", log);
    }

    [SkippableFact]
    public void Stack_probe_and_cancellation()
    {
        Runtime.Sync();
        Assert.Equal(ProbeScenario(M, MRts), ProbeScenario(typeof(AppException), typeof(Rts)));
    }
}
