using Quire.Mirror;

namespace Quire.Calculation;

public partial class EngineBuilder
{
    [MirrorVerified("PtcCalculationEngineBuilderCctorMechanicalDifferentialTests")]
    static EngineBuilder()
    {
        ImportRequestFormat = "(import ( \"McdTranslator\" \"\" \"false\" \"false\")         ( \"McdDynamicEfi\" \"MC_mc\" \"false\" \"false\"  )         ( \"McdUnitSystem{0}\" \"MC_unit\" \"false\" \"false\" ))";
        unitRL = "(unit (tuple (unboxed (string \"{0}\"))(unboxed (string \"{1}\"))(unboxed (string \"{2}\")) {3} (typeof {4})))";
        unitSystemRL = "(eval-top-top(numeric (set_unit_system_details (qualified MC_unit system) (list {0}) {1})))";
        unitSystemDerivedRL = "(list {0})";
        unitSystemEmptyDerivedRL = "(list)";
        RL_NIL = "nil";
    }
}
