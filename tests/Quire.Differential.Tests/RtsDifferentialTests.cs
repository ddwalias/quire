using System.Reflection;
using Quire.Mpl.Run;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

/// <summary>
/// Compares mplrun's <c>Mpl.Run.Rts</c> and <c>Mpl.Run.True</c> with Quire's: the stack budget, cancellation latch,
/// reflection handles and assembly search path; cases are listed in docs/mirror/evidence/Mpl.Run.Rts.md.
/// </summary>
public class RtsDifferentialTests
{
    private static System.Type M => Mathcad.Type("mplrun", "Mpl.Run.Rts");
    private static System.Type MTrue => Mathcad.Type("mplrun", "Mpl.Run.True");

    private static string Member(object? o) => o switch
    {
        null => "null",
        ConstructorInfo c => $"{c.DeclaringType!.Name}({string.Join(", ", c.GetParameters().Select(p => p.ParameterType.Name))})",
        FieldInfo f => $"{f.DeclaringType!.Name}.{f.Name}: {f.FieldType.Name} static={f.IsStatic}",
        System.Type t => t.Namespace?.StartsWith("Quire.") == true ? t.FullName![6..] : t.FullName!,
        _ => Text(o),
    };

    [SkippableFact]
    public void Handles_and_settings()
    {
        Runtime.Sync();
        var diffs = new List<string>();
        foreach (var f in new[] { "ExceptionCtor", "RethrowCtor", "TagCtor", "FcnType", "BoolTrue", "TagField", "VoidType", "TagType" })
            Same(diffs, f, () => Call(typeof(Rts), f), () => Call(M, f), Member);
        foreach (var p in new[] { "m_max_stack", "m_worker_stack_size", "StackDepth", "AssemblyPath" })
            Same(diffs, p, () => Get(typeof(Rts), p), () => Get(M, p), Member);
        foreach (var path in new[] { "a;b", "", "single", ";;", @"C:\x;D:\y;" })
        {
            Same(diffs, $"AssemblyPath = {Text(path)}", () => { Set(typeof(Rts), "AssemblyPath", path); return Get(typeof(Rts), "AssemblyPath"); },
                () => { Set(M, "AssemblyPath", path); return Get(M, "AssemblyPath"); });
        }
        Same(diffs, "AssemblyPath = null", () => { Set(typeof(Rts), "AssemblyPath", null); return "ok"; }, () => { Set(M, "AssemblyPath", null); return "ok"; });
        Set(typeof(Rts), "AssemblyPath", "");
        Set(M, "AssemblyPath", "");
        Same(diffs, "True.singleton", () => Get(typeof(True), "singleton")?.ToString(), () => Get(MTrue, "singleton")?.ToString());
        Same(diffs, "new True()", () => new True().ToString(), () => Activator.CreateInstance(MTrue)!.ToString());
        NoDiffs(diffs);
    }

    private static string Scenario(System.Type rts)
    {
        var log = new List<string>();
        var t = new Thread(() =>
        {
            log.Add("cancel unregistered " + Call(rts, "IsCancellationRequested"));
            log.Add("depth " + Get(rts, "m_stack_depth"));
            Set(rts, "m_stack_depth", 3);
            log.Add("set depth " + Get(rts, "m_stack_depth"));
            Call(rts, "ResetStackDepth");
            log.Add("reset depth " + Get(rts, "m_stack_depth"));
            var cts = new CancellationTokenSource();
            Call(rts, "RegisterThreadForCancellationChecking", cts);
            log.Add("registered " + Call(rts, "IsCancellationRequested"));
            cts.Cancel();
            log.Add("cancelled " + Call(rts, "IsCancellationRequested") + " then " + Call(rts, "IsCancellationRequested"));
            Call(rts, "RegisterThreadForCancellationChecking", cts);
            log.Add("re-registered cancelled " + Call(rts, "IsCancellationRequested"));
            Call(rts, "RegisterThreadForCancellationChecking", new object?[] { null });
            log.Add("null source " + Call(rts, "IsCancellationRequested"));
            var disposed = new CancellationTokenSource();
            disposed.Dispose();
            Call(rts, "RegisterThreadForCancellationChecking", disposed);
            log.Add("disposed " + Outcome(() => Call(rts, "IsCancellationRequested")));
        });
        t.Start();
        t.Join();
        return string.Join("\n", log);
    }

    [SkippableFact]
    public void Stack_depth_and_cancellation_latch()
    {
        Runtime.Sync();
        Assert.Equal(Scenario(M), Scenario(typeof(Rts)));
        // the budget a new thread starts with follows m_max_stack
        string NewThreadDepth(System.Type rts)
        {
            object old = Get(rts, "m_max_stack")!;
            Set(rts, "m_max_stack", 12);
            string result = "";
            var t = new Thread(() => result = Get(rts, "m_stack_depth")!.ToString()!);
            t.Start();
            t.Join();
            Set(rts, "m_max_stack", old);
            return result;
        }
        Assert.Equal(NewThreadDepth(M), NewThreadDepth(typeof(Rts)));
    }

    [SkippableFact]
    public void Resolves_assemblies_from_the_search_path()
    {
        Runtime.Sync();
        var diffs = new List<string>();
        string dir = Path.Combine(Path.GetTempPath(), "quire-rts-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(dir);
        try
        {
            File.Copy(typeof(Quire.Mirror.MirrorOfAttribute).Assembly.Location, Path.Combine(dir, "Probe.dll"));
            string Resolve(System.Type rts, object? sender, string name)
            {
                Set(rts, "AssemblyPath", Path.Combine(dir, "missing") + ";" + dir);
                try { return Outcome(() => Call(rts, "OnResolveAssembly", sender, new ResolveEventArgs(name)), a => ((Assembly?)a)?.GetName().Name ?? "null"); }
                finally { Set(rts, "AssemblyPath", ""); }
            }
            foreach (var name in new[] { "Probe", "Probe, Version=1.0.0.0, Culture=neutral", "Absent", "" })
            {
                Same(diffs, $"resolve {Text(name)}", Resolve(typeof(Rts), AppDomain.CurrentDomain, name), Resolve(M, AppDomain.CurrentDomain, name));
                Same(diffs, $"resolve {Text(name)} null sender", Resolve(typeof(Rts), null, name), Resolve(M, null, name));
            }
            Same(diffs, "non-domain sender", Resolve(typeof(Rts), "x", "Probe"), Resolve(M, "x", "Probe"));
        }
        finally
        {
            try { Directory.Delete(dir, true); } catch (IOException) { } catch (UnauthorizedAccessException) { }
        }
        NoDiffs(diffs);
    }
}
