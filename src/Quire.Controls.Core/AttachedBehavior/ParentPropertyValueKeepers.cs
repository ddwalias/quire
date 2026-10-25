#nullable disable
using System.Windows;
using Quire.Mirror;

namespace Quire.Controls.Core.AttachedBehavior;

public partial class ParentPropertyValueKeepers : PropertyValueCollection
{
    protected override void OnLoaded(FrameworkElement element)
    {
        ForEachDictionary((ResourceDictionary dictionary) =>
        {
            foreach (PropertyValueKeeper value in dictionary.Values)
            {
                value.SetBinding(element);
            }
        });
        AddValuesToResources(element);
    }

    protected override void OnUnloaded(FrameworkElement element)
    {
        ForEachDictionary((ResourceDictionary dictionary) =>
        {
            foreach (PropertyValueKeeper value in dictionary.Values)
            {
                value.RemoveBinding(element);
            }
        });
        RemoveValuesFromResources(element);
    }

    [MirrorVerified("PtcControlsCoreAttachedBehaviorParentPropertyValueKeepersMechanicalDifferentialTests")]
    public ParentPropertyValueKeepers() { }
}
