#nullable disable
using System.ComponentModel;
using Quire.Mirror;

namespace Quire.Controls.Core.Serialization;

public partial class Property
{
    [MirrorVerified("PtcControlsCoreSerializationPropertyMechanicalDifferentialTests")]
    protected internal PropertyDescriptor Descriptor { get; set; }

    [MirrorVerified("PtcControlsCoreSerializationPropertyMechanicalDifferentialTests")]
    protected internal TypeConverter Converter { get; set; }

    [MirrorVerified("PtcControlsCoreSerializationPropertyMechanicalDifferentialTests")]
    public bool UseDependencyPropertyDefaultValue { get; set; }

    [MirrorVerified("PtcControlsCoreSerializationPropertyMechanicalDifferentialTests")]
    public bool DontResetValue { get; set; }
}
