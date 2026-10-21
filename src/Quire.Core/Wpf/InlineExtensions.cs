#nullable disable
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using Quire.Mirror;

namespace Quire.Wpf;

public static partial class InlineExtensions
{
    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty IsSearchScopeProperty = IsSearchScopePropertyKey.DependencyProperty;

    [MirrorVerified("Release IL")]
    public static bool GetIsSearchScope(DependencyObject obj) => (bool)obj.GetValue(IsSearchScopeProperty);

    [MirrorVerified("Release IL")]
    public static Inline InlineAt(this Inline inline, int index)
    {
        return (inline as Span)?.Inlines.ElementAt(index);
    }

    [MirrorVerified("Release IL")]
    public static InlineCollection Inlines(this Inline inline)
    {
        return (inline as Span)?.Inlines;
    }
}
