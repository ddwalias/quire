#nullable disable

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Quire.Calculation.Core;
using Quire.Controls;
using Quire.Controls.Core;
using Quire.Controls.Worksheet;
using Quire.Freemium;
using Quire.FunctionalitiesLimitation;
using Quire.Services;
using Quire.Wpf;

namespace Quire;

public partial class SystemUnitEditDialogViewModel
{
    public static RoutedUICommand RbCustomCommand = new RoutedUICommand();

    private Dictionary<string, List<UnitDescriptor>> _categories = new Dictionary<string, List<UnitDescriptor>>();

    private Dictionary<UnitSystems, List<UnitDescriptor>> _systemUnitsBaseDim = new Dictionary<UnitSystems, List<UnitDescriptor>>();

    private Dictionary<UnitSystems, List<UnitDescriptor>> _systemUnitsDerivedDim = new Dictionary<UnitSystems, List<UnitDescriptor>>();

    public SystemUnitEditDialogViewModel(ICollectionView visualUnitDescriptorTable, IDictionary visualCGSUnitDescriptorTable, Dictionary<string, UnitDescriptor> calculatorUnitDescriptors, SpiritMainWindow owner)
    {
        _owner = owner;
        _currentUnitSystem = owner.ActiveDocument.Worksheet.DefaultUnitSystem;
        _customBaseUnitSystem = (UnitSystemsDataSource)owner.ActiveDocument.Worksheet.CustomBaseUnitSystem;
        BuildDefaultViewLists(calculatorUnitDescriptors, visualUnitDescriptorTable.SourceCollection as IDictionary, visualCGSUnitDescriptorTable);
        BuildCustomViewLists(owner.ActiveDocument.Worksheet as WorksheetControl);
    }

    private bool IsCustomEqualsBase()
    {
        if (_systemUnitsDerivedDim[UnitSystems.Custom].Count != _systemUnitsDerivedDim[(UnitSystems)_customBaseUnitSystem].Count)
        {
            return false;
        }

        if (_systemUnitsDerivedDim[UnitSystems.Custom].Except(_systemUnitsDerivedDim[(UnitSystems)_customBaseUnitSystem]).ToList().Count != 0)
        {
            return false;
        }

        if (_systemUnitsBaseDim[UnitSystems.Custom].Except(_systemUnitsBaseDim[(UnitSystems)_customBaseUnitSystem]).ToList().Count != 0)
        {
            return false;
        }

        return true;
    }

    public void Commit()
    {
        if (_owner.ActiveDocument.Worksheet is WorksheetControl worksheetControl)
        {
            worksheetControl.CustomBaseUnitDescriptors = new List<UnitDescriptor>(_systemUnitsBaseDim[UnitSystems.Custom]);
            worksheetControl.CustomDerivedUnitDescriptors = new List<UnitDescriptor>(_systemUnitsDerivedDim[UnitSystems.Custom]);
            worksheetControl.CustomBaseUnitSystem = (UnitSystems)_customBaseUnitSystem;
            if (_currentUnitSystem == UnitSystems.Custom)
            {
                if (IsCustomEqualsBase())
                {
                    worksheetControl.DefaultUnitSystem = (UnitSystems)_customBaseUnitSystem;
                }
                else if (worksheetControl.DefaultUnitSystem != _currentUnitSystem)
                {
                    worksheetControl.DefaultUnitSystem = _currentUnitSystem;
                }
                else
                {
                    worksheetControl.HandleCustomBaseDerivedUnitsSystemChanged();
                }
            }
            else
            {
                worksheetControl.DefaultUnitSystem = _currentUnitSystem;
            }
        }
    }

    public bool IsFunctionalityEnabled => FunctionalityService.Instance.IsFunctionalityEnabled(EnumFunctionalityKey.CustomSystemUnitCommand);

    public string DiscardMessage { get; set; }

    public void HandleRbCustomClicked(FrameworkElement fe)
    {
        if (!IsFunctionalityEnabled)
        {
            FreemiumAboutBox.Instance.Show(new DisplayFreemiumAboutBoxCommandParameter(fe, EnumFunctionalityKey.CustomSystemUnitCommand), _owner);
            OnPropertyChanged("CurrentUnitSystem");
        }
        else
        {
            CurrentUnitSystem = UnitSystems.Custom;
        }
    }

