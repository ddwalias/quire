#nullable disable
using System.Windows;
using Quire.Mirror;
using Quire.Services;

namespace Quire.FunctionalitiesLimitation;

public partial class UIFunctionalityCommandWrapper
{
    [MirrorVerified("Release IL")]
    public UIFunctionalityCommandWrapper(global::System.Windows.Input.RoutedUICommand nestedCommand, FrameworkElement lockedUIItem)
        : base(nestedCommand)
    {
        if (nestedCommand is global::Quire.Wpf.RoutedUICommandWithFunctionalityKey)
        {
            _commandFunctionalityBehaviorKey = FunctionalityServiceWrapper.Instance.GetFunctionalityBehaviorKey(FunctionalityKey);
            FunctionalityServiceWrapper.Instance.FunctionalityServiceLicenseTypeChanged += OnFunctionalityServiceWrapperPropertyChanged;
        }
        else
        {
            DebugTools.FailAssertion(nestedCommand.Name + " : " + nestedCommand.GetType().Name);
        }
        Initialize(lockedUIItem);
    }

    [MirrorVerified("Release IL")]
    public UIFunctionalityCommandWrapper(UIFunctionalityCommandWrapper previousUIFunctionalityCommandWrapper, FrameworkElement lockedUIItem)
        : base(previousUIFunctionalityCommandWrapper)
    {
        _commandFunctionalityBehaviorKey = previousUIFunctionalityCommandWrapper.CommandFunctionalityBehaviorKey;
        CommandTarget = previousUIFunctionalityCommandWrapper.CommandTarget;
        FunctionalityServiceWrapper.Instance.FunctionalityServiceLicenseTypeChanged += OnFunctionalityServiceWrapperPropertyChanged;
        Initialize(lockedUIItem);
    }

    [MirrorVerified("Release IL")]
    private void Initialize(FrameworkElement lockedUIItem)
    {
        LockedUIItem = lockedUIItem;
        base.CanExecuteChanged += OnBaseCanExecuteChanged;
    }

    [MirrorVerified("Release IL")]
    public void Execute(object parameter, out bool executed)
    {
        executed = false;
        DisplayFreemiumAboutBoxCommandParameter displayFreemiumAboutBoxCommandParameter;
        switch (CommandFunctionalityBehaviorKey)
        {
            case EnumFunctionalityBehaviorKey.FreemiumBehavior:
            {
                displayFreemiumAboutBoxCommandParameter = new DisplayFreemiumAboutBoxCommandParameter(LockedUIItem, FunctionalityKey);
                FunctionalitiesLimitationCommands.DisplayFreemiumAboutBox.Execute(displayFreemiumAboutBoxCommandParameter, null);
                break;
            }
            case EnumFunctionalityBehaviorKey.FullBehavior:
            case EnumFunctionalityBehaviorKey.TrialBehavior:
                NestedCommand.Execute(parameter, CommandTarget);
                executed = true;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    [MirrorVerified("Release IL")]
    public override void Execute(object parameter)
    {
        Execute(parameter, out bool executed);
    }

    [MirrorVerified("Release IL")]
    public override bool CanExecute(object parameter) => NestedCommand.CanExecute(parameter, CommandTarget);

    [MirrorVerified("Release IL")]
    private void OnFunctionalityServiceWrapperPropertyChanged(object sender, EventArgs eventArgs) => UpdateFunctionalityBehaviour();

    [MirrorVerified("Release IL")]
    private EnumFunctionalityBehaviorKey _commandFunctionalityBehaviorKey;

    [MirrorVerified("Release IL")]
    public override EnumFunctionalityBehaviorKey CommandFunctionalityBehaviorKey => _commandFunctionalityBehaviorKey;

    [MirrorVerified("Release IL")]
    public IInputElement CommandTarget { private get; set; }

    [MirrorVerified("Release IL")]
    private FrameworkElement LockedUIItem { get; set; }

    [MirrorVerified("Release IL")]
    public event EventHandler<EventArgs> FunctionalityBehaviorChanged = delegate { };

    [MirrorVerified("Release IL")]
    private void UpdateFunctionalityBehaviour()
    {
        EnumFunctionalityBehaviorKey commandFunctionalityBehaviorKey = CommandFunctionalityBehaviorKey;
        _commandFunctionalityBehaviorKey = FunctionalityServiceWrapper.Instance.GetFunctionalityBehaviorKey(FunctionalityKey);
        if (CommandFunctionalityBehaviorKey != commandFunctionalityBehaviorKey)
        {
            FunctionalityBehaviorChanged(this, EventArgs.Empty);
        }
    }
}
