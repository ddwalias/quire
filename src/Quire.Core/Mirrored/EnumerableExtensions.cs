#nullable disable
#pragma warning disable

namespace Quire;

static partial class EnumerableExtensions
{
    public static bool IsNullOrEmpty<T>(this global::System.Collections.Generic.IEnumerable<T> items)
    {
        if (items == null)
        {
            goto NullItems;
        }

        if (items is string value)
        {
            goto StringItems;
        }

        global::System.Collections.ICollection collection = items as global::System.Collections.ICollection;
        if (collection == null)
        {
            goto OtherItems;
        }

        goto CollectionItems;

    NullItems:
        return true;

    StringItems:
        return string.IsNullOrEmpty(value);

    CollectionItems:
        return collection.Count == 0;

    OtherItems:
        return !items.Any();
    }
}
