#nullable disable
using System.ComponentModel;
using Quire.Mirror;
using Quire.Mirror;

namespace Quire;

public partial class Discrete2DCoordinateConverter
{
    [MirrorVerified("PtcDiscrete2DCoordinateConverterMechanicalDifferentialTests")]
    public Discrete2DCoordinateConverter() { }

    [MirrorVerified("Release IL")]
    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) => destinationType == typeof(string);
}
