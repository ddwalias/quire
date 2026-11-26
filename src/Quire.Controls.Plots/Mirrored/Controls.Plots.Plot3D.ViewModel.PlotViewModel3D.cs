#nullable disable
#pragma warning disable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Quire;
using Quire.Calculation.Core;
using Quire.Controls.Core;
using Quire.Controls.Plots.Base3D.ViewModel;
using Quire.Controls.Plots.Plot3D.Model;
using Quire.Controls.Plots.PlotCommands;
using Quire.Controls.Plots.Presenter;
using Quire.Controls.Plots.View;
using Quire.Controls.Plots.ViewModel;
using Quire.PersistentDataObjects;
using Quire.Undo;
using Quire.Wpf;

namespace Quire.Controls.Plots.Plot3D.ViewModel;

public sealed partial class PlotViewModel3D
{
    internal new sealed partial class PropertyNames : PlotViewModelBase<PlotModel3D>.PropertyNames
    {
        internal const string CameraControlState = "CameraControlState";

        internal const string GridCellSize = "GridCellSize";

        internal const string CameraTransform = "CameraTransform";

        internal const string PlotCameraType = "PlotCameraType";

        internal const string ActiveTrace = "ActiveTrace";

        private PropertyNames()
        {
        }
    }

    private static readonly Type _typeOfThis = typeof(PlotViewModel3D);

    public static readonly RoutedCommand RefitCameraCommand = new RoutedCommand("RefitCamera", _typeOfThis);

    public static readonly RoutedCommand SurfaceColorCommand = new RoutedCommand("SurfaceColor", _typeOfThis);

    private static readonly double _initialGraphDimensionInDips = WpfUtils.InchesToDips(2.4);

    private readonly IList<IPlotTraceViewModel3D> _traces = new List<IPlotTraceViewModel3D>();

    private readonly IList<IPlotEquationViewModel3D> _equations = new List<IPlotEquationViewModel3D>();

    internal event Action<IPlotTraceViewModel3D, int> TraceInserted;

    internal event Action<IPlotTraceViewModel3D> TraceRemoved;

    internal event Action<int, int, IPlotEquationViewModel3D> EquationAdded;

    internal event Action<int, int, Direction> EquationRemoved;

    internal event Action RequestActivation;

    internal Base3DAxisViewModel XAxisViewModel { get; private set; }

    internal Base3DAxisViewModel YAxisViewModel { get; private set; }

    internal Base3DAxisViewModel ZAxisViewModel { get; private set; }

    internal PlotViewModel3D()
    {
        Model = new PlotModel3D();
        Model.TraceInserted += ModelTraceInserted;
        Model.TraceRemoved += ModelTraceRemoved;
        Model.EquationAdded += ModelEquationAddedHandler;
        Model.EquationRemoved += ModelEquationRemovedHandler;
        Model.PlotCalculatableError += ModelPlotCalculatableError;
        InitializeAxis();
        BindCommandsAndEvents();
        GraphHeight = (GraphWidth = _initialGraphDimensionInDips);
        ActiveAxis = ZAxisViewModel;
    }

