#nullable disable
#pragma warning disable

using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using Quire.Controls.Plots.Model;
using Quire.Controls.Plots.PlotCommands;
using Quire.Controls.Plots.ViewModel;
using Quire.Controls.Text;

namespace Quire.Controls.Plots;

public partial class PlotTitle
{
    public PlotTitleViewModel ViewModel { get; private set; }

    public bool IsRotationAllowed { get; private set; }

    public RichTextTitle Title { get; private set; }

    private bool _isEditingYAxisTitle;
    public static readonly DependencyProperty TitleTypeProperty = DependencyProperty.Register("TitleType", typeof(PlotTitleType), typeof(PlotTitle), new FrameworkPropertyMetadata(PlotTitleType.Plot));
    internal ContentPresenter PART_TitleHost;
    private bool _contentLoaded;

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

    private void RegisterToEvents(RichTextTitle title)
    {
        // The IL holds the argument in a FrameworkElement local for the null test and every hook-up.
        FrameworkElement element = title;
        if (element != null)
        {
            element.SizeChanged += TitleSizeChanged;
            element.LostFocus += OnElementOnLostFocus;
            element.GotFocus += OnElementOnGotFocus;
        }
    }

    private void TitleSizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (ViewModel == null || Visibility == Visibility.Collapsed)
        {
            return;
        }
        SetWidthFromContent(e.NewSize.Width);
        MinHeight = e.NewSize.Height;
        InvalidateMeasure();
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

    private void HandleEditingStarted()
    {
        if (TitleType != PlotTitleType.YAxis || !IsRotationAllowed)
        {
            return;
        }
        _isEditingYAxisTitle = true;
        if (ViewModel?.IsVertical ?? false)
        {
            ApplyTemporaryOrientation(isVertical: false);
        }
    }

    private void HandleEditingEnded()
    {
        if (TitleType != PlotTitleType.YAxis || !IsRotationAllowed)
        {
            return;
        }
        if (!_isEditingYAxisTitle)
        {
            return;
        }
        _isEditingYAxisTitle = false;
        bool isVertical = ViewModel?.IsVertical ?? false;
        ApplyTemporaryOrientation(isVertical);
    }

    private void ApplyTemporaryOrientation(bool isVertical)
    {
        LayoutTransform = (isVertical ? new RotateTransform(270.0) : Transform.Identity);
    }

    private void HideTitleIfEmpty()
    {
        if (ViewModel == null || Title == null)
        {
            return;
        }
        if (!string.IsNullOrWhiteSpace(Title.GetText()))
        {
            return;
        }
        if (ViewModel.ShowTitle)
        {
            ViewModel.ShowTitle = false;
            Title.ResetText();
            ResetWidthConstraints();
        }
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

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "10.0.11.0")]
    public void InitializeComponent()
    {
        if (_contentLoaded)
        {
            return;
        }
        _contentLoaded = true;
        Uri resourceLocator = new Uri("/Quire.Controls.Plots;component/plottitle.xaml", UriKind.Relative);
        Application.LoadComponent(this, resourceLocator);
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "10.0.11.0")]
    internal Delegate _CreateDelegate(Type delegateType, string handler)
    {
        return Delegate.CreateDelegate(delegateType, this, handler);
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "10.0.11.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
        if (connectionId == 1)
        {
            PART_TitleHost = (ContentPresenter)target;
        }
        else
        {
            _contentLoaded = true;
        }
    }
}
