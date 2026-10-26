#nullable disable
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Quire.Mirror;

namespace Quire.Controls.Core;

public abstract partial class ViewModelBase : INotifyPropertyChanged, IDisposable
{
    [MirrorVerified("PtcControlsCoreViewModelBaseMechanicalDifferentialTests")]
    private readonly CommandBindingCollection _commandBindings = new CommandBindingCollection();

    [MirrorVerified("PtcControlsCoreViewModelBaseMechanicalDifferentialTests")]
    public virtual string DisplayName { get; protected set; }

    [MirrorVerified("PtcControlsCoreViewModelBaseMechanicalDifferentialTests")]
    public CommandBindingCollection CommandBindings => _commandBindings;

    [MirrorVerified("PtcControlsCoreViewModelBaseMechanicalDifferentialTests")]
    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    [MirrorVerified("PtcControlsCoreViewModelBaseMechanicalDifferentialTests")]
    protected virtual void ModelPropertyChanged(string propertyName)
    {
    }

    protected void ModelPropertyChangedInner(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
    {
        string propertyName = propertyChangedEventArgs.PropertyName;
        ModelPropertyChanged(propertyName);
        if (TypeDescriptor.GetProperties(this)[propertyName] != null)
        {
            OnPropertyChanged(propertyName);
        }
    }

    public void Dispose()
    {
        CommandBindings.Clear();
        OnDispose();
    }

    [MirrorVerified("PtcControlsCoreViewModelBaseMechanicalDifferentialTests")]
    protected virtual void OnDispose()
    {
    }

    [MirrorVerified("PtcControlsCoreViewModelBaseMechanicalDifferentialTests")]
    protected void CreateCommandBinding(ICommand command, Action<object> execute)
    {
        CreateCommandBinding(command, execute, null);
    }

    protected void CreateCommandBinding(ICommand command, Action<object> execute, Predicate<object> canExecute)
    {
        CommandBinding commandBinding = new CommandBinding(command);
        commandBinding.Executed += (object param1, ExecutedRoutedEventArgs param2) =>
        {
            execute(param2.Parameter);
        };
        if (canExecute != null)
        {
            commandBinding.CanExecute += (object param1, CanExecuteRoutedEventArgs param2) =>
            {
                param2.CanExecute = canExecute(param2.Parameter);
            };
        }
        CommandBindings.Add(commandBinding);
    }

    protected void CreateCommandBinding(ICommand command, Action<RoutedCommand, object> execute, Predicate<object> canExecute)
    {
        CommandBinding commandBinding = new CommandBinding(command);
        commandBinding.Executed += (object param1, ExecutedRoutedEventArgs param2) =>
        {
            execute((RoutedCommand)param2.Command, param2.Parameter);
        };
        if (canExecute != null)
        {
            commandBinding.CanExecute += (object param1, CanExecuteRoutedEventArgs param2) =>
            {
                param2.CanExecute = canExecute(param2.Parameter);
            };
        }
        CommandBindings.Add(commandBinding);
    }

    protected void CreatePreviewCommandBinding(ICommand command, Action<object> previewExecute, Predicate<object> previewCanExecute)
    {
        CommandBinding commandBinding = new CommandBinding(command);
        commandBinding.PreviewExecuted += (object sender, ExecutedRoutedEventArgs eventArgs) =>
        {
            previewExecute(eventArgs.Parameter);
        };
        if (previewCanExecute != null)
        {
            commandBinding.PreviewCanExecute += (object sender, CanExecuteRoutedEventArgs eventArgs) =>
            {
                eventArgs.CanExecute = previewCanExecute(eventArgs.Parameter);
            };
            commandBinding.CanExecute += (object sender, CanExecuteRoutedEventArgs eventArgs) =>
            {
                eventArgs.CanExecute = previewCanExecute(eventArgs.Parameter);
            };
        }
        CommandBindings.Add(commandBinding);
    }

    protected bool GetNewValue(bool isActive, bool currentValue, object parameter)
    {
        if (isActive)
        {
            return !currentValue;
        }
        if (parameter is bool)
        {
            return !(bool)parameter;
        }
        return !currentValue;
    }

    protected ViewModelBase()
    {
    }
}
