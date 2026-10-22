#nullable disable
using System.Windows;
using Quire.Mirror;
using Quire.Services;

namespace Quire.FunctionalitiesLimitation;

public partial class UIFunctionalityCommandWrapper
{
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
    public event EventHandler<EventArgs> FunctionalityBehaviorChanged;

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
