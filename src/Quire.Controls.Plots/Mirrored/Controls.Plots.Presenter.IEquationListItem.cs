using System;

namespace Quire.Controls.Plots.Presenter;

public partial interface IEquationListItem
{
    IPlotEquation Equation { get; }
    bool IsItemActive { get; }
    event Action<EquationListItem> ItemChange;
    void ClearTraces();
}
