using System.Reflection;
using System.Reflection.Emit;
using Quire.Mpl.Run;

namespace Quire.Mpl.Tests.Run;

/// <summary>
/// Behaviour of <see cref="Rts"/> and <see cref="True"/> against docs/mirror/evidence/Mpl.Run.Rts.md;
/// RtsDifferentialTests compares with Mathcad.
/// </summary>
public class RtsTests
{
    private static T OnNewThread<T>(Func<T> f)
    {
        T result = default!;
        var t = new Thread(() => result = f());
        t.Start();
        t.Join();
        return result;
    }

    [Fact]
    public void Each_thread_starts_with_the_full_stack_budget()
    {
        Assert.Equal(250, Rts.m_max_stack);
        Assert.Equal(250, OnNewThread(() => Rts.m_stack_depth));
        Assert.Equal(249, OnNewThread(() => { Rts.m_stack_depth--; return Rts.m_stack_depth; }));
        Assert.Null(Rts.StackDepth);
    }

    [Fact]
    public void Cancellation_is_reported_once()
    {
        var seen = OnNewThread(() =>
        {
            var src = new CancellationTokenSource();
            Rts.RegisterThreadForCancellationChecking(src);
            bool before = Rts.IsCancellationRequested();
            src.Cancel();
            return (before, Rts.IsCancellationRequested(), Rts.IsCancellationRequested());
        });
        Assert.Equal((false, true, false), seen);
        Assert.False(OnNewThread(Rts.IsCancellationRequested));
    }

    [Fact]
    public void Handles_for_generated_code()
    {
        Assert.Equal([typeof(string)], Rts.ExceptionCtor().GetParameters().Select(p => p.ParameterType));
        Assert.Equal([typeof(Exception), typeof(uint)], Rts.RethrowCtor().GetParameters().Select(p => p.ParameterType));
        Assert.Equal([typeof(int)], Rts.TagCtor().GetParameters().Select(p => p.ParameterType));
        Assert.Same(typeof(object), Rts.FcnType());
        Assert.Same(True.singleton, Rts.BoolTrue().GetValue(null));
        Assert.Equal("True", True.singleton.ToString());
        Assert.Equal("m_tag", Rts.TagField().Name);
        Assert.Same(typeof(void), Rts.VoidType());
        Assert.Same(typeof(Tag), Rts.TagType());
    }

    [Fact]
    public void Unresolved_assemblies_are_found_on_the_search_path()
    {
        string dir = Path.Combine(Path.GetTempPath(), "QuireRts" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(dir);
        string name = "RtsProbe" + Guid.NewGuid().ToString("N");
        var builder = new PersistedAssemblyBuilder(new AssemblyName(name), typeof(object).Assembly);
        builder.DefineDynamicModule(name).DefineType("Probe", TypeAttributes.Public).CreateType();
        builder.Save(Path.Combine(dir, name + ".dll"));
        string saved = Rts.AssemblyPath;
        try
        {
            Rts.AssemblyPath = "missing;" + dir;
            Assert.Equal("missing;" + dir, Rts.AssemblyPath);
            Assert.Equal(name, Assembly.Load(name + ", Version=0.0.0.0").GetName().Name);
            Assert.Throws<FileNotFoundException>(() => Assembly.Load("RtsAbsent" + Guid.NewGuid().ToString("N")));
        }
        finally
        {
            Rts.AssemblyPath = saved;
        }
    }
}
