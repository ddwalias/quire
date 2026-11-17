#nullable disable
#pragma warning disable

using System;
using System.ComponentModel;
using System.Xml;
using Quire.Controls.Core;

namespace Quire.Controls.Plots.Presenter;

internal partial class AxisPresenter
{
    private readonly IPlotAxisView _view;
    private readonly IAxisModel _model;

    public AxisPresenter(IPlotAxisView view, IAxisModel model)
    {
        _view = view;
        _model = model;
        _view.Changed += ViewAxis_Changed;
        _view.NumberFormatChanged += _view_NumberFormatChanged;
        _view.LegendWidthChanged += _view_LegendWidthChanged;
        _model.NumberFormatInitialized += _model_NumberFormatInitialized;
        _model.RequestAddEquation += modelAxis_RequestAddEquation;
        _model.Changed += ModelAxis_Changed;
        _model.RangeChanged += ModelAxis_RangeChanged;
        EquationControl startExpressionEqn = _view.StartExpressionEqn;
        _model.StartExpression = (_view.StartLimitEditor = new AxisLimitEditor(startExpressionEqn, startExpressionEqn));
        EquationControl secondTickExpressionEqn = _view.SecondTickExpressionEqn;
        _model.SecondTickExpression = (_view.SecondTickEditor = new AxisLimitEditor(secondTickExpressionEqn, secondTickExpressionEqn));
        EquationControl endExpressionEqn = _view.EndExpressionEqn;
        _model.EndExpression = (_view.EndLimitEditor = new AxisLimitEditor(endExpressionEqn, endExpressionEqn));
        _model.StartExpression.HasErrorsChanged += StartExpression_HasErrorsChanged;
        _model.SecondTickExpression.HasErrorsChanged += SecondTickExpression_HasErrorsChanged;
        _model.EndExpression.HasErrorsChanged += EndExpression_HasErrorsChanged;
        _model.ScalingChanged += _model_ScalingChanged;
        _view.PropertyChanged += _view_PropertyChanged;
        _view_NumberFormatChanged(_view.DisplayNumberFormat, _view.IsDisplayNumberFormatLocal);
        _view.TickMarksValues = _model.MajorTickMarks.Values;
        ModelAxis_Changed();
    }

    private void _view_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if ("Scaling".Equals(e.PropertyName))
        {
            _model.Scaling = _view.Scaling;
        }
    }

    private void _view_LegendWidthChanged(double width)
    {
        _model.LegendWidth = width;
    }

    private void _model_ScalingChanged(AxisScaling scaling)
    {
        _view.Scaling = _model.Scaling;
    }

    private void _model_NumberFormatInitialized()
    {
        _view.OnNumberFormatInitialized(_model.DisplayNumberFormat);
        _model.NumberFormatInitialized -= _model_NumberFormatInitialized;
    }

    private void _view_NumberFormatChanged(NumberFormat numberFormat, bool isLocalValue)
    {
        _model.SetDisplayNumberFormat(numberFormat, isLocalValue);
        _model.FormatTickMarkValues();
        _view.Labels = _model.MajorTickMarks.Labels;
    }

    private void ViewAxis_Changed()
    {
        _model.PositionTickMark = _view.PositionTickMark;
        _model.Position = _view.Position;
        _model.ShowNumbers = _view.ShowNumbers;
        _model.MajorTickMarks.Show = _view.ShowTicks;
        _model.ShowGridLines = _view.ShowGridLines;
        _model.GridLineColor = _view.GridLineColor;
    }

    private IPlotEquation modelAxis_RequestAddEquation(IAxisModel axisModel, XmlNode expression)
    {
        return _view.AddEquation(expression);
    }

    private void ModelAxis_Changed()
    {
        _view.Changed -= ViewAxis_Changed;
        _view.PositionTickMark = _model.PositionTickMark;
        _view.Position = _model.Position;
        _view.ShowNumbers = _model.ShowNumbers;
        _view.ShowTicks = _model.MajorTickMarks.Show;
        _view.LegendActualWidth = _model.LegendWidth;
        _view.ShowGridLines = _model.ShowGridLines;
        _view.GridLineColor = _model.GridLineColor;
        _view.Changed += ViewAxis_Changed;
    }

    private void ModelAxis_RangeChanged()
    {
        _view.IsDiscrete = _model.IsDiscrete;
        _view.Labels = _model.MajorTickMarks.Labels;
        _view.TickMarksValues = _model.MajorTickMarks.Values;
        _view.TickMarks = _model.MajorTickMarks.Count;
        _view.SetTickmarkPattern(_model.MajorTickMarks.StartGap, _model.MajorTickMarks.Pattern);
        _view.TickMarksVisabilityPattern = _model.MajorTickMarks.VisabilityPattern;
        _view.MinorTickmarks = _model.MinorTickMarks;
        _view.SetRange(_model.Start, _model.End);
    }

    private void StartExpression_HasErrorsChanged()
    {
        if (_model.StartExpression.HasErrors)
        {
            _view.ShowStartExpression();
        }
        else
        {
            _view.HideStartExpression();
        }
    }

    private void SecondTickExpression_HasErrorsChanged()
    {
        if (_model.SecondTickExpression.HasErrors)
        {
            _view.ShowSecondTickExpression();
        }
        else
        {
            _view.HideSecondTickExpression();
        }
    }

    private void EndExpression_HasErrorsChanged()
    {
        if (_model.EndExpression.HasErrors)
        {
            _view.ShowEndExpression();
        }
        else
        {
            _view.HideEndExpression();
        }
    }
}