    private List<string> BuildDerivedInsertDataSource()
    {
        IEnumerable<string> enumerable = _categories.Keys.Except(DerivedDimDataSource.Select((UnitDescriptor unitDescriptor) => unitDescriptor.Category)).Except(BaseDimDataSource.Select((UnitDescriptor unitDescriptor) => unitDescriptor.Category));
        List<string> list = new List<string>();
        foreach (string item in enumerable)
        {
            if (_categories[item].FindAll((UnitDescriptor unitDescriptor) => unitDescriptor.ExcludeUnitSystems == null || !unitDescriptor.ExcludeUnitSystems.Contains((UnitSystems)_customBaseUnitSystem)).Count > 0)
            {
                list.Add(item);
            }
        }

        return new List<string>(list);
    }

    private void BuildDefaultViewLists(Dictionary<string, UnitDescriptor> calculatorUnitDescriptors, IDictionary visualUnitDescriptorTable, IDictionary visualCgaUnitDescriptorTable)
    {
        foreach (KeyValuePair<string, UnitDescriptor> calculatorUnitDescriptor in calculatorUnitDescriptors)
        {
            VisualUnitDescriptor visualUnitDescriptor = null;
            if (visualCgaUnitDescriptorTable.Contains(calculatorUnitDescriptor.Key))
            {
                visualUnitDescriptor = (VisualUnitDescriptor)visualCgaUnitDescriptorTable[calculatorUnitDescriptor.Key];
            }
            else if (visualUnitDescriptorTable.Contains(calculatorUnitDescriptor.Key))
            {
                visualUnitDescriptor = (VisualUnitDescriptor)visualUnitDescriptorTable[calculatorUnitDescriptor.Key];
            }

            UnitDescriptor value = calculatorUnitDescriptor.Value;
            if (value.IsFunction || visualUnitDescriptor == null)
            {
                continue;
            }

            value.Category = visualUnitDescriptor.Category;
            value.DisplayName = visualUnitDescriptor.DisplayName;
            if (!_categories.ContainsKey(visualUnitDescriptor.Category))
            {
                _categories.Add(visualUnitDescriptor.Category, new List<UnitDescriptor> { value });
            }
            else
            {
                _categories[visualUnitDescriptor.Category].Add(value);
            }

            if (value.BaseDimensions != null)
            {
                foreach (UnitSystems baseDimension in value.BaseDimensions)
                {
                    if (!_systemUnitsBaseDim.ContainsKey(baseDimension))
                    {
                        _systemUnitsBaseDim.Add(baseDimension, new List<UnitDescriptor> { value });
                    }
                    else if (!_systemUnitsBaseDim[baseDimension].Contains(value))
                    {
                        _systemUnitsBaseDim[baseDimension].Add(value);
                    }
                }
            }

            if (value.DerivedDimensions == null)
            {
                continue;
            }

            foreach (UnitSystems derivedDimension in value.DerivedDimensions)
            {
                if (!_systemUnitsDerivedDim.ContainsKey(derivedDimension))
                {
                    _systemUnitsDerivedDim.Add(derivedDimension, new List<UnitDescriptor> { value });
                }
                else if (!_systemUnitsDerivedDim[derivedDimension].Contains(value))
                {
                    _systemUnitsDerivedDim[derivedDimension].Add(value);
                }
            }
        }
    }

    private void BuildCustomViewLists(WorksheetControl worksheet)
    {
        if (worksheet.CustomBaseUnitDescriptors != null)
        {
            _systemUnitsBaseDim.Add(UnitSystems.Custom, new List<UnitDescriptor>(worksheet.CustomBaseUnitDescriptors));
            _systemUnitsDerivedDim.Add(UnitSystems.Custom, new List<UnitDescriptor>(worksheet.CustomDerivedUnitDescriptors ?? new List<UnitDescriptor>()));
            _isCustomDataChanged = !IsCustomEqualsBase();
        }
        else
        {
            _systemUnitsBaseDim.Add(UnitSystems.Custom, new List<UnitDescriptor>(_systemUnitsBaseDim[_currentUnitSystem]));
            _systemUnitsDerivedDim.Add(UnitSystems.Custom, new List<UnitDescriptor>(_systemUnitsDerivedDim[_currentUnitSystem]));
        }
    }

