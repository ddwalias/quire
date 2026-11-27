using System.Windows;
using System.Windows.Controls;
using Quire.FunctionalitiesLimitation;

namespace Quire.Behaviors;

public partial class BaseUIFunctionalityAttachedBehavior
{
    public static EnumFunctionalityBehaviorKey GetUIFunctionalityBehaviorKey(DependencyObject d)
    {
        return (EnumFunctionalityBehaviorKey)d.GetValue(UIFunctionalityBehaviorKeyProperty);
    }

    protected static void SetUIFunctionalityBehaviorKey(DependencyObject d, EnumFunctionalityBehaviorKey functionalityBehaviorKey)
    {
        d.SetValue(_uiFunctionalityBehaviorKeyPropertyKey, functionalityBehaviorKey);
    }

    protected void UpdateUIElementImageForButtonDropDown(object buttonDropDownImage, object buttonDropDownImageSmall, object buttonDropDownHeaderImage, EnumFunctionalityBehaviorKey functionalityBehaviorKey)
    {
        if (ShouldReplaceWithLockImages(functionalityBehaviorKey))
        {
            ReplaceWithAppropiateLockImageIfOriginalImageExist(_originalImageSource, buttonDropDownImage);
            ReplaceWithAppropiateLockImageIfOriginalImageExist(_originalImageSmallSource, buttonDropDownImageSmall);
            ReplaceWithAppropiateLockImageForHeaderImageIfOriginalImageExist(_originalHeaderImageSource, buttonDropDownHeaderImage);
            return;
        }

        RestoreOriginalImagesIfExist(_originalImageSource, buttonDropDownImage);
        RestoreOriginalImagesIfExist(_originalImageSmallSource, buttonDropDownImageSmall);
        RestoreOriginalImagesIfExist(_originalHeaderImageSource, buttonDropDownHeaderImage);
    }
}
