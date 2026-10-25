#nullable disable
using Quire.Mirror;

namespace Quire.Controls.Core;

internal partial class PropertiesChangeInfo<TSource> where TSource : class
{
    [MirrorVerified("PtcControlsCorePropertiesChangeInfo1MechanicalDifferentialTests")]
    public string[] PropertyNames { get; protected set; }

    [MirrorVerified("PtcControlsCorePropertiesChangeInfo1MechanicalDifferentialTests")]
    public TSource PropertiesSource { get; protected set; }

    public PropertiesChangeInfo(string[] propertyNames, TSource propertySource)
    {
        PropertyNames = propertyNames;
        PropertiesSource = propertySource;
    }

    public virtual bool IsValid()
    {
        if (PropertyNames != null)
        {
            return PropertiesSource != null;
        }
        return false;
    }
}
