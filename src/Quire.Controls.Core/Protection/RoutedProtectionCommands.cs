#nullable disable
using System.Windows.Input;
using Quire.FunctionalitiesLimitation;
using Quire.Wpf;

namespace Quire.Controls.Core.Protection;

public static partial class RoutedProtectionCommands
{
    public static readonly RoutedUICommand ToggleProtectState = new RoutedUICommandWithFunctionalityKey("ToggleProtectState", "Toggle Protect State", typeof(RoutedProtectionCommands), EnumFunctionalityKey.ToggleProtectStateCommand);
}
