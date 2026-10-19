using System.Collections;
using System.Reflection;
using Quire.PersistentData;
using Quire.PersistentDataObjects;

namespace Quire.Differential.Tests;

public class PtcPersistentDataAreaTypeCctorMechanicalDifferentialTests
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

    [SkippableFact]
    public void Region_and_null_protection_accessors_match_PtcCore()
    {
        Mathcad.Require();
        System.Type qType = typeof(AreaType);
        System.Type mType = Mathcad.Type("Ptc.Core", "Ptc.PersistentData.AreaType");

        MethodInfo qGetRegions = qType.GetMethod("get_Regions", BindingFlags.Public | BindingFlags.Instance)!;
        MethodInfo mGetRegions = mType.GetMethod("get_Regions", BindingFlags.Public | BindingFlags.Instance)!;
        MethodInfo qSetRegions = qType.GetMethod("set_Regions", BindingFlags.Public | BindingFlags.Instance)!;
        MethodInfo mSetRegions = mType.GetMethod("set_Regions", BindingFlags.Public | BindingFlags.Instance)!;

        object qNullRegions = Reflect.New(qType);
        object mNullRegions = Reflect.New(mType);
        MethodComparison.Compare("get_Regions(null backing array)", Contract,
            qGetRegions, qNullRegions, [], mGetRegions, mNullRegions, [], ShowRegions, ShowRegions, State, State);

        object qSetNull = Reflect.New(qType);
        object mSetNull = Reflect.New(mType);
        MethodComparison.Compare("set_Regions(null)", Contract,
            qSetRegions, qSetNull, [null], mSetRegions, mSetNull, [null], ShowRegions, ShowRegions, State, State);

        object qSetRegionsTarget = Reflect.New(qType);
        object mSetRegionsTarget = Reflect.New(mType);
        Array qRegions = MakeRegions(typeof(worksheetRegionType), "one", "two");
        Array mRegions = MakeRegions(Mathcad.Type("Ptc.Core", "Ptc.PersistentDataObjects.worksheetRegionType"), "one", "two");
        MethodComparison.Compare("set_Regions(worksheetRegionType array)", Contract,
            qSetRegions, qSetRegionsTarget, [qRegions], mSetRegions, mSetRegionsTarget, [mRegions],
            ShowRegions, ShowRegions, State, State);
        MethodComparison.Compare("get_Regions(after set)", Contract,
            qGetRegions, qSetRegionsTarget, [], mGetRegions, mSetRegionsTarget, [],
            ShowRegions, ShowRegions, State, State);

        MethodInfo qGetProtect = qType.GetMethod("get_Protect", BindingFlags.Public | BindingFlags.Instance)!;
        MethodInfo mGetProtect = mType.GetMethod("get_Protect", BindingFlags.Public | BindingFlags.Instance)!;
        MethodInfo qGetData = qType.GetMethod("get_ProtectionDataWrapper", BindingFlags.Public | BindingFlags.Instance)!;
        MethodInfo mGetData = mType.GetMethod("get_ProtectionDataWrapper", BindingFlags.Public | BindingFlags.Instance)!;
        MethodInfo qSetData = qType.GetMethod("set_ProtectionDataWrapper", BindingFlags.Public | BindingFlags.Instance)!;
        MethodInfo mSetData = mType.GetMethod("set_ProtectionDataWrapper", BindingFlags.Public | BindingFlags.Instance)!;

        object qProtection = Reflect.New(qType);
        object mProtection = Reflect.New(mType);
        MethodComparison.Compare("get_Protect(no ProtectionData)", Contract,
            qGetProtect, qProtection, [], mGetProtect, mProtection, [], Reflect.Text, Reflect.Text, State, State);
        MethodComparison.Compare("get_ProtectionDataWrapper(null persistent data)", Contract,
            qGetData, qProtection, [], mGetData, mProtection, [], Reflect.Text, Reflect.Text, State, State);
        MethodComparison.Compare("set_ProtectionDataWrapper(null)", Contract,
            qSetData, qProtection, [null], mSetData, mProtection, [null], Reflect.Text, Reflect.Text, State, State);
    }

    private static Array MakeRegions(System.Type regionType, params string[] values)
    {
        Array regions = Array.CreateInstance(regionType, values.Length);
        for (int i = 0; i < values.Length; i++)
        {
            object region = Reflect.New(regionType);
            Reflect.Set(region, "Item", values[i]);
            Reflect.Set(region, "regionid", values[i] + "-id");
            regions.SetValue(region, i);
        }
        return regions;
    }

    private static string ShowRegions(object? value)
    {
        if (value is not IEnumerable regions || value is string)
            return Reflect.Text(value);
        return "[" + string.Join(", ", regions.Cast<object?>().Select(region =>
            region == null ? "null" : $"{Reflect.Text(Reflect.Get(region, "regionid"))}|{Reflect.Text(Reflect.Get(region, "Item"))}")) + "]";
    }

    private static string State(object target) =>
        $"regions={ShowRegions(Reflect.Get(target, "regions"))}; data={Reflect.Text(Reflect.Get(target, "ProtectionData"))}";
}
