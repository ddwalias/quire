#nullable disable
using System.Windows.Input;
using Quire.Mirror;
using Quire.Services;

namespace Quire.FunctionalitiesLimitation;

public partial class InputBindingFunctionalityCommandWrapper
{
    [MirrorVerified("Release IL")]
    public InputBindingFunctionalityCommandWrapper(RoutedUICommand nestedCommand) : base(nestedCommand) { }

    [MirrorVerified("Release IL")]
    public override void Execute(object parameter) => NestedCommand.Execute(parameter, null);

    [MirrorVerified("Release IL")]
    public override EnumFunctionalityBehaviorKey CommandFunctionalityBehaviorKey => FunctionalityServiceWrapper.Instance.GetFunctionalityBehaviorKey(FunctionalityKey);

    [MirrorVerified("Release IL")]
    public override bool CanExecute(object parameter)
    {
        EnumFunctionalityBehaviorKey behaviorKey = CommandFunctionalityBehaviorKey;
        if (NestedCommand as Quire.Wpf.IHasFunctionalityKeyAccordingToParameter != null)
        {
            behaviorKey = GetFunctionalityBehaviorAccordingParameter(parameter, NestedCommand as Quire.Wpf.IHasFunctionalityKeyAccordingToParameter);
        }
        if ((uint)behaviorKey <= 1u)
        {
            goto CanExecute;
        }
        if (behaviorKey != EnumFunctionalityBehaviorKey.FreemiumBehavior)
        {
            goto InvalidBehavior;
        }
        return false;
    CanExecute:
        return true;
    InvalidBehavior:
        throw new ArgumentOutOfRangeException();
    }

    [MirrorVerified("Release IL")]
    public bool CanExecute(object parameter, System.Windows.IInputElement commandTarget)
    {
        if (((BaseFunctionalityCommandWrapper)this).CanExecute(parameter))
        {
            return NestedCommand.CanExecute(parameter, commandTarget);
        }
        return false;
    }

    [MirrorVerified("Release IL")]
    public void Execute(object parameter, System.Windows.IInputElement commandTarget)
    {
        if (CanExecute(parameter, commandTarget))
        {
            NestedCommand.Execute(parameter, commandTarget);
        }
    }

    [MirrorVerified("Release IL")]
    private static EnumFunctionalityBehaviorKey GetFunctionalityBehaviorAccordingParameter(object parameter, Quire.Wpf.IHasFunctionalityKeyAccordingToParameter commandWithParameter)
    {
        EnumFunctionalityKey functionalityKey = commandWithParameter.GetFunctionalityKeyAccordingParameter(parameter, false);
        return FunctionalityServiceWrapper.Instance.GetFunctionalityBehaviorKey(functionalityKey);
    }
}
