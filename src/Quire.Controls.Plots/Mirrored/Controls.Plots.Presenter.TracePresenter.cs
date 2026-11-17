#nullable disable
#pragma warning disable

using System;
using Quire.Calculation.Core;
using Quire.Calculation.Core.Trace2DResults;
using Quire.Controls.Core;
using Quire.Controls.Plots.Model;
using Quire.Controls.Plots.ViewModel;
using Quire.Wpf;

namespace Quire.Controls.Plots.Presenter;

internal partial class TracePresenter
{
    private readonly IPlotTrace _view;
    private readonly ITraceModel _model;
    private readonly PropertyChangedNotificationHandler _propertyChangedNotificationHandler = new PropertyChangedNotificationHandler();

    public TracePresenter(IPlotTrace view, ITraceModel model)
    {
        _view = view;
        _model = model;
        TraceViewModel viewModel = new TraceViewModel(_model);
        _view.ViewModel = viewModel;
        _view.ViewModel.ImageRendererViewModel.TargetDpi = ImageUtils.GetScreenDpiForApplication();
        _view.TraceChanged += ViewStyleChanged;
        ModelStyleChanged();
        ModelIsTraceCommittedChanged(_model.IsCommitted);
        ModelResultValidChanged(_model.ResultValid);
        ModelHasEquationErrorChanged(_model.Error != null);
        HandleModelCalculatingStatusChanged();
        HandleModelSynchronizationStatusChanged();
        _model.PlottingPointsChanged += ModelPlottingPointsChanged;
        _model.PlotCalculatableError += ModelPlotCalculatableError;
        _propertyChangedNotificationHandler.AddPropertyChangedNotificationHandler("CalculatingStatus", HandleModelCalculatingStatusChanged);
        _propertyChangedNotificationHandler.AddPropertyChangedNotificationHandler("SynchronizationStatus", HandleModelSynchronizationStatusChanged);
        _propertyChangedNotificationHandler.AddPropertyChangedNotificationHandler("IsNumOfPointsMutable", ModelIsNumOfPointsMutableChanged);
        _propertyChangedNotificationHandler.AttachPropertyChangedHandler(_model);
        _model.EquationError += ModelEquationError;
        _model.StyleChanged += ModelStyleChanged;
        _model.IsTraceCommittedChanged += ModelIsTraceCommittedChanged;
        _model.ResultValidChanged += ModelResultValidChanged;
        _view.TraceClicked += ViewTraceClicked;
        _view.NumberOfPointsChanged += ViewNumberOfPointsChanged;
        _view.RequestResetIsTypeDefault += ViewRequestResetIsTypeDefault;
    }

    private void ViewRequestResetIsTypeDefault()
    {
        _model.ResetIsTypeDefault();
    }

    private void ModelHasEquationErrorChanged(bool hasError)
    {
        _view.HasEquationError = hasError;
    }

    private void ModelResultValidChanged(bool resultValid)
    {
        _view.ResultValid = resultValid;
    }

    private void ModelIsTraceCommittedChanged(bool isCommitted)
    {
        _view.IsCommitted = isCommitted;
    }

    private void ViewTraceClicked(IPlotTrace obj)
    {
        AxesIndex id = ((_model.GetAxis(AxesIndex.Result).GetNumberOfTracesForEquation(_model.GetEquation(AxesIndex.Result)) == 1) ? AxesIndex.Result : AxesIndex.Range);
        _view.MoveFocusToEquation(id);
    }

    private void HandleModelCalculatingStatusChanged()
    {
        _view.CalculatingStatus = _model.CalculatingStatus;
    }

    private void HandleModelSynchronizationStatusChanged()
    {
        _view.SynchronizationStatus = _model.SynchronizationStatus;
    }

    private void ModelEquationError(EngineError equationError)
    {
        bool flag = equationError != null;
        _view.HasEquationError = flag;
        if (!flag)
        {
            _view.PlotCalculatableError = null;
        }
    }

    private void ModelPlotCalculatableError(object sender, EngineErrorEventArgs engineErrorEventArgs)
    {
        if ((AssertionE)DebugTools.DevAssert(sender == _model, null) == AssertionE.Passed)
        {
            _view.PlotCalculatableError = engineErrorEventArgs.EngineError;
            _view.HasEquationError = engineErrorEventArgs.EngineError != null;
        }
    }

    private void ModelPlottingPointsChanged()
    {
        _view.PlottingPoints = _model.GetPlottingPointsForDisplay();
    }

    private void ViewStyleChanged(IPlotTrace trace)
    {
        _model.Color = _view.Color;
        _model.Symbol = _view.Symbol;
        _model.SymbolWeight = _view.SymbolWeight;
        _model.SymbolFrequency = _view.SymbolFrequency;
        _model.LineStyle = _view.LineStyle;
        _model.LineWeight = _view.LineWeight;
        _model.Type = _view.Type;
        _model.Label = _view.Label;
    }

    private void ModelStyleChanged()
    {
        _view.TraceChanged -= ViewStyleChanged;
        _view.Color = _model.Color;
        _view.Symbol = _model.Symbol;
        _view.SymbolWeight = _model.SymbolWeight;
        _view.SymbolFrequency = _model.SymbolFrequency;
        _view.LineStyle = _model.LineStyle;
        _view.LineWeight = _model.LineWeight;
        _view.Type = _model.Type;
        _view.Label = _model.Label;
        _view.TraceChanged += ViewStyleChanged;
    }

    private void ViewNumberOfPointsChanged()
    {
        _model.NumberOfPoints = _view.NumberOfPoints;
        _model.Commit();
    }

    private void ModelIsNumOfPointsMutableChanged()
    {
        _view.IsNumOfPointsMutable = _model.IsNumOfPointsMutable;
        if (_view.IsNumOfPointsMutable)
        {
            _view.NumberOfPoints = _model.NumberOfPoints;
        }
    }
}
