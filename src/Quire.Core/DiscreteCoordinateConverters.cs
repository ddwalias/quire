#nullable disable
using Quire.Mirror;

namespace Quire;

public partial class DiscretePointConverter
{
    [MirrorVerified("Release IL")]
    public override object ConvertFrom(global::System.ComponentModel.ITypeDescriptorContext context, global::System.Globalization.CultureInfo culture, object value)
    {
        Discrete2DCoordinate? coordinate = base.ConvertFrom(context, culture, value) as Discrete2DCoordinate?;
        if (coordinate.HasValue)
        {
            return new DiscretePoint(coordinate.Value.First, coordinate.Value.Second);
        }
        return null;
    }
}

public partial class TableCoordinateConverter
{
    [MirrorVerified("Release IL")]
    public override object ConvertFrom(global::System.ComponentModel.ITypeDescriptorContext context, global::System.Globalization.CultureInfo culture, object value)
    {
        Discrete2DCoordinate? coordinate = base.ConvertFrom(context, culture, value) as Discrete2DCoordinate?;
        if (coordinate.HasValue)
        {
            return new TableCoordinate(coordinate.Value.First, coordinate.Value.Second);
        }
        return null;
    }
}

public partial class TableSizeConverter
{
    [MirrorVerified("Release IL")]
    public override object ConvertFrom(global::System.ComponentModel.ITypeDescriptorContext context, global::System.Globalization.CultureInfo culture, object value)
    {
        Discrete2DCoordinate? coordinate = base.ConvertFrom(context, culture, value) as Discrete2DCoordinate?;
        if (coordinate.HasValue)
        {
            return new TableSize(coordinate.Value.First, coordinate.Value.Second);
        }
        return null;
    }
}
