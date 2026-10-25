#nullable disable
using System;
using System.Diagnostics;
using System.Windows.Input;

namespace Quire.Controls.Core;

public partial class RelayCommand : ICommand
{
    private readonly Action<object> _execute;

    private readonly Predicate<object> _canExecute;

    public event EventHandler CanExecuteChanged
    {
        add
        {
            CommandManager.RequerySuggested += value;
        }
        remove
        {
            CommandManager.RequerySuggested -= value;
        }
    }

    public RelayCommand(Action<object> execute)
        : this(execute, null)
    {
    }

    public RelayCommand(Action<object> execute, Predicate<object> canExecute)
    {
        if (execute == null)
        {
            throw new ArgumentNullException("execute");
        }
        _execute = execute;
        _canExecute = canExecute;
    }

    [DebuggerStepThrough]
    public bool CanExecute(object parameter)
    {
        if (_canExecute != null)
        {
            return _canExecute(parameter);
        }
        return true;
    }

    public void Execute(object parameter)
    {
        _execute(parameter);
    }
}
