#nullable disable
using System;
using System.Windows;
using System.Windows.Input;
using Quire.FunctionalitiesLimitation;
using Quire.Wpf;

namespace Quire.Controls.Core;

public static partial class RegionCommands
{
    private static Type _typeOfThis;

    public static readonly RoutedUICommand Replace;

    public static readonly RoutedUICommand DeleteRegion;

    public static readonly RoutedCommand ToggleOrSetPerspectiveCameraCommand;

    public static readonly RoutedCommand ChangePlotBackgroundCommand;

    public static readonly RoutedUICommand ChangePlotBackgroundGrayCommand;

    public static readonly RoutedCommand ShowAxisExpressionsCommand;

    public static readonly RoutedUICommand TagCommand;

    public static readonly RoutedUICommand RemoveTagCommand;

    public static readonly RoutedUICommand VisibleRegionBorderCommand;

    public static void ExecuteCommandWithAssertion(RoutedCommand command, object param, IInputElement elem, bool assert)
    {
        if (command.CanExecute(param, elem))
        {
            command.Execute(param, elem);
        }
        else if (assert)
        {
            DebugTools.DevAssert(false, "Execution of this command is forbidden in this content !");
        }
    }

    public static CameraType3D ValueForToggleOrSetPerspectiveCameraCommand(object parameter, CameraType3D currentValue)
    {
        if (!(parameter is CameraType3D))
        {
            if (currentValue != CameraType3D.Perspective)
            {
                return CameraType3D.Perspective;
            }
            return CameraType3D.Orthographic;
        }
        return (CameraType3D)parameter;
    }

    static RegionCommands()
    {
        _typeOfThis = typeof(RegionCommands);
        Replace = new RoutedUICommand();
        DeleteRegion = new RoutedUICommand("DeleteRegionByRibbonButton", "Delete Active Region or Selected Regions Using Ribbon Button", _typeOfThis);
        ToggleOrSetPerspectiveCameraCommand = new RoutedCommand("ToggleOrSetPerspectiveCameraCommand", _typeOfThis);
        ChangePlotBackgroundCommand = new RoutedCommand("ChangeBackground", _typeOfThis);
        ChangePlotBackgroundGrayCommand = new RoutedUICommandWithFunctionalityKey("Change Background to Gray", "ChangeBackgroundGray", _typeOfThis, EnumFunctionalityKey.PlotsCategoryBackgroundColorGray);
        ShowAxisExpressionsCommand = new RoutedCommand("ShowAxisExpressions", _typeOfThis);
        TagCommand = new RoutedUICommandWithFunctionalityKey("Tag", "Tag", _typeOfThis, EnumFunctionalityKey.TagCommand);
        RemoveTagCommand = new RoutedUICommandWithFunctionalityKey("RemoveTag", "Remove Tag", _typeOfThis, EnumFunctionalityKey.RemoveTagCommand);
        VisibleRegionBorderCommand = new RoutedUICommandWithFunctionalityKey("Visibility of the region border", "VisibleRegionBorderCommand", _typeOfThis, EnumFunctionalityKey.ToggleVisibleRegionBorderCommand);
    }
}
