using System;
using System.Collections.Generic;
using Quire.Automation;
using Quire.Calculation.Core;
using Quire.Engine;
using Quire.IO;
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

    public EngineBuilder()
    {
        var factory = new Factory();
        factory.AppendPath(FileUtils.GetAssemblyFolder(factory.GetType().Assembly));
        _factory = factory.NewFactory();
        _factory.WorkerStackSize = 20000000;
        _factory.Initialize(string.Empty);
        Value.MathNamespaceUri = "http://schemas.mathsoft.com/math50";
        Value.UnitNamespaceUri = "http://schemas.mathsoft.com/units10";
    }

    private static string GenerateDimensionUnitRL(List<UnitDescriptor> unitList)
    {
        string text = string.Empty;
        if (unitList != null)
        {
            foreach (var unit in unitList)
            {
                text += string.Format(unitRL, unit.CanonicalName, unit.RL, unit.Quantity, "mc_" + unit.RL, "mc_" + unit.RL);
            }
        }
        return text;
    }

    private static string GenerateUnitSystemCustomRL(UnitSystems unitSystem, List<UnitDescriptor> baseList, List<UnitDescriptor> derivedList)
    {
        string text = GenerateDimensionUnitRL(baseList);
        string derivedUnitsText = unitSystemEmptyDerivedRL;
        if (derivedList?.Count > 0)
        {
            derivedUnitsText = string.Format(unitSystemDerivedRL, GenerateDimensionUnitRL(derivedList));
        }
        return string.Format(unitSystemRL, text, derivedUnitsText);
    }

    public static string GenerateUnitSystemRL(UnitSystems unitSystem, List<UnitDescriptor> baseList, List<UnitDescriptor> derivedList)
    {
        if (baseList == null)
        {
            return RL_NIL;
        }
        return GenerateUnitSystemCustomRL(unitSystem, baseList, derivedList);
    }

    public void Dispose()
    {
        _factory.Dispose();
    }
}
