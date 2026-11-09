#nullable disable
#pragma warning disable
using System;

namespace Quire.PersistentData;

public partial class cachedWorksheetRegionType
{
    public static readonly Type[] SupportedTypes =
    {
        typeof(AreaType),
        typeof(chartComponentType),
        typeof(excelComponentType),
        typeof(global::Quire.PersistentDataObjects.mathType),
        typeof(global::Quire.PersistentDataObjects.includeWorksheetType),
        typeof(global::Quire.PersistentDataObjects.solveBlockType),
        typeof(worksheetRegionTypeSpectable),
        typeof(global::Quire.PersistentDataObjects.textType),
    };
}
