#nullable disable
#pragma warning disable

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using Quire.Calculation.Core;
using Quire.Controls.Core;
using Quire.Controls.Plots.Presenter;

namespace Quire.Controls.Plots;

public partial class PlotEquationWOUnit
{
    [Obsolete("Usage of Exp property is deprecated. Use Expression instead.")]
    public EquationControl Exp => _exp;

    public override IEquationControl Expression => _exp;

    public IWhiteboardItem ExpressionItem => _exp;

    [Obsolete("Scaling EquationControl should not be exposed directly.")]
    public EquationControl Scaling => _scaling;

    public IWhiteboardItem ScalingItem => _scaling;

    public bool IsExpEmpty => Expression.EquationSExpression == SExpressionUtils.PlaceholderNode;

    public bool IsPlotEquationEmpty => IsExpEmpty;

    public string EmptyPlotEquationSExp => "(@TRACE_EXPR @PLACEHOLDER )";

    public bool AtLeftEdge => _exp.AtLeftEdge;

    public string RegionErrorSExp
    {
        get
        {
            if (_exp.ErrorSExpression != string.Empty)
            {
                DebugTools.DevAssert(false, null);
            }
            if (_exp.ErrorSExpression != string.Empty)
            {
                return _exp.ErrorSExpression;
            }
            return string.Empty;
        }
    }

    public string RegionErrorCode
    {
        get
        {
            if (_exp.ErrorCode != string.Empty)
            {
                DebugTools.DevAssert(false, null);
            }
            if (_exp.ErrorCode != string.Empty)
            {
                return _exp.ErrorCode;
            }
            return string.Empty;
        }
    }

    public NumberFormat DisplayNumberFormat
    {
        get
        {
            return (NumberFormat)GetValue(DisplayNumberFormatProperty);
        }
        set
        {
            SetValue(DisplayNumberFormatProperty, value);
            _exp.DisplayNumberFormat = DisplayNumberFormat;
        }
    }

    public EquationControl ActiveEquation { get; private set; }

    public IEquationControl FocusedEquation
    {
        get
        {
            if (!_exp.IsKeyboardFocusWithin)
            {
                return null;
            }
            return _exp;
        }
    }

    public SynchronizationStatus SynchronizationStatus => _exp.SynchronizationStatus;

    public CalculatingStatus CalculatingStatus => _exp.CalculatingStatus;

}
