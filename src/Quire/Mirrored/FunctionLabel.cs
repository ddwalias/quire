#nullable disable

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Quire.FunctionalitiesLimitation;

namespace Quire;

public partial class FunctionLabel
{
    private bool _canExecute = true;

    public ICommand Command
    {
        get => (ICommand)GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    public object CommandParameter
    {
        get => GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }

    public IInputElement CommandTarget => this;

    public bool CanExecute
    {
        get => _canExecute;
        private set
        {
            if (_canExecute == value)
            {
                return;
            }

            _canExecute = value;
            CoerceValue(UIElement.IsEnabledProperty);
        }
    }

    protected override bool IsEnabledCore
    {
        get
        {
            if (base.IsEnabledCore)
            {
                return CanExecute;
            }

            return false;
        }
    }

    public event RoutedEventHandler ItemCommandExecuted
    {
        add => AddHandler(ItemCommandExecutedEvent, value);
        remove => RemoveHandler(ItemCommandExecutedEvent, value);
    }

    public FunctionLabel()
    {
        Focusable = true;
    }

    protected override void OnPreviewKeyDown(KeyEventArgs e)
    {
        base.OnPreviewKeyDown(e);
        if ((int)e.Key == 6)
        {
            ExecuteCommandWithParameter();
            e.Handled = true;
        }
    }

    protected override void OnPreviewMouseDoubleClick(MouseButtonEventArgs e)
    {
        base.OnPreviewMouseDoubleClick(e);
        ExecuteCommandWithParameter();
        e.Handled = true;
    }

    private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((FunctionLabel)d).OnCommandChanged((ICommand)e.OldValue, (ICommand)e.NewValue);
    }

    private void OnCommandChanged(ICommand oldCommand, ICommand newCommand)
    {
        if (oldCommand != null)
        {
            oldCommand.CanExecuteChanged -= CanExecuteChanged;
        }

        if (newCommand != null)
        {
            newCommand.CanExecuteChanged += CanExecuteChanged;
        }

        UpdateCanExecute();
    }

    private void UpdateCanExecute()
    {
        CanExecute = (!(Command is RoutedCommand routedCommand))
            ? Command.CanExecute(CommandParameter)
            : routedCommand.CanExecute(CommandParameter, CommandTarget);
    }

    private void CanExecuteChanged(object sender, EventArgs e)
    {
        UpdateCanExecute();
    }
}
