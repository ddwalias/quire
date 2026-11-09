using System.Windows.Input;
using Quire.FunctionalitiesLimitation;
using Quire.Wpf;

namespace Quire.Controls.InputControls;

public static partial class EditableComboBoxCommands
{
    public static readonly RoutedUICommand OpenEditingMode = new RoutedUICommandWithFunctionalityKey(
        "Edit ComboBox Control",
        "EditComboBoxControl",
        typeof(ComboBoxControl),
        EnumFunctionalityKey.OpenComboBoxEditingModeCommand);

    public static readonly RoutedUICommand CloseEditingMode = new RoutedUICommand();
}
