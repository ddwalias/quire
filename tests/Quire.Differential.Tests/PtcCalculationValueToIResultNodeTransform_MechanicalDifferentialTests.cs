using System.Reflection;
using Quire.Automation;
using Quire.Calculation;
using Quire.Calculation.Core;

namespace Quire.Differential.Tests;

/// <summary>Direct comparisons for the reachable ValueToIResultNodeTransform paths.</summary>
public class PtcCalculationValueToIResultNodeTransform_MechanicalDifferentialTests
{
    private const string Contract = "Ptc.Calculation.ValueToIResultNodeTransform";
    private static readonly BindingFlags Instance = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
    private static readonly BindingFlags Declared = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
    private static readonly Type Q = typeof(ValueToIResultNodeTransform);
    private static readonly Type M = Mathcad.Type("Ptc.Calculation", Contract);
    private static readonly Type QFactory = typeof(IResultNodeFactory);
    private static readonly Type MFactory = Mathcad.Type("Ptc.Calculation.Core", "Ptc.Calculation.Core.IResultNodeFactory");
    private static readonly Type QValueProxy = typeof(IValueProxy);
    private static readonly Type MValueProxy = Mathcad.Type("automationmgd", "Mathcad.Automation.IValueProxy");
    private static readonly Type QAutomationValue = typeof(IAutomationValue);
    private static readonly Type MAutomationValue = Mathcad.Type("automationmgd", "Mathcad.Automation.IAutomationValue");

    [SkippableFact]
    public void Constructor_and_SimplifyUnits_accessors_06000083_06000084_06000085()
    {
        Mathcad.Require();
        StubProxy qFactory = CreateFactory(QFactory);
        StubProxy mFactory = CreateFactory(MFactory);
        ConstructorInfo qCtor = Q.GetConstructor(Instance, null, [QFactory], null)!;
        ConstructorInfo mCtor = M.GetConstructor(Instance, null, [MFactory], null)!;
        MethodComparison.Compare(".ctor", Contract, qCtor, null, [qFactory], mCtor, null, [mFactory], Show, Show);
        object q = qCtor.Invoke([qFactory]);
        object m = mCtor.Invoke([mFactory]);

        PropertyInfo qProperty = Q.GetProperty("SimplifyUnits")!;
        PropertyInfo mProperty = M.GetProperty("SimplifyUnits")!;
        MethodComparison.Compare("get_SimplifyUnits", Contract, qProperty.GetMethod!, q, [], mProperty.GetMethod!, m, [],
            Show, Show, State, State);
        MethodComparison.Compare("set_SimplifyUnits(true)", Contract, qProperty.SetMethod!, q, [true], mProperty.SetMethod!, m, [true],
            Show, Show, State, State);
        MethodComparison.Compare("set_SimplifyUnits(false)", Contract, qProperty.SetMethod!, q, [false], mProperty.SetMethod!, m, [false],
            Show, Show, State, State);
    }

    [SkippableFact]
    public void Transform_and_automation_value_dispatch_06000086_06000087()
    {
        Mathcad.Require();
        CompareInstance("Transform(null)", "Transform", [QAutomationValue], [MAutomationValue], null, null);
        CompareInstance("MakeResultNode(automation null)", "MakeResultNode", [QAutomationValue], [MAutomationValue], null, null);

        object qValueProxy = CreateValueProxy(QValueProxy, "string", text: "a\r\nb\rc\nd");
        object mValueProxy = CreateValueProxy(MValueProxy, "string", text: "a\r\nb\rc\nd");
        object qAutomation = CreateAutomationValue(QAutomationValue, qValueProxy);
        object mAutomation = CreateAutomationValue(MAutomationValue, mValueProxy);
        CompareInstance("Transform(string value)", "Transform", [QAutomationValue], [MAutomationValue], qAutomation, mAutomation, simplify: true);
        CompareInstance("MakeResultNode(automation string)", "MakeResultNode", [QAutomationValue], [MAutomationValue], qAutomation, mAutomation, simplify: true);
    }

