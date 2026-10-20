#nullable disable
using System.Windows;
using Quire.Mirror;

namespace Quire.Wpf;

public static partial class InlineExtensions
{
    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty IsSearchScopeProperty;

    [MirrorVerified("Release IL")]
    public static bool GetIsSearchScope(DependencyObject obj) => (bool)obj.GetValue(IsSearchScopeProperty);
}
