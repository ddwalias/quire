using System;
using System.Windows.Input;
using Quire.Controls.Internal;
using Quire.FunctionalitiesLimitation;
using Quire.Wpf;

namespace Quire.Controls.InputControls;

public partial class InputScriptingControlWithInputCommands
{
    private static readonly Type _typeOfThis = typeof(InputControlCommands);

    public static readonly RoutedUICommand AddScriptingInputField = new RoutedUICommandWithFunctionalityKey(
        "Add scripting input field",
        "AddScriptingInputField",
        typeof(VisualInputScriptableControlNode),
        EnumFunctionalityKey.InputScriptableControlCategory);

    public static readonly RoutedUICommand RemoveScriptingInputField = new RoutedUICommandWithFunctionalityKey(
        "Remove scripting input field",
        "RemoveScriptingInputField",
        typeof(VisualInputScriptableControlNode),
        EnumFunctionalityKey.InputScriptableControlCategory);

    public static readonly RoutedUICommand AddScriptingOutputField = new RoutedUICommandWithFunctionalityKey(
        "Add scripting output field",
        "AddScriptingOutputField",
        typeof(VisualInputScriptableControlNode),
        EnumFunctionalityKey.InputScriptableControlCategory);

    public InputScriptingControlWithInputCommands()
    {
    }
}
