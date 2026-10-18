#nullable disable
using System.ComponentModel;
using Quire.Mirror;

namespace Quire.Controls.Core.Serialization;

public partial class PropertyToAttribute
{
    [MirrorVerified("PtcControlsCoreSerializationPropertyToAttributeMechanicalDifferentialTests")]
    protected internal readonly string attributeName;

    [MirrorVerified("PtcControlsCoreSerializationPropertyToAttributeMechanicalDifferentialTests")]
    public PropertyToAttribute(object o, string propertyName, string attributeName) : base(o, propertyName)
    {
        this.attributeName = attributeName;
    }

    [MirrorVerified("PtcControlsCoreSerializationPropertyToAttributeMechanicalDifferentialTests")]
    public PropertyToAttribute(object o, string propertyName, string attributeName, TypeConverter typeConverter) : base(o, propertyName, typeConverter)
    {
        this.attributeName = attributeName;
    }
}
