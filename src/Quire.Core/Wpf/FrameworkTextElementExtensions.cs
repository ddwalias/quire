#nullable disable
using System.Windows.Documents;
using Quire.Mirror;

namespace Quire.Wpf;

public static partial class FrameworkTextElementExtensions
{
    [MirrorVerified("PtcWpfFrameworkTextElementExtensionsMechanicalDifferentialTests")]
    public static T GetTextElementParentOrSelf<T>(this TextElement element) where T : TextElement => WpfUtils.TryFindAncestorIncludingSelfLimited<T, FlowDocument>(element, false);
}
