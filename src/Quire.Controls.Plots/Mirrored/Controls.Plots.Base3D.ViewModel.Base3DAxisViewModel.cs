#nullable disable
#pragma warning disable

using System;
using System.Windows.Input;
using System.Windows.Media;
using Quire.Calculation.Core;
using Quire.Controls.Plots.Presenter;

namespace Quire.Controls.Plots.Base3D.ViewModel;

public abstract partial class Base3DAxisViewModel
{
    private bool _isActive;

    private bool _isHighlighted;

    private Color _lineColor;

    private static readonly Color _defaultLineColor = new Color
    {
        A = byte.MaxValue,
        R = 112,
        G = 112,
        B = 112
    };

    private static readonly Color _higlightedLineColor = Colors.Black;

    private static readonly Color _activeLineColor = Colors.DodgerBlue;

    public Color LineColor
    {
        get
        {
            return _lineColor;
        }
        private set
        {
            if (!_lineColor.Equals(value))
            {
                _lineColor = value;
                FirePropertyChanged("LineColor");
            }
        }
    }

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
