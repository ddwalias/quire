#nullable disable
using System.Collections;
using System.Windows;
using Quire;
using Quire.Mirror;

namespace Quire.Controls.Core.AttachedBehavior;

public partial class ChildPropertyValueFinders : PropertyValueCollection
{
    protected override void OnLoaded(FrameworkElement element)
    {
        ForEachDictionary((ResourceDictionary dictionary) =>
        {
            InitializeValues(element, dictionary);
        });
    }

    private static void InitializeValues(FrameworkElement element, ResourceDictionary dictionary)
    {
        foreach (DictionaryEntry item in dictionary)
        {
            if (item.Value is PropertyValueFinder propertyValueFinder)
            {
                propertyValueFinder.InitializeValue(element, item.Key);
            }
            else
            {
                DebugTools.DevAssert(false, string.Format("ChildPropertyValueFinders contains an object that is not PropertyValueFinder but {0}", (item.Value == null) ? "null" : item.Value.GetType().Name));
            }
        }
    }

    [MirrorVerified("PtcControlsCoreAttachedBehaviorChildPropertyValueFindersMechanicalDifferentialTests")]
    public ChildPropertyValueFinders() { }
}
