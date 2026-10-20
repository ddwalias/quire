using System.Collections;
using System.Reflection;
using System.Runtime.CompilerServices;
using Quire.Calculation.Core;

namespace Quire.Differential.Tests;

public class PtcCalculationCoreDefaultUnitDescriptorTableInitializationMechanicalDifferentialTests
{
    private const string Contract = "Ptc.Calculation.Core.DefaultUnitDescriptorTable";
    private const BindingFlags Instance = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
    private static readonly Guid OriginalMvid = Guid.Parse("402d6e7b-941e-48f3-b17b-e1cd1a92b615");
    private static readonly Type Original = Mathcad.Type("Ptc.Calculation.Core", Contract);
    private static readonly Type OriginalDescriptor = Mathcad.Type("Ptc.Calculation.Core", "Ptc.Calculation.Core.UnitDescriptor");
    private static readonly Type OriginalSystems = Mathcad.Type("Ptc.Calculation.Core", "Ptc.Calculation.Core.UnitSystems");
    private static readonly Type QuireType = typeof(DefaultUnitDescriptorTable);
    private static readonly MethodInfo QuireInitialize = QuireType.GetMethod("InitializeUnitSystems", Instance)!;
    private static readonly MethodInfo OriginalInitialize = Original.GetMethod("InitializeUnitSystems", Instance)!;

    [SkippableFact]
    public void InitializeUnitSystems_cases()
    {
        Mathcad.Require();
        CheckOriginal(OriginalInitialize, 0x06000009);

        object qTarget = NewTable(QuireType, typeof(Dictionary<string, UnitDescriptor>), typeof(UnitDescriptor), typeof(UnitSystems));
        object mTarget = NewTable(Original, Original.BaseType!.BaseType!, OriginalDescriptor, OriginalSystems);
        MethodComparison.Compare("partitions base and derived descriptors by system", Contract,
            QuireInitialize, qTarget, [], OriginalInitialize, mTarget, [], null, null, TableState, TableState);
    }

    [SkippableFact]
    public void ComponentConnectorConnect_cases()
    {
        Mathcad.Require();
        Type qConnector = typeof(System.Windows.Markup.IComponentConnector);
        Type mConnector = Original.GetInterfaces().Single(type => type.FullName == "System.Windows.Markup.IComponentConnector");
        MethodInfo qMethod = QuireType.GetInterfaceMap(qConnector).TargetMethods
            .Single(method => method.Name.EndsWith(".Connect", StringComparison.Ordinal));
        MethodInfo mMethod = Original.GetInterfaceMap(mConnector).TargetMethods
            .Single(method => method.Name.EndsWith(".Connect", StringComparison.Ordinal));
        CheckOriginal(mMethod, 0x06000012);

        CompareConnect("connection 0 with null target", qMethod, mMethod, 0, null);
        CompareConnect("connection 10 with object target", qMethod, mMethod, 10, new object());
    }

    private static void CompareConnect(string caseId, MethodInfo qMethod, MethodInfo mMethod, int id, object? target)
    {
        object qTarget = RuntimeHelpers.GetUninitializedObject(QuireType);
        object mTarget = RuntimeHelpers.GetUninitializedObject(Original);
        MethodComparison.Compare(caseId, Contract, qMethod, qTarget, [id, target], mMethod, mTarget, [id, target],
            null, null, ConnectorState, ConnectorState);
    }

    private static object NewTable(Type type, Type dictionaryType, Type descriptorType, Type systemsType)
    {
        object target = RuntimeHelpers.GetUninitializedObject(type);
        object initializedDictionary = Activator.CreateInstance(dictionaryType)!;
        foreach (FieldInfo field in dictionaryType.GetFields(Instance | BindingFlags.DeclaredOnly))
        {
            field.SetValue(target, field.GetValue(initializedDictionary));
        }

        string[] tableFields = ["_siBase", "_siDerived", "_usBase", "_usDerived", "_cgsBase", "_cgsDerived"];
        foreach (string name in tableFields)
        {
            Type fieldType = type.GetField(name, Instance)!.FieldType;
            object table = Activator.CreateInstance(fieldType)!;
            type.GetField(name, Instance)!.SetValue(target, table);
        }

        Type listType = typeof(List<>).MakeGenericType(systemsType);
        AddDescriptor(target, dictionaryType, descriptorType, systemsType, listType, "base-all", ["SI", "US", "CGS"], ["US"]);
        AddDescriptor(target, dictionaryType, descriptorType, systemsType, listType, "base-us", ["US"], null);
        AddDescriptor(target, dictionaryType, descriptorType, systemsType, listType, "derived-si-cgs", null, ["SI", "CGS"]);
        return target;
    }

    private static void AddDescriptor(object target, Type dictionaryType, Type descriptorType, Type systemsType,
        Type listType, string key, string[]? bases, string[]? derived)
    {
        object descriptor = Activator.CreateInstance(descriptorType)!;
        descriptorType.GetProperty("CanonicalName")!.SetValue(descriptor, key);
        descriptorType.GetField("_baseUnitSystems", Instance)!.SetValue(descriptor, MakeSystems(listType, systemsType, bases));
        descriptorType.GetField("_derivedUnitSystems", Instance)!.SetValue(descriptor, MakeSystems(listType, systemsType, derived));
        dictionaryType.GetMethod("Add", [typeof(string), descriptorType])!.Invoke(target, [key, descriptor]);
    }

    private static object? MakeSystems(Type listType, Type systemsType, string[]? names)
    {
        if (names == null) return null;
        IList list = (IList)Activator.CreateInstance(listType)!;
        foreach (string name in names)
        {
            list.Add(Enum.Parse(systemsType, name));
        }
        return list;
    }

    private static string TableState(object target)
    {
        string[] names = ["_siBase", "_siDerived", "_usBase", "_usDerived", "_cgsBase", "_cgsDerived"];
        return string.Join("|", names.Select(name =>
        {
            IDictionary table = (IDictionary)target.GetType().GetField(name, Instance)!.GetValue(target)!;
            string entries = string.Join(",", table.Keys.Cast<object>().OfType<string>().OrderBy(key => key)
                .Select(key => $"{key}:{table[key]!.GetType().GetProperty("CanonicalName")!.GetValue(table[key])}"));
            return $"{name}={entries}";
        }));
    }

    private static string ConnectorState(object target)
    {
        bool loaded = (bool)target.GetType().GetField("_contentLoaded", Instance)!.GetValue(target)!;
        return loaded ? "loaded" : "not-loaded";
    }

    private static void CheckOriginal(MethodInfo method, int token)
    {
        Assert.Equal(OriginalMvid, method.Module.ModuleVersionId);
        Assert.Equal(token, method.MetadataToken);
    }
}