    [SkippableFact]
    public void Value_proxy_dispatch_06000089()
    {
        Mathcad.Require();
        CompareValueProxy("string with mixed line endings", "string", text: "a\r\nb\rc\nd");
        CompareValueProxy("function proxy display", "function", text: "F(x)");
        CompareValueProxy("function display without wrapper", "function", text: "Raw:plain");
        CompareValueProxy("function display with open wrapper", "function", text: "Raw:{ValueProxy for F(x)");
        CompareValueProxy("function display with close wrapper", "function", text: "Raw:F(x)}");
        CompareValueProxy("symbolic XML", "symbolic", text: "<ml:symbolic/> ");
        CompareValueProxy("real scalar", "scalar");
        CompareValueProxy("unrecognized proxy", "none");
    }

    [SkippableFact]
    public void CleanLineBreake_replaces_CRLF_CR_and_LF_0600008a()
    {
        Mathcad.Require();
        MethodInfo q = Get("CleanLineBreake", [typeof(string), typeof(string)], isStatic: true);
        MethodInfo m = Get("CleanLineBreake", [typeof(string), typeof(string)], isStatic: true, original: true);
        foreach (string? input in new string?[] { null, "", "plain", "a\r\nb\rc\nd" })
            MethodComparison.Compare($"CleanLineBreake({Show(input)})", Contract, q, null, [input, "|"], m, null, [input, "|"], Show, Show);
    }

    [SkippableFact]
    public void BuildScalar_real_imaginary_complex_and_unitful_0600008b()
    {
        Mathcad.Require();
        CompareScalar("real", 2.5, 0.0, units: []);
        CompareScalar("imaginary", 0.0, -4.25, units: []);
        CompareScalar("complex", 2.5, -4.25, units: []);
        CompareScalar("unitful real", 2.5, 0.0, units: [("m", 2, 3), ("s", -1, 2)]);
    }

    [SkippableFact]
    public void MakeUnitedValue_adds_each_exact_rational_unit_0600008d()
    {
        Mathcad.Require();
        StubProxy qFactory = CreateFactory(QFactory);
        StubProxy mFactory = CreateFactory(MFactory);
        object qTarget = Construct(Q, qFactory);
        object mTarget = Construct(M, mFactory);
        object qValue = CreateNodeProxy(typeof(IResultNode), "input-result");
        object mValue = CreateNodeProxy(Mathcad.Type("Ptc.Calculation.Core", "Ptc.Calculation.Core.IResultNode"), "input-result");
        Array qUnits = CreateUnits(typeof(global::Quire.Automation.UnitPower), typeof(global::Quire.Mcd.Rational), [("m", 2, 3), ("s", -1, 2)]);
        Array mUnits = CreateUnits(Mathcad.Type("automationmgd", "Mathcad.Automation.UnitPower"),
            Mathcad.Type("mplrun", "Mcd.Rational"), [("m", 2, 3), ("s", -1, 2)]);
        MethodComparison.Compare("MakeUnitedValue(two units)", Contract,
            Get("MakeUnitedValue", [typeof(IResultNode), typeof(global::Quire.Automation.UnitPower[])], isStatic: false), qTarget, [qValue, qUnits],
            Get("MakeUnitedValue", [Mathcad.Type("Ptc.Calculation.Core", "Ptc.Calculation.Core.IResultNode"), mUnits.GetType()], isStatic: false, original: true),
            mTarget, [mValue, mUnits], Show, Show, State, State);
    }

