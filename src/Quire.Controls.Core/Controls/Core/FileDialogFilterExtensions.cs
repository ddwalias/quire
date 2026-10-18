#nullable disable
using Quire.Mirror;

namespace Quire.Controls.Core;

public static partial class FileDialogFilterExtensions
{
    [MirrorVerified("PtcControlsCoreFileDialogFilterExtensionsMechanicalDifferentialTests")]
    public static string ToFilterString(this IEnumerable<FileDialogFilterPair> filterList) => string.Join<FileDialogFilterPair>("|", filterList);
}
