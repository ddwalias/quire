using System.Reflection;
using Quire.Mpl.Run;
using Xunit;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

/// <summary>
/// Compares mplrun's <c>Mpl.Run.Rts</c> and <c>Mpl.Run.True</c> with Quire's: the stack budget, cancellation latch,
/// reflection handles and assembly search path; cases are listed in docs/mirror/evidence/Mpl.Run.Rts.md.
/// </summary>
public class RtsDifferentialTests
{
    private const string Contract = "Mpl.Run.Rts";
    private const BindingFlags DeclaredStaticPublic = BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly;
    private const BindingFlags DeclaredStaticNonPublic = BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly;

    private static System.Type QRts => typeof(Rts);
    private static System.Type MRts => Mathcad.Type("mplrun", "Mpl.Run.Rts");
    private static System.Type MTrue => Mathcad.Type("mplrun", "Mpl.Run.True");

    private static (MethodInfo q, MethodInfo m) Method(string name, BindingFlags flags, params System.Type[] paramTypes)
    {
        var q = QRts.GetMethod(name, flags, paramTypes)
            ?? throw new HarnessException($"Quire method not found: Rts.{name}");
        var m = MRts.GetMethod(name, flags, paramTypes)
            ?? throw new HarnessException($"Mathcad method not found: Rts.{name}");
        return (q, m);
    }

    private static string Strip(string? name) => name?.StartsWith("Quire.") == true ? name[6..] : (name ?? "null");

    private static string Member(object? o) => o switch
    {
        null => "null",
        ConstructorInfo c => $"{Strip(c.DeclaringType!.FullName)}({string.Join(", ", c.GetParameters().Select(p => Strip(p.ParameterType.FullName ?? p.ParameterType.Name)))})",
        FieldInfo f => $"{Strip(f.DeclaringType!.FullName)}.{f.Name}: {Strip(f.FieldType.FullName ?? f.FieldType.Name)} static={f.IsStatic}",
        System.Type t => Strip(t.FullName ?? t.Name),
        Assembly a => a.GetName().Name ?? "null",
        ResolveEventArgs r => $"ResolveEventArgs({Text(r.Name)})",
        AppDomain => "AppDomain",
        CancellationTokenSource cts => $"CancellationTokenSource(cancelled={cts.IsCancellationRequested})",
        _ => Text(o),
    };

