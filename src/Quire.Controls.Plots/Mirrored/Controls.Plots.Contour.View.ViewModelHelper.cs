using System;
using System.Windows.Input;
using Quire.Controls.Core;

namespace Quire.Controls.Plots.Contour.View;

internal partial class ViewModelHelper : ViewModelBase
{
    public ViewModelHelper()
    {
    }

    internal void HelperCreateCommandBinding(ICommand command, Action<object> execute)
    {
        CreateCommandBinding(command, execute);
    }

    internal void HelperCreateCommandBinding(ICommand command, Action<object> execute, Predicate<object> canExecute)
    {
        CreateCommandBinding(command, execute, canExecute);
    }
}
