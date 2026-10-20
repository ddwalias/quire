using System.Globalization;
using System.Reflection;

namespace System.Reflection.Emit.Quire.Mpl;

public partial class FieldBuilder
{
    private Type _declaringType;

    public override Module Module { get; }
    public override FieldAttributes Attributes { get; }
    public override RuntimeFieldHandle FieldHandle => throw new NotImplementedException();
    public override Type FieldType { get; }
    public override Type DeclaringType => _declaringType;
    public override string Name { get; }
    public override Type ReflectedType => throw new NotImplementedException();

    public FieldBuilder(Module module, Assembly assembly, string fieldName, Type type, FieldAttributes attributes, TypeBuilder declaringType)
    {
        Name = fieldName;
        Attributes = attributes;
        FieldType = type;
        _declaringType = declaringType;
        Module = declaringType.Module;
    }

    public override object[] GetCustomAttributes(bool inherit)
    {
        throw new NotImplementedException();
    }

    public override object[] GetCustomAttributes(Type attributeType, bool inherit)
    {
        throw new NotImplementedException();
    }

    public override object GetValue(object obj)
    {
        throw new NotImplementedException();
    }

    public override bool IsDefined(Type attributeType, bool inherit)
    {
        throw new NotImplementedException();
    }

    public override void SetValue(object obj, object value, BindingFlags invokeAttr, Binder binder, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
