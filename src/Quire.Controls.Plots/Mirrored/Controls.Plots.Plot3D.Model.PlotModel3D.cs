#nullable disable
#pragma warning disable

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Media.Media3D;
using Quire.Calculation.Core;
using Quire.Controls.Core;
using Quire.Controls.Plots.Presenter;

namespace Quire.Controls.Plots.Plot3D.Model;

public partial class PlotModel3D
{
    internal IEnumerable<IPlotEquationModel3D> Equations => _equationListModel3D;

    internal IEnumerable<IPlotTraceModel3D> Traces => _traces;

    public SynchronizationStatus SynchronizationStatus
    {
        get
        {
            return _synchronizationStatus;
        }
        private set
        {
            if (_synchronizationStatus != value)
            {
                _synchronizationStatus = value;
                FirePropertyChanged("SynchronizationStatus");
            }
        }
    }

    public IAxisModel XAxis
    {
        get
        {
            return _xAxis;
        }
        set
        {
            DetachAxisEventHandlers(_xAxis);
            _xAxis = value;
            AttachAxisEventHandlers(_xAxis);
        }
    }

    public IAxisModel YAxis
    {
        get
        {
            return _yAxis;
        }
        set
        {
            DetachAxisEventHandlers(_yAxis);
            _yAxis = value;
            AttachAxisEventHandlers(_yAxis);
        }
    }

    public IAxisModel ZAxis
    {
        get
        {
            return _zAxis;
        }
        set
        {
            DetachAxisEventHandlers(_zAxis);
            _zAxis = value;
            AttachAxisEventHandlers(_zAxis);
        }
    }

    public Transform3D CameraTransform
    {
        get
        {
            return _cameraTransform;
        }
        set
        {
            _cameraTransform = value;
            OnPropertyChanged("CameraTransform");
        }
    }

    public CameraType3D PlotCameraType
    {
        get
        {
            return _plotCameraType;
        }
        set
        {
            if (value != PlotCameraType)
            {
                _plotCameraType = value;
                OnPropertyChanged("PlotCameraType");
            }
        }
    }

    private static bool AssertThatAxisAndAxisCalculatableIsNotNull(IAxisModel axisModel)
    {
        if (DebugTools.AssertNotNull(axisModel, null).Passed)
        {
            return DebugTools.AssertNotNull(axisModel.AxisCalculatables, null).Passed;
        }
        return false;
    }

    private void AttachAxisEventHandlers(IAxisModel axisModel)
    {
        if (axisModel != null)
        {
            axisModel.Changed += OnChanged;
            axisModel.PropertyChanged += AxisPropertyChanged;
            axisModel.RangeChanged += AxisRangeChanged;
            AttachOrDetachLimitEditorEventHandlers(axisModel, null, attach: true);
        }
    }

    private void DetachAxisEventHandlers(IAxisModel axisModel)
    {
        if (axisModel != null)
        {
            axisModel.Changed -= OnChanged;
            axisModel.PropertyChanged -= AxisPropertyChanged;
            axisModel.RangeChanged -= AxisRangeChanged;
            AttachOrDetachLimitEditorEventHandlers(axisModel, null, attach: false);
        }
    }

    private void AxisRangeChanged()
    {
        SetRangeOnTraces(GetAxesRange());
    }

    private void AxisPropertyChanged(object sender, PropertyChangedEventArgs eArgs)
    {
        IAxisModel axisModel = sender as IAxisModel;
        switch (eArgs.PropertyName)
        {
            case "StartExpression":
            case "SecondTickExpression":
            case "EndExpression":
                AttachOrDetachLimitEditorEventHandlers(axisModel, eArgs.PropertyName, attach: true);
                break;
        }
    }
}
