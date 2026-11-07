namespace Quire.EquationEditor.SpecTable.Model
{
    public partial interface ISpecTableMemento
    {
        void Restore(global::Quire.EquationEditor.SpecTable.ISpecTableModel model);
    }

    public partial interface IColumnMemento
    {
        ISpecTableColumn RestoreData();
    }
}

namespace Quire.EquationEditor.SpecTable
{
    public partial interface ISpecTableCellMap
    {
        global::System.Collections.Generic.IEnumerable<global::Quire.Controls.Core.IEquationControl> Views { get; }

        void Map(global::Quire.EquationEditor.IEquationModel eqModel, global::Quire.Controls.Core.IEquationControl eqView);

        global::Quire.Controls.Core.IEquationControl GetAssociatedView(global::Quire.EquationEditor.IEquationModel cellModel);

        global::Quire.EquationEditor.IEquationModel GetAssociatedModel(global::Quire.Controls.Core.IEquationControl viewObj);
    }

    public partial interface ISpecTableModel
    {
        global::System.Collections.Generic.List<global::Quire.EquationEditor.SpecTable.Model.ISpecTableColumn> Columns { get; }

        int ColumnCount { get; }

        string SExpression { get; }

        bool IsRestrictedReadOnly { get; }

        event global::Quire.Delegate1<global::Quire.EquationEditor.IEquationModel> CellAdded;

        event global::Quire.Delegate1<global::Quire.EquationEditor.SpecTable.Model.ISpecTableColumn> ColumnAdded;

        event global::Quire.Delegate0 TableChanged;

        event global::Quire.Delegate1<global::Quire.EquationEditor.SpecTable.Model.ISpecTableColumn> ColumnRemoved;

        event global::Quire.Delegate0 RequestRemove;

        event global::System.Action<int, int> RequestAddSelection;

        event global::System.Action RequestClearSelection;

        event global::System.Action RequestUpdateCellsVisibility;

        event global::System.Action<bool> RestrictedReadonlyChanged;

        void AddColumnAtEnd();

        void AddRowAtEnd();

        void AddColumn(global::Quire.EquationEditor.IEquationModel relativeToCell, global::Quire.Direction direction);

        void AddRow(global::Quire.EquationEditor.IEquationModel relativeToCell, global::Quire.Direction direction);

        void RemoveRow(global::Quire.EquationEditor.IEquationModel relativeToCell);

        void RemoveColumn(global::Quire.EquationEditor.IEquationModel relativeToCell);

        global::Quire.EquationEditor.SpecTable.Model.ISpecTableColumn GetColumn(int i);

        bool IsExtraDataCellOrEmptyScale(global::Quire.EquationEditor.IEquationModel cell);

        void HandleOnTraverseForward(global::Quire.EquationEditor.IEquationModel equationControl);

        void HandleOnTraverseBackward(global::Quire.EquationEditor.IEquationModel equationModel);

        void HandleNavigateToLastCell();

        void HandleNavigateToFirstCell();

        void HandleOnDelete(global::Quire.EquationEditor.IEquationModel equationModel);

        void HandleClearActiveCell(global::Quire.EquationEditor.IEquationModel equationModel);

        void HandleRemoveOrClearSelection(global::Quire.EquationEditor.IEquationModel equationModel, int cellIndexA, int cellIndexB, global::Quire.Direction direction);

        void HandleNavigationBetweenCells(global::Quire.EquationEditor.IEquationModel cell, global::Quire.Direction navigationDirection, bool isJumpNavigation);

        bool CanNavigationBetweenCells(global::Quire.EquationEditor.IEquationModel cell, global::Quire.Direction navigationDirection, bool isJumpNavigation);

        void Commit();

        global::System.Collections.Generic.IEnumerable<global::Quire.EquationEditor.SpecTable.Model.SpecTableCalculatableWrapper> GetWrappedColumns();

        global::Quire.EquationEditor.SpecTable.Model.SpecTableCalculatableWrapper GetColumnWrapper(global::Quire.EquationEditor.SpecTable.Model.ISpecTableColumn column);

        global::Quire.EquationEditor.Tree.MathNode CreateMatrixFromTableCells(global::Quire.DiscreteRect selected);

        global::Quire.PersistentDataObjects.mathType[] GetColumnsRegionPersistentData();

        void SetColumnsRegionPersistentData(global::Quire.PersistentDataObjects.mathType[] math);

        bool SubstituteAndCommitInputValue(string sExprId, string valueSExpr, string scalingSExpr, bool shouldMoveFocus = false);

        void HandleInputTagChanged(bool isRestrictedReadonly);

        void RemoveEmptyColumns();

        global::Quire.EquationEditor.SpecTable.Model.ISpecTableMemento CreateMemento();

        void RestoreMemento(global::Quire.EquationEditor.SpecTable.Model.SpecTableMemento modelMemento);
    }
}
