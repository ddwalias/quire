#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using Quire.Serialization;

namespace Quire.Controls.Core.Serialization;

public static partial class ListOfRegionPersistentDataExtensions
{
    public static IEnumerable<Exception> AllDeserializationExceptions(this IList<IRegionPersistentData> source)
    {
        if (source != null)
        {
            foreach (IRegionPersistentData item in source)
            {
                if (item == null)
                {
                    continue;
                }

                IEnumerable<Exception> allDeserializationExceptions = item.AllDeserializationExceptions;
                if (allDeserializationExceptions == null)
                {
                    continue;
                }

                foreach (Exception item2 in allDeserializationExceptions)
                {
                    yield return item2;
                }
            }
        }
    }

    public static bool HasDeserializationErrors(this IList<IRegionPersistentData> elements)
    {
        return elements.Any((IRegionPersistentData regionsPersistentData) => regionsPersistentData.HasDeserializationErrors);
    }
}