    private void CustomBaseChange(UnitSystems newUnitSystem)
    {
        _systemUnitsBaseDim[UnitSystems.Custom] = new List<UnitDescriptor>(_systemUnitsBaseDim[newUnitSystem]);
        _systemUnitsDerivedDim[UnitSystems.Custom] = new List<UnitDescriptor>(_systemUnitsDerivedDim[newUnitSystem]);
    }

    public UnitSystemsDataSource CustomBaseUnitSystem
    {
        get
        {
            return _customBaseUnitSystem;
        }
        set
        {
            if (IsUserDiscardChanges())
            {
                CustomBaseChange((UnitSystems)value);
                _customBaseUnitSystem = value;
                _isCustomDataChanged = false;
                OnPropertyChanged("BaseDimDataSource");
                OnPropertyChanged("DerivedDimDataSource");
                OnPropertyChanged("IsChange");
                OnPropertyChanged("IsInsert");
            }
        }
    }

    public bool ShowDimEditDialog(DimensionEditDialogViewModel vm)
    {
        return new DimensionsEditDialog(vm)
        {
            Owner = _owner
        }.ShowDialog() == true;
    }

    private bool IsUserDiscardChanges()
    {
        if (!_isCustomDataChanged)
        {
            return true;
        }

        string[] buttonsKey = new string[2] { "Yes", "No" };
        string defaultButtonKey = "Yes";
        string cancelButtonKey = "No";
        return _owner.MessageBoxManager.ShowQuestionWithOptions(_owner, DiscardMessage, WpfUtils.ApplicationFullName, MessageBoxImage.Exclamation, null, buttonsKey, defaultButtonKey, cancelButtonKey) == 0;
    }

    public ListBox DerivedListBox { get; set; }

    public bool IsCustom => _currentUnitSystem == UnitSystems.Custom;

    public bool IsCustomExpress
    {
        get
        {
            if (IsCustom)
            {
                return IsFunctionalityEnabled;
            }

            return false;
        }
    }

    public bool IsInsert
    {
        get
        {
            if (IsCustom)
            {
                return BuildDerivedInsertDataSource().Count > 0;
            }

            return false;
        }
    }

    public bool IsRemove
    {
        get
        {
            if (IsCustom)
            {
                return DerivedDimSelectedItem != null;
            }

            return false;
        }
    }

    public bool IsChange
    {
        get
        {
            if (IsCustom)
            {
                return DerivedDimDataSource.Count != 0;
            }

            return false;
        }
    }

    public void HandlePostCustomBaseOnSelectionChanged(ComboBox comboBox)
    {
        comboBox.SelectedValue = comboBox.SelectedItem;
    }

    public UnitDescriptor BaseDimSelectedItem { get; set; }

    public int BaseDimSelectedIndex { get; set; }

    public int DerivedDimSelectedIndex { get; set; }

    public UnitDescriptor DerivedDimSelectedItem
    {
        get
        {
            return _derivedDimSelectedItem;
        }
        set
        {
            _derivedDimSelectedItem = value;
            OnPropertyChanged("IsRemove");
            OnPropertyChanged("DerivedDimSelectedItem");
        }
    }

    public List<UnitDescriptor> BaseDimDataSource
    {
        get
        {
            return _systemUnitsBaseDim[_currentUnitSystem];
        }
        set
        {
            _systemUnitsBaseDim[_currentUnitSystem] = value;
            OnPropertyChanged("BaseDimDataSource");
        }
    }

    public List<UnitDescriptor> DerivedDimDataSource
    {
        get
        {
            return _systemUnitsDerivedDim[_currentUnitSystem];
        }
        set
        {
            _systemUnitsDerivedDim[_currentUnitSystem] = value;
            OnPropertyChanged("DerivedDimDataSource");
            OnPropertyChanged("IsChange");
        }
    }

    public UnitSystems CurrentUnitSystem
    {
        get
        {
            return _currentUnitSystem;
        }
        set
        {
            _currentUnitSystem = value;
            OnPropertyChanged("CurrentUnitSystem");
            OnPropertyChanged("BaseDimDataSource");
            OnPropertyChanged("DerivedDimDataSource");
            OnPropertyChanged("IsCustom");
            OnPropertyChanged("IsCustomExpress");
            OnPropertyChanged("IsChange");
            OnPropertyChanged("IsRemove");
            OnPropertyChanged("IsInsert");
        }
    }
}
