#nullable disable
#pragma warning disable

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Quire.Controls.Core;
using Quire.Controls.Plots.Base3D.ViewModel;
using Quire.Controls.Plots.Model;
using Quire.Media3D;

namespace Quire.Controls.Plots.Plot3D.View;

internal partial class PlotAxis3D
{
    public static class PropertyNames
    {
        public const string ViewModel = "ViewModel";
    }

    private static readonly Type _typeOfThis;

    public static readonly double AlmostZero;

    private readonly MeshGeometry3D _tickmarksGeometry = new MeshGeometry3D();

    private readonly Cylinder _arrowHead = new Cylinder
    {
        Radius2 = 0.0
    };

    private Viewport2DVisual3D[] _labelVisuals = new Viewport2DVisual3D[0];

    private IPlotAxisViewModel3D _viewModel;

    private Vector3D _axisDirection;

    public static readonly DependencyProperty LabelsColorProperty;

    public static readonly DependencyProperty ShowTicksProperty;

    public static readonly DependencyProperty ShowNumbersProperty;

    public PlotLabelsContainer3D LabelsContainer { get; set; }

    private const double WorldEdge1 = -0.5;

    private const double WorldEdge2 = 0.5;

    private const double AxisLengthWithoutMargins = 1.0;

    private const double AxisMarginProportions = 1.0 / 12.0;

    private static readonly Matrix3D _axisToTickTransform;

    private static readonly Material _labelMaterial;

    private static readonly Vector3D _axisStartPointOffset;

    private static readonly Thickness _zeroThickness;

    private const double LabelVerticalMargin = 2.0;

    private static readonly PointCollection _labelTextureCoordinates;

    public Color LabelsColor
    {
        get
        {
            return (Color)GetValue(LabelsColorProperty);
        }
        set
        {
            SetValue(LabelsColorProperty, value);
        }
    }

    public bool ShowTicks
    {
        get
        {
            return (bool)GetValue(ShowTicksProperty);
        }
        set
        {
            SetValue(ShowTicksProperty, value);
        }
    }

    public bool ShowNumbers
    {
        get
        {
            return (bool)GetValue(ShowNumbersProperty);
        }
        set
        {
            SetValue(ShowNumbersProperty, value);
        }
    }

    public new event PropertyChangedEventHandler PropertyChanged;

    protected void FirePropertyChanged(string propertyName)
    {
        if (PropertyChanged != null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    static PlotAxis3D()
    {
        _typeOfThis = typeof(PlotAxis3D);
        AlmostZero = Math.Pow(2.0, -30.0);
        LabelsColorProperty = DependencyProperty.Register("LabelsColor", typeof(Color), _typeOfThis, new PropertyMetadata(LabelsColorChanged));
        _axisToTickTransform = new Matrix3D(0.0, 1.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 1.0);
        _axisStartPointOffset = new Vector3D(-0.5, -0.5, -0.5);
        _zeroThickness = new Thickness(0.0);
        _labelTextureCoordinates = new PointCollection
        {
            new Point(0.0, 0.0),
            new Point(1.0, 0.0),
            new Point(0.0, 1.0),
            new Point(1.0, 1.0)
        };
        _labelMaterial = new DiffuseMaterial();
        _labelMaterial.SetValue(Viewport2DVisual3D.IsVisualHostMaterialProperty, true);
        _labelMaterial.Freeze();
        WireBase.ColorProperty.OverrideMetadata(_typeOfThis, new PropertyMetadata(OnColorChanged));
        ShowTicksProperty = PlotCommonProperties.ShowTicksProperty.AddOwner(_typeOfThis, PlotCommonProperties.CreateShowTicksMetadata(WireBase.PropertyChanged));
        ShowNumbersProperty = PlotCommonProperties.ShowNumbersProperty.AddOwner(_typeOfThis, PlotCommonProperties.CreateShowNumbersMetadata(WireBase.PropertyChanged));
    }

    private static void OnColorChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        if (sender is PlotAxis3D plotAxis3D)
        {
            plotAxis3D.ColorChanged();
        }
        else
        {
            DebugTools.DevAssert(false, null);
        }
    }

    public PlotAxis3D()
    {
        Children.Add(_arrowHead);
        Children.Add(CreateTickmarkModelVisual3D(_tickmarksGeometry));
    }

    private GeometryModel3D BaseModel
    {
        get
        {
            GeometryModel3D obj = Content as GeometryModel3D;
            DebugTools.DevAssert(obj != null, null);
            return obj;
        }
    }

    private static void LabelsColorChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
        if (!(obj is PlotAxis3D plotAxis3D))
        {
            DebugTools.DevAssert(false, null);
            return;
        }
        Brush newColor = new SolidColorBrush((Color)args.NewValue);
        newColor.Freeze();
        plotAxis3D._labelVisuals.Each(delegate(Viewport2DVisual3D v)
        {
            ((Label)v.Visual).Foreground = newColor;
        });
    }

