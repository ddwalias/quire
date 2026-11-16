using System.Collections.Generic;
using System.Linq;
using Quire.EquationEditor.Tree;
using Quire.EquationEditor.View;

namespace Quire.Controls.InputControls;

public abstract partial class ComboBoxViewModelBase
{
    [global::Quire.Mirror.MirrorOf("Ptc.Controls.InputControls.ComboBoxViewModelBase+ComboBoxTableModelAdapter, Ptc.Controls.EquationControl")]
    private partial class ComboBoxTableModelAdapter : IComboBoxInputControlDataAdapter
    {
        private readonly IComboBoxTableModel _comboBoxTableModel;

        public ComboBoxTableModelAdapter(IComboBoxTableModel comboBoxTableModel)
        {
            _comboBoxTableModel = comboBoxTableModel;
        }

        public IList<string> Units => _comboBoxTableModel.UnitsForValueColumns.Select((IReadonlyCellData cell) => cell.Text).ToList();

        public IList<(string, IList<string>)> GroupByRows => _comboBoxTableModel.NameValuesRows.Select((INameValuesRow row) => (row.NameCell.Text, (IList<string>)row.ValueCells.Select((IReadonlyCellData cell) => cell.Text).ToList())).ToList();

        public string TableTitle => _comboBoxTableModel.TableTitle;

        public int SelectedValueRow => _comboBoxTableModel.SelectedNameValuesRowIndex;
    }

    protected ComboBoxTableModel TableModel { get; }

    protected ComboBoxViewModelBase(ComboBoxTableModel tableModel)
    {
        TableModel = tableModel;
    }

    public IComboBoxInputControlDataAdapter CreateDataAdapter()
    {
        return new ComboBoxTableModelAdapter(TableModel);
    }
}
