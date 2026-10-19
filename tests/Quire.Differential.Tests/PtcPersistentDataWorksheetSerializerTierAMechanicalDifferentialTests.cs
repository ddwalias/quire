using System.Collections;
using System.Reflection;
using Quire.PersistentData;
using Quire.PersistentDataObjects;

namespace Quire.Differential.Tests;

public class PtcPersistentDataWorksheetSerializerTierAMechanicalDifferentialTests
{
    private const string WorksheetContract = "Ptc.PersistentData.worksheet";
    private const string SerializerContract = "Ptc.PersistentData.worksheetRegionCollectionSerializer";

    [SkippableFact]
    public void Regions_get_matches_PtcCore()
    {
        Mathcad.Require();
        (System.Type qType, System.Type mType, object qTarget, object mTarget) = NewWorksheetPair();
        MethodInfo qGetter = qType.GetMethod("get_Regions")!;
        MethodInfo mGetter = mType.GetMethod("get_Regions")!;
        MethodComparison.Compare("worksheet/Regions/get/null", WorksheetContract,
            qGetter, qTarget, [], mGetter, mTarget, [], ShowSequence, ShowSequence,
            RenderWorksheet, RenderWorksheet);

        Set(qTarget, "regions", MakeRegionArray(isOriginal: false));
        Set(mTarget, "regions", MakeRegionArray(isOriginal: true));
        MethodComparison.Compare("worksheet/Regions/get/null-and-populated", WorksheetContract,
            qGetter, qTarget, [], mGetter, mTarget, [], ShowSequence, ShowSequence,
            RenderWorksheet, RenderWorksheet);
    }

    [SkippableFact]
    public void Regions_set_matches_PtcCore()
    {
        Mathcad.Require();
        (System.Type qType, System.Type mType, object qTarget, object mTarget) = NewWorksheetPair();
        MethodInfo qSet = qType.GetMethod("set_Regions")!;
        MethodInfo mSet = mType.GetMethod("set_Regions")!;

        MethodComparison.Compare("worksheet/Regions/set/null", WorksheetContract,
            qSet, qTarget, [null], mSet, mTarget, [null], ShowSequence, ShowSequence,
            RenderWorksheet, RenderWorksheet);

        object?[] qRegions = [MakeRegionArray(isOriginal: false)];
        object?[] mRegions = [MakeRegionArray(isOriginal: true)];
        MethodComparison.Compare("worksheet/Regions/set/one-region", WorksheetContract,
            qSet, qTarget, qRegions, mSet, mTarget, mRegions, ShowSequence, ShowSequence,
            RenderWorksheet, RenderWorksheet);
    }

    [SkippableFact]
    public void WorksheetConvertedFromMC14_matches_PtcCore()
    {
        Mathcad.Require();
        (System.Type qType, System.Type mType, object qTarget, object mTarget) = NewWorksheetPair();
        MethodInfo qGetter = qType.GetMethod("get_WorksheetConvertedFromMC14")!;
        MethodInfo mGetter = mType.GetMethod("get_WorksheetConvertedFromMC14")!;

        MethodComparison.Compare("worksheet/WorksheetConvertedFromMC14/not-specified", WorksheetContract,
            qGetter, qTarget, [], mGetter, mTarget, [], ShowValue, ShowValue,
            RenderWorksheet, RenderWorksheet);

        Set(qTarget, "convertedfrom", Enum.Parse(typeof(applicationVersionType), "Mathcad14"));
        Set(qTarget, "convertedfromSpecified", true);
        System.Type mVersion = Mathcad.Type("Ptc.Core", "Ptc.PersistentDataObjects.applicationVersionType");
        Set(mTarget, "convertedfrom", Enum.Parse(mVersion, "Mathcad14"));
        Set(mTarget, "convertedfromSpecified", true);
        MethodComparison.Compare("worksheet/WorksheetConvertedFromMC14/specified-mathcad14", WorksheetContract,
            qGetter, qTarget, [], mGetter, mTarget, [], ShowValue, ShowValue,
            RenderWorksheet, RenderWorksheet);
    }

