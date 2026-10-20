#nullable disable
using System.ComponentModel;
using System.Globalization;
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
