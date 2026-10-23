#nullable disable
#pragma warning disable

namespace Quire.FunctionalitiesLimitation;

partial class FunctionalitiesLimitationDictionary
{
    private static readonly global::Quire.FunctionalitiesLimitation.FunctionalitiesLimitationDictionary _default = new global::Quire.FunctionalitiesLimitation.FunctionalitiesLimitationDictionary();
    private bool _contentLoaded;
    public static global::Quire.FunctionalitiesLimitation.EnumFunctionalityKey GetParentFunctionalityKey(global::Quire.FunctionalitiesLimitation.EnumFunctionalityKey functionalityKey)
    {
        global::Quire.FunctionalitiesLimitation.EnumFunctionalityKey result = global::Quire.FunctionalitiesLimitation.EnumFunctionalityKey.GlobalCategory;
        global::Quire.FunctionalitiesLimitation.FunctionalityItem functionalityItem = _default[functionalityKey] as global::Quire.FunctionalitiesLimitation.FunctionalityItem;
        if (functionalityItem != null)
        {
            global::Quire.FunctionalitiesLimitation.FunctionalityItem parent = functionalityItem.Parent;
            if (parent != null)
            {
                result = parent.FunctionalityKey;
            }
        }
        else
        {
            global::Quire.DebugTools.DevAssert(false, "FunctionalityItem corresponding the functionality key given was not found in dictionary.");
        }

        return result;
    }

    public void InitializeComponent()
    {
        if (_contentLoaded)
        {
            return;
        }

        _contentLoaded = true;
        global::System.Uri resourceLocator = new("/Quire.Core;component/functionalitieslimitation/functionalitieslimitationdictionary.xaml", global::System.UriKind.Relative);
        global::System.Windows.Application.LoadComponent(this, resourceLocator);
    }

    void global::System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target)
    {
        _contentLoaded = true;
    }
}
