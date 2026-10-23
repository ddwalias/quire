#nullable disable

namespace Quire;

public static partial class TextElementPropertiesBlocker
{
    public static void FixatePropertiesValues(global::System.Windows.DependencyObject objToFixate)
    {
        if (DebugTools.DevAssert(objToFixate != null, null).Failed)
        {
            return;
        }
        foreach (global::System.Windows.DependencyProperty property in _dependencyPropertiesToFixate)
        {
            try
            {
                objToFixate.SetValue(property, objToFixate.GetValue(property));
            }
            catch (global::System.Exception exception)
            {
                DebugTools.FailAssertion(exception.Message);
            }
        }
    }
}
