#nullable disable
using System;
using System.Windows.Input;

namespace Quire.Controls.Core.Ole;

public abstract partial class OleContainerViewModelBase : ViewModelBase
{
    private static readonly Type _typeOfThis = typeof(OleContainerViewModelBase);

    public static readonly RoutedCommand ToggleActiveState = new RoutedCommand("ToggleActiveState", _typeOfThis, new InputGestureCollection
    {
        new MouseGesture(MouseAction.LeftDoubleClick)
    });

    protected OleContainerViewModelBase()
    {
    }
}