    [SkippableFact]
    public void HasUnits_checks_unit_array_length_0600008e()
    {
        Mathcad.Require();
        MethodInfo q = Get("HasUnits", [QValueProxy], isStatic: true);
        MethodInfo m = Get("HasUnits", [MValueProxy], isStatic: true, original: true);
        CompareHasUnits("no units", []);
        CompareHasUnits("two units", [("m", 1, 1), ("s", -2, 1)]);

        void CompareHasUnits(string label, (string Name, int Numerator, int Denominator)[] units)
        {
            object qProxy = CreateValueProxy(QValueProxy, "none", units: CreateUnits(typeof(global::Quire.Automation.UnitPower), typeof(global::Quire.Mcd.Rational), units));
            object mProxy = CreateValueProxy(MValueProxy, "none", units: CreateUnits(Mathcad.Type("automationmgd", "Mathcad.Automation.UnitPower"),
                Mathcad.Type("mplrun", "Mcd.Rational"), units));
            MethodComparison.Compare($"HasUnits({label})", Contract, q, null, [qProxy], m, null, [mProxy], Show, Show);
        }
    }

    private static void CompareInstance(string label, string name, Type[] qParameters, Type[] mParameters,
        object? qArgument, object? mArgument, bool simplify = false)
    {
        StubProxy qFactory = CreateFactory(QFactory);
        StubProxy mFactory = CreateFactory(MFactory);
        object q = Construct(Q, qFactory);
        object m = Construct(M, mFactory);
        if (simplify)
        {
            Q.GetProperty("SimplifyUnits")!.SetValue(q, true);
            M.GetProperty("SimplifyUnits")!.SetValue(m, true);
        }
        MethodComparison.Compare(label, Contract, Get(name, qParameters), q, [qArgument],
            Get(name, mParameters, original: true), m, [mArgument], Show, Show, State, State);
    }

    private static void CompareValueProxy(string label, string kind, string? text = null)
    {
        StubProxy qFactory = CreateFactory(QFactory);
        StubProxy mFactory = CreateFactory(MFactory);
        object q = Construct(Q, qFactory);
        object m = Construct(M, mFactory);
        object qProxy = CreateValueProxy(QValueProxy, kind, text);
        object mProxy = CreateValueProxy(MValueProxy, kind, text);
        MethodComparison.Compare($"MakeResultNode({label})", Contract,
            Get("MakeResultNode", [QValueProxy]), q, [qProxy], Get("MakeResultNode", [MValueProxy], original: true), m, [mProxy], Show, Show, State, State);
    }

    private static void CompareScalar(string label, double real, double imaginary,
        (string Name, int Numerator, int Denominator)[] units)
    {
        StubProxy qFactory = CreateFactory(QFactory);
        StubProxy mFactory = CreateFactory(MFactory);
        object q = Construct(Q, qFactory);
        object m = Construct(M, mFactory);
        object qScalar = CreateScalar(typeof(IScalarValueProxy), real, imaginary,
            CreateUnits(typeof(global::Quire.Automation.UnitPower), typeof(global::Quire.Mcd.Rational), units));
        object mScalar = CreateScalar(Mathcad.Type("automationmgd", "Mathcad.Automation.IScalarValueProxy"), real, imaginary,
            CreateUnits(Mathcad.Type("automationmgd", "Mathcad.Automation.UnitPower"), Mathcad.Type("mplrun", "Mcd.Rational"), units));
        MethodComparison.Compare($"BuildScalar({label})", Contract,
            Get("BuildScalar", [typeof(IScalarValueProxy)]), q, [qScalar],
            Get("BuildScalar", [Mathcad.Type("automationmgd", "Mathcad.Automation.IScalarValueProxy")], original: true), m, [mScalar], Show, Show, State, State);
    }

    private static object CreateScalar(Type scalarType, double real, double imaginary, Array units) =>
        CreateProxy(scalarType, (method, _) => method.Name switch
        {
            "get_RealPart" => real,
            "get_ImagPart" => imaginary,
            "get_Units" => units,
            _ => Default(method.ReturnType),
        }, () => $"scalar({real:R},{imaginary:R};{Show(units)})");

