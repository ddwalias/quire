#nullable disable
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Quire.Mirror;
using Quire.Wpf;

namespace Quire.Controls.Core;

public static partial class PlotCommonConstants
{
    public const int NumberOfPointsDefaultFor2DPlots = 500;

    public const int NumberOfPointsDefaultFor3DPlots = 41;

    public const int NumberOfPointsDefaultInvalidValue = -1;

    public static readonly double PlotsGraphMinHeight;

    public static readonly double PlotsGraphMinWidth;

    public const double Plot3DMinScale = 0.5;

    public const double Plot3DMaxScale = 10.0;

    public const int Plot3DCameraTransformMatrixRows = 4;

    public const int Plot3DCameraTransformMatrixColumns = 4;

    public static readonly Point3D Plot3DCameraPosition;

    public static readonly Vector3D Plot3DCameraLookDirection;

    public static readonly Vector3D Plot3DCameraUpDirection;

    public const double Plot3DPerspectiveCameraFOV = 11.5;

    public const double Plot3DOrthographicCameraWidth = 1.65;

    public const double OrthographicCameraNearPlaneDistance = 0.0;

    public const double OrthographicCameraFarPlaneDistance = 10000000000.0;

    public static readonly Rect3D Plot3DBoundingBox;

    public static readonly MatrixTransform3D Plot3DLogicalToVisualTransform;

    public static readonly MatrixTransform3D Plot3DVisualToLogicalTransform;

    public static readonly string GridlinesDefaultColorHex;

    public static readonly Color DefaultGridLineColor;

    static PlotCommonConstants()
    {
        PlotsGraphMinHeight = WpfUtils.InchesToDips(1.0);
        PlotsGraphMinWidth = WpfUtils.InchesToDips(1.0);
        Plot3DCameraPosition = new Point3D(3.5, 3.5, 6.486);
        Plot3DCameraLookDirection = new Vector3D(-3.5, -3.5, -6.486);
        Plot3DCameraUpDirection = new Vector3D(0.0, 1.0, 0.0);
        Plot3DBoundingBox = new Rect3D(-0.5, -0.5, -0.5, 1.0, 1.0, 1.0);
        Plot3DLogicalToVisualTransform = new MatrixTransform3D(new Matrix3D(0.0, 0.0, 1.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 1.0));
        Plot3DVisualToLogicalTransform = new MatrixTransform3D(new Matrix3D(0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 1.0));
        GridlinesDefaultColorHex = "#FFA1A3A6";
        DefaultGridLineColor = (Color)ColorConverter.ConvertFromString(GridlinesDefaultColorHex);
        Plot3DLogicalToVisualTransform.Freeze();
    }

    public static double GetPlotLineThickness(PlotLineTicknessType type)
    {
        switch (type)
        {
            case PlotLineTicknessType.Thickness0_5:
                return 0.5;
            case PlotLineTicknessType.Thickness1:
                return 1.0;
            case PlotLineTicknessType.Thickness2:
                return 2.0;
            case PlotLineTicknessType.Thickness4:
                return 4.0;
            case PlotLineTicknessType.Thickness6:
                return 6.0;
            default:
                DebugTools.DevAssert(false, null);
                return 0.0;
        }
    }
}
