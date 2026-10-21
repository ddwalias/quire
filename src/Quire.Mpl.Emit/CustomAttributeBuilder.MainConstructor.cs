using System;
using System.IO;
using System.Reflection;

namespace System.Reflection.Emit.Quire.Mpl;

public partial class CustomAttributeBuilder
{
    public CustomAttributeBuilder(ConstructorInfo con, object?[] constructorArgs, PropertyInfo[] namedProperties, object?[] propertyValues, FieldInfo[] namedFields, object?[] fieldValues)
    {
        ArgumentNullException.ThrowIfNull(con, "con");
        ArgumentNullException.ThrowIfNull(constructorArgs, "constructorArgs");
        ArgumentNullException.ThrowIfNull(namedProperties, "namedProperties");
        ArgumentNullException.ThrowIfNull(propertyValues, "propertyValues");
        ArgumentNullException.ThrowIfNull(namedFields, "namedFields");
        ArgumentNullException.ThrowIfNull(fieldValues, "fieldValues");
        if (namedProperties.Length != propertyValues.Length)
        {
            throw new ArgumentException("namedProperties and propertyValues should have the same length.");
        }
        if (namedFields.Length != fieldValues.Length)
        {
            throw new ArgumentException("namedFields and fieldValues should have the same length.");
        }
        if ((con.Attributes & MethodAttributes.Static) == MethodAttributes.Static || (con.Attributes & MethodAttributes.MemberAccessMask) == MethodAttributes.Private)
        {
            throw new ArgumentException("The passed-in constructor is either static or private");
        }
        if ((con.CallingConvention & CallingConventions.Standard) != CallingConventions.Standard)
        {
            throw new ArgumentException("Non standard calling convention for constructor.");
        }
        _constructorInfo = con;
        _constructorArgs = new object[constructorArgs.Length];
        Array.Copy(constructorArgs, _constructorArgs, constructorArgs.Length);
        Type[] array = Array.ConvertAll(con.GetParameters(), (ParameterInfo parameter) => parameter.ParameterType);
        int num;
        if (array.Length != constructorArgs.Length)
        {
            throw new ArgumentException("Bad Parameter Count For Constructor");
        }
        for (num = 0; num < array.Length; num++)
        {
            if (!ValidateType(array[num]))
            {
                throw new ArgumentException("Bad Type In Custom Attribute: " + array[num]);
            }
        }
        for (num = 0; num < array.Length; num++)
        {
            object obj = constructorArgs[num];
            if (obj == null)
            {
                if (array[num].IsValueType)
                {
                    throw new ArgumentNullException($"{"constructorArgs"}[{num}]");
                }
            }
            else
            {
                VerifyTypeAndPassedObjectType(array[num], obj.GetType(), $"{"constructorArgs"}[{num}]");
            }
        }
        BinaryWriter binaryWriter = new BinaryWriter(new MemoryStream());
        binaryWriter.Write((ushort)1);
        for (num = 0; num < constructorArgs.Length; num++)
        {
            EmitValue(binaryWriter, array[num], constructorArgs[num]);
        }
        binaryWriter.Write((ushort)(namedProperties.Length + namedFields.Length));
        for (num = 0; num < namedProperties.Length; num++)
        {
            PropertyInfo obj2 = namedProperties[num];
            if (obj2 == null)
            {
                throw new ArgumentNullException("namedProperties[" + num + "]");
            }
            Type propertyType = obj2.PropertyType;
            object obj3 = propertyValues[num];
            if (obj3 == null && propertyType.IsValueType)
            {
                throw new ArgumentNullException("propertyValues[" + num + "]");
            }
            if (!ValidateType(propertyType))
            {
                throw new ArgumentException("Bad Type In Custom Attribute");
            }
            if (!obj2.CanWrite)
            {
                throw new ArgumentException("Not A Writable Property");
            }
            if (obj3 != null)
            {
                VerifyTypeAndPassedObjectType(propertyType, obj3.GetType(), $"{"propertyValues"}[{num}]");
            }
            binaryWriter.Write((byte)84);
            EmitType(binaryWriter, propertyType);
            EmitString(binaryWriter, namedProperties[num].Name);
            EmitValue(binaryWriter, propertyType, obj3);
        }
        for (num = 0; num < namedFields.Length; num++)
        {
            FieldInfo fieldInfo = namedFields[num];
            if (fieldInfo == null)
            {
                throw new ArgumentNullException("namedFields[" + num + "]");
            }
            Type fieldType = fieldInfo.FieldType;
            object obj4 = fieldValues[num];
            if (obj4 == null && fieldType.IsValueType)
            {
                throw new ArgumentNullException("fieldValues[" + num + "]");
            }
            if (!ValidateType(fieldType))
            {
                throw new ArgumentException("Bad Type In Custom Attribute");
            }
            if (obj4 != null)
            {
                VerifyTypeAndPassedObjectType(fieldType, obj4.GetType(), $"{"fieldValues"}[{num}]");
            }
            binaryWriter.Write((byte)83);
            EmitType(binaryWriter, fieldType);
            EmitString(binaryWriter, fieldInfo.Name);
            EmitValue(binaryWriter, fieldType, obj4);
        }
        _blob = ((MemoryStream)binaryWriter.BaseStream).ToArray();
    }
}
