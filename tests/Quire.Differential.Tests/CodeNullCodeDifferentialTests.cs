using Quire.Mpl.Run;
using static Quire.Differential.Tests.MplRunSides;

namespace Quire.Differential.Tests;

public class CodeNullCodeDifferentialTests
{
    [SkippableFact]
    public void NullCode_type_initializer_matches()
    {
        Runtime.Sync();
        System.Type quire = typeof(Code);
        System.Type original = M("Mpl.Run.Code");
        MethodComparison.Compare("Code.cctor", "Mpl.Run.Code",
            quire.TypeInitializer!, null, [], original.TypeInitializer!, null, [],
            _ => "NullCode=" + Describe(Code.NullCode),
            _ => "NullCode=" + Describe(Reflect.Get(original, "NullCode")));
    }
}
