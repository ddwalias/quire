using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Quire.EquationEditor.View;

namespace Quire.Controls.InputControls;

public partial class NameValuesRow : ComboBoxRow, INameValuesRow, IComboBoxRow, INotifyPropertyChanged
{
    public const string DisplayNameValue = "DisplayName";

    private string _displayName = string.Empty;

    public NameValuesRow(int rowIndex, int columns)
        : base(rowIndex, columns)
    {
    }

    public string DisplayName
    {
        get => _displayName;
        set
        {
            if (_displayName != value)
            {
                _displayName = value;
                OnPropertyChanged("DisplayName");
            }
        }
    }

    public ICellData NameCell => FirstCell;

    public IEnumerable<ICellData> ValueCells => AllButFirstCell;

    IReadonlyCellData INameValuesRow.NameCell => NameCell;

    IEnumerable<IReadonlyCellData> INameValuesRow.ValueCells => ValueCells;

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
