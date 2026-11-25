using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Media3D;
using Quire.Controls.Core;
using Quire.Controls.Plots.Plot3D.ViewModel;
using Quire.Services;
using Quire.Wpf;

namespace Quire.Controls.Plots.Plot3D.Tools;

public abstract partial class BaseTrackball : IBaseTrackball
{
    private IHasCameraTransform _transformOwner;

    public event Action<Matrix3D> MouseUp;

    public event Action<Matrix3D> MouseWheel;

    protected MatrixTransform3D ScaleTransform { get; private set; }

    protected Transform3D BaseTransform { get; private set; }

    protected bool IsTracking => CurrentTrackingInfo != null;

    protected TrackingInfo CurrentTrackingInfo { get; private set; }

    private ITrackingHelper _trackingHelper;

    private Matrix3D _previousTransform;

    public Viewport3D Viewport { get; private set; }

    protected FrameworkElement MouseEventsSource { get; private set; }

    protected double CurrentScale
    {
        get
        {
            if (ScaleTransform == null)
            {
                return 0.0;
            }
            return ScaleTransform.Matrix.M44;
        }
    }

    public Transform3D CameraTransform
    {
        get
        {
            if (_transformOwner == null)
            {
                return null;
            }
            return _transformOwner.CameraTransform;
        }
        protected set
        {
            if (_transformOwner != null)
            {
                _transformOwner.CameraTransform = value;
            }
            else
            {
                DebugTools.FailAssertion(null, Array.Empty<object>());
            }
        }
    }

    internal ITrackingHelper TrackingHelper
    {
        get
        {
            if (_trackingHelper == null)
            {
                ITrackingHelperFactory trackingHelperFactory = AppServiceRegistry.GetService<ITrackingHelperFactory>() ?? DummyTrackingHelperFactory.Instance;
                _trackingHelper = trackingHelperFactory.CreateHelper(this);
            }
            return _trackingHelper;
        }
    }

    public Rect3D Model3DBoundingBox => PlotCommonConstants.Plot3DBoundingBox;

    public void Attach(FrameworkElement mouseEventsSource, IHasCameraTransform hasCameraTransform, Viewport3D viewport)
    {
        DebugTools.DevAssert(MouseEventsSource == null && _transformOwner == null && !IsTracking, null);
        MouseEventsSource = mouseEventsSource;
        _transformOwner = hasCameraTransform;
        _transformOwner.ResetTrackball += Reset;
        Viewport = viewport;
        CurrentTrackingInfo = null;
        MouseEventsSource.MouseDown += OnMouseDown;
        MouseEventsSource.MouseUp += OnMouseUp;
        MouseEventsSource.MouseMove += OnMouseMove;
        MouseEventsSource.MouseWheel += OnMouseWheel;
        DebugTools.DevAssert(MouseEventsSource != null && _transformOwner != null, null);
        TrackingHelper.Attach();
        Reset();
    }

    protected abstract Transform3D InitializeTransform();

    public void Reset()
    {
        MatrixTransform3D matrixTransform3D = CameraTransform != null ? new MatrixTransform3D(CameraTransform.Value) : new MatrixTransform3D();
        Matrix3D value = matrixTransform3D.Value;
        Matrix3D matrix = new Matrix3D
        {
            M44 = value.M44
        };
        value.M44 = 1.0;
        matrixTransform3D.Matrix = value;
        ((Freezable)matrixTransform3D).Freeze();
        BaseTransform = CloneAndFreeze(matrixTransform3D);
        Transform3DGroup transform3DGroup = new Transform3DGroup();
        transform3DGroup.Children.Add(matrixTransform3D);
        transform3DGroup.Children.Add(InitializeTransform());
        transform3DGroup.Children.Add(ScaleTransform = new MatrixTransform3D(matrix));
        CameraTransform = transform3DGroup;
        TrackingHelper.Reset();
    }

    protected static TTransform3D CloneAndFreeze<TTransform3D>(TTransform3D source) where TTransform3D : Transform3D
    {
        Transform3D transform3D = source.CloneCurrentValue();
        if (((Freezable)transform3D).CanFreeze)
        {
            ((Freezable)transform3D).Freeze();
        }
        else
        {
            DebugTools.FailAssertion(null, Array.Empty<object>());
        }
        return transform3D as TTransform3D;
    }

    public virtual void Detach()
    {
        TrackingHelper.Detach();
        if (MouseEventsSource != null)
        {
            MouseEventsSource.MouseDown -= OnMouseDown;
            MouseEventsSource.MouseUp -= OnMouseUp;
            MouseEventsSource.MouseMove -= OnMouseMove;
            MouseEventsSource.MouseWheel -= OnMouseWheel;
            MouseEventsSource = null;
        }
        if (IsTracking)
        {
            Mouse.Capture(MouseEventsSource, CaptureMode.None);
            CurrentTrackingInfo = null;
        }
        if (_transformOwner != null)
        {
            _transformOwner.ResetTrackball -= Reset;
            _transformOwner = null;
        }
    }

