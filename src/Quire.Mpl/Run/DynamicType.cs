#nullable disable
using System;
using System.Reflection;
using System.Reflection.Emit;
using Quire.Mirror;
using ModuleBuilder = System.Reflection.Emit.Quire.Mpl.ModuleBuilder;
using TypeBuilder = System.Reflection.Emit.Quire.Mpl.TypeBuilder;
using ConstructorBuilder = System.Reflection.Emit.Quire.Mpl.ConstructorBuilder;
using MethodBuilder = System.Reflection.Emit.Quire.Mpl.MethodBuilder;
using FieldBuilder = System.Reflection.Emit.Quire.Mpl.FieldBuilder;
using ILGenerator = System.Reflection.Emit.Quire.Mpl.ILGenerator;

namespace Quire.Mpl.Run;

public partial class DynamicType
{
    [MirrorVerified("DynamicTypeDifferentialTests")]
    public static TypeBuilder DefineTuple(ModuleBuilder mod, string tyName)
    {
        TypeBuilder typeBuilder = DefineRecord(mod, tyName, TupleNameDemangle(tyName));
        typeBuilder.CreateType();
        return typeBuilder;
    }

    [MirrorVerified("DynamicTypeDifferentialTests")]
    public static TypeBuilder DefineTuple(ModuleBuilder mod, Type[] fieldTypes)
    {
        TypeBuilder typeBuilder = DefineRecord(mod, TupleNameMangle(fieldTypes), fieldTypes);
        typeBuilder.CreateType();
        return typeBuilder;
    }

    [MirrorVerified("DynamicTypeDifferentialTests")]
    public static TypeBuilder DefineNullaryRecord(ModuleBuilder mod, string tyName, int tag, out ConstructorInfo ctor, out FieldInfo[] fields)
    {
        TypeBuilder typeBuilder = DefineRecord(mod, tyName, new Type[0], tag, out ctor, out fields);
        FieldInfo field = typeBuilder.DefineField("singleton", typeBuilder, FieldAttributes.Public | FieldAttributes.Static);
        ILGenerator iLGenerator = typeBuilder.DefineTypeInitializer().GetILGenerator();
        iLGenerator.Emit(OpCodes.Newobj, ctor);
        iLGenerator.Emit(OpCodes.Stsfld, field);
        iLGenerator.Emit(OpCodes.Ret);
        return typeBuilder;
    }

    [MirrorVerified("DynamicTypeDifferentialTests")]
    public static TypeBuilder DefineRecord(ModuleBuilder mod, string tyName, Type[] fieldTypes, int tag, out ConstructorInfo ctor, out FieldInfo[] fields)
    {
        TypeBuilder typeBuilder = ((tag < 0) ? mod.DefineType(tyName, TypeAttributes.Public) : mod.DefineType(tyName, TypeAttributes.Public, typeof(Tag)));
        fields = DefineFields(typeBuilder, fieldTypes);
        ctor = DefineConstructor(typeBuilder, tag, fields, fieldTypes);
        DefinePrinter(typeBuilder, tyName, fields, fieldTypes);
        return typeBuilder;
    }

    [MirrorVerified("DynamicTypeDifferentialTests")]
    public static TypeBuilder DefineRecord(ModuleBuilder mod, string tyName, Type[] fieldTypes, int tag)
    {
        FieldInfo[] fields;
        ConstructorInfo ctor;
        return DefineRecord(mod, tyName, fieldTypes, tag, out ctor, out fields);
    }

    [MirrorVerified("DynamicTypeDifferentialTests")]
    public static TypeBuilder DefineRecord(ModuleBuilder mod, string tyName, Type[] fieldTypes)
    {
        return DefineRecord(mod, tyName, fieldTypes, -1);
    }

