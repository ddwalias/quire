#nullable disable
using System;
using System.ComponentModel;
using System.Globalization;
using Quire.Mirror;

namespace Quire.Controls.Core;

public partial class XmlBooleanConverter
{
    [MirrorVerified("PtcControlsCoreXmlBooleanConverterMechanicalDifferentialTests")]
    public XmlBooleanConverter() { }

    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
    {
        object result;
        if (destinationType == typeof(string) && value != null && value is bool)
        {
            if ((bool)value)
            {
                result = "true";
            }
            else
            {
                result = "false";
            }
        }
        else
        {
            result = base.ConvertTo(context, culture, value, destinationType);
        }
        return result;
    }
}
