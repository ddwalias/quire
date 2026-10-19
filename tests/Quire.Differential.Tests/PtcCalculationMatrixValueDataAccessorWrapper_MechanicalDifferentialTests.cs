using System.Collections;
using System.Reflection;
using System.Runtime.CompilerServices;
using Quire.Automation;
using Quire.Calculation;

namespace Quire.Differential.Tests;

/// <summary>Direct Mathcad comparisons for MatrixValueDataAccessorWrapper's property and forwarding methods.</summary>
public class PtcCalculationMatrixValueDataAccessorWrapper_MechanicalDifferentialTests
{
    private const string Contract = "Ptc.Calculation.MatrixValueDataAccessorWrapper";
    private static Type Q => typeof(IValueToIResultNodeTransform).Assembly.GetType("Quire.Calculation.MatrixValueDataAccessorWrapper", true)!;
    private static Type M => Mathcad.Type("Ptc.Calculation", Contract);
    private static Type QAccessor => typeof(IMatrixValueDataAccessor);
    private static Type MAccessor => Mathcad.Type("automationmgd", "Mathcad.Automation.IMatrixValueDataAccessor");

    [SkippableFact]
    public void Constructor_and_AccessorProxy_accessors_06000078_06000079_0600007a()
    {
        Mathcad.Require();
        object qAccessor = CreateAccessorProxy(QAccessor, "initial");
        object mAccessor = CreateAccessorProxy(MAccessor, "initial");
        ConstructorInfo qConstructor = Q.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
            null, [QAccessor], null)!;
        ConstructorInfo mConstructor = M.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
            null, [MAccessor], null)!;
        MethodComparison.Compare("MatrixValueDataAccessorWrapper .ctor", Contract, qConstructor, null, [qAccessor],
            mConstructor, null, [mAccessor], Show, Show);

        object q = qConstructor.Invoke([qAccessor]);
        object m = mConstructor.Invoke([mAccessor]);
        PropertyInfo qProperty = Q.GetProperty("AccessorProxy", BindingFlags.Instance | BindingFlags.NonPublic)!;
        PropertyInfo mProperty = M.GetProperty("AccessorProxy", BindingFlags.Instance | BindingFlags.NonPublic)!;
        MethodComparison.Compare("get_AccessorProxy", Contract, qProperty.GetMethod!, q, [], mProperty.GetMethod!, m, [],
            Show, Show, State, State);

        object qReplacement = CreateAccessorProxy(QAccessor, "replacement");
        object mReplacement = CreateAccessorProxy(MAccessor, "replacement");
        MethodComparison.Compare("set_AccessorProxy", Contract, qProperty.SetMethod!, q, [qReplacement],
            mProperty.SetMethod!, m, [mReplacement], Show, Show, State, State);
    }

    [SkippableFact]
    public void ForwardAccessor_methods_0600007e_0600007f_06000080_06000082()
    {
        Mathcad.Require();
        object q = CreateWrapper(Q, QAccessor, "initial");
        object m = CreateWrapper(M, MAccessor, "initial");
        CompareForwarder(q, m, "GetColumnsPlainDataValues", [1, 3]);
        CompareForwarder(q, m, "GetRowsPlainDataValues", [2, 4]);
        CompareForwarder(q, m, "GetFirstNonPlainDataColumnIndex", []);
        CompareForwarder(q, m, "GetFirstNonPlainDataRowIndex", []);
    }

    private static void CompareForwarder(object q, object m, string name, object?[] args)
    {
        MethodInfo qMethod = Q.GetMethod(name)!;
        MethodInfo mMethod = M.GetMethod(name)!;
        MethodComparison.Compare(name, Contract, qMethod, q, args, mMethod, m, args,
            Show, Show, State, State);
    }

    private static object CreateWrapper(Type wrapperType, Type accessorType, string marker)
    {
        ConstructorInfo constructor = wrapperType.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
            null, [accessorType], null)!;
        return constructor.Invoke([CreateAccessorProxy(accessorType, marker)]);
    }

    private static object CreateAccessorProxy(Type accessorType, string marker)
    {
        MethodInfo create = typeof(DispatchProxy).GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Single(method => method.Name == "Create" && method.IsGenericMethodDefinition
                && method.GetGenericArguments().Length == 2 && method.GetParameters().Length == 0)
            .MakeGenericMethod(accessorType, typeof(AccessorDispatchProxy));
        var proxy = (AccessorDispatchProxy)create.Invoke(null, null)!;
        proxy.Marker = marker;
        proxy.Handler = name => name switch
        {
            "GetColumnsPlainDataValues" => Rows("column", 1, 3),
            "GetRowsPlainDataValues" => Rows("row", 2, 4),
            "GetFirstNonPlainDataColumnIndex" => 5,
            "GetFirstNonPlainDataRowIndex" => 7,
            _ => null,
        };
        return proxy;
    }

    private static List<IList> Rows(string axis, int start, int end) =>
    [
        new ArrayList { axis, start, end },
        new ArrayList { end, start },
    ];

    private static string State(object target)
    {
        PropertyInfo property = target.GetType().GetProperty("AccessorProxy", BindingFlags.Instance | BindingFlags.NonPublic)!
            ?? FindBaseProperty(target.GetType(), "AccessorProxy");
        return "accessor=" + Show(property.GetValue(target));
    }

    private static PropertyInfo FindBaseProperty(Type type, string name)
    {
        for (Type? current = type.BaseType; current != null; current = current.BaseType)
        {
            PropertyInfo? property = current.GetProperty(name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
            if (property != null) return property;
        }

        throw new MissingMemberException(type.FullName, name);
    }

    private static string Show(object? value) => value switch
    {
        null => "null",
        _ when value.GetType().Name == "MatrixValueDataAccessorWrapper" => State(value),
        AccessorDispatchProxy proxy => "accessor:" + proxy.Marker,
        Enum e => Convert.ToUInt32(e).ToString(),
        IEnumerable sequence when value is not string => "[" + string.Join(";", sequence.Cast<object?>().Select(Show)) + "]",
        _ => Reflect.Text(value),
    };

    public class AccessorDispatchProxy : DispatchProxy
    {
        public string Marker { get; set; } = "";
        public Func<string, object?>? Handler { get; set; }

        protected override object? Invoke(MethodInfo? targetMethod, object?[]? args) => Handler!(targetMethod!.Name);
    }
}
