#nullable disable
#pragma warning disable
using System;

namespace Quire.PersistentDataObjects;

public partial class solveBlockRegionType
{
    public static readonly Type[] SupportedTypes =
    {
        typeof(global::Quire.PersistentData.oleObjectRegionType),
        typeof(mathType),
        typeof(pictureType),
        typeof(plotType),
        typeof(includeWorksheetType),
        typeof(global::Quire.PersistentData.worksheetRegionTypeSpectable),
        typeof(textType),
    };
}
