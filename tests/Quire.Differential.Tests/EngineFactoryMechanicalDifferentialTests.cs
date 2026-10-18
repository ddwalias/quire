using System.Reflection;
using Xunit;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

public class EngineFactoryDifferentialTests
{
    private const string Contract = "Engine.Factory";
    private static System.Type QFactory => typeof(global::Quire.Engine.Factory);
    private static System.Type MFactory => Mathcad.Type("iengine", "Engine.Factory");
    private static System.Type QRts => typeof(global::Quire.Mpl.Run.Rts);
    private static System.Type MRts => Mathcad.Type("mplrun", "Mpl.Run.Rts");

    [SkippableFact]
    public void ctor_060000bd() => Mechanical.Compare(typeof(global::Quire.Engine.Factory), 0x060000bd);

    [SkippableFact]
    public void Factory_AppendPath_differential()
    {
        Runtime.Sync();
        var qInstance = Activator.CreateInstance(QFactory)!;
        var mInstance = Activator.CreateInstance(MFactory)!;

        var qAppendPath = QFactory.GetMethod("AppendPath", BindingFlags.Public | BindingFlags.Instance, [typeof(string)])!;
        var mAppendPath = MFactory.GetMethod("AppendPath", BindingFlags.Public | BindingFlags.Instance, [typeof(string)])!;

        var qSetAssemblyPath = QRts.GetMethod("set_AssemblyPath", BindingFlags.Public | BindingFlags.Static, [typeof(string)])!;
        var mSetAssemblyPath = MRts.GetMethod("set_AssemblyPath", BindingFlags.Public | BindingFlags.Static, [typeof(string)])!;
        var qGetAssemblyPath = QRts.GetMethod("get_AssemblyPath", BindingFlags.Public | BindingFlags.Static, [])!;
        var mGetAssemblyPath = MRts.GetMethod("get_AssemblyPath", BindingFlags.Public | BindingFlags.Static, [])!;

        // Reset AssemblyPath on both sides
        qSetAssemblyPath.Invoke(null, [""]);
        mSetAssemblyPath.Invoke(null, [""]);

        string[] testPaths = ["test1", "test2;test3", "", "path/with/slashes"];
        foreach (var p in testPaths)
        {
            MethodComparison.Compare($"AppendPath({Text(p)})", Contract,
                qAppendPath, qInstance, [p], mAppendPath, mInstance, [p],
                quireState: _ => (string)qGetAssemblyPath.Invoke(null, [])!,
                originalState: _ => (string)mGetAssemblyPath.Invoke(null, [])!);
        }

        // null argument
        MethodComparison.Compare("AppendPath(null)", Contract,
            qAppendPath, qInstance, [null], mAppendPath, mInstance, [null],
            quireState: _ => (string)qGetAssemblyPath.Invoke(null, [])!,
            originalState: _ => (string)mGetAssemblyPath.Invoke(null, [])!);
    }
}
