#nullable disable
using System;
using System.ComponentModel;
using System.Globalization;
using Quire.Calculation.Core;

namespace Quire.Controls.Core.Settings;

internal partial class UnitSystemsConverter
{
    public UnitSystemsConverter()
        : base(typeof(UnitSystems))
    {
    }

    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
    {
        return ((value is UnitSystems) ? (UnitSystems)value : UnitSystems.SI).ToString().ToLowerInvariant();
    }

    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
        if (value is string && (string.Compare(value as string, "mks", true) == 0 || string.Compare(value as string, "none", true) == 0))
        {
            return UnitSystems.SI;
        }
        return base.ConvertFrom(context, culture, value);
    }
}