    private static object CreateAutomationValue(Type type, object proxy)
    {
        List<string> requests = [];
        return CreateProxy(type, (method, args) =>
        {
            if (method.Name == "GetProxy")
            {
                requests.Add(Show(args?[0]));
                return proxy;
            }
            return Default(method.ReturnType);
        }, () => "automation-value[" + string.Join(";", requests) + "]");
    }

    private static object CreateValueProxy(Type type, string kind, string? text = null, Array? units = null)
    {
        object? scalar = kind == "scalar" ? CreateScalar(ProxyType(type, "IScalarValueProxy"), 2.5, -4.25, units ?? Array.CreateInstance(ProxyType(type, "UnitPower"), 0)) : null;
        object? stringProxy = kind == "string" ? CreateProxy(ProxyType(type, "IStringValueProxy"), (method, _) => method.Name == "get_String" ? text : Default(method.ReturnType), () => "string-proxy") : null;
        object? function = kind == "function" ? CreateProxy(ProxyType(type, "IFunctionValueProxy"), (method, _) => Default(method.ReturnType),
            () => text != null && text.StartsWith("Raw:", StringComparison.Ordinal) ? text[4..] : $"{{ValueProxy for {text}}}") : null;
        object? symbolic = kind == "symbolic" ? CreateProxy(ProxyType(type, "ISymbolicValueProxy"), (method, _) => method.Name == "get_Xml" ? text : Default(method.ReturnType), () => "symbolic-proxy") : null;
        units ??= Array.CreateInstance(ProxyType(type, "UnitPower"), 0);
        return CreateProxy(type, (method, _) => method.Name switch
        {
            "get_AsScalar" => scalar,
            "get_AsString" => stringProxy,
            "get_AsFunction" => function,
            "get_AsSymbolic" => symbolic,
            "get_AsMatrix" => null,
            "get_Units" => units,
            "get_Xml" when kind == "symbolic" => text,
            _ => Default(method.ReturnType),
        }, () => $"value-proxy({kind};{text};{Show(units)})");
    }

    private static Type ProxyType(Type valueProxyType, string name)
    {
        if (valueProxyType.Assembly == typeof(IValueProxy).Assembly)
            return name switch
            {
                "IScalarValueProxy" => typeof(IScalarValueProxy),
                "IStringValueProxy" => typeof(IStringValueProxy),
                "IFunctionValueProxy" => typeof(IFunctionValueProxy),
                "ISymbolicValueProxy" => typeof(ISymbolicValueProxy),
                "UnitPower" => typeof(global::Quire.Automation.UnitPower),
                _ => throw new ArgumentException(name),
            };
        return Mathcad.Type("automationmgd", "Mathcad.Automation." + name);
    }

    private static StubProxy CreateFactory(Type type)
    {
        List<string> calls = [];
        return (StubProxy)CreateProxy(type, (method, args) =>
        {
            string call = method.Name + "(" + string.Join(",", (args ?? []).Select(Show)) + ")";
            calls.Add(call);
            if (method.Name == "CreateUnitedValueResultNode")
            {
                string? value = null;
                List<string> units = [];
                return CreateProxy(method.ReturnType, (unitedMethod, unitedArgs) => unitedMethod.Name switch
                {
                    "set_Value" => SetValue(unitedArgs),
                    "get_Value" => null,
                    "AddUnit" => AddUnit(unitedArgs),
                    _ => Default(unitedMethod.ReturnType),
                }, () => $"united(value={value ?? "null"};units=[{string.Join(";", units)}])");

                object? SetValue(object?[]? values)
                {
                    value = Show(values?[0]);
                    return null;
                }

                object? AddUnit(object?[]? values)
                {
                    units.Add($"{values?[0]}^{values?[1]}/{values?[2]}");
                    return null;
                }
            }
            if (method.ReturnType.IsInterface)
                return CreateNodeProxy(method.ReturnType, call);
            return Default(method.ReturnType);
        }, () => "factory[" + string.Join(";", calls) + "]");
    }