    public IPlotAxisViewModel3D ViewModel
    {
        get
        {
            return _viewModel;
        }
        set
        {
            if (value == _viewModel)
            {
                return;
            }
            if (_viewModel != null)
            {
                _viewModel.TickMarkLabelsUpdated -= UpdateLabelVisuals;
                ClearBinding(ShowTicksProperty);
                ClearBinding(ShowNumbersProperty);
            }
            _viewModel = value;
            FirePropertyChanged("ViewModel");
            if (_viewModel != null)
            {
                _viewModel.TickMarkLabelsUpdated += UpdateLabelVisuals;
                SetBinding(ShowTicksProperty);
                SetBinding(ShowNumbersProperty);
                UpdateLabelVisuals();
            }
            else
            {
                DebugTools.DevAssert(false, null);
            }
        }
    }

    private void ClearBinding(DependencyProperty dependencyProperty)
    {
        BindingOperations.ClearBinding(this, dependencyProperty);
    }

    private void SetBinding(DependencyProperty dependencyProperty)
    {
        BindingOperations.SetBinding(this, dependencyProperty, new Binding
        {
            Source = _viewModel,
            Path = new PropertyPath(dependencyProperty.Name)
        });
    }

    private void UpdateLabelVisuals()
    {
        CreateLabelVisuals();
        ForceGenerate();
    }

    private void ColorChanged()
    {
        _arrowHead.Material = BaseModel.Material;
        DebugTools.DevAssert(_arrowHead.Material != null, null);
    }

    private Point3D BaseLineEnd => _arrowHead.Point1;

    public void RecalculateNow()
    {
        Recalculate();
    }

    protected override void Recalculate()
    {
        if (_needToAddLabels)
        {
            AddAllLabelsToViewport();
        }
        RecalculateArrowhead();
        LineCollection[1] = BaseLineEnd;
        base.Recalculate();
        Point3DCollection positions = _tickmarksGeometry.Positions;
        _tickmarksGeometry.Positions = null;
        positions.Clear();
        Int32Collection triangleIndices = _tickmarksGeometry.TriangleIndices;
        _tickmarksGeometry.TriangleIndices = null;
        triangleIndices.Clear();
        AddTickmarksToMeshAndPositionLabels(positions, triangleIndices);
        _tickmarksGeometry.TriangleIndices = triangleIndices;
        _tickmarksGeometry.Positions = positions;
    }

    private Vector3D GetAxisDirectionScreen(double edgeDistance)
    {
        Point4D start = new Point4D(0.0, 0.0, 0.0, 1.0) * matxVisualToScreen;
        Point4D end = (Point4D)(Point3D)(AxisDirection * edgeDistance) * matxVisualToScreen;
        double x = end.X / end.W - start.X / start.W;
        double y = end.Y / end.W - start.Y / start.W;
        Vector3D result = new Vector3D(x, y, 0.0);
        if (result.Length != 0.0)
        {
            result.Normalize();
        }
        return result;
    }

