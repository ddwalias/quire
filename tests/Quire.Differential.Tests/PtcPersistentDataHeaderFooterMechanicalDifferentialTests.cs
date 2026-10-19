using System.Collections;
using System.Reflection;
using Quire.PersistentData;
using Quire.PersistentDataObjects;

namespace Quire.Differential.Tests;

public class PtcPersistentDataHeaderFooterMechanicalDifferentialTests
{
    private const BindingFlags AllInstance = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

    [SkippableFact]
    public void Footer_Regions_and_conversion_flag_match_PtcCore()
    {
        Mathcad.Require();
        CompareRegions(typeof(footer), Mathcad.Type("Ptc.Core", "Ptc.PersistentData.footer"), "Ptc.PersistentData.footer");
        CompareConversionFlag(typeof(footer), Mathcad.Type("Ptc.Core", "Ptc.PersistentData.footer"),
            "get_FooterConvertedFromMC14", "Ptc.PersistentData.footer");
    }

    [SkippableFact]
    public void Header_Regions_and_conversion_flag_match_PtcCore()
    {
        Mathcad.Require();
        CompareRegions(typeof(header), Mathcad.Type("Ptc.Core", "Ptc.PersistentData.header"), "Ptc.PersistentData.header");
        CompareConversionFlag(typeof(header), Mathcad.Type("Ptc.Core", "Ptc.PersistentData.header"),
            "get_HeaderConvertedFromMC14", "Ptc.PersistentData.header");
    }

    private static void CompareRegions(System.Type qType, System.Type mType, string contract)
    {
        MethodInfo qGet = qType.GetMethod("get_Regions", AllInstance)!;
        MethodInfo mGet = mType.GetMethod("get_Regions", AllInstance)!;
        MethodInfo qSet = qType.GetMethod("set_Regions", AllInstance)!;
        MethodInfo mSet = mType.GetMethod("set_Regions", AllInstance)!;

        object qEmpty = Reflect.New(qType);
        object mEmpty = Reflect.New(mType);
        MethodComparison.Compare("get_Regions(null backing array)", contract,
            qGet, qEmpty, [], mGet, mEmpty, [], ShowRegions, ShowRegions, RegionCollectionState, RegionCollectionState);

        object qNullSet = Reflect.New(qType);
        object mNullSet = Reflect.New(mType);
        MethodComparison.Compare("set_Regions(null)", contract,
            qSet, qNullSet, [null], mSet, mNullSet, [null], ShowRegions, ShowRegions, RegionCollectionState, RegionCollectionState);

        object qValidSet = Reflect.New(qType);
        object mValidSet = Reflect.New(mType);
        Array qRegions = MakeRegions(typeof(headerFooterRegionType), "one", "two");
        Array mRegions = MakeRegions(Mathcad.Type("Ptc.Core", "Ptc.PersistentData.headerFooterRegionType"), "one", "two");
        MethodComparison.Compare("set_Regions(headerFooterRegionType array)", contract,
            qSet, qValidSet, [qRegions], mSet, mValidSet, [mRegions], ShowRegions, ShowRegions, RegionCollectionState, RegionCollectionState);
        MethodComparison.Compare("get_Regions(after set)", contract,
            qGet, qValidSet, [], mGet, mValidSet, [], ShowRegions, ShowRegions, RegionCollectionState, RegionCollectionState);

        object qBadSet = Reflect.New(qType);
        object mBadSet = Reflect.New(mType);
        Array qWrongType = MakeRegions(typeof(worksheetRegionType), "wrong");
        Array mWrongType = MakeRegions(Mathcad.Type("Ptc.Core", "Ptc.PersistentDataObjects.worksheetRegionType"), "wrong");
        Type qException = DirectExceptionType(qSet, qBadSet, qWrongType)!;
        Type mException = DirectExceptionType(mSet, mBadSet, mWrongType)!;
        Assert.Equal(typeof(InvalidCastException), qException);
        Assert.Equal(mException, qException);
        Assert.Equal(RegionCollectionState(qBadSet), RegionCollectionState(mBadSet));
    }

    private static void CompareConversionFlag(System.Type qType, System.Type mType, string accessor, string contract)
    {
        MethodInfo qGet = qType.GetMethod(accessor, AllInstance)!;
        MethodInfo mGet = mType.GetMethod(accessor, AllInstance)!;
        System.Type qVersion = typeof(applicationVersionType);
        System.Type mVersion = Mathcad.Type("Ptc.Core", "Ptc.PersistentDataObjects.applicationVersionType");
        foreach (bool specified in new[] { false, true })
        {
            object qTarget = Reflect.New(qType);
            object mTarget = Reflect.New(mType);
            Reflect.Set(qTarget, "convertedfrom", Enum.Parse(qVersion, "Mathcad14"));
            Reflect.Set(mTarget, "convertedfrom", Enum.Parse(mVersion, "Mathcad14"));
            Reflect.Set(qTarget, "convertedfromSpecified", specified);
            Reflect.Set(mTarget, "convertedfromSpecified", specified);
            MethodComparison.Compare($"{accessor}(convertedfromSpecified={specified})", contract,
                qGet, qTarget, [], mGet, mTarget, [], Reflect.Text, Reflect.Text, RegionCollectionState, RegionCollectionState);
        }
    }

    private static Array MakeRegions(System.Type regionType, params string[] values)
    {
        Array regions = Array.CreateInstance(regionType, values.Length);
        for (int i = 0; i < values.Length; i++)
        {
            object region = Reflect.New(regionType);
            Reflect.Set(region, "Item", values[i]);
            Reflect.Set(region, "regionid", values[i] + "-id");
            Reflect.Set(region, "Tag", values[i] + "-tag");
            Reflect.Set(region, "mc14conversionlossannotations", new[] { values[i] + "-annotation" });
            regions.SetValue(region, i);
        }
        return regions;
    }

    private static string ShowRegions(object? value)
    {
        if (value is not IEnumerable regions || value is string)
            return Reflect.Text(value);
        return "[" + string.Join(", ", regions.Cast<object?>().Select(ShowRegion)) + "]";
    }

    private static string ShowRegion(object? region) => region == null
        ? "null"
        : $"{Reflect.Text(Reflect.Get(region, "Item"))}|{Reflect.Text(Reflect.Get(region, "regionid"))}|"
            + $"{Reflect.Text(Reflect.Get(region, "Tag"))}|{Reflect.Text(Reflect.Get(region, "mc14conversionlossannotations"))}";

    private static string RegionCollectionState(object target) =>
        $"regions={ShowRegions(Reflect.Get(target, "regions"))}; convertedfrom={Convert.ToInt32(Reflect.Get(target, "convertedfrom"))}; "
        + $"convertedfromSpecified={Reflect.Text(Reflect.Get(target, "convertedfromSpecified"))}";

    private static Type? DirectExceptionType(MethodInfo method, object target, object? argument)
    {
        try
        {
            method.Invoke(target, [argument]);
            return null;
        }
        catch (TargetInvocationException exception) when (exception.InnerException != null)
        {
            return exception.InnerException.GetType();
        }
    }
}
