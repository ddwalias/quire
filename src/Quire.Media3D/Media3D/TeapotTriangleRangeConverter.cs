#nullable disable
using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;

namespace Quire.Media3D;

public partial class TeapotTriangleRangeConverter : TypeConverter
{
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    {
        if (sourceType == typeof(string))
        {
            return true;
        }
        return base.CanConvertFrom(context, sourceType);
    }

    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
    {
        if (destinationType == typeof(string))
        {
            return true;
        }
        return base.CanConvertTo(context, destinationType);
    }

    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
        if (value.GetType() != typeof(string))
        {
            return base.ConvertFrom(context, culture, value);
        }
        string strA = (value as string).Trim();
        PropertyInfo[] properties = typeof(TeapotTriangleRange).GetProperties(BindingFlags.Static | BindingFlags.Public);
        foreach (PropertyInfo propertyInfo in properties)
        {
            if (propertyInfo.PropertyType == typeof(TeapotTriangleRange) && string.Compare(strA, propertyInfo.Name, ignoreCase: true) == 0)
            {
                return propertyInfo.GetValue(null, null);
            }
        }
        return TeapotTriangleRange.Parse(value as string);
    }

    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
    {
        if (destinationType == typeof(string))
        {
            return base.ConvertTo(context, culture, value, destinationType);
        }
        return value.ToString();
    }

    public TeapotTriangleRangeConverter()
    {
    }
}
