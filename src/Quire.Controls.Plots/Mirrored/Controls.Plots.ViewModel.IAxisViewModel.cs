using System;
using System.Windows.Input;
using Quire.Controls.Plots.Presenter;

namespace Quire.Controls.Plots.ViewModel;

public partial interface IAxisViewModel
{
    AxisRangeForXamlSerialization AxisRangeForXamlSerialization { get; set; }

    bool ShowTicks { get; set; }

    bool ShowNumbers { get; set; }

    event Func<FocusNavigationDirection, bool> CanTraverseEvent;

    event Action<FocusNavigationDirection> HandleTraverseEvent;

    void HookupView(IPlotAxisView axisView);

    void LimitEquationRequestFocus();
}