    private static object CreateNodeProxy(Type type, string value) => CreateProxy(type, (method, _) => Default(method.ReturnType), () => value);

    private static object CreateProxy(Type interfaceType, Func<MethodInfo, object?[]?, object?> handler, Func<string> render)
    {
        MethodInfo create = typeof(DispatchProxy).GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Single(method => method.Name == "Create" && method.IsGenericMethodDefinition
                && method.GetGenericArguments().Length == 2 && method.GetParameters().Length == 0)
            .MakeGenericMethod(interfaceType, typeof(StubProxy));
        var proxy = (StubProxy)create.Invoke(null, null)!;
        proxy.Handler = handler;
        proxy.Render = render;
        return proxy;
    }

    private static Array CreateUnits(Type unitPowerType, Type rationalType,
        (string Name, int Numerator, int Denominator)[] values)
    {
        Array result = Array.CreateInstance(unitPowerType, values.Length);
        FieldInfo nameField = unitPowerType.GetField("Name", Instance)!;
        FieldInfo powerField = unitPowerType.GetField("Power", Instance)!;
        ConstructorInfo rationalCtor = rationalType.GetConstructor([typeof(int), typeof(int)])!;
        for (int i = 0; i < values.Length; i++)
        {
            var value = values[i];
            object power = rationalCtor.Invoke([value.Numerator, value.Denominator]);
            object unit = Activator.CreateInstance(unitPowerType)!;
            nameField.SetValue(unit, value.Name);
            powerField.SetValue(unit, power);
            result.SetValue(unit, i);
        }
        return result;
    }

    private static object Construct(Type type, StubProxy factory) =>
        type.GetConstructor(Instance, null, [type == Q ? QFactory : MFactory], null)!.Invoke([factory]);

    private static MethodInfo Get(string name, Type[] parameters, bool isStatic = false, bool original = false)
    {
        Type type = original ? M : Q;
        return type.GetMethod(name, Declared, null, parameters, null)
            ?? throw new MissingMethodException(type.FullName, name);
    }

    private static string State(object target)
    {
        object? factory = target.GetType().GetField("_resultNodeFactory", Instance)!.GetValue(target);
        object? simplify = target.GetType().GetProperty("SimplifyUnits", Instance)!.GetValue(target);
        return $"factory={Show(factory)};simplify={Show(simplify)}";
    }

    private static object? Default(Type type) => type == typeof(void) ? null : type.IsValueType ? Activator.CreateInstance(type) : null;

    private static string Show(object? value)
    {
        if (value == null) return "null";
        if (value is StubProxy proxy) return proxy.Render?.Invoke() ?? "proxy";
        if (value.GetType().Name == nameof(ValueToIResultNodeTransform)) return State(value);
        if (value is Array array) return "[" + string.Join(";", array.Cast<object?>().Select(Show)) + "]";
        if (value is Enum e) return Convert.ToUInt64(e).ToString();
        Type type = value.GetType();
        if (type.Name == "UnitPower")
        {
            string name = (string)type.GetField("Name", Instance)!.GetValue(value)!;
            object power = type.GetField("Power", Instance)!.GetValue(value)!;
            Type rational = power.GetType();
            int numerator = (int)rational.GetField("Numerator", Instance)!.GetValue(power)!;
            int denominator = (int)rational.GetField("Denominator", Instance)!.GetValue(power)!;
            return $"unit({name}^{numerator}/{denominator})";
        }
        return Reflect.Text(value);
    }

    public class StubProxy : DispatchProxy
    {
        public Func<MethodInfo, object?[]?, object?>? Handler { get; set; }
        public Func<string>? Render { get; set; }
        protected override object? Invoke(MethodInfo? targetMethod, object?[]? args) => Handler!(targetMethod!, args);
        public override string ToString() => Render?.Invoke() ?? "proxy";
    }
}
