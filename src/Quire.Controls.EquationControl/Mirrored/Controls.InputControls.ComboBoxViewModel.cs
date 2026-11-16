using System.Collections.Generic;
using System.Linq;
using Quire.Core;
using Quire.EquationEditor.Tree;

namespace Quire.Controls.InputControls;

public partial class ComboBoxViewModel
{
    private int _selectedRowNumber;
    private IEnumerable<INameValuesRow> _dataItems;
    private bool _refreshInProgress;

    public ComboBoxViewModel()
        : base(new ComboBoxTableModel())
    {
        DataItems = TableModel.NameValuesRows;
    }

    public string TableTitle => TableModel.TableTitle;

    public IEnumerable<INameValuesRow> DataItems
    {
        get => _dataItems;
        private set
        {
            _dataItems = value;
            OnPropertyChanged("DataItems");
        }
    }

    public int SelectedRowIndex
    {
        get => _selectedRowNumber;
        set
        {
            TableModel.SelectedNameValuesRowIndex = value;
            if (_selectedRowNumber != value)
            {
                _selectedRowNumber = value;
                OnPropertyChanged("SelectedRowIndex");
            }
        }
    }

    public bool RefreshInProgress
    {
        get => _refreshInProgress;
        private set
        {
            if (value == _refreshInProgress)
            {
                return;
            }

            _refreshInProgress = value;
            OnPropertyChanged("RefreshInProgress");
        }
    }

    public void LoadFrom(IComboBoxInputControlDataAdapter data)
    {
        TableModel.UpdateComboBoxTable(data);
        RefreshDataItems();
    }

    public void RefreshDataItems()
    {
        int selectedNameValuesRowIndex = TableModel.SelectedNameValuesRowIndex;
        bool isUsingFirstValueAsName = TableModel.NameValuesRows.All((INameValuesRow row) => row.NameCell.Text.IsNullOrEmpty());
        TableModel.NameValuesRows.Each((INameValuesRow nameValuesRow) =>
        {
            string text = (isUsingFirstValueAsName ? nameValuesRow.ValueCells.First().Text : nameValuesRow.NameCell.Text);
            nameValuesRow.DisplayName = TableTitle + (TableTitle.IsNotEmpty() ? ": " : string.Empty) + text;
        });
        DebugTools.DevAssert(!RefreshInProgress, null);
        try
        {
            RefreshInProgress = true;
            DataItems = TableModel.NameValuesRows.ToList();
            SelectedRowIndex = selectedNameValuesRowIndex;
        }
        finally
        {
            RefreshInProgress = false;
        }
    }

    public EditableComboBoxViewModel CreatedEditableViewModel()
    {
        return new EditableComboBoxViewModel(TableModel);
    }
}