    protected override void BindCommandsAndEvents()
    {
        base.BindCommandsAndEvents();
        CreateCommandBinding(RefitCameraCommand, HandleRefitCamera);
        CreateCommandBinding(RegionCommands.ToggleOrSetPerspectiveCameraCommand, HandleTogglePerspectiveCamera);
        CreateCommandBinding(PlotControlBase.ChangeTraceColorCommand, delegate(object arg)
        {
            CommandBuffer.Execute(new TraceColorCommand(SelectedTraces, ((SolidColorBrush)arg).Color));
        }, CanExecuteChangeTraceCommand);
        CreateCommandBinding(PlotControlBase.ChangeTraceWeightCommand, delegate(object arg)
        {
            CommandBuffer.Execute(new TraceWeightCommand(SelectedTraces, (double)arg));
        }, CanExecuteChangeTraceCommand);
        CreateCommandBinding(SurfaceColorCommand, HandleSurfaceFill, CanSurfaceFill);
        CreateCommandBinding(PlotControlBase.ChangeTraceLineCommand, HandleChangeTraceLineStyle, CanChangeTraceLine);
        CreateCommandBinding(PlotControlBase.ChangeTraceSymbolCommand, HandleChangeTraceSymbol, CanChangeTraceSymbol);
        CreateCommandBinding(PlotControlBase.ChangeNumOfPointsCommand, HandleChangeNumOfPoints);
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

    private bool CanSurfaceFill(object obj)
    {
        if (SelectedTraces.IsNullOrEmpty())
        {
            return false;
        }
        return SelectedTraces.All((IPlotTraceViewModel3D tr) => tr.VisualDataTypeOrDefault == TraceType3D.Surface);
    }

    protected override void OnPropertyChanged(string propertyName)
    {
        base.OnPropertyChanged(propertyName);
        if (propertyName == "GraphHeight")
        {
            XAxisViewModel.Height = YAxisViewModel.Height = ZAxisViewModel.Height = GraphHeight;
        }
    }

    private IEnumerable<IPlotTraceViewModel3D> SelectedTraces
    {
        get
        {
            if (ActiveTrace != null)
            {
                yield return ActiveTrace;
            }
        }
    }

    private void HandleChangeTraceSymbol(object argument)
    {
        if (DebugTools.DevAssert(argument is TraceSymbol, null).Failed)
        {
            return;
        }
        CommandBuffer.Execute(new TraceSymbolCommand(SelectedTraces, (TraceSymbol)argument));
        SelectedTraces.Each((IPlotTraceViewModel3D tr) =>
        {
            tr.UserChangedStyle();
        });
    }

    private void HandleChangeTraceLineStyle(object argument)
    {
        if (DebugTools.DevAssert(argument is LineStyle, null).Failed)
        {
            return;
        }
        CommandBuffer.Execute(new TraceLineCommand(SelectedTraces, (LineStyle)argument));
        SelectedTraces.Each((IPlotTraceViewModel3D tr) =>
        {
            tr.UserChangedStyle();
        });
    }

    private bool CanChangeTraceSymbol(object argument)
    {
        if (SelectedTraces.IsNullOrEmpty())
        {
            return false;
        }
        if (argument == null)
        {
            return true;
        }
        if (DebugTools.DevAssert(argument is TraceSymbol, null).Failed)
        {
            return false;
        }
        // Release IL branches on != point/Solid then == none/None with false/true returns at method tail.
        TraceSymbol traceSymbol = (TraceSymbol)argument;
        if (traceSymbol != TraceSymbol.point)
        {
            if (traceSymbol == TraceSymbol.none)
            {
                return !SelectedTraces.Any((IPlotTraceViewModel3D tr) => tr.LineStyle == LineStyle.None);
            }
            return false;
        }
        return true;
    }

    private bool CanChangeTraceLine(object argument)
    {
        if (SelectedTraces.IsNullOrEmpty())
        {
            return false;
        }
        if (argument == null)
        {
            return true;
        }
        if (DebugTools.DevAssert(argument is LineStyle, null).Failed)
        {
            return false;
        }
        // Release IL branches on != Solid then == None with false/true returns at method tail.
        LineStyle lineStyle = (LineStyle)argument;
        if (lineStyle != LineStyle.Solid)
        {
            if (lineStyle == LineStyle.None)
            {
                return !SelectedTraces.Any((IPlotTraceViewModel3D tr) => tr.Symbol == TraceSymbol.none);
            }
            return false;
        }
        return true;
    }

    private void HandleTogglePerspectiveCamera(object parameter)
    {
        CameraType3D oldType = PlotCameraType;
        CameraType3D newType = RegionCommands.ValueForToggleOrSetPerspectiveCameraCommand(parameter, oldType);
        CommandBuffer.Execute(new UndoableCommand("ChangePlotCameraType", () =>
        {
            bool flag = oldType != newType;
            if (flag)
            {
                PlotCameraType = newType;
                DebugTools.DevAssert(PlotCameraType == newType, null);
            }
            return flag;
        }, () =>
        {
            PlotCameraType = oldType;
        }));
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

    private void ModelTraceRemoved(IPlotTraceModel3D traceModel)
    {
        try
        {
            IPlotTraceViewModel3D plotTraceViewModel3D = _traces.First((IPlotTraceViewModel3D vm) => vm.IsOwnModel(traceModel));
            _traces.Remove(plotTraceViewModel3D);
            OnCalculatableRemoved(traceModel);
            FireTraceRemoved(plotTraceViewModel3D);
        }
        catch (InvalidOperationException ex)
        {
            DebugTools.DevAssert(false, ex.Message);
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

    private Size _gridCellSize;

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

    private Base3DAxisViewModel _highlightedAxis;

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

    private IPlotAxisViewModel3D _activeAxis;

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

    public SynchronizationStatus SynchronizationStatus => Model.SynchronizationStatus;

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

    public event Action ResetTrackball;

    public void RequestResetTrackball()
    {
        // Release IL requires an explicit local to emit stloc.0 rather than dup.
        Action handler = ResetTrackball;
        if (handler != null)
        {
            handler();
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

    private CameraControlState _cameraControlState = CameraControlState.Spin;

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

    private IPlotTraceViewModel3D _activeTrace;

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

    private IPlotEquationViewModel3D _activePlotEquation;

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

    private bool _equationListHasFocus;

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

    private void UpdateActiveTrace()
    {
        IPlotTraceViewModel3D plotTraceViewModel3D = null;
        if (EquationListHasFocus)
        {
            plotTraceViewModel3D = _traces.SingleOrDefault((IPlotTraceViewModel3D trace) => trace.IsOwnModel(ActivePlotEquation.TraceModel));
            DebugTools.AssertNotNull(plotTraceViewModel3D, null);
        }
        ActiveTrace = plotTraceViewModel3D;
    }

    // Placed in Mathcad member order (between UpdateActiveTrace and ApplyActivePlotEquation) so closure ordinals 113 and 115 match.
    [global::Quire.Mirror.MirrorOf("Ptc.Controls.Plots.Plot3D.ViewModel.PlotViewModel3D+AddEquationCommand, Ptc.Controls.Plots")]
    private class AddEquationCommand : IUndoableCommand
    {
        private readonly int _index;

        private readonly int _previousIndex;

        private readonly PlotModel3D _model;

        private IPlotEquationModel3D _equationModel;

        public AddEquationCommand(PlotModel3D plot3DModel, int index, int previousIndex)
        {
            _index = index;
            _previousIndex = previousIndex;
            _model = plot3DModel;
        }

        public bool Do()
        {
            _equationModel = _model.AddNewEquation(_index, _previousIndex);
            return true;
        }

        public void Undo()
        {
            _model.RemoveEquation(_equationModel.EquationProxy, _index, _previousIndex, Direction.Indeterminate);
        }

        public void Redo()
        {
            _model.AddEquation(_equationModel, _index, _previousIndex);
        }

        public string Name => "Add trace";
    }

    [global::Quire.Mirror.MirrorOf("Ptc.Controls.Plots.Plot3D.ViewModel.PlotViewModel3D+RemoveEquationCommand, Ptc.Controls.Plots")]
    private class RemoveEquationCommand : IUndoableCommand
    {
        private readonly int _index;

        private readonly int _previousIndex;

        private readonly PlotModel3D _model;

        private readonly IPlotEquation _equationProxy;

        private IPlotEquationModel3D _equationModel;

        private readonly Direction _direction;

        public RemoveEquationCommand(PlotModel3D plot3DModel, IPlotEquation equationProxy, int deletedItemIndex, int priviousItemIndex, Direction direction)
        {
            _index = deletedItemIndex;
            _previousIndex = priviousItemIndex;
            _equationProxy = equationProxy;
            _direction = direction;
            _model = plot3DModel;
        }

        public bool Do()
        {
            _equationModel = _model.RemoveEquation(_equationProxy, _index, _previousIndex, _direction);
            return true;
        }

        public void Undo()
        {
            _model.AddEquation(_equationModel, _index, _previousIndex);
        }

        public void Redo()
        {
            Do();
        }

        public string Name => "Remove trace";
    }

    internal void ApplyActivePlotEquation(IPlotEquationViewModelBase equation)
    {
        IPlotEquationViewModel3D plotEquationViewModel3D = _equations.Where((IPlotEquationViewModel3D eq) => eq == equation).SingleOrDefault();
        if (DebugTools.AssertNotNull(plotEquationViewModel3D, null).Failed)
        {
            return;
        }
        ActivePlotEquation = plotEquationViewModel3D;
    }

    private void HandleChangeNumOfPoints(object obj)
    {
        // Release IL requires callvirt on newobj without a local slot.
        (new TraceSetNumberOfPointsCommand(SelectedTraces, (Pair<int, bool>)obj, RequestActivation) switch { var c => c }).Execute();
    }

    public void UpdateSelectedTracesNumberOfPoints(int numOfPoints)
    {
        if (SelectedTraces != null && SelectedTraces.Any((IPlotTraceViewModel3D trace) => trace.IsNumOfPointsMutable))
        {
            HandleChangeNumOfPoints(new Pair<int, bool>(numOfPoints, second: true));
        }
    }
}
