using System.Linq;
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

    protected static Image GetImage(object value)
    {
        Image image = value as Image;
        if (image != null)
        {
            return image;
        }

        Panel panel = value as Panel;
        if (panel != null)
        {
            return panel.Children.OfType<Image>().FirstOrDefault();
        }

        return null;
    }
}
