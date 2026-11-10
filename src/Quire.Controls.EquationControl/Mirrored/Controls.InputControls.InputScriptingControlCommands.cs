using System;
using System.Windows.Input;
using Quire.EquationEditor.View;
using Quire.FunctionalitiesLimitation;
using Quire.Wpf;

namespace Quire.Controls.InputControls;

public partial class InputScriptingControlCommands
{
    private static readonly Type _typeOfThis = typeof(InputControlCommands);

    public static readonly RoutedUICommand OpenEditingMode = new RoutedUICommandWithFunctionalityKey("Edit Input Controls", "EditInputControls", typeof(IVisualInputControlNode), EnumFunctionalityKey.InputScriptableControlCategory);
}
