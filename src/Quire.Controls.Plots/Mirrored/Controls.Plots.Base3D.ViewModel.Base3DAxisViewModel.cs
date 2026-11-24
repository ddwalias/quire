#nullable disable
#pragma warning disable

using System;
using System.Windows.Input;
using Quire.Calculation.Core;
using Quire.Controls.Plots.Presenter;

namespace Quire.Controls.Plots.Base3D.ViewModel;

public abstract partial class Base3DAxisViewModel
{
    private void FireRangeChanged()
    {
        if (RangeChanged != null)
        {
            RangeChanged();
        }
    }

    private void InitializeLimitExpression(IAxisLimitEditor limitExpression, Action limitValueChangedHandler)
    {
        if (limitExpression != null)
        {
            FireCalculatableAdded(limitExpression);
            limitExpression.ValueChanged += limitValueChangedHandler;
        }
    }

    private void FireCalculatableAdded(ICalculatable calculatable)
    {
        if (CalculatableAdded != null)
        {
            CalculatableAdded(calculatable);
        }
    }

    public void HandleTraverse(FocusNavigationDirection direction)
    {
        if (HandleTraverseEvent != null)
        {
            HandleTraverseEvent(direction);
        }
    }

    public bool CanTraverse(FocusNavigationDirection direction)
    {
        if (CanTraverseEvent != null)
        {
            return CanTraverseEvent(direction);
        }
        return false;
    }
}
