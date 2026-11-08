#nullable disable
using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Media;

namespace Quire.Controls;

internal partial class ColorNullableConverter
{
    public ColorNullableConverter()
        : base(typeof(Color?))
    {
    }

    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
    {
        if (destinationType == typeof(string) && value == null)
        {
            return null;
        }
        return base.ConvertTo(context, culture, value, destinationType);
    }
}
