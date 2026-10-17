using System.Reflection;
using Quire.Mpl.Run;
using static Quire.Differential.Tests.MplRunSides;

namespace Quire.Differential.Tests;

public class PrimitiveConstructorDifferentialTests
{
    private const string Contract = "Mpl.Run.Primitive";
    private const BindingFlags Instance = BindingFlags.NonPublic | BindingFlags.Instance;
    private const BindingFlags Static = BindingFlags.Public | BindingFlags.Static;
    private static readonly Type OpCodeType = typeof(Primitive).GetField("m_opcodes")!.FieldType.GetElementType()!;

    [SkippableFact]
    public void Private_primitive_constructors_match_field_initialization_and_references()
    {
        Type quire = typeof(Primitive);
        Type original = M("Mpl.Run.Primitive");
        object mNeg = Opcode(original, "neg_i4");
        object mAdd = Opcode(original, "add_i4");
        object qNeg = mNeg;
        object qAdd = mAdd;
        Compare(quire, original,
            [typeof(string), OpCodeType, typeof(MethodInfo)], [typeof(string), OpCodeType, typeof(MethodInfo)],
            "opcode with explicit method", "opcode", ["one", qNeg, Method(quire, "neg_i4")],
            ["one", mNeg, Method(original, "neg_i4")]);

        Array qOps = Codes(qNeg, qAdd);
        Array mOps = Codes(mNeg, mAdd);
        Compare(quire, original,
            [typeof(string), OpCodeArrayType, typeof(MethodInfo)], [typeof(string), OpCodeArrayType, typeof(MethodInfo)],
            "opcode array with explicit method", "codes", ["many", qOps, Method(quire, "identity")],
            ["many", mOps, Method(original, "identity")], qOps, mOps);

        Compare(quire, original, [typeof(string), OpCodeType], [typeof(string), OpCodeType],
            "opcode with name lookup", "opcode", ["neg_i4", qNeg], ["neg_i4", mNeg]);
        object qCode = new Code();
        object mCode = New(M("Mpl.Run.Code"), [])!;
        Compare(quire, original, [typeof(string), typeof(Code)], [typeof(string), M("Mpl.Run.Code")],
            "code with name lookup", "code", ["identity", qCode], ["identity", mCode], null, null, null, null, qCode, mCode);

        qOps = Codes(qNeg, qAdd);
        mOps = Codes(mNeg, mAdd);
        Compare(quire, original, [typeof(string), OpCodeArrayType], [typeof(string), OpCodeArrayType],
            "opcode array with name lookup", "codes", ["add_i4", qOps], ["add_i4", mOps], qOps, mOps);
        Compare(quire, original, [typeof(string), typeof(MethodInfo)], [typeof(string), typeof(MethodInfo)],
            "explicit method", "method", ["foreign-method", Method(quire, "nrt_f8")],
            ["foreign-method", Method(original, "nrt_f8")], null, null, Method(quire, "nrt_f8"), Method(original, "nrt_f8"));
        Compare(quire, original, [typeof(string)], [typeof(string)],
            "name lookup", "method", ["seq"], ["seq"]);
        Compare(quire, original, [typeof(string), typeof(string)], [typeof(string), typeof(string)],
            "foreign definition", "foreign", ["foreign-name", "[assembly]Type::Member"],
            ["foreign-name", "[assembly]Type::Member"]);
    }

    private static void Compare(Type quireType, Type originalType, Type[] quireSignature, Type[] originalSignature,
        string caseId, string kind, object?[] quireArgs, object?[] originalArgs,
        Array? quireArray = null, Array? originalArray = null,
        MethodInfo? quireMethod = null, MethodInfo? originalMethod = null,
        object? quireCode = null, object? originalCode = null)
    {
        var quireConstructor = quireType.GetConstructor(Instance, null, quireSignature, null)!;
        var originalConstructor = originalType.GetConstructor(Instance, null, originalSignature, null)!;
        MethodComparison.Compare(caseId, Contract, quireConstructor, null, quireArgs, originalConstructor, null,
            originalArgs, value => Describe(value, kind, quireArray, quireMethod, quireCode),
            value => Describe(value, kind, originalArray, originalMethod, originalCode));
    }

    private static Type OpCodeArrayType => OpCodeType.MakeArrayType();

    private static object Opcode(Type type, string name)
    {
        var primitives = (Array)Reflect.Get(type, "m_primitive")!;
        foreach (object primitive in primitives)
            if ((string)Reflect.Get(primitive, "m_name")! == name)
                return ((Array)Reflect.Get(primitive, "m_opcodes")!).GetValue(0)!;
        throw new MissingMemberException(type.Name, name);
    }

    private static Array Codes(object first, object second)
    {
        Array codes = Array.CreateInstance(OpCodeType, 2);
        codes.SetValue(first, 0);
        codes.SetValue(second, 1);
        return codes;
    }

    private static MethodInfo Method(Type type, string name) => type.GetMethod(name, Static)!;

    private static string Describe(object? value, string kind, Array? expectedArray, MethodInfo? expectedMethod, object? expectedCode)
    {
        if (value == null) return "null";
        if (ReferenceEquals(value, expectedCode)) return "code argument";
        if (value.GetType() == OpCodeType) return Reflect.Text(value);
        if (value is Array codes && codes.GetType().GetElementType() == OpCodeType)
        {
            string label = ReferenceEquals(codes, expectedArray) ? "argument" : "value";
            return label + "[" + string.Join(",", codes.Cast<object>().Select(Reflect.Text)) + "]";
        }
        if (value is MethodInfo method)
        {
            string label = ReferenceEquals(method, expectedMethod) ? "argument" : "method";
            return label + ":" + MethodName(method);
        }
        if (value.GetType().Name == "Primitive")
        {
            object primitive = value;
            object? opcodes = Reflect.Get(primitive, "m_opcodes");
            object? codeValue = Reflect.Get(primitive, "m_code");
            object? methodValue = Reflect.Get(primitive, "m_method");
            object? foreign = Reflect.Get(primitive, "m_foreign");
            string opcodeText = opcodes is Array array && array.GetType().GetElementType() == OpCodeType
                ? (ReferenceEquals(array, expectedArray) ? "argument" : "value") + "[" + string.Join(",", array.Cast<object>().Select(Reflect.Text)) + "]"
                : "null";
            string methodText = methodValue is MethodInfo mi
                ? (ReferenceEquals(mi, expectedMethod) ? "argument" : "method") + ":" + MethodName(mi)
                : "null";
            string codeText = codeValue == null ? "null" : ReferenceEquals(codeValue, expectedCode) ? "argument" : MplRunSides.Describe(codeValue);
            return $"Primitive[{Reflect.Get(primitive, "m_name")}; opcodes={opcodeText}; code={codeText}; method={methodText}; foreign={Reflect.Text(foreign)}]";
        }
        return Reflect.Text(value);
    }

    private static string MethodName(MethodInfo method) => method.Name + "(" +
        string.Join(",", method.GetParameters().Select(p => p.ParameterType.Name)) + ")->" + method.ReturnType.Name;
}
