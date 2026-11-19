using System;
using System.Windows;
using Quire.FunctionalitiesLimitation;
using Quire.Services;

namespace Quire.Behaviors;

public partial class UIFunctionalityWithoutCommandAttachedBehavior
{
    private EnumFunctionalityBehaviorKey _uiElementBehaviorKey;

    public EnumFunctionalityKey FunctionalityKey
    {
        get => (EnumFunctionalityKey)GetValue(FunctionalityKeyProperty);
        set => SetValue(FunctionalityKeyProperty, value);
    }

    private void OnFunctionalityServicePropertyChanged(object sender, EventArgs eventArgs)
    {
        EnumFunctionalityBehaviorKey functionalityBehaviorKey = FunctionalityServiceWrapper.Instance.GetFunctionalityBehaviorKey(FunctionalityKey);
        if (functionalityBehaviorKey != _uiElementBehaviorKey)
        {
            SetAndUpdateElementBehavior(functionalityBehaviorKey);
        }
    }

    public UIFunctionalityWithoutCommandAttachedBehavior() { }
}
