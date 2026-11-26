#nullable disable
#pragma warning disable

using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Quire;
using Quire.Calculation.Core;
using Quire.Controls.Core;
using Quire.Controls.Plots.Base3D.ViewModel;
using Quire.Controls.Plots.Plot3D.Model;
using Quire.Controls.Plots.Presenter;
using Quire.Controls.Plots.ViewModel;
using Quire.Undo;
using Quire.PersistentDataObjects;
using Quire.Controls.Plots.PlotCommands;

namespace Quire.Controls.Plots.Plot3D.ViewModel;

public sealed partial class PlotViewModel3D
{
    public SynchronizationStatus SynchronizationStatus => Model.SynchronizationStatus;

    internal Base3DAxisViewModel XAxisViewModel { get; private set; }

    internal Base3DAxisViewModel YAxisViewModel { get; private set; }

    internal Base3DAxisViewModel ZAxisViewModel { get; private set; }

    public override bool IsActiveRegion
    {
        get
        {
            return base.IsActiveRegion;
        }
        set
        {
            base.IsActiveRegion = value;
            if (ActiveAxis != null)
            {
                ActiveAxis.IsActive = value;
            }
        }
    }

    public Size GridCellSize
    {
        get
        {
            return _gridCellSize;
        }
        internal set
        {
            if (_gridCellSize != value)
            {
                _gridCellSize = value;
                OnPropertyChanged("GridCellSize");
            }
        }
    }

    internal Base3DAxisViewModel HighlightedAxis
    {
        get
        {
            return _highlightedAxis;
        }
        set
        {
            if (value == _activeAxis)
            {
                return;
            }
            if (DebugTools.DevAssert(value == null || value == XAxisViewModel || value == YAxisViewModel || value == ZAxisViewModel, null).Failed)
            {
                return;
            }
            if (_highlightedAxis != null)
            {
                _highlightedAxis.IsHighlighted = false;
            }
            _highlightedAxis = value;
            if (_highlightedAxis != null)
            {
                _highlightedAxis.IsHighlighted = true;
            }
        }
    }

    internal IPlotAxisViewModel3D ActiveAxis
    {
        get
        {
            return _activeAxis;
        }
        set
        {
            if (value == _activeAxis)
            {
                return;
            }
            if (DebugTools.DevAssert(value == XAxisViewModel || value == YAxisViewModel || value == ZAxisViewModel, null).Failed)
            {
                return;
            }
            if (_activeAxis != null)
            {
                _activeAxis.IsActive = false;
            }
            _activeAxis = value;
            _activeAxis.IsActive = IsActiveRegion;
        }
    }

    internal plotType XamlDataObject
    {
        get
        {
            return Model.GetDataObject(null, null, HideFromCalculationContext);
        }
        set
        {
            Model.SetDataObject(null, value, null);
        }
    }

    public Transform3D CameraTransform
    {
        get
        {
            return Model.CameraTransform;
        }
        set
        {
            Model.CameraTransform = value;
        }
    }

    public CameraType3D PlotCameraType
    {
        get
        {
            return Model.PlotCameraType;
        }
        set
        {
            Model.PlotCameraType = value;
        }
    }

    public CameraControlState CameraControlState
    {
        get
        {
            return _cameraControlState;
        }
        set
        {
            _cameraControlState = value;
            OnPropertyChanged("CameraControlState");
        }
    }

    public IPlotTraceViewModel3D ActiveTrace
    {
        get
        {
            return _activeTrace;
        }
        private set
        {
            if (_activeTrace != value)
            {
                _activeTrace = value;
                OnPropertyChanged("ActiveTrace");
            }
        }
    }

    private IPlotEquationViewModel3D ActivePlotEquation
    {
        get
        {
            return _activePlotEquation;
        }
        set
        {
            if (_activePlotEquation != value)
            {
                _activePlotEquation = value;
                UpdateActiveTrace();
            }
        }
    }

    public bool EquationListHasFocus
    {
        get
        {
            return _equationListHasFocus;
        }
        set
        {
            if (_equationListHasFocus != value)
            {
                _equationListHasFocus = value;
                UpdateActiveTrace();
            }
        }
    }

    private bool CanExecuteChangeTraceCommand(object argument)
    {
        return !SelectedTraces.IsNullOrEmpty();
    }

    private void HandleSurfaceFill(object argument)
    {
        Color? newColor = ((argument == null) ? ((Color?)null) : new Color?(((SolidColorBrush)argument).Color));
        CommandBuffer.Execute(new SurfaceFillCommand(SelectedTraces, newColor));
    }

