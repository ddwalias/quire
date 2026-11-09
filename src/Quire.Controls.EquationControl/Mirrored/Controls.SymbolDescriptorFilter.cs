using System.Collections;
using System.Windows.Data;
using Quire.Wpf;

namespace Quire.Controls;

public static partial class SymbolDescriptorFilter
{
    public static void FilterByCategoryKey(FilterEventArgs e, string categoryKey)
    {
        string category = (string)ResourceProvider<VisualCategoriesDictionary>.Instance[categoryKey];
        DictionaryEntry entry = (DictionaryEntry)e.Item;
        e.Accepted = MatchCategory(entry.Value as SymbolDescriptor, category);
    }

    public static void FilterExcludingCategoryKey(FilterEventArgs e, string categoryKey)
    {
        string category = (string)ResourceProvider<VisualCategoriesDictionary>.Instance[categoryKey];
        DictionaryEntry entry = (DictionaryEntry)e.Item;
        if (entry.Value is not SymbolDescriptor symbolDescriptor) goto NotAccepted;
        if (symbolDescriptor.Categories != null) goto Accepted;

    NotAccepted:
        e.Accepted = false;
        return;

    Accepted:
        e.Accepted = !MatchCategory(symbolDescriptor, category);
    }

    public static void FilterByCanonicalName(FilterEventArgs e, object[] visualOperatorDescriptors)
    {
        DictionaryEntry entry = (DictionaryEntry)e.Item;
        e.Accepted = MatchCanonicalName(entry.Value as VisualOperatorDescriptor, visualOperatorDescriptors);
    }

    private static bool MatchCategory(SymbolDescriptor item, string category)
    {
        if (item == null || item.Categories == null)
        {
            return false;
        }

        category = category.Trim();
        foreach (string itemCategory in item.Categories)
        {
            if (itemCategory.Trim().CompareTo(category) == 0)
            {
                return true;
            }
        }

        return false;
    }

    private static bool MatchCanonicalName(VisualOperatorDescriptor item, object[] visualDescriptors)
    {
        if (item == null)
        {
            return false;
        }

        object[] descriptors = visualDescriptors;
        for (int i = 0; i < descriptors.Length; i++)
        {
            if (descriptors[i] is VisualOperatorDescriptor visualOperatorDescriptor &&
                visualOperatorDescriptor.CanonicalName == item.CanonicalName)
            {
                return true;
            }
        }

        return false;
    }
}
