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
    public const string NullLanguage = "MixedLanguages";

    public const string EmptyLanguage = "NoLanguage";

    private CultureItemsManager _itemsManager;

    public override void BeginInit()
    {
        _itemsManager = new CultureItemsManager();
        ItemsSource = _itemsManager.CulturesDataItems;
        base.BeginInit();
    }

    public ProofingLanguageDropDownSelector()
    {
    }

    protected override void EnsureItemHeader(object newValue)
    {
        if (newValue == null || DebugTools.AssertNotNull(_itemsManager, null).Failed)
        {
            return;
        }

        CultureInfo cultureInfo = newValue as CultureInfo;
        // Declared before the type check: Mathcad's IL gives this local the slot before the interpolation handler.
        ProofingLanguageDataItem proofingLanguageDataItem;
        if (cultureInfo == null)
        {
            DebugTools.FailAssertion($"{newValue}, {newValue.GetType().Name}");
            return;
        }

        if (object.Equals(cultureInfo, CultureInfo.InvariantCulture))
        {
            return;
        }

        proofingLanguageDataItem = _itemsManager.Insert(cultureInfo);
        if (proofingLanguageDataItem != null && UpdateHeader)
        {
            double maxWidth = (_headerDict?.Values.OfType<FrameworkElement>().FirstOrDefault()?.MinWidth).GetValueOrDefault();
            AddMeasuredDuplicateElemToHeaderDict(proofingLanguageDataItem, ref maxWidth);
            SetHeadersMinimumWidth(maxWidth);
        }
    }

    private partial class CultureItemsManager
    {
        private static readonly Comparer<CultureInfo> CulturesComparer = Comparer<CultureInfo>.Create((CultureInfo culture, CultureInfo culture2) => string.Compare(ProofingLanguageDataItem.GetCultureName(culture), ProofingLanguageDataItem.GetCultureName(culture2), StringComparison.OrdinalIgnoreCase));

        private readonly List<CultureInfo> _sortedCultures;

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
