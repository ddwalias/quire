#nullable disable
using System.Windows;
using System.Windows.Media;

namespace Quire.Controls.Core;

public static partial class RegionBorderConstants
{
    public const string InactiveMoveDraggerVisibilityKey = "InactiveMoveDraggerVisibility";

    public const string InactiveHeaderBorderVisibilityKey = "InactiveHeaderBorderVisibility";

    public const Visibility InactiveElementVisibility = Visibility.Collapsed;

    public const Visibility HiddenBusyStateElementsVisibility = Visibility.Hidden;

    private const double ResizeDraggerTransparentZoneWidth = 7.5;

    private const double BorderLineDashSmallLength = 2.0;

    private const double BorderLineDashLargeLength = 4.0;

    public const double DraggerMarginWidth = 10.0;

    public const double BorderStrokeThickness = 0.5;

    public const double HeaderFontSize = 6.0;

    public static readonly Brush BorderBrush;

    public const double ActiveDraggerBorderLineThickness = 0.5;

    public const double SelectedDraggerBorderLineThickness = 0.75;

    public static readonly Thickness ResizeDraggerTransparentZoneThickness;

    public const double ResizeDraggerEdgeWidth = 5.0;

    public const double ActiveRegionBackgroundOpacity = 0.7;

    public const double SelectedRegionForegroundOpacity = 0.4;

    public static readonly DoubleCollection BorderLineStrokeDashArray;

    public const double MarchingAntsStrokeDashEndOffset = -12.0;

    public const double BorderCornerRadiusLength = 1.0;

    public static readonly CornerRadius BorderCornerRadius;

    public static readonly Thickness DraggersMarginThickness;

    public const double BusyClockRadius = 14.0;

    public const double BusyClockEndAngle = 720.0;

    public static readonly Thickness BusyClockMarginThickness;

    public static readonly Brush BusyClockFillColor;

    public static readonly Brush SelectedDraggerBackground;

    public static readonly Brush SelectedBackgroundBrush;

    public const double AlignmentPointBigCircleRadius = 10.0;

    public const double AlignmentPointSmallCircleRadius = 5.0;

    public static readonly Thickness AlignmentPointMarginThickness;

    static RegionBorderConstants()
    {
        BorderBrush = new SolidColorBrush(Color.FromArgb(byte.MaxValue, 78, 126, 155));
        ResizeDraggerTransparentZoneThickness = new Thickness(7.5);
        BorderLineStrokeDashArray = new DoubleCollection { 8.0, 4.0 };
        BorderCornerRadius = new CornerRadius(1.0);
        DraggersMarginThickness = new Thickness(-10.0);
        BusyClockMarginThickness = new Thickness(0.0, 0.0, -7.0, -7.0);
        BusyClockFillColor = Brushes.Orange;
        SelectedDraggerBackground = Brushes.LightCyan;
        SelectedBackgroundBrush = Brushes.DodgerBlue;
        AlignmentPointMarginThickness = new Thickness(-5.0);
        BorderLineStrokeDashArray.Freeze();
        BusyClockFillColor.Freeze();
        SelectedDraggerBackground.Freeze();
        SelectedBackgroundBrush.Freeze();
        BorderBrush.Freeze();
    }
}
