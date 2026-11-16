using System;
using System.Collections.Generic;

namespace Quire.Controls.Plots.Presenter;

public partial interface IPlotEquationBase
{
    bool IsCommitted { get; }
    IEnumerable<Core.IEquationControl> ContainedEquationControls { get; }
    event Action SExpressionValueChanged;
    void ResetBeforeAdding();
}
