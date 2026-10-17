using System.Globalization;
using System.Reflection;
using Quire.Mpl.Run;
using static Quire.Differential.Tests.MplRunSides;

namespace Quire.Differential.Tests;

public class PrimitiveInitializerDifferentialTests
{
    private const string Contract = "Mpl.Run.Primitive";
    private const BindingFlags Instance = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

    [SkippableFact]
    public void Primitive_type_initializer_matches()
    {
        Runtime.Sync();
        System.Type quireType = typeof(Primitive);
        System.Type originalType = M("Mpl.Run.Primitive");
        MethodInfo quireCall = quireType.GetMethod("Call", [typeof(int), typeof(object), typeof(object)])!;
        MethodInfo originalCall = originalType.GetMethod("Call", [typeof(int), typeof(object), typeof(object)])!;
        MethodComparison.Compare("Primitive initializer: first table call", Contract,
            quireCall, null, [1, "left", "right"], originalCall, null, [1, "left", "right"],
            Reflect.Text, Reflect.Text);
        MethodComparison.Compare("Primitive.cctor: static table", Contract,
            quireType.TypeInitializer!, null, [], originalType.TypeInitializer!, null, [],
            _ => State(quireType), _ => State(originalType));
    }

    private static string State(object type)
    {
        var seen = new Dictionary<object, int>(ReferenceEqualityComparer.Instance);
        return "m_stelem_vec=" + Describe(Reflect.Get(type, "m_stelem_vec"), seen)
            + "\nm_primitive=" + Describe(Reflect.Get(type, "m_primitive"), seen);
    }

    private static string Describe(object? value, Dictionary<object, int> seen)
    {
        if (value == null) return "null";
        if (value is string text) return "\"" + text + "\"";
        if (value is bool or byte or short or int or long or uint or ulong or float or double or decimal)
            return Convert.ToString(value, CultureInfo.InvariantCulture)!;
        if (value.GetType().Name == "OpCode")
            return "opcode:" + Reflect.Get(value, "Name") + ":" + Convert.ToInt16(Reflect.Get(value, "Value"), CultureInfo.InvariantCulture).ToString("x4", CultureInfo.InvariantCulture);
        if (value is System.Type reflectedType) return TypeName(reflectedType);
        if (value is MethodInfo method)
            return "method:" + TypeName(method.DeclaringType!) + "." + method.Name + "("
                + string.Join(",", method.GetParameters().Select(p => TypeName(p.ParameterType))) + ")->"
                + TypeName(method.ReturnType);
        if (value is FieldInfo field)
            return "field:" + TypeName(field.DeclaringType!) + "." + field.Name + ":" + TypeName(field.FieldType);
        if (value is ConstructorInfo constructor)
            return "constructor:" + TypeName(constructor.DeclaringType!) + "("
                + string.Join(",", constructor.GetParameters().Select(p => TypeName(p.ParameterType))) + ")";
        if (value.GetType().IsEnum) return value.GetType().Name + "." + value;
        if (seen.TryGetValue(value, out int existing)) return "ref#" + existing.ToString(CultureInfo.InvariantCulture);
        int id = seen.Count;
        seen.Add(value, id);
        if (value is Array array)
            return "array#" + id.ToString(CultureInfo.InvariantCulture) + "<" + TypeName(array.GetType()) + ">["
                + string.Join(",", array.Cast<object?>().Select(item => Describe(item, seen))) + "]";

        System.Type type = value.GetType();
        var fields = new List<FieldInfo>();
        for (System.Type? current = type; current != null && Mirrored(current); current = current.BaseType)
            fields.AddRange(current.GetFields(Instance | BindingFlags.DeclaredOnly));
        fields.Sort((left, right) => string.CompareOrdinal(left.Name, right.Name));
        return TypeName(type) + "#" + id.ToString(CultureInfo.InvariantCulture) + "{"
            + string.Join(",", fields.Select(f => f.Name + "=" + Describe(f.GetValue(value), seen))) + "}";
    }

    private static bool Mirrored(System.Type type) => type.Namespace is "Mpl.Run" or "Quire.Mpl.Run" or "Mcd" or "Quire.Mcd" or "Mpl" or "Quire.Mpl";

    private static string TypeName(System.Type type)
    {
        if (type.IsArray) return TypeName(type.GetElementType()!) + "[]";
        string name = type.FullName ?? type.Name;
        return name.StartsWith("Quire.", StringComparison.Ordinal) ? name[6..] : name;
    }
}
