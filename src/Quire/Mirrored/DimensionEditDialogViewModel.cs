#nullable disable

using System.Collections.Generic;
using System.Windows.Controls;
using Quire.Calculation.Core;

namespace Quire;

public partial class DimensionEditDialogViewModel
{
    public DimensionEditDialogViewModel(UnitSystems unitSystem, DialogAction? action, Dictionary<string, List<UnitDescriptor>> categories, List<string> dimensionDataSource, string dimensionSelectedItem, UnitDescriptor unitSelectedItem)
    {
        _categories = categories;
        Action = action;
        _unitSystem = unitSystem;
        _dimensionDataSource = dimensionDataSource;
        _dimensionSelectedItem = dimensionSelectedItem ?? ((_dimensionDataSource.Count > 0) ? _dimensionDataSource[0] : null);
        _unitSelectedItem = unitSelectedItem ?? UnitDataSource[0];
    }

    public List<string> DimensionDataSource => _dimensionDataSource;

    public List<UnitDescriptor> UnitDataSource => _categories[_dimensionSelectedItem].FindAll((UnitDescriptor unitDescriptor) => unitDescriptor.ExcludeUnitSystems == null || !unitDescriptor.ExcludeUnitSystems.Contains(_unitSystem));

    public DialogAction? Action { get; set; }

    public bool IsDimensionEnabled => Action == DialogAction.DerivedInsert;

    public string DimensionSelectedItem
    {
        get
        {
            return _dimensionSelectedItem;
        }
        set
        {
            if (value != null)
            {
                _dimensionSelectedItem = value;
                OnPropertyChanged("UnitSelectedItem");
                OnPropertyChanged("UnitDataSource");
            }
        }
    }

    public UnitDescriptor UnitSelectedItem
    {
        get
        {
            return _unitSelectedItem;
        }
        set
        {
            _unitSelectedItem = value;
            _isCustomDataChanged = true;
        }
    }

    public bool IsCustomDataChanged => _isCustomDataChanged;

    public void HandlePostDimensionChanged(ListBox listBox)
    {
        if (listBox.SelectedIndex == -1)
        {
            listBox.SelectedIndex = DimensionDataSource.IndexOf(DimensionSelectedItem);
        }
    }
}
