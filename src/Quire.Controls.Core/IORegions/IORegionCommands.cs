#nullable disable
using System;
using System.Windows.Input;
using Quire.FunctionalitiesLimitation;
using Quire.Wpf;

namespace Quire.Controls.Core.IORegions;

public partial class IORegionCommands
{
    private static readonly Type _typeOfThis;

    public static readonly RoutedUICommand ToggleInputRegion;

    public static readonly RoutedUICommand ToggleOutputRegion;

    public static readonly RoutedUICommand UpdateInputs;

    public static readonly RoutedUICommand ToggleAnInputRegion;

    public static readonly RoutedUICommand ToggleAnOutputRegion;

    static IORegionCommands()
    {
        _typeOfThis = typeof(IORegionCommands);
        ToggleInputRegion = new RoutedUICommandWithFunctionalityKey("Toggle Input Region", "ToggleInputRegion", _typeOfThis, EnumFunctionalityKey.CreoElementsProIntegrationAssignInputCommand);
        ToggleOutputRegion = new RoutedUICommandWithFunctionalityKey("Toggle Output Region", "ToggleOutputRegion", _typeOfThis, EnumFunctionalityKey.CreoElementsProIntegrationAssignOutputCommand);
        UpdateInputs = new RoutedUICommandWithFunctionalityKey("Update Inputs", "UpdateInputs", _typeOfThis, EnumFunctionalityKey.CreoElementsProIntegrationUpdateInputsCommand);
        ToggleAnInputRegion = new RoutedUICommand("ToggleAnInputRegion", "Toggle a specific input region", _typeOfThis);
        ToggleAnOutputRegion = new RoutedUICommand("ToggleAnOutputRegion", "Toggle a specific output region", _typeOfThis);
    }

    public IORegionCommands()
    {
    }
}
