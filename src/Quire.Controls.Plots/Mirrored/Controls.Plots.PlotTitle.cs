#nullable disable
#pragma warning disable

using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Quire.Controls.Plots.Model;
using Quire.Controls.Plots.PlotCommands;
using Quire.Controls.Plots.ViewModel;
using Quire.Controls.Text;

namespace Quire.Controls.Plots;

public partial class PlotTitle
{
    private bool _isEditingYAxisTitle;
    public static readonly DependencyProperty TitleTypeProperty = DependencyProperty.Register("TitleType", typeof(PlotTitleType), typeof(PlotTitle), new FrameworkPropertyMetadata(PlotTitleType.Plot));
    internal ContentPresenter PART_TitleHost;

    public PlotTitleViewModel ViewModel { get; private set; }

    public bool IsRotationAllowed { get; private set; }

    public RichTextTitle Title { get; private set; }

    public PlotTitleType TitleType
    {
        get
        {
            return (PlotTitleType)GetValue(TitleTypeProperty);
        }
        set
        {
            SetValue(TitleTypeProperty, value);
        }
    }

    public PlotTitle(PlotTitleType type)
    {
        InitializeComponent();
        TitleType = type;
        string defaultTitleText = GetDefaultTitleText(type);
        Title = new RichTextTitle(defaultTitleText)
        {
            BorderThickness = new Thickness(0.0),
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };
        PART_TitleHost.Content = Title;
        RegisterToEvents(Title);
        PlotTitleModel plotTitleModel = new PlotTitleModel
        {
            TitleType = TitleType,
            Title = Title
        };
        if (TitleType == PlotTitleType.YAxis)
        {
            InitializeYAxisOrientationSupport();
            plotTitleModel.IsVertical = true;
        }
        ViewModel = new PlotTitleViewModel(plotTitleModel);
        DataContext = ViewModel;
    }

    private void SetWidthFromContent(double newWidth)
    {
        Width = newWidth;
        MinWidth = newWidth;
    }

    private void ResetWidthConstraints()
    {
        Width = double.NaN;
        MinWidth = 0.0;
    }

    private void OnElementOnGotFocus(object sender, RoutedEventArgs e)
    {
        HandleEditingStarted();
    }

    private void OnElementOnLostFocus(object sender, RoutedEventArgs e)
    {
        HandleEditingEnded();
        HideTitleIfEmpty();
    }

    private void ApplyTemporaryOrientation(bool isVertical)
    {
        LayoutTransform = (isVertical ? new RotateTransform(270.0) : Transform.Identity);
    }

    private void InitializeYAxisOrientationSupport()
    {
        IsRotationAllowed = true;
        if (Title != null)
        {
            Title.ToggleOrientationRequest = ToggleOrientation;
        }
    }

    public void ToggleOrientation()
    {
        DebugTools.DevAssert(IsRotationAllowed, "ToggleOrientation was called on a non-rotatable title.");
        new ToggleAxisTitleOrientationCommand(this).Execute();
    }

    private static string GetDefaultTitleText(PlotTitleType type)
    {
        switch (type)
        {
            case PlotTitleType.Plot:
                return "Plot Title";
            case PlotTitleType.XAxis:
                return "X-axis Title";
            case PlotTitleType.YAxis:
                return "Y-axis Title";
            default:
                return string.Empty;
        }
    }
}
