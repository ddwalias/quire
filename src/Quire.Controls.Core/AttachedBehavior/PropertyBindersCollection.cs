#nullable disable
using System.Windows;
using Quire.Mirror;

namespace Quire.Controls.Core.AttachedBehavior;

public partial class PropertyBindersCollection
{
    [MirrorVerified("PtcControlsCoreAttachedBehaviorPropertyBindersCollectionMechanicalDifferentialTests")]
    public PropertyBindersCollection() { }

    public void Attach(FrameworkElement element)
    {
        foreach (PropertiesBinder item in this)
        {
            item.Attach(element);
        }
    }

    public void Detach(FrameworkElement element)
    {
        foreach (PropertiesBinder item in this)
        {
            item.Detach(element);
        }
    }
}