    [SkippableFact]
    public void Handles_and_settings()
    {
        Runtime.Sync();
        var qs = Member;
        var ms = Member;

        // The first call into Rts runs its .cctor; a .cctor that throws fails every call with the same exception.
        var (qFcnType, mFcnType) = Method("FcnType", DeclaredStaticPublic);
        MethodComparison.Compare("FcnType() first call", Contract,
            qFcnType, null, [], mFcnType, null, [], qs, ms);

        // Static constructor (.cctor)
        var qCctor = QRts.TypeInitializer ?? QRts.GetConstructors(BindingFlags.Static | BindingFlags.NonPublic)[0];
        var mCctor = MRts.TypeInitializer ?? MRts.GetConstructors(BindingFlags.Static | BindingFlags.NonPublic)[0];
        MethodComparison.Compare(".cctor() initialization", Contract,
            qCctor, null, [], mCctor, null, [], qs, ms);

        // What the .cctor set: the stack budget a reset restores, and an empty search path that resolves nothing.
        var (qResetStackDepth, mResetStackDepth) = Method("ResetStackDepth", DeclaredStaticPublic);
        var (qGetStackDepthProp, mGetStackDepthProp) = Method("get_m_stack_depth", DeclaredStaticPublic);
        MethodComparison.Compare("ResetStackDepth() after .cctor", Contract,
            qResetStackDepth, null, [], mResetStackDepth, null, [], qs, ms);
        MethodComparison.Compare("get_m_stack_depth() after .cctor", Contract,
            qGetStackDepthProp, null, [], mGetStackDepthProp, null, [], qs, ms);
        var (qOnResolve, mOnResolve) = Method("OnResolveAssembly", DeclaredStaticNonPublic, typeof(object), typeof(ResolveEventArgs));
        var qAbsent = new object?[] { AppDomain.CurrentDomain, new ResolveEventArgs("Absent") };
        var mAbsent = new object?[] { AppDomain.CurrentDomain, new ResolveEventArgs("Absent") };
        MethodComparison.Compare("OnResolveAssembly('Absent') after .cctor", Contract,
            qOnResolve, null, qAbsent, mOnResolve, null, mAbsent, qs, ms);

        // Reflection handles
        var (qExceptionCtor, mExceptionCtor) = Method("ExceptionCtor", DeclaredStaticPublic);
        MethodComparison.Compare("ExceptionCtor()", Contract,
            qExceptionCtor, null, [], mExceptionCtor, null, [], qs, ms);

        var (qRethrowCtor, mRethrowCtor) = Method("RethrowCtor", DeclaredStaticPublic);
        MethodComparison.Compare("RethrowCtor()", Contract,
            qRethrowCtor, null, [], mRethrowCtor, null, [], qs, ms);

        var (qTagCtor, mTagCtor) = Method("TagCtor", DeclaredStaticPublic);
        MethodComparison.Compare("TagCtor()", Contract,
            qTagCtor, null, [], mTagCtor, null, [], qs, ms);

        MethodComparison.Compare("FcnType()", Contract,
            qFcnType, null, [], mFcnType, null, [], qs, ms);

        var (qBoolTrue, mBoolTrue) = Method("BoolTrue", DeclaredStaticPublic);
        MethodComparison.Compare("BoolTrue()", Contract,
            qBoolTrue, null, [], mBoolTrue, null, [], qs, ms);

        var (qTagField, mTagField) = Method("TagField", DeclaredStaticPublic);
        MethodComparison.Compare("TagField()", Contract,
            qTagField, null, [], mTagField, null, [], qs, ms);

        var (qVoidType, mVoidType) = Method("VoidType", DeclaredStaticPublic);
        MethodComparison.Compare("VoidType()", Contract,
            qVoidType, null, [], mVoidType, null, [], qs, ms);

        var (qTagType, mTagType) = Method("TagType", DeclaredStaticPublic);
        MethodComparison.Compare("TagType()", Contract,
            qTagType, null, [], mTagType, null, [], qs, ms);

        // StackDepth property getter (initial and cached)
        var (qGetStackDepth, mGetStackDepth) = Method("get_StackDepth", DeclaredStaticPublic);
        MethodComparison.Compare("StackDepth initial lookup", Contract,
            qGetStackDepth, null, [], mGetStackDepth, null, [], qs, ms);
        MethodComparison.Compare("StackDepth cached lookup", Contract,
            qGetStackDepth, null, [], mGetStackDepth, null, [], qs, ms);

        // AssemblyPath property getter and setter
        var (qGetAssemblyPath, mGetAssemblyPath) = Method("get_AssemblyPath", DeclaredStaticPublic);
        var (qSetAssemblyPath, mSetAssemblyPath) = Method("set_AssemblyPath", DeclaredStaticPublic, typeof(string));

        foreach (var path in new[] { "a;b", "", "single", ";;", @"C:\x;D:\y;" })
        {
            MethodComparison.Compare($"set_AssemblyPath({Text(path)})", Contract,
                qSetAssemblyPath, null, [path], mSetAssemblyPath, null, [path], qs, ms);
            MethodComparison.Compare($"get_AssemblyPath() after {Text(path)}", Contract,
                qGetAssemblyPath, null, [], mGetAssemblyPath, null, [], qs, ms);
        }

        MethodComparison.Compare("set_AssemblyPath(null) throws", Contract,
            qSetAssemblyPath, null, [null], mSetAssemblyPath, null, [null], qs, ms);
        MethodComparison.Compare("set_AssemblyPath('') reset", Contract,
            qSetAssemblyPath, null, [""], mSetAssemblyPath, null, [""], qs, ms);

        // Non-comparison assertions for fields
        Assert.Equal(Get(MRts, "m_max_stack"), Get(QRts, "m_max_stack"));
        Assert.Equal(Get(MRts, "m_worker_stack_size"), Get(QRts, "m_worker_stack_size"));
        Assert.Equal(Get(MTrue, "singleton")?.ToString(), Get(typeof(True), "singleton")?.ToString());

        // True constructor and ToString
        var qTrueCtor = typeof(True).GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, [], null)!;
        var mTrueCtor = MTrue.GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, [], null)!;
        MethodComparison.Compare("True() constructor", Contract,
            qTrueCtor, null, [], mTrueCtor, null, [], qs, ms);

        var qTrueToString = typeof(True).GetMethod("ToString", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly, [])!;
        var mTrueToString = MTrue.GetMethod("ToString", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly, [])!;
        var qTrueInstance = new True();
        var mTrueInstance = Activator.CreateInstance(MTrue)!;
        MethodComparison.Compare("True.ToString()", Contract,
            qTrueToString, qTrueInstance, [], mTrueToString, mTrueInstance, [], qs, ms, qs, ms);
    }

    [SkippableFact]
    public void Stack_depth_and_cancellation_latch()
    {
        Runtime.Sync();
        var qs = Member;
        var ms = Member;

        var (qGetStackDepthProp, mGetStackDepthProp) = Method("get_m_stack_depth", DeclaredStaticPublic);
        var (qSetStackDepthProp, mSetStackDepthProp) = Method("set_m_stack_depth", DeclaredStaticPublic, typeof(int));
        var (qResetStackDepth, mResetStackDepth) = Method("ResetStackDepth", DeclaredStaticPublic);
        var (qRegisterThread, mRegisterThread) = Method("RegisterThreadForCancellationChecking", DeclaredStaticPublic, typeof(CancellationTokenSource));
        var (qIsCancellationRequested, mIsCancellationRequested) = Method("IsCancellationRequested", DeclaredStaticPublic);

        // 1. Unregistered cancellation: branch ms_thread_cancellation_token == null
        MethodComparison.Compare("IsCancellationRequested() unregistered", Contract,
            qIsCancellationRequested, null, [], mIsCancellationRequested, null, [], qs, ms);

        // 2. Initial stack depth read: branch m_stack_depth_init__ == 0 -> initializes to m_max_stack
        MethodComparison.Compare("get_m_stack_depth() uninitialized", Contract,
            qGetStackDepthProp, null, [], mGetStackDepthProp, null, [], qs, ms);

        // 3. Set stack depth
        MethodComparison.Compare("set_m_stack_depth(3)", Contract,
            qSetStackDepthProp, null, [3], mSetStackDepthProp, null, [3], qs, ms);
        MethodComparison.Compare("get_m_stack_depth() after set to 3", Contract,
            qGetStackDepthProp, null, [], mGetStackDepthProp, null, [], qs, ms);

        // 4. ResetStackDepth(): resets to m_max_stack
        MethodComparison.Compare("ResetStackDepth()", Contract,
            qResetStackDepth, null, [], mResetStackDepth, null, [], qs, ms);
        MethodComparison.Compare("get_m_stack_depth() after reset", Contract,
            qGetStackDepthProp, null, [], mGetStackDepthProp, null, [], qs, ms);

        // 4b. A thread's first read (init flag clear) starts from m_max_stack, not the budget set before it
        MethodComparison.Compare("set_m_stack_depth(3) before the first read", Contract,
            qSetStackDepthProp, null, [3], mSetStackDepthProp, null, [3], qs, ms);
        Set(QRts, "m_stack_depth_init__", 0);
        Set(MRts, "m_stack_depth_init__", 0);
        MethodComparison.Compare("get_m_stack_depth() first read on the thread", Contract,
            qGetStackDepthProp, null, [], mGetStackDepthProp, null, [], qs, ms);

        // 5. Register active source: branch Token.IsCancellationRequested == false
        var cts = new CancellationTokenSource();
        MethodComparison.Compare("RegisterThreadForCancellationChecking(cts live)", Contract,
            qRegisterThread, null, [cts], mRegisterThread, null, [cts], qs, ms);
        MethodComparison.Compare("IsCancellationRequested() live uncancelled", Contract,
            qIsCancellationRequested, null, [], mIsCancellationRequested, null, [], qs, ms);

        // 6. Cancel source: branch Token.IsCancellationRequested == true -> latches to ms_dummy_cancellation_token and returns true
        cts.Cancel();
        MethodComparison.Compare("IsCancellationRequested() once cancelled", Contract,
            qIsCancellationRequested, null, [], mIsCancellationRequested, null, [], qs, ms);

        // 7. Latch check: branch ms_thread_cancellation_token == ms_dummy_cancellation_token -> returns false
        MethodComparison.Compare("IsCancellationRequested() second call after latch", Contract,
            qIsCancellationRequested, null, [], mIsCancellationRequested, null, [], qs, ms);

        // 8. Re-register cancelled source: returns true again
        MethodComparison.Compare("RegisterThreadForCancellationChecking(cts re-registered)", Contract,
            qRegisterThread, null, [cts], mRegisterThread, null, [cts], qs, ms);
        MethodComparison.Compare("IsCancellationRequested() re-registered cancelled", Contract,
            qIsCancellationRequested, null, [], mIsCancellationRequested, null, [], qs, ms);

        // 9. Register null source: branch null
        MethodComparison.Compare("RegisterThreadForCancellationChecking(null)", Contract,
            qRegisterThread, null, [null], mRegisterThread, null, [null], qs, ms);
        MethodComparison.Compare("IsCancellationRequested() after null register", Contract,
            qIsCancellationRequested, null, [], mIsCancellationRequested, null, [], qs, ms);

        // 10. Disposed source: accessing Token throws ObjectDisposedException
        var disposed = new CancellationTokenSource();
        disposed.Dispose();
        MethodComparison.Compare("RegisterThreadForCancellationChecking(disposed)", Contract,
            qRegisterThread, null, [disposed], mRegisterThread, null, [disposed], qs, ms);
        MethodComparison.Compare("IsCancellationRequested() disposed source throws", Contract,
            qIsCancellationRequested, null, [], mIsCancellationRequested, null, [], qs, ms);

        // Cleanup cancellation token
        qRegisterThread.Invoke(null, [null]);
        mRegisterThread.Invoke(null, [null]);

        // Non-comparison assertion: the budget a new thread starts with follows m_max_stack
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
        Assert.Equal(NewThreadDepth(MRts), NewThreadDepth(QRts));
    }

    [SkippableFact]
    public void Resolves_assemblies_from_the_search_path()
    {
        Runtime.Sync();
        var qs = Member;
        var ms = Member;

        var (qSetAssemblyPath, mSetAssemblyPath) = Method("set_AssemblyPath", DeclaredStaticPublic, typeof(string));
        var (qOnResolve, mOnResolve) = Method("OnResolveAssembly", DeclaredStaticNonPublic, typeof(object), typeof(ResolveEventArgs));

        string dir = Path.Combine(Path.GetTempPath(), "quire-rts-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(dir);
        try
        {
            File.Copy(typeof(Quire.Mirror.MirrorOfAttribute).Assembly.Location, Path.Combine(dir, "Probe.dll"));
            string searchPath = Path.Combine(dir, "missing") + ";" + dir;
            MethodComparison.Compare("set_AssemblyPath(missing;dir)", Contract,
                qSetAssemblyPath, null, [searchPath], mSetAssemblyPath, null, [searchPath], qs, ms);
            try
            {
                foreach (var name in new[] { "Probe", "Probe, Version=1.0.0.0, Culture=neutral", "Absent", "" })
                {
                    var qArgs = new object?[] { AppDomain.CurrentDomain, new ResolveEventArgs(name) };
                    var mArgs = new object?[] { AppDomain.CurrentDomain, new ResolveEventArgs(name) };
                    MethodComparison.Compare($"OnResolveAssembly('{name}') current domain", Contract,
                        qOnResolve, null, qArgs, mOnResolve, null, mArgs, qs, ms);

                    var qArgsNull = new object?[] { null, new ResolveEventArgs(name) };
                    var mArgsNull = new object?[] { null, new ResolveEventArgs(name) };
                    MethodComparison.Compare($"OnResolveAssembly('{name}') null sender", Contract,
                        qOnResolve, null, qArgsNull, mOnResolve, null, mArgsNull, qs, ms);
                }

                // The probe in the first (only) search directory.
                qSetAssemblyPath.Invoke(null, [dir]);
                mSetAssemblyPath.Invoke(null, [dir]);
                var qFirst = new object?[] { AppDomain.CurrentDomain, new ResolveEventArgs("Probe") };
                var mFirst = new object?[] { AppDomain.CurrentDomain, new ResolveEventArgs("Probe") };
                MethodComparison.Compare("OnResolveAssembly('Probe') first directory", Contract,
                    qOnResolve, null, qFirst, mOnResolve, null, mFirst, qs, ms);

                var qArgsNonDomain = new object?[] { "x", new ResolveEventArgs("Probe") };
                var mArgsNonDomain = new object?[] { "x", new ResolveEventArgs("Probe") };
                MethodComparison.Compare("OnResolveAssembly non-domain sender throws", Contract,
                    qOnResolve, null, qArgsNonDomain, mOnResolve, null, mArgsNonDomain, qs, ms);
            }
            finally
            {
                qSetAssemblyPath.Invoke(null, [""]);
                mSetAssemblyPath.Invoke(null, [""]);
            }
        }
        finally
        {
            try { Directory.Delete(dir, true); } catch (IOException) { } catch (UnauthorizedAccessException) { }
        }
    }
}
