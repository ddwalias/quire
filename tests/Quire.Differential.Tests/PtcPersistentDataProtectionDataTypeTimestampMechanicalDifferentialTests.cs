using System.Reflection;
using Quire.PersistentData;

namespace Quire.Differential.Tests;

public class PtcPersistentDataProtectionDataTypeTimestampDifferentialTests
{
    private const string Contract = "Ptc.PersistentData.ProtectionDataType";
    private const BindingFlags AllInstance = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

    [SkippableFact]
    public void TimestampWrapper_getter_and_setter_match_PtcCore()
    {
        Mathcad.Require();
        System.Type qType = typeof(ProtectionDataType);
        System.Type mType = Mathcad.Type("Ptc.Core", "Ptc.PersistentData.ProtectionDataType");
        MethodInfo qGet = qType.GetMethod("get_TimestampWrapper", AllInstance)!;
        MethodInfo mGet = mType.GetMethod("get_TimestampWrapper", AllInstance)!;
        MethodInfo qSet = qType.GetMethod("set_TimestampWrapper", AllInstance)!;
        MethodInfo mSet = mType.GetMethod("set_TimestampWrapper", AllInstance)!;

        DateTime baseTime = new(638500000000000000L, DateTimeKind.Local);
        foreach (bool specified in new[] { false, true })
            foreach (DateTimeKind kind in Enum.GetValues<DateTimeKind>())
            {
                DateTime timestamp = DateTime.SpecifyKind(baseTime, kind);
                object qTarget = Reflect.New(qType);
                object mTarget = Reflect.New(mType);
                SetBackingTimestamp(qTarget, timestamp, specified);
                SetBackingTimestamp(mTarget, timestamp, specified);
                MethodComparison.Compare($"get_TimestampWrapper(specified={specified}, kind={kind})", Contract,
                    qGet, qTarget, [], mGet, mTarget, [], ShowNullableDate, ShowNullableDate, TimestampState, TimestampState);
            }

        foreach (DateTimeKind kind in Enum.GetValues<DateTimeKind>())
        {
            object qTarget = Reflect.New(qType);
            object mTarget = Reflect.New(mType);
            SetBackingTimestamp(qTarget, baseTime, true);
            SetBackingTimestamp(mTarget, baseTime, true);
            DateTime value = DateTime.SpecifyKind(baseTime.AddTicks((int)kind + 1), kind);
            MethodComparison.Compare($"set_TimestampWrapper({kind})", Contract,
                qSet, qTarget, [value], mSet, mTarget, [value], Reflect.Text, Reflect.Text, TimestampState, TimestampState);
        }

        object qNullTarget = Reflect.New(qType);
        object mNullTarget = Reflect.New(mType);
        SetBackingTimestamp(qNullTarget, baseTime, true);
        SetBackingTimestamp(mNullTarget, baseTime, true);
        MethodComparison.Compare("set_TimestampWrapper(null) preserves Timestamp", Contract,
            qSet, qNullTarget, [null], mSet, mNullTarget, [null], ShowNullableDate, ShowNullableDate, TimestampState, TimestampState);
    }

    private static void SetBackingTimestamp(object target, DateTime value, bool specified)
    {
        Reflect.Set(target, "Timestamp", value);
        Reflect.Set(target, "TimestampSpecified", specified);
    }

    private static string ShowNullableDate(object? value) => value is DateTime date
        ? $"{date.Ticks}/{date.Kind}"
        : "null";

    private static string TimestampState(object target)
    {
        DateTime timestamp = (DateTime)Reflect.Get(target, "Timestamp")!;
        return $"{timestamp.Ticks}/{timestamp.Kind}; specified={Reflect.Text(Reflect.Get(target, "TimestampSpecified"))}";
    }
}
