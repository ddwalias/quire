#nullable disable

using System;
using System.ComponentModel;
using System.Threading;

namespace Quire;

public static partial class LocBamlHelper
{
    public static Thread CurrentThreadProperty => Thread.CurrentThread;

    public static ListSortDirection Ascending => ListSortDirection.Ascending;

    public static ListSortDirection Descending => ListSortDirection.Descending;

    public static Type UnitSystemsEnumType => typeof(UnitSystemsDataSource);

    public static Array GetEnumValues(Type enumType)
    {
        return Enum.GetValues(enumType);
    }
}
