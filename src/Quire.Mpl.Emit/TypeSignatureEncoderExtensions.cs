using System;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

namespace System.Reflection.Emit.Quire.Mpl;

public static partial class TypeSignatureEncoderExtensions
{
    public static void FromType(this SignatureTypeEncoder encoder, Type type, TypeBuilder typeBuilder = null, TypeDefinitionHandle? typeHandle = null)
    {
        Type type2;
        TypeDefinitionHandle? nullTypeHandle;
        if (type.IsArray && type.GetArrayRank() == 1)
        {
            type2 = type.GetElementType() ?? throw new InvalidOperationException("Operand must be not null");
            encoder.SZArray().FromType(type2, null, (nullTypeHandle = default));
            return;
        }
        if (type.GetType() != typeof(TypeBuilder) && type.IsGenericParameter)
        {
            encoder.GenericMethodTypeParameter((byte)type.GenericParameterPosition);
            return;
        }
        if (type == typeof(int))
        {
            encoder.PrimitiveType(PrimitiveTypeCode.Int32);
            return;
        }
        if (type == typeof(uint))
        {
            encoder.PrimitiveType(PrimitiveTypeCode.UInt32);
            return;
        }
        if (type == typeof(string))
        {
            encoder.PrimitiveType(PrimitiveTypeCode.String);
            return;
        }
        if (type == typeof(bool))
        {
            encoder.PrimitiveType(PrimitiveTypeCode.Boolean);
            return;
        }
        if (type == typeof(void))
        {
            encoder.PrimitiveType(PrimitiveTypeCode.Void);
            return;
        }
        if (type == typeof(object))
        {
            encoder.PrimitiveType(PrimitiveTypeCode.Object);
            return;
        }
        if (type == typeof(float))
        {
            encoder.PrimitiveType(PrimitiveTypeCode.Single);
            return;
        }
        if (type == typeof(double))
        {
            encoder.PrimitiveType(PrimitiveTypeCode.Double);
            return;
        }
        if (type == typeof(long))
        {
            encoder.PrimitiveType(PrimitiveTypeCode.Int64);
            return;
        }
        if (type == typeof(short))
        {
            encoder.PrimitiveType(PrimitiveTypeCode.Int16);
            return;
        }
        if (type == typeof(byte))
        {
            encoder.PrimitiveType(PrimitiveTypeCode.Byte);
            return;
        }
        if (type == typeof(char))
        {
            encoder.PrimitiveType(PrimitiveTypeCode.Char);
            return;
        }
        if (type == typeof(nint))
        {
            encoder.PrimitiveType(PrimitiveTypeCode.IntPtr);
            return;
        }
        AssemblyName resolvedAssemblyName = AssemblyNameResolver.GetResolvedAssemblyName(type);
        if (typeBuilder != null && resolvedAssemblyName == typeBuilder.Assembly.GetName())
        {
            ModuleBuilder module = typeBuilder.Module;
            EntityWrappers.TypeReferenceWrapper key = new EntityWrappers.TypeReferenceWrapper(type);
            if (module._definedTypes.ContainsKey(key))
            {
                encoder.Type(module._definedTypes[key], isValueType: false);
            }
            else
            {
                throw new InvalidOperationException("Type " + type.Name + " should be defined in current assembly");
            }
        }
        else
        {
            TypeReferenceHandle orAddTypeReference = (MetadataReferenceTracker.GetInstance() ?? throw new ArgumentNullException("Object is null.")).GetOrAddTypeReference(type);
            encoder.Type(orAddTypeReference, isValueType: false);
        }
    }
}
