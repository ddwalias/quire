#nullable disable
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using Quire.Mirror;
using Quire.Mirror;

namespace Quire;

public partial class Discrete2DCoordinateConverter
{
    [MirrorVerified("PtcDiscrete2DCoordinateConverterMechanicalDifferentialTests")]
    public Discrete2DCoordinateConverter() { }

    [MirrorVerified("Release IL")]
    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) => destinationType == typeof(string);

    [MirrorVerified("Release IL")]
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    {
        if (!(sourceType == typeof(string)) && !(sourceType == typeof(System.Windows.Size)))
        {
            return sourceType == typeof(TableSize);
        }
        return true;
    }

    [MirrorVerified("Release IL")]
    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
        Discrete2DCoordinate discrete2DCoordinate = default;
        if (value is TableSize)
        {
            TableSize? tableSize = value as TableSize?;
            discrete2DCoordinate.First = tableSize.Value.Rows;
            discrete2DCoordinate.Second = tableSize.Value.Columns;
            return discrete2DCoordinate;
        }
        if (value is Size)
        {
            Size? val = value as Size?;
            discrete2DCoordinate.First = (int)val.Value.Height;
            discrete2DCoordinate.Second = (int)val.Value.Width;
            return discrete2DCoordinate;
        }
        if (value is string)
        {
            string text = value as string;
            if (string.IsNullOrEmpty(text))
            {
                return null;
            }
            string[] array = text.Split(',');
            if (!int.TryParse(array[0], out var result) || !int.TryParse(array[1], out var result2))
            {
                return null;
            }
            discrete2DCoordinate.First = result;
            discrete2DCoordinate.Second = result2;
            return discrete2DCoordinate;
        }
        return null;
    }

    [MirrorVerified("Release IL")]
    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
    {
        Discrete2DCoordinate discrete2DCoordinate;
        try
        {
            if (value is TableSize)
            {
                discrete2DCoordinate.First = ((TableSize)value).Rows;
                discrete2DCoordinate.Second = ((TableSize)value).Columns;
            }
            else if (value is TableCoordinate)
            {
                discrete2DCoordinate.First = ((TableCoordinate)value).Row;
                discrete2DCoordinate.Second = ((TableCoordinate)value).Column;
            }
            else
            {
                discrete2DCoordinate = (Discrete2DCoordinate)value;
            }
        }
        catch (InvalidCastException)
        {
            return null;
        }
        return discrete2DCoordinate.ToString();
    }
}
