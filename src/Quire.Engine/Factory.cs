#nullable disable
using System.Collections;

namespace Quire.Engine;

public partial interface IFactoryFactory
{
    void AppendPath(string path);
    IFactory NewFactory();
}

/// <summary>Creates engines.</summary>
public partial interface IFactory : IDisposable
{
    void Initialize(string appData);
    bool VerifyIntegrity();

    int WorkerStackSize { get; set; }

    IEngine NewEngine();

    /// <summary>The engines this factory created that are still alive.</summary>
    ICollection GetEngines();
}

/// <summary>The entry point for creating calculation engines: <c>new Factory().NewFactory().NewEngine()</c>.</summary>
public sealed partial class Factory : IFactoryFactory
{
#pragma warning disable CS0649
    private int m_worker_stack_size;
#pragma warning restore CS0649

    public Factory() { }

    public void AppendPath(string path)
    {
        if (path != null)
        {
            string assemblyPath = Quire.Mpl.Run.Rts.AssemblyPath;
            if (assemblyPath.Length != 0)
            {
                path = string.Concat(assemblyPath + ";", path);
            }
            Quire.Mpl.Run.Rts.AssemblyPath = path;
        }
    }

    public IFactory NewFactory()
    {
        var f = CreateFactory();
        if (m_worker_stack_size != 0)
        {
            f.WorkerStackSize = m_worker_stack_size;
        }
        return f;
    }

    private static IFactory CreateFactory() => new Internal.EngineFactory();
}