    internal void TrackballMouseUp(Matrix3D previousTransform)
    {
        AddChangeTransformCommand(null, previousTransform, isAttachable: false);
    }

    internal void TrackballMouseWheel(Matrix3D previousTransform)
    {
        AddChangeTransformCommand(null, previousTransform, isAttachable: true);
    }

    private void AddChangeTransformCommand(Matrix3D? newTransform, Matrix3D previousTransform, bool isAttachable)
    {
        CommandBuffer.Execute(new ChangeCameraTransformCommand(this, newTransform, previousTransform, isAttachable));
    }

    private void HandleRefitCamera(object commandParameter)
    {
        AddChangeTransformCommand(Matrix3D.Identity, CameraTransform.Value, isAttachable: false);
    }

    private void InitializeAxis()
    {
        XAxisViewModel = new Plot3DAxisViewModel(AxesType.X);
        AddAxisViewModelEventHandlers(XAxisViewModel);
        Model.XAxis = XAxisViewModel.PlotAxisModel;
        YAxisViewModel = new Plot3DAxisViewModel(AxesType.Y);
        AddAxisViewModelEventHandlers(YAxisViewModel);
        Model.YAxis = YAxisViewModel.PlotAxisModel;
        ZAxisViewModel = new Plot3DAxisViewModel(AxesType.Z);
        AddAxisViewModelEventHandlers(ZAxisViewModel);
        Model.ZAxis = ZAxisViewModel.PlotAxisModel;
    }

    private void AddAxisViewModelEventHandlers(Base3DAxisViewModel axisViewModel)
    {
        axisViewModel.CalculatableAdded += OnCalculatableAdded;
    }

    public void HandleRequestAddEquation(int index, int previousIndex)
    {
        CommandBuffer.Execute(new AddEquationCommand(Model, index, previousIndex));
    }

    public void HandleEquationRemoved(IPlotEquation equation, int deletedItemIndex, int previousItemIndex, Direction direction)
    {
        CommandBuffer.Execute(new RemoveEquationCommand(Model, equation, deletedItemIndex, previousItemIndex, direction));
    }

    private void ModelEquationAddedHandler(int index, int previousIndex, IPlotEquationModel3D equationModel)
    {
        DebugTools.DevAssert(equationModel.EquationProxy == null || equationModel.EquationProxy.ViewModel != null, null);
        IPlotEquationViewModel3D plotEquationViewModel3D = ((equationModel.EquationProxy != null) ? (equationModel.EquationProxy.ViewModel as PlotEquationViewModel3D) : null) ?? new PlotEquationViewModel3D(equationModel);
        _equations.Insert(index, plotEquationViewModel3D);
        FireEquationAdded(index, previousIndex, plotEquationViewModel3D);
    }

    private void ModelEquationRemovedHandler(int index, int previousIndex, Direction direction)
    {
        _equations.RemoveAt(index);
        FireEquationRemoved(index, previousIndex, direction);
    }

    private void FireEquationAdded(int index, int previousIndex, IPlotEquationViewModel3D equationViewModel3D)
    {
        if (EquationAdded != null)
        {
            EquationAdded(index, previousIndex, equationViewModel3D);
        }
    }

    private void FireEquationRemoved(int index, int previousIndex, Direction direction)
    {
        if (EquationRemoved != null)
        {
            EquationRemoved(index, previousIndex, direction);
        }
    }

    public void FireRequestActivation()
    {
        if (RequestActivation != null)
        {
            RequestActivation();
        }
    }

    private void ModelTraceInserted(IPlotTraceModel3D traceModel, int index)
    {
        IPlotTraceViewModel3D plotTraceViewModel3D = new PlotTraceViewModel3D(traceModel);
        _traces.Insert(index, plotTraceViewModel3D);
        OnCalculatableAdded(traceModel);
        FireTraceInserted(plotTraceViewModel3D, index);
    }

    private void FireTraceInserted(IPlotTraceViewModel3D traceViewModel, int index)
    {
        if (TraceInserted != null)
        {
            TraceInserted(traceViewModel, index);
        }
    }

    private void FireTraceRemoved(IPlotTraceViewModel3D traceViewModel)
    {
        if (TraceRemoved != null)
        {
            TraceRemoved(traceViewModel);
        }
    }

    public void RequestResetTrackball()
    {
        Action resetTrackball = ResetTrackball;
        if (resetTrackball != null)
        {
            resetTrackball();
        }
    }

    private void HandleChangeNumOfPoints(object obj)
    {
        TraceSetNumberOfPointsCommand command = new TraceSetNumberOfPointsCommand(SelectedTraces, (Pair<int, bool>)obj, RequestActivation);
        command.Execute();
    }
}
