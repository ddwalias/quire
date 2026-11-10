#nullable disable

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Quire.Wpf;

namespace Quire;

public partial class ProofingLanguageDropDownSelector
{
    public override void BeginInit()
    {
        _itemsManager = new CultureItemsManager();
        ItemsSource = _itemsManager.CulturesDataItems;
        base.BeginInit();
    }

    public ProofingLanguageDropDownSelector()
    {
    }

    private partial class CultureItemsManager
    {
        private static readonly Comparer<CultureInfo> CulturesComparer = Comparer<CultureInfo>.Create((CultureInfo culture, CultureInfo culture2) => string.Compare(ProofingLanguageDataItem.GetCultureName(culture), ProofingLanguageDataItem.GetCultureName(culture2), StringComparison.OrdinalIgnoreCase));

        public ObservableRangeCollection<ProofingLanguageDataItem> CulturesDataItems { get; }

        public CultureItemsManager()
        {
            _sortedCultures = InputLanguageManager.Current.AvailableInputLanguages?.OfType<CultureInfo>().ToList() ?? new List<CultureInfo>();
            _sortedCultures.Sort(CulturesComparer);
            CulturesDataItems = new ObservableRangeCollection<ProofingLanguageDataItem>(_sortedCultures.Select((CultureInfo culture) => new ProofingLanguageDataItem(culture)));
        }

        public ProofingLanguageDataItem Insert(CultureInfo culture)
        {
            ProofingLanguageDataItem proofingLanguageDataItem = null;
            int num = _sortedCultures.BinarySearch(culture, CulturesComparer);
            if (num < 0)
            {
                proofingLanguageDataItem = new ProofingLanguageDataItem(culture);
                _sortedCultures.Insert(~num, culture);
                CulturesDataItems.Insert(~num, proofingLanguageDataItem);
            }

            return proofingLanguageDataItem;
        }
    }
}
