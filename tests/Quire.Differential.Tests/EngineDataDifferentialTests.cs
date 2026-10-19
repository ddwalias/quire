using System;
using System.Reflection;
using Quire.Engine;
using Xunit;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

public class EngineDataDifferentialTests
{
    // AlgoInfo
    [SkippableFact]
    public void AlgoInfo_ctor_060000b4() => Mechanical.Compare(typeof(global::Quire.Engine.AlgoInfo), 0x060000b4);

    [SkippableFact]
    public void AlgoInfo_get_Name_060000ae() => Mechanical.Compare(typeof(global::Quire.Engine.AlgoInfo), 0x060000ae);

    [SkippableFact]
    public void AlgoInfo_set_Name_060000af() => Mechanical.Compare(typeof(global::Quire.Engine.AlgoInfo), 0x060000af);

    [SkippableFact]
    public void AlgoInfo_get_Tag_060000b0() => Mechanical.Compare(typeof(global::Quire.Engine.AlgoInfo), 0x060000b0);

    [SkippableFact]
    public void AlgoInfo_set_Tag_060000b1() => Mechanical.Compare(typeof(global::Quire.Engine.AlgoInfo), 0x060000b1);

    [SkippableFact]
    public void AlgoInfo_get_Region_060000b2() => Mechanical.Compare(typeof(global::Quire.Engine.AlgoInfo), 0x060000b2);

    [SkippableFact]
    public void AlgoInfo_set_Region_060000b3() => Mechanical.Compare(typeof(global::Quire.Engine.AlgoInfo), 0x060000b3);

    // RedefInfo
    [SkippableFact]
    public void RedefInfo_ctor_060000ad() => Mechanical.Compare(typeof(global::Quire.Engine.RedefInfo), 0x060000ad);

    [SkippableFact]
    public void RedefInfo_get_Name_060000a5() => Mechanical.Compare(typeof(global::Quire.Engine.RedefInfo), 0x060000a5);

    [SkippableFact]
    public void RedefInfo_set_Name_060000a6() => Mechanical.Compare(typeof(global::Quire.Engine.RedefInfo), 0x060000a6);

    [SkippableFact]
    public void RedefInfo_get_OldType_060000a7() => Mechanical.Compare(typeof(global::Quire.Engine.RedefInfo), 0x060000a7);

    [SkippableFact]
    public void RedefInfo_set_OldType_060000a8() => Mechanical.Compare(typeof(global::Quire.Engine.RedefInfo), 0x060000a8);

    [SkippableFact]
    public void RedefInfo_get_OldLocation_060000a9() => Mechanical.Compare(typeof(global::Quire.Engine.RedefInfo), 0x060000a9);

    [SkippableFact]
    public void RedefInfo_set_OldLocation_060000aa() => Mechanical.Compare(typeof(global::Quire.Engine.RedefInfo), 0x060000aa);

    [SkippableFact]
    public void RedefInfo_get_NewType_060000ab() => Mechanical.Compare(typeof(global::Quire.Engine.RedefInfo), 0x060000ab);

    [SkippableFact]
    public void RedefInfo_set_NewType_060000ac() => Mechanical.Compare(typeof(global::Quire.Engine.RedefInfo), 0x060000ac);

    // Entry
    [SkippableFact]
    public void Entry_ctor_060000c1() => Mechanical.Compare(typeof(global::Quire.Engine.Entry), 0x060000c1);

    // ComponentHelpers
    [SkippableFact]
    public void ComponentHelpers_Unlock_060000cb() => Mechanical.Compare(typeof(global::Quire.Engine.ComponentHelpers), 0x060000cb);

    [SkippableFact]
    public void ComponentHelpers_direct_comparisons()
    {
        var mHelpersType = Mathcad.Type("iengine", "Engine.ComponentHelpers");
        Assert.NotNull(mHelpersType);

        var qLock = typeof(global::Quire.Engine.ComponentHelpers).GetMethod("Lock")!;
        var mLock = mHelpersType.GetMethod("Lock")!;
        Assert.NotNull(qLock);
        Assert.NotNull(mLock);

        var qUnlock = typeof(global::Quire.Engine.ComponentHelpers).GetMethod("Unlock")!;
        var mUnlock = mHelpersType.GetMethod("Unlock")!;
        Assert.NotNull(qUnlock);
        Assert.NotNull(mUnlock);

        var qSetValue = typeof(global::Quire.Engine.ComponentHelpers).GetMethod("SetValue")!;
        var mSetValue = mHelpersType.GetMethod("SetValue")!;
        Assert.NotNull(qSetValue);
        Assert.NotNull(mSetValue);

        var qGetValue = typeof(global::Quire.Engine.ComponentHelpers).GetMethod("GetValue")!;
        var mGetValue = mHelpersType.GetMethod("GetValue")!;
        Assert.NotNull(qGetValue);
        Assert.NotNull(mGetValue);
    }
}
