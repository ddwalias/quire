#nullable disable
using System.Collections;

namespace Quire.Engine;

public partial interface IFactoryFactory
{
    IFactory NewFactory();
}

/// <summary>Creates engines.</summary>
public partial interface IFactory : IDisposable
{
    int WorkerStackSize { get; set; }

    IEngine NewEngine();

    /// <summary>The engines this factory created that are still alive.</summary>
    ICollection GetEngines();
}

/// <summary>The entry point for creating calculation engines: <c>new Factory().NewFactory().NewEngine()</c>.</summary>
public sealed partial class Factory : IFactoryFactory
{
    private int m_worker_stack_size = 64 * 1024 * 1024;

    public Factory() { }

    public IFactory NewFactory()
    {
        var f = CreateFactory();
        f.WorkerStackSize = m_worker_stack_size;
        return f;
    }

    private static IFactory CreateFactory() => new Internal.EngineFactory();
}
