#nullable disable
using System.Windows;
using Quire;

namespace Quire.Controls.Core.BookmarkTags;

public static partial class BookmarkTag
{
    public static readonly DependencyProperty BookmarkTagProperty = DependencyProperty.RegisterAttached("BookmarkTag", typeof(string), typeof(BookmarkTag), new PropertyMetadata(string.Empty));

    public static void SetBookmarkTag(DependencyObject element, string tag)
    {
        element.SetValue(BookmarkTagProperty, tag);
    }

    public static void ClearBookmarkTag(DependencyObject element)
    {
        element.ClearValue(BookmarkTagProperty);
    }

    public static string GetBookmarkTag(DependencyObject element)
    {
        return (string)element.GetValue(BookmarkTagProperty);
    }

    public static bool HasBookmarkTag(DependencyObject element, string tagValue)
    {
        if (tagValue.IsNotEmpty())
        {
            return tagValue == GetBookmarkTag(element);
        }
        return false;
    }

    public static bool HasBookmarkTag(DependencyObject element)
    {
        return GetBookmarkTag(element).IsNotEmpty();
    }
}
