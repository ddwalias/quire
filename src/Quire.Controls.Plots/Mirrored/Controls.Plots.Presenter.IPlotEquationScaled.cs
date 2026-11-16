using System;

namespace Quire.Controls.Plots.Presenter;

public partial interface IPlotEquationScaled
{
    event Action<IPlotEquationScaled> ScalingChanged;
    void ShowOrHideScaling(bool isEquationListKeyboardFocusWithin);
}
