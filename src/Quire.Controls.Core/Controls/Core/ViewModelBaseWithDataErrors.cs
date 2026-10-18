#nullable disable
using System.ComponentModel;
using Quire.Mirror;

namespace Quire.Controls.Core;

public partial class ViewModelBaseWithDataErrors
{
    [MirrorVerified("PtcControlsCoreViewModelBaseWithDataErrorsMechanicalDifferentialTests")]
    public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
}
