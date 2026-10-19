using System.Reflection;
using Quire.PersistentData;

namespace Quire.Differential.Tests;

public class PtcPersistentDataAreaTypeCctorDifferentialTests
{
    private const string Contract = "Ptc.PersistentData.AreaType";

    [SkippableFact]
    public void TypeOfThis_initializer_matches_PtcCore()
    {
        Mathcad.Require();
        System.Type q = typeof(AreaType);
        System.Type m = Mathcad.Type("Ptc.Core", "Ptc.PersistentData.AreaType");
        MethodComparison.Compare(".cctor TypeOfThis", Contract,
            q.TypeInitializer!, null, [], m.TypeInitializer!, null, [], _ => "void", _ => "void");
        Assert.Equal("AreaType", ((System.Type)Reflect.Get(q, "TypeOfThis")!).Name);
        Assert.Equal(((System.Type)Reflect.Get(m, "TypeOfThis")!).Name,
            ((System.Type)Reflect.Get(q, "TypeOfThis")!).Name);
    }
}
