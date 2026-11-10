namespace Quire.Controls.InputControls;

public partial class ComboBoxViewModel
{

    public Quire.Controls.InputControls.EditableComboBoxViewModel CreatedEditableViewModel()
    {
        return new EditableComboBoxViewModel(TableModel);
    }

    public string TableTitle => TableModel.TableTitle;

    public System.Collections.Generic.IEnumerable<Quire.Controls.InputControls.INameValuesRow> DataItems
    {
        get => _dataItems;
        private set
        {
            _dataItems = value;
            OnPropertyChanged(nameof(DataItems));
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
            OnPropertyChanged(nameof(RefreshInProgress));
        }
    }
}
