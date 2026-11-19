#nullable disable
#pragma warning disable

using System;
using System.Windows;
using Quire.Controls.Core;
using Quire.Controls.Plots.Presenter;

namespace Quire.Controls.Plots;

public partial class PlotEquation
{
    public Visibility ScalingHeaderVisibility
    {
        get
        {
            return (Visibility)GetValue(ScalingHeaderVisibilityProperty);
        }
        private set
        {
            SetValue(_scalingHeaderVisibilityPropertyKey, value);
        }
    }

    public string ScalingRl
    {
        get
        {
            string rl = GetUserScalingExpression().Rl;
            if (!IsScalingEmpty && ScalingChangedByUser)
            {
                return rl;
            }
            return "(number 1)";
        }
    }

    [Obsolete("Usage of Exp property is deprecated. Use Expression instead.")]
    public EquationControl Exp => _exp;

    public override IEquationControl Expression => _exp;

    [Obsolete("Scaling EquationControl should not be exposed directly.")]
    public EquationControl Scaling => _scaling;

    public IWhiteboardItem ScalingItem => _scaling;

    public bool IsScalingEmpty => ScalingEquation.EquationSExpression == SExpressionUtils.PlaceholderNode;

    public bool IsExpEmpty => Expression.EquationSExpression == SExpressionUtils.PlaceholderNode;

    public bool IsPlotEquationEmpty
    {
        get
        {
            if (IsScalingEmpty)
            {
                return IsExpEmpty;
            }
            return false;
        }
    }

    public bool ScalingChangedByUser { get; private set; }

    public string UserScalingExpression { get; private set; }

    public string EmptyPlotEquationSExp => FormatPlotEquationSExpression("@PLACEHOLDER", "@PLACEHOLDER");
}