    [SkippableFact]
    public void RegionCollection_get_matches_PtcCore()
    {
        Mathcad.Require();
        (System.Type qType, System.Type mType, object qTarget, object mTarget) = NewSerializerPair();
        MethodInfo qGetter = qType.GetMethod("get_RegionCollection")!;
        MethodInfo mGetter = mType.GetMethod("get_RegionCollection")!;
        MethodComparison.Compare("worksheet serializer/RegionCollection/get/lazy-new-and-cached", SerializerContract,
            qGetter, qTarget, [], mGetter, mTarget, [], RenderWorksheet, RenderWorksheet,
            RenderSerializer, RenderSerializer);

        (qType, mType, qTarget, mTarget) = NewSerializerPair();
        object qWorksheet = NewWorksheet(typeof(worksheet), MakeRegionArray(isOriginal: false), true, isOriginal: false);
        System.Type mWorksheetType = Mathcad.Type("Ptc.Core", "Ptc.PersistentData.worksheet");
        object mWorksheet = NewWorksheet(mWorksheetType, MakeRegionArray(isOriginal: true), true, isOriginal: true);
        SetField(qTarget, "_worksheet", qWorksheet);
        SetField(mTarget, "_worksheet", mWorksheet);
        qGetter = qType.GetMethod("get_RegionCollection")!;
        mGetter = mType.GetMethod("get_RegionCollection")!;
        MethodComparison.Compare("worksheet serializer/RegionCollection/get/existing", SerializerContract,
            qGetter, qTarget, [], mGetter, mTarget, [], RenderWorksheet, RenderWorksheet,
            RenderSerializer, RenderSerializer);
    }

    [SkippableFact]
    public void ConvertedFromMC14_matches_PtcCore()
    {
        Mathcad.Require();
        CompareSerializerValue("get_ConvertedFromMC14", "worksheet serializer/ConvertedFromMC14/lazy-default",
            ShowValue, ConfigureWorksheet: null);
        CompareSerializerValue("get_ConvertedFromMC14", "worksheet serializer/ConvertedFromMC14/true",
            ShowValue, ConfigureWorksheet: static (worksheet, isOriginal) =>
            {
                System.Type versionType = isOriginal
                    ? Mathcad.Type("Ptc.Core", "Ptc.PersistentDataObjects.applicationVersionType")
                    : typeof(applicationVersionType);
                Set(worksheet, "convertedfrom", Enum.Parse(versionType, "Mathcad14"));
                Set(worksheet, "convertedfromSpecified", true);
            });
    }

    [SkippableFact]
    public void MsgId_matches_PtcCore()
    {
        Mathcad.Require();
        CompareSerializerValue("get_MsgId", "worksheet serializer/MsgId/default", ShowValue, ConfigureWorksheet: null);
        CompareSerializerValue("get_MsgId", "worksheet serializer/MsgId/scriptable-warning", ShowValue,
            ConfigureWorksheet: static (worksheet, isOriginal) =>
            {
                System.Type enumType = isOriginal
                    ? Mathcad.Type("Ptc.Core", "Ptc.PersistentDataObjects.worksheetMessageIdType")
                    : typeof(worksheetMessageIdType);
                Set(worksheet, "msgId", Enum.Parse(enumType, "ScriptableWarning"));
            });
    }

