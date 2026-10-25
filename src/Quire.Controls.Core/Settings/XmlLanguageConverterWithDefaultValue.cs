#nullable disable
using System;
using System.ComponentModel;
using System.Globalization;
using Quire.Mirror;

namespace Quire.Controls.Core.Settings;

internal partial class XmlLanguageConverterWithDefaultValue
{
    internal string AttributeName { get; }

    private string AttributeDefaultValue { get; }

    public XmlLanguageConverterWithDefaultValue(string attributeName, object attributeDefaultValue)
    {
        AttributeName = attributeName;
        AttributeDefaultValue = attributeDefaultValue?.ToString();
    }

    public override object ConvertTo(ITypeDescriptorContext typeDescriptorContext, CultureInfo cultureInfo, object value, Type destinationType)
    {
        if (!(destinationType == typeof(string)) || !string.Equals(value?.ToString(), AttributeDefaultValue, StringComparison.OrdinalIgnoreCase))
        {
            return base.ConvertTo(typeDescriptorContext, cultureInfo, value, destinationType);
        }
        return null;
    }
}
