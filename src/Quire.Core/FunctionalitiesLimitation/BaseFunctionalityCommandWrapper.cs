#nullable disable
using System.Windows.Input;
using Quire.Mirror;

namespace Quire.FunctionalitiesLimitation;

public partial class BaseFunctionalityCommandWrapper
{
    [MirrorVerified("Release IL")]
    protected BaseFunctionalityCommandWrapper(BaseFunctionalityCommandWrapper previousBaseFunctionalityCommandWrapper)
    {
        NestedCommand = previousBaseFunctionalityCommandWrapper.NestedCommand;
        FunctionalityKey = previousBaseFunctionalityCommandWrapper.FunctionalityKey;
    }

    [MirrorVerified("Release IL")]
    public virtual void Execute(object parameter) => throw new NotImplementedException();

    [MirrorVerified("Release IL")]
    public virtual bool CanExecute(object parameter) => throw new NotImplementedException();

    [MirrorVerified("Release IL")]
    protected RoutedUICommand NestedCommand { get; private set; }

    [MirrorVerified("Release IL")]
    protected EnumFunctionalityKey FunctionalityKey { get; private set; }
}
