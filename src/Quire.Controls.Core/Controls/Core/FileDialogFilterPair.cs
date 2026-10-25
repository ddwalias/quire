#nullable disable
using Quire.Mirror;

namespace Quire.Controls.Core;

public partial struct FileDialogFilterPair
{
    public string FilterString;

    public string Extension;

    public override string ToString() => FilterString;
}
