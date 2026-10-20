#nullable disable
using Quire.Mirror;

namespace Quire.FunctionalitiesLimitation;

public partial class FunctionalitiesLimitationDictionary
{
    [MirrorVerified("Release IL")]
    public FunctionalitiesLimitationDictionary()
    {
        InitializeComponent();
    }

    [MirrorVerified("Release IL")]
    internal Delegate _CreateDelegate(Type delegateType, string handler) => Delegate.CreateDelegate(delegateType, this, handler);
}
