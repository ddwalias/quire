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
}
