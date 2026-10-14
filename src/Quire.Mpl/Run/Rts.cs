#nullable disable
using System.Reflection;
using Quire.Mirror;

namespace Quire.Mpl.Run;

/// <summary>
/// Runtime services for compiled Mpl code: the per-thread stack budget and cancellation flag, reflection handles
/// that the code generator emits calls to, and an assembly search path. Behaviour follows mplrun 12.0.1.0
/// (docs/mirror/evidence/Mpl.Run.Rts.md).
/// </summary>
public static partial class Rts
{
    [MirrorTested("Mpl.Run.Rts")]
    private static ConstructorInfo m_tag_ctor;
    [MirrorTested("Mpl.Run.Rts")]
    private static ConstructorInfo m_exception_ctor;
    [MirrorTested("Mpl.Run.Rts")]
    private static ConstructorInfo m_rethrow_ctor;
    [MirrorTested("Mpl.Run.Rts")]
    private static System.Type m_fcn_type;
    [MirrorVerified("RtsDifferentialTests")]
    public static int m_max_stack;
    [MirrorVerified("RtsDifferentialTests")]
    public static int m_worker_stack_size;

    [ThreadStatic]
    [MirrorTested("Mpl.Run.Rts")]
    private static int m_stack_depth__;

    [ThreadStatic]
    [MirrorTested("Mpl.Run.Rts")]
    private static int m_stack_depth_init__;

    [MirrorTested("Mpl.Run.Rts")]
    private static FieldInfo m_stack_depth_field_info;
    [MirrorTested("Mpl.Run.Rts")]
    private static string[] s_paths;

    [ThreadStatic]
    [MirrorTested("Mpl.Run.Rts")]
    private static CancellationTokenSource ms_thread_cancellation_token;

    [MirrorTested("Mpl.Run.Rts")]
    private static readonly CancellationTokenSource ms_dummy_cancellation_token;

    /// <summary>A stack budget of 250, no search paths, and assembly resolution through <see cref="AssemblyPath"/>.</summary>
    [MirrorTested("Mpl.Run.Rts")]
    static Rts()
    {
        m_max_stack = 250;
        s_paths = new string[0];
        ms_dummy_cancellation_token = new CancellationTokenSource();
        m_tag_ctor = typeof(Tag).GetConstructor(new System.Type[1] { typeof(int) });
        m_exception_ctor = typeof(AppException).GetConstructor(new System.Type[1] { typeof(string) });
        m_rethrow_ctor = typeof(AppException).GetConstructor(new System.Type[2] { typeof(Exception), typeof(uint) });
        m_fcn_type = typeof(object);
        _ = AppDomain.CurrentDomain;
        AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(OnResolveAssembly);
    }

    /// <summary>The calling thread's remaining stack budget, starting at <see cref="m_max_stack"/>.</summary>
    [MirrorVerified("RtsDifferentialTests")]
    public static int m_stack_depth
    {
        get
        {
            if (m_stack_depth_init__ == 0)
            {
                m_stack_depth_init__ = 1;
                m_stack_depth__ = m_max_stack;
            }
            return m_stack_depth__;
        }
        set => m_stack_depth__ = value;
    }

    /// <summary>Looks up a field named <c>m_stack_depth</c>, which is a property, so this is always null.</summary>
    [MirrorVerified("RtsDifferentialTests")]
    public static FieldInfo StackDepth
    {
        get
        {
            if (m_stack_depth_field_info == null)
            {
                m_stack_depth_field_info = typeof(Rts).GetField("m_stack_depth");
            }
            return m_stack_depth_field_info;
        }
    }

    /// <summary>Directories searched for unresolved assemblies, separated by semicolons.</summary>
    [MirrorVerified("RtsDifferentialTests")]
    public static string AssemblyPath
    {
        get => string.Join(";", s_paths);
        set => s_paths = value.Split(';');
    }

    /// <summary>Makes <see cref="IsCancellationRequested"/> on the calling thread follow <paramref name="src"/>.</summary>
    [MirrorVerified("RtsDifferentialTests")]
    public static void RegisterThreadForCancellationChecking(CancellationTokenSource src) => ms_thread_cancellation_token = src;

    /// <summary>
    /// True once when the calling thread's registered source is cancelled: the thread then switches to a source
    /// that never is, so later calls return false until another source is registered.
    /// </summary>
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    [MirrorVerified("RtsDifferentialTests")]
    public static bool IsCancellationRequested()
    {
        if (ms_thread_cancellation_token != null)
        {
            if (ms_thread_cancellation_token == ms_dummy_cancellation_token)
            {
                return false;
            }
            if (ms_thread_cancellation_token.Token.IsCancellationRequested)
            {
                ms_thread_cancellation_token = ms_dummy_cancellation_token;
                return true;
            }
        }
        return false;
    }

    [MirrorVerified("RtsDifferentialTests")]
    public static void ResetStackDepth() => m_stack_depth = m_max_stack;

    /// <summary><c>AppException(string)</c>.</summary>
    [MirrorVerified("RtsDifferentialTests")]
    public static ConstructorInfo ExceptionCtor() => m_exception_ctor;

    /// <summary><c>AppException(Exception, uint)</c>.</summary>
    [MirrorVerified("RtsDifferentialTests")]
    public static ConstructorInfo RethrowCtor() => m_rethrow_ctor;

    /// <summary><c>Tag(int)</c>.</summary>
    [MirrorVerified("RtsDifferentialTests")]
    public static ConstructorInfo TagCtor() => m_tag_ctor;

    /// <summary>The static type of compiled function values: <see cref="object"/>.</summary>
    [MirrorVerified("RtsDifferentialTests")]
    public static System.Type FcnType() => m_fcn_type;

    /// <summary><c>True.singleton</c>.</summary>
    [MirrorVerified("RtsDifferentialTests")]
    public static FieldInfo BoolTrue() => typeof(True).GetField("singleton");

    /// <summary><c>Tag.m_tag</c>.</summary>
    [MirrorVerified("RtsDifferentialTests")]
    public static FieldInfo TagField() => typeof(Tag).GetField("m_tag");

    [MirrorVerified("RtsDifferentialTests")]
    public static System.Type VoidType() => typeof(void);

    [MirrorVerified("RtsDifferentialTests")]
    public static System.Type TagType() => typeof(Tag);

    /// <summary>For the current domain, the first <c>name.dll</c> found in a search directory, loaded by path.</summary>
    [MirrorVerified("RtsDifferentialTests")]
    private static Assembly OnResolveAssembly(object s, ResolveEventArgs a)
    {
        if ((AppDomain)s != AppDomain.CurrentDomain) return null;
        foreach (string dir in s_paths)
        {
            string path = Path.Combine(dir, a.Name.Split(',')[0]) + ".dll";
            if (File.Exists(path)) return Assembly.LoadFile(path);
        }
        return null;
    }
}
