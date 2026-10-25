#nullable disable
using System;

namespace Quire.Controls.Core.AttachedBehavior;

public static partial class OneTimePropertyOwnerExtensionMethods
{
    public static void OneTimeSetExt<TProperty>(this IOneTimePropertyOwner owner, ref TProperty field, TProperty value, string propertyName) where TProperty : class
    {
        if (field == null && value == null)
        {
            return;
        }
        bool flag = ((value is string) ? string.IsNullOrEmpty(value as string) : (value == null));
        if (field == null && !flag)
        {
            field = value;
            return;
        }
        throw new InvalidOperationException(propertyName + " can be set only once, to a non-empty value.");
    }
}