    [MirrorVerified("DynamicTypeDifferentialTests")]
    public static ConstructorInfo DefineConstructor(TypeBuilder tb, int tag, FieldInfo[] fields, Type[] types)
    {
        int num = fields.Length;
        ConstructorBuilder constructorBuilder = tb.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, types);
        ILGenerator iLGenerator = constructorBuilder.GetILGenerator();
        if (tag < 0)
        {
            iLGenerator.Emit(OpCodes.Ldarg_0);
            iLGenerator.Emit(OpCodes.Call, tb.BaseType.GetConstructor(new Type[0]));
        }
        else
        {
            iLGenerator.Emit(OpCodes.Ldarg_0);
            iLGenerator.Emit(OpCodes.Ldc_I4, tag);
            iLGenerator.Emit(OpCodes.Call, tb.BaseType.GetConstructor(new Type[1] { typeof(int) }));
        }
        for (int i = 0; i < num; i++)
        {
            iLGenerator.Emit(OpCodes.Ldarg_0);
            Ldarg(iLGenerator, i + 1);
            iLGenerator.Emit(OpCodes.Stfld, fields[i]);
        }
        iLGenerator.Emit(OpCodes.Ret);
        return constructorBuilder;
    }

    [MirrorVerified("DynamicTypeDifferentialTests")]
    public static MethodInfo DefinePrinter(TypeBuilder tb, FieldInfo[] fields, Type[] types)
    {
        return DefinePrinter(tb, tb.Name, fields, types);
    }

    [MirrorVerified("DynamicTypeDifferentialTests")]
    public static MethodInfo DefinePrinter(TypeBuilder tb, string tyName, FieldInfo[] fields, Type[] types)
    {
        int num = fields.Length;
        MethodBuilder methodBuilder = tb.DefineMethod("ToString", MethodAttributes.Public | MethodAttributes.Virtual, CallingConventions.Standard, typeof(string), new Type[0]);
        ILGenerator iLGenerator = methodBuilder.GetILGenerator();
        if (num == 0)
        {
            iLGenerator.Emit(OpCodes.Ldstr, GetShortName(tyName));
            iLGenerator.Emit(OpCodes.Ret);
            return methodBuilder;
        }
        bool flag = string.Compare(tyName, 0, TuplePrefix, 0, TuplePrefix.Length) == 0;
        if (flag)
        {
            iLGenerator.Emit(OpCodes.Ldstr, "(");
        }
        else
        {
            iLGenerator.Emit(OpCodes.Ldstr, "(" + GetShortName(tyName) + " ");
        }
        for (int i = 0; i < num; i++)
        {
            iLGenerator.Emit(OpCodes.Ldarg_0);
            iLGenerator.Emit(OpCodes.Ldfld, fields[i]);
            if (types[i].IsValueType)
            {
                iLGenerator.Emit(OpCodes.Box, types[i]);
                iLGenerator.Emit(OpCodes.Ldstr, "$");
                iLGenerator.Emit(OpCodes.Call, Concat2Mi());
            }
            iLGenerator.Emit(OpCodes.Ldstr, (i < num - 1) ? (flag ? "," : " ") : ")");
            iLGenerator.Emit(OpCodes.Call, Concat3Mi());
        }
        iLGenerator.Emit(OpCodes.Ret);
        return methodBuilder;
    }

    [MirrorVerified("DynamicTypeDifferentialTests")]
    public static FieldInfo[] DefineFields(TypeBuilder tb, Type[] types)
    {
        int num = types.Length;
        FieldInfo[] array = new FieldInfo[num];
        for (int i = 0; i < num; i++)
        {
            FieldBuilder fieldBuilder = tb.DefineField(FieldName(i), types[i], FieldAttributes.Public);
            array[i] = fieldBuilder;
        }
        return array;
    }

    [MirrorVerified("DynamicTypeDifferentialTests")]
    private static string RemovePrefix(string tyName, string prefix)
    {
        int num = tyName.IndexOf(prefix);
        if (num < 0)
        {
            AppException.Raise("demangle error");
        }
        return tyName.Substring(num + prefix.Length + 1);
    }

    [MirrorVerified("DynamicTypeDifferentialTests")]
    private static Type[] NameDemangle(string tyName, string prefix)
    {
        string[] array = Explode(RemovePrefix(tyName, prefix));
        int num = array.Length;
        Type[] array2 = new Type[num];
        for (int i = 0; i < num; i++)
        {
            array2[i] = NameToType(array[i]);
        }
        return array2;
    }

    [MirrorVerified("DynamicTypeDifferentialTests")]
    public static Type[] TupleNameDemangle(string tyName)
    {
        return NameDemangle(tyName, TuplePrefix);
    }

    [MirrorVerified("DynamicTypeDifferentialTests")]
    public static void Ldarg(ILGenerator m_il, int index)
    {
        switch (index)
        {
        case 0:
            m_il.Emit(OpCodes.Ldarg_0);
            return;
        case 1:
            m_il.Emit(OpCodes.Ldarg_1);
            return;
        case 2:
            m_il.Emit(OpCodes.Ldarg_2);
            return;
        case 3:
            m_il.Emit(OpCodes.Ldarg_3);
            return;
        }
        if (index > -128 && index < 127)
        {
            m_il.Emit(OpCodes.Ldarg_S, (byte)index);
        }
        else
        {
            m_il.Emit(OpCodes.Ldarg, (short)index);
        }
    }

    [MirrorVerified("MplRunDynamicTypeMechanicalDifferentialTests")]
    public DynamicType() { }

    [MirrorVerified("DynamicTypeMplConcat3DifferentialTests")]
    public static string MplConcat3(object o1, object o2, object o3) =>
        string.Concat(o1 == null ? "null" : o1, o2 == null ? "null" : o2, o3 == null ? "null" : o3);

    [MirrorVerified("DynamicTypeConcat2MiDifferentialTests")]
    private static MethodInfo Concat2Mi()
    {
        return typeof(string).GetMethod("Concat", new Type[] { typeof(object), typeof(object) });
    }

    [MirrorVerified("DynamicTypeConcat3MiDifferentialTests")]
    private static MethodInfo Concat3Mi()
    {
        return typeof(DynamicType).GetMethod("MplConcat3");
    }

    [MirrorVerified("DynamicTypeGetShortNameDifferentialTests")]
    private static string GetShortName(string nm)
    {
        int num = nm.IndexOf(":");
        if (num > 0)
        {
            return nm.Substring(num);
        }
        if (num == 0)
        {
            return nm;
        }
        num = nm.LastIndexOf(".");
        if (num > 0)
        {
            return nm.Substring(num + 1);
        }
        return nm;
    }
}
