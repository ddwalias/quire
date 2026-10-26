#nullable disable
using System;
using System.ComponentModel;
using System.Data;
using Quire.Mirror;

namespace Quire.Controls.Core.Serialization;

public partial class Property
{
    protected internal object component;

    protected internal string _propertyName;

    [MirrorVerified("PtcControlsCoreSerializationPropertyMechanicalDifferentialTests")]
    protected internal PropertyDescriptor Descriptor { get; set; }

    [MirrorVerified("PtcControlsCoreSerializationPropertyMechanicalDifferentialTests")]
    protected internal TypeConverter Converter { get; set; }

    [MirrorVerified("PtcControlsCoreSerializationPropertyMechanicalDifferentialTests")]
    public bool UseDependencyPropertyDefaultValue { get; set; }

    [MirrorVerified("PtcControlsCoreSerializationPropertyMechanicalDifferentialTests")]
    public bool DontResetValue { get; set; }

    protected internal object PropertyValue
    {
        get
        {
            return Descriptor.GetValue(component);
        }
        set
        {
            PropertyDescriptor descriptor = Descriptor;
            if (descriptor.IsReadOnly)
            {
                throw new ReadOnlyException(string.Format("Property \"{0}\" is read only.", _propertyName));
            }
            descriptor.SetValue(component, value);
        }
    }

    protected internal string StringPropertyValue
    {
        get
        {
            return Converter.ConvertToInvariantString(PropertyValue);
        }
        set
        {
            if (value == null && Descriptor.PropertyType.IsValueType)
            {
                ResetValue();
            }
            else
            {
                PropertyValue = Converter.ConvertFromInvariantString(value);
            }
        }
    }

    protected internal Property(object o, string propertyName, TypeConverter converter)
    {
        component = o;
        _propertyName = propertyName;
        if (component != null)
        {
            Descriptor = TypeDescriptor.GetProperties(component)[_propertyName];
            Converter = converter;
        }
    }

    protected internal Property(object o, string propertyName)
    {
        component = o;
        _propertyName = propertyName;
        if (component != null)
        {
            Descriptor = TypeDescriptor.GetProperties(component)[_propertyName];
            Converter = Descriptor.Converter;
        }
    }

    protected internal bool AttemptToGetDefaultValue(out object defVal)
    {
        defVal = null;
        DefaultValueAttribute defaultValueAttribute = (DefaultValueAttribute)Descriptor.Attributes[typeof(DefaultValueAttribute)];
        if (defaultValueAttribute != null)
        {
            defVal = defaultValueAttribute.Value;
            return true;
        }
        return false;
    }

    protected void ResetValue()
    {
        if (DontResetValue)
        {
            return;
        }
        if (UseDependencyPropertyDefaultValue)
        {
            DependencyPropertyDescriptor val = DependencyPropertyDescriptor.FromProperty(Descriptor);
            if (val != null)
            {
                PropertyValue = val.Metadata.DefaultValue;
                return;
            }
            DebugTools.DevAssert(false, string.Concat(new string[6]
            {
                "Can't retrieve default DependencyProperty value for a non Dependency Property",
                Environment.NewLine,
                "Object type: ",
                component.GetType()?.ToString(),
                ", Property name: ",
                Descriptor.Name
            }));
        }
        else if (Descriptor.CanResetValue(component))
        {
            Descriptor.ResetValue(component);
        }
        else if (Descriptor.PropertyType.GetConstructor(Type.EmptyTypes) != null)
        {
            object propertyValue = Activator.CreateInstance(Descriptor.PropertyType);
            PropertyValue = propertyValue;
        }
    }
}
