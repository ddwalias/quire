namespace Quire.EquationEditor.SpecTable
{
    public partial interface ISpecTableView
    {
        global::Quire.Controls.Core.MathFormatDobjController MathFormat { get; }

        event global::Quire.Delegate2<global::Quire.Controls.Core.IEquationControl, global::Quire.Direction> AddColumn;

        event global::Quire.Delegate2<global::Quire.Controls.Core.IEquationControl, global::Quire.Direction> AddRow;

        event global::Quire.Delegate1<global::Quire.Controls.Core.IEquationControl> CellAdded;

        event global::Quire.Delegate1<global::Quire.Controls.Core.IEquationControl> DeleteColumn;

        event global::Quire.Delegate1<global::Quire.Controls.Core.IEquationControl> DeleteRow;

        event global::System.Action<global::Quire.Controls.Core.IEquationControl> RequestRemove;

        event global::System.Action<global::Quire.Controls.Core.IEquationControl, int, int, global::Quire.Direction> RequestRemoveSelection;

        event global::System.Action<global::Quire.Controls.Core.IEquationControl> RequestClearActiveCell;

        event ShouldHideCellEventHandler ShouldHideCell;

        event global::System.Action<global::Quire.Controls.Core.IEquationControl> OnTraverseForward;

        event global::System.Action<global::Quire.Controls.Core.IEquationControl> OnTraverseBackward;

        event global::System.Action NavigateToSpecTableLastCell;

        event global::System.Action NavigateToSpecTableFirstCell;

        event global::System.Action<bool> InputTagChanged;

        void Clear();

        void AddChild(global::Quire.Controls.Core.IEquationControl cellView, int rowIndex, int columnIndex);

        void SelectCells(int cellIndexA, int cellIndexB);

        void ClearSelection();

        void FireCellAdded(global::Quire.Controls.Core.IEquationControl eq);

        void SetDeserializedMathFormatData(global::Quire.Controls.Core.IEquationControl equation);

        void UpdateCellsVisibility();

        void HandleRestrictedReadonlyChanged(bool isRestrictedReadonly);
    }
}