    private static void CompareSerializerValue(string methodName, string caseId, Func<object?, string> show,
        Action<object, bool>? ConfigureWorksheet)
    {
        (System.Type qType, System.Type mType, object qTarget, object mTarget) = NewSerializerPair();
        if (ConfigureWorksheet != null)
        {
            object qWorksheet = NewWorksheet(typeof(worksheet), null, false, isOriginal: false);
            System.Type mWorksheetType = Mathcad.Type("Ptc.Core", "Ptc.PersistentData.worksheet");
            object mWorksheet = NewWorksheet(mWorksheetType, null, false, isOriginal: true);
            ConfigureWorksheet(qWorksheet, false);
            ConfigureWorksheet(mWorksheet, true);
            SetField(qTarget, "_worksheet", qWorksheet);
            SetField(mTarget, "_worksheet", mWorksheet);
        }
        MethodInfo qGetter = qType.GetMethod(methodName)!;
        MethodInfo mGetter = mType.GetMethod(methodName)!;
        MethodComparison.Compare(caseId, SerializerContract,
            qGetter, qTarget, [], mGetter, mTarget, [], show, show,
            RenderSerializer, RenderSerializer);
    }

    private static (System.Type qType, System.Type mType, object qTarget, object mTarget) NewWorksheetPair()
    {
        System.Type qType = typeof(worksheet);
        System.Type mType = Mathcad.Type("Ptc.Core", "Ptc.PersistentData.worksheet");
        return (qType, mType, Activator.CreateInstance(qType)!, Activator.CreateInstance(mType)!);
    }

    private static (System.Type qType, System.Type mType, object qTarget, object mTarget) NewSerializerPair()
    {
        System.Type qType = typeof(worksheetRegionCollectionSerializer);
        System.Type mType = Mathcad.Type("Ptc.Core", "Ptc.PersistentData.worksheetRegionCollectionSerializer");
        return (qType, mType, Activator.CreateInstance(qType)!, Activator.CreateInstance(mType)!);
    }

    private static object NewWorksheet(System.Type type, object? regions, bool converted, bool isOriginal)
    {
        object worksheet = Activator.CreateInstance(type)!;
        Set(worksheet, "regions", regions);
        System.Type versionType = isOriginal
            ? Mathcad.Type("Ptc.Core", "Ptc.PersistentDataObjects.applicationVersionType")
            : typeof(applicationVersionType);
        Set(worksheet, "convertedfrom", Enum.Parse(versionType, "Mathcad14"));
        Set(worksheet, "convertedfromSpecified", converted);
        System.Type messageType = isOriginal
            ? Mathcad.Type("Ptc.Core", "Ptc.PersistentDataObjects.worksheetMessageIdType")
            : typeof(worksheetMessageIdType);
        Set(worksheet, "msgId", Enum.Parse(messageType, "ScriptableWarning"));
        return worksheet;
    }

    private static Array MakeRegionArray(bool isOriginal)
    {
        System.Type regionType = isOriginal
            ? Mathcad.Type("Ptc.Core", "Ptc.PersistentDataObjects.worksheetRegionType")
            : typeof(worksheetRegionType);
        Array regions = Array.CreateInstance(regionType, 1);
        regions.SetValue(Activator.CreateInstance(regionType), 0);
        return regions;
    }

    private static string RenderWorksheet(object? value)
    {
        if (value is null)
            return "null";
        object? regions = Reflect.Get(value, "regions");
        return $"regions={ShowSequence(regions)};converted={ShowValue(Reflect.Get(value, "convertedfrom"))};specified={ShowValue(Reflect.Get(value, "convertedfromSpecified"))};msgId={ShowValue(Reflect.Get(value, "msgId"))}";
    }

    private static string RenderSerializer(object? value)
    {
        if (value is null)
            return "null";
        return RenderWorksheet(Reflect.Get(value, "_worksheet"));
    }

    private static string ShowSequence(object? value)
    {
        if (value is null)
            return "null";
        if (value is not IEnumerable sequence || value is string)
            return ShowValue(value);
        return "[" + string.Join(",", sequence.Cast<object?>().Select(item => item?.GetType().Name ?? "null")) + "]";
    }

    private static string ShowValue(object? value) => value?.ToString() ?? "null";

    private static void Set(object target, string property, object? value) =>
        target.GetType().GetProperty(property)!.SetValue(target, value);

    private static void SetField(object target, string field, object? value) =>
        target.GetType().GetField(field, BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(target, value);
}