    private Point3D GetPointFromAxisStart(double distance)
    {
        return (Point3D)_axisStartPointOffset + AxisDirection * distance;
    }

    private void RecalculateArrowhead()
    {
        Point3D pointFromAxisStart = GetPointFromAxisStart(1.0 + AxisMarginProportions);
        Vector3D axisDirectionScreen = GetAxisDirectionScreen(1.0 + AxisMarginProportions);
        axisDirectionScreen.Z = 0.0;
        if (axisDirectionScreen.Length != 0.0)
        {
            axisDirectionScreen.Normalize();
        }
        Vector vecScreen = new Vector(axisDirectionScreen.X, axisDirectionScreen.Y);
        vecScreen *= -5.0;
        _arrowHead.Point2 = pointFromAxisStart;
        try
        {
            Point3D baseCenter = Tools3D.Widen(pointFromAxisStart, vecScreen, matxVisualToScreen, matxScreenToVisual);
            _arrowHead.Point1 = baseCenter;
            Vector3D orthoScreen = axisDirectionScreen * _axisToTickTransform;
            Point3D rimPoint = Tools3D.Widen(baseCenter, new Vector(orthoScreen.X, orthoScreen.Y) * 5.0 / 2.0, matxVisualToScreen, matxScreenToVisual);
            _arrowHead.Radius1 = (rimPoint - baseCenter).Length;
        }
        catch (ArgumentException ex)
        {
            DebugTools.FailAssertion(ex.Message);
            _arrowHead.Point1 = pointFromAxisStart;
            _arrowHead.Radius1 = 0.0;
        }
    }

    private double GetAxisOrthogonalDistanceInWorld(Point3D origin, double screenDist)
    {
        return GetRelativeDistanceInWorld(origin, screenDist, _axisToTickTransform);
    }

    private double GetAxisDistanceInWorld(Point3D origin, double screenDist)
    {
        return GetRelativeDistanceInWorld(origin, screenDist, Matrix3D.Identity);
    }

    // Parked: Mathcad emits three unreferenced locals ([0] Vector3D, [1..2] Point4D) and allocates the length temporary before user locals; Roslyn cannot emit those unused locals or that slot order.
    private double GetRelativeDistanceInWorld(Point3D origin, double screenDist, Matrix3D transformRelativeToAxis) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void ForceGenerate()
    {
        base.PropertyChanged(default(DependencyPropertyChangedEventArgs));
    }

    public Vector3D AxisDirection
    {
        get
        {
            return _axisDirection;
        }
        set
        {
            _axisDirection = value;
            _axisDirection.Normalize();
            ForceGenerate();
        }
    }

    private void CreateLabelVisuals()
    {
        RemoveAllLabelsFromViewport();
        Labels = ((ViewModel == null) ? new string[0] : ViewModel.FilteredLabels);
        int count = Labels.Length;
        _labelVisuals = new Viewport2DVisual3D[count];
        Brush brush = new SolidColorBrush(LabelsColor);
        brush.Freeze();
        for (int i = 0; i < count; i++)
        {
            Visual visual = new Label
            {
                Margin = _zeroThickness,
                Padding = _zeroThickness,
                Foreground = brush,
                Background = Brushes.Transparent,
                Content = Labels[i],
                FontFamily = FontConstants.DefaultMathFontFamily,
                FontSize = 8.0
            };
            Viewport2DVisual3D viewport2DVisual3D = new Viewport2DVisual3D
            {
                Material = _labelMaterial,
                Visual = visual
            };
            _labelVisuals[i] = viewport2DVisual3D;
        }
        AddAllLabelsToViewport();
    }

    private void RemoveAllLabelsFromViewport()
    {
        if (LabelsContainer != null)
        {
            _labelVisuals.Each(delegate(Viewport2DVisual3D label)
            {
                DebugTools.DevAssert(LabelsContainer.Remove(label), null);
            });
        }
    }

    private bool _needToAddLabels;

    public string[] Labels { get; private set; }

