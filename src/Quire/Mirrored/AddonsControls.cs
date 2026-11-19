using System.Windows.Input;

namespace Quire;

public partial class AddonsControls
{
    private const string WwgmAddonId = "WWGM";

    internal AddonsControls(CommandBindingCollection commandBindings)
    {
        BindCommands(commandBindings);
    }

    private void BindCommands(CommandBindingCollection commandBindings)
    {
        commandBindings.Add(new CommandBinding(IntegrationCommands.AutoCheckIn, HandleAutoCheckIn, CanExecuteHandleAutoCheckIn));
        commandBindings.Add(new CommandBinding(IntegrationCommands.CustomCheckIn, HandleCustomCheckIn, CanExecuteCustomCheckIn));
        commandBindings.Add(new CommandBinding(IntegrationCommands.CheckOut, HandleCheckOut, CanExecuteHandleCheckOut));
        commandBindings.Add(new CommandBinding(IntegrationCommands.UndoCheckOut, HandleUndoCheckOut, CanExecuteHandleUndoCheckOut));
        commandBindings.Add(new CommandBinding(IntegrationCommands.WorkgroupManager, HandleWorkgroupManager, CanExecuteHandleWorkgroupManager));
        commandBindings.Add(new CommandBinding(IntegrationCommands.ServerManagerWC, HandleServerManagerWc, CanExecuteHandleServerManagerWc));
        commandBindings.Add(new CommandBinding(IntegrationCommands.EventManager, HandleEventManager, CanExecuteHandleEventManager));
        commandBindings.Add(new CommandBinding(IntegrationCommands.WorkgroupManagerHelp, HandleWorkgroupManagerHelp, CanExecuteHandleWorkgroupManagerHelp));
    }

    private void HandleAutoCheckIn(object sender, ExecutedRoutedEventArgs e) => HandleCommandHelper((int)AddonsProperties.AddonMenuItemCommandId.CheckInAuto, WwgmAddonId);

    private void CanExecuteHandleAutoCheckIn(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = CanExecuteConmmandHelper((int)AddonsProperties.AddonMenuItemCommandId.CheckInAuto, WwgmAddonId);

    private void HandleCustomCheckIn(object sender, ExecutedRoutedEventArgs e) => HandleCommandHelper((int)AddonsProperties.AddonMenuItemCommandId.CheckInCustom, WwgmAddonId);

    private void CanExecuteCustomCheckIn(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = CanExecuteConmmandHelper((int)AddonsProperties.AddonMenuItemCommandId.CheckInCustom, WwgmAddonId);

    private void HandleCheckOut(object sender, ExecutedRoutedEventArgs e) => HandleCommandHelper((int)AddonsProperties.AddonMenuItemCommandId.CheckOut, WwgmAddonId);

    private void CanExecuteHandleCheckOut(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = CanExecuteConmmandHelper((int)AddonsProperties.AddonMenuItemCommandId.CheckOut, WwgmAddonId);

    private void HandleUndoCheckOut(object sender, ExecutedRoutedEventArgs e) => HandleCommandHelper((int)AddonsProperties.AddonMenuItemCommandId.UndoCheckOut, WwgmAddonId);

    private void CanExecuteHandleUndoCheckOut(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = CanExecuteConmmandHelper((int)AddonsProperties.AddonMenuItemCommandId.UndoCheckOut, WwgmAddonId);

    private void HandleWorkgroupManager(object sender, ExecutedRoutedEventArgs e) => HandleCommandHelper((int)AddonsProperties.AddonMenuItemCommandId.AccessWorkgroupManager, WwgmAddonId);

    private void CanExecuteHandleWorkgroupManager(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = CanExecuteConmmandHelper((int)AddonsProperties.AddonMenuItemCommandId.AccessWorkgroupManager, WwgmAddonId);

    private void HandleServerManagerWc(object sender, ExecutedRoutedEventArgs e) => HandleCommandHelper((int)AddonsProperties.AddonMenuItemCommandId.AccessServerManagerWc, WwgmAddonId);

    private void CanExecuteHandleServerManagerWc(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = CanExecuteConmmandHelper((int)AddonsProperties.AddonMenuItemCommandId.AccessServerManagerWc, WwgmAddonId);

    private void HandleEventManager(object sender, ExecutedRoutedEventArgs e) => HandleCommandHelper((int)AddonsProperties.AddonMenuItemCommandId.AccessEventManager, WwgmAddonId);

    private void CanExecuteHandleEventManager(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = CanExecuteConmmandHelper((int)AddonsProperties.AddonMenuItemCommandId.AccessEventManager, WwgmAddonId);

    private void HandleWorkgroupManagerHelp(object sender, ExecutedRoutedEventArgs e) => HandleCommandHelper((int)AddonsProperties.AddonMenuItemCommandId.AccessHelp, WwgmAddonId);

    private void CanExecuteHandleWorkgroupManagerHelp(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = CanExecuteConmmandHelper((int)AddonsProperties.AddonMenuItemCommandId.AccessHelp, WwgmAddonId);

    private void HandleCommandHelper(int cmdId, string addonId) => SpiritMainWindow.Addons.CallAddonsOnCommand(cmdId, addonId);

    private bool CanExecuteConmmandHelper(int cmdId, string addonId) => SpiritMainWindow.Addons.CallAddonsUpdateMenuItem(cmdId, addonId);
}
