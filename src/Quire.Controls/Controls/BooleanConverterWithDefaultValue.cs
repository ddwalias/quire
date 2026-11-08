#nullable disable
using Quire.Mirror;

namespace Quire.Controls;

internal partial class BooleanConverterWithDefaultValue
{
    [MirrorVerified("PtcControlsBooleanConverterWithDefaultValueMechanicalDifferentialTests")]
    private string AttributeName { get; }

    [MirrorVerified("PtcControlsBooleanConverterWithDefaultValueMechanicalDifferentialTests")]
    private Dictionary<string, object> AttributeNameToDefaultValueDictionary { get; }

    public BooleanConverterWithDefaultValue(string attributeName, Dictionary<string, object> attributeNameToDefaultValueDictionary)
    {
        AttributeName = attributeName;
        AttributeNameToDefaultValueDictionary = attributeNameToDefaultValueDictionary;
    }

    public override object ConvertTo(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
    {
        if (destinationType == typeof(string) && AttributeNameToDefaultValueDictionary.ContainsKey(AttributeName) && object.Equals(value, AttributeNameToDefaultValueDictionary[AttributeName]))
        {
            return null;
        }
        return base.ConvertTo(context, culture, value, destinationType);
    }
}