    private void AddAllLabelsToViewport()
    {
        if (LabelsContainer == null)
        {
            _needToAddLabels = true;
            return;
        }
        _needToAddLabels = false;
        LabelsContainer.Add(_labelVisuals, shouldReorder: true);
    }

    private bool IsAxisVertical()
    {
        return AxisDirection.Y > 0.0;
    }

    // Parked: Mathcad stores the inner MaterialGroup and GeometryModel3D in locals before duping ModelVisual3D for Content; modern Roslyn emits dup immediately after newobj with no locals.
    private ModelVisual3D CreateTickmarkModelVisual3D(MeshGeometry3D mesh) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void AddTickmarksToMeshAndPositionLabels(Point3DCollection meshVertices, Int32Collection meshIndices)
    {
        int count = _labelVisuals.Length;
        double step = 1.0 / (double)(count - 1);
        for (int i = 0; i < _labelVisuals.Length; i++)
        {
            double distance = (double)i * step;
            Point3D pointFromAxisStart = GetPointFromAxisStart(distance);
            Vector3D tickOffset = GetAxisOrthogonalDistanceInWorld(pointFromAxisStart, 2.0) * (AxisDirection * _axisToTickTransform);
            Point3D tickStart = pointFromAxisStart + tickOffset;
            Point3D tickEnd = pointFromAxisStart - tickOffset;
            if (ShowTicks)
            {
                Tools3D.GetLineGeometry(tickStart, tickEnd, Thickness, matxVisualToScreen, matxScreenToVisual, meshVertices, meshIndices);
            }
            if (ShowNumbers || Labels[i] == AxisModel.ErrorIndicator)
            {
                AddLabelGeometry(i, tickEnd);
            }
            else
            {
                _labelVisuals[i].Geometry = null;
            }
        }
        LabelsContainer.ReorderChildren();
    }

    private void AddLabelGeometry(int iTick, Point3D tickEnd)
    {
        FrameworkElement label = _labelVisuals[iTick].Visual as FrameworkElement;
        if (DebugTools.AssertNotNull(label, null).Passed)
        {
            MeshGeometry3D meshGeometry3D = new MeshGeometry3D
            {
                Positions = new Point3DCollection(),
                TriangleIndices = new Int32Collection(),
                TextureCoordinates = _labelTextureCoordinates
            };
            double labelHeightWorld;
            Point3D labelStart;
            double heightWithMargin = label.ActualHeight + LabelVerticalMargin;
            if (IsAxisVertical())
            {
                labelHeightWorld = GetAxisDistanceInWorld(tickEnd, heightWithMargin);
                double offset = GetAxisOrthogonalDistanceInWorld(tickEnd, 3.0);
                double labelWidthWorld = GetAxisOrthogonalDistanceInWorld(tickEnd + offset * AxisDirection * _axisToTickTransform, label.ActualWidth);
                labelStart = tickEnd + -new Vector3D(offset + labelWidthWorld / 2.0, -labelHeightWorld / 2.0, 0.0);
            }
            else
            {
                labelStart = tickEnd - AxisDirection * _axisToTickTransform * GetAxisOrthogonalDistanceInWorld(tickEnd, 3.0);
                labelHeightWorld = GetAxisOrthogonalDistanceInWorld(labelStart, heightWithMargin);
            }
            Tools3D.GetLineGeometry(labelStart, labelStart - new Vector3D(0.0, labelHeightWorld, 0.0), label.ActualWidth, matxVisualToScreen, matxScreenToVisual, meshGeometry3D.Positions, meshGeometry3D.TriangleIndices);
            _labelVisuals[iTick].Geometry = meshGeometry3D;
        }
    }

    protected override void Generate(DependencyPropertyChangedEventArgs args, Point3DCollection lines)
    {
        lines.Clear();
        Point3D pointFromAxisStart = GetPointFromAxisStart(-AxisMarginProportions);
        Point3D baseLineEnd = BaseLineEnd;
        lines.Add(pointFromAxisStart);
        lines.Add(baseLineEnd);
    }
}