    protected virtual void UpdateCurrentTrackingInfo(Point current)
    {
        CurrentTrackingInfo.LastPosition = current;
        if (CurrentTrackingInfo.HasSignificantMove)
        {
            return;
        }
        double zoom = DocumentAttachedProperties.GetZoom(Viewport);
        double threshold = 5.0 / zoom;
        CurrentTrackingInfo.HasSignificantMove = 1 == DoubleComparer.DefaultDoubleComparer.Compare((current - CurrentTrackingInfo.CaptureStartPosition).Length, threshold);
    }

    private void OnMouseDown(object sender, MouseButtonEventArgs args)
    {
        HandleMouseDown(GetPositionRelativeToViewport(args), args.ChangedButton, useTimer: true);
        if (IsTracking)
        {
            Mouse.Capture(MouseEventsSource, CaptureMode.Element);
        }
    }

    public void HandleMouseDown(Point rawLocation, MouseButton mouseButton, bool useTimer)
    {
        if (IsTrackingDifferentButton(mouseButton))
        {
            return;
        }
        Point position = CoercePosition(rawLocation);
        if (TrackStart(position, useTimer))
        {
            CurrentTrackingInfo = new TrackingInfo
            {
                TrackingButton = mouseButton,
                CaptureStartPosition = position,
                LastPosition = position
            };
            _previousTransform = CameraTransform.Value;
        }
    }

    private void OnMouseUp(object sender, MouseButtonEventArgs args)
    {
        bool wasTracking = IsTracking;
        HandleMouseUp(GetPositionRelativeToViewport(args), args.ChangedButton);
        if (wasTracking && !IsTracking)
        {
            Mouse.Capture(MouseEventsSource, CaptureMode.None);
        }
        args.Handled = false;
    }

    public void HandleMouseUp(Point rawLocation, MouseButton mouseButton)
    {
        if (!IsTracking || IsTrackingDifferentButton(mouseButton))
        {
            return;
        }
        Point current = CoercePosition(rawLocation);
        UpdateCurrentTrackingInfo(current);
        TrackEnd(current);
        FireMouseUp();
        CurrentTrackingInfo = null;
    }

    private void OnMouseMove(object sender, MouseEventArgs args)
    {
        HandleMouseMove(GetPositionRelativeToViewport(args));
    }

    public void HandleMouseMove(Point rawLocation)
    {
        if (!IsTracking)
        {
            return;
        }
        Point current = CoercePosition(rawLocation);
        UpdateCurrentTrackingInfo(current);
        if (CurrentTrackingInfo.HasSignificantMove)
        {
            Track(current);
        }
    }

    private void OnMouseWheel(object sender, MouseWheelEventArgs args)
    {
        _previousTransform = CameraTransform.Value;
        HandleMouseWheel(args.Delta / WpfUtils.MouseWheelScrollDelta);
        args.Handled = true;
    }

    protected Point GetPositionRelativeToViewport(MouseEventArgs args)
    {
        return args.GetPosition(Viewport);
    }

    private void FireMouseUp()
    {
        Action<Matrix3D> mouseUp = MouseUp;
        if (mouseUp != null)
        {
            mouseUp(_previousTransform);
        }
    }

    private void FireMouseWheel()
    {
        Action<Matrix3D> mouseWheel = MouseWheel;
        if (mouseWheel != null)
        {
            mouseWheel(_previousTransform);
        }
    }

    public void HandleMouseWheel(int ticks)
    {
        if (IsTracking)
        {
            return;
        }
        AddScale(ScaleFromTicks(ticks));
        FireMouseWheel();
    }

    protected double ScaleFromTicks(int ticks)
    {
        return Math.Pow(1.1, ticks);
    }

    protected void AddScale(double scale)
    {
        SetScale(CurrentScale * scale);
    }

    protected void SetScale(double scale)
    {
        scale = Math.Min(10.0, Math.Max(0.5, scale));
        Matrix3D matrix = ScaleTransform.Matrix;
        matrix.M44 = scale;
        ScaleTransform.Matrix = matrix;
    }

    private bool IsTrackingDifferentButton(MouseButton button)
    {
        if (IsTracking)
        {
            return CurrentTrackingInfo.TrackingButton != button;
        }
        return false;
    }

    protected abstract bool TrackStart(Point current, bool useTimer);

    protected abstract bool Track(Point current);

    protected abstract bool TrackEnd(Point current);

    protected virtual Point CoercePosition(Point rawPosition)
    {
        return rawPosition;
    }

    protected BaseTrackball()
    {
    }

    protected partial class TrackingInfo
    {
        public MouseButton TrackingButton { get; set; }

        public bool HasSignificantMove { get; set; }

        public Point CaptureStartPosition { get; set; }

        public Point LastPosition { get; set; }

        public TrackingInfo()
        {
        }
    }
}
