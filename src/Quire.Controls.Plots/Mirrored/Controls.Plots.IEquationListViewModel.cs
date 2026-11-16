namespace Quire.Controls.Plots;

public partial interface IEquationListViewModel
{
    bool EquationListHasFocus { get; set; }
    void HandleRequestAddEquation(int index, int previousIndex);
    void HandleEquationRemoved(Presenter.IPlotEquation plotEquation, int deletedItemIndex, int previousItemIndex, Direction direction);
}
