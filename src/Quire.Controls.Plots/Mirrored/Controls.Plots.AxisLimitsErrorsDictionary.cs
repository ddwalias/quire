#nullable disable
#pragma warning disable

using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Markup;

namespace Quire.Controls.Plots;

public partial class AxisLimitsErrorsDictionary
{
    public const string ErrorMinToBeSmallerThanMax = "Ptc.Plots.Axis.MIN_TO_BE_SMALLER_THAN_MAX";
    public const string ErrorMaxToBeGreaterThanMin = "Ptc.Plots.Axis.MAX_TO_BE_GREATER_THAN_MIN";
    public const string ErrorSecondTickToBeGreaterThanMin = "Ptc.Plots.Axis.SECOND_TICK_TO_BE_GREATER_THAN_MIN";
    public const string ErrorTooManyTickmarks = "Ptc.Plots.Axis.TOO_MANY_TICKMARKS";
    public const string ErrorOneLimitMustBePositive = "Ptc.Plots.Axis.ERROR_ONE_LIMIT_MUST_BE_POSITIVE";
    public const string ErrorAngularMaxToBeSmallerThan355 = "Ptc.Plots.Axis.ANGULAR_MAX_TO_BE_SMALLER_THAN_355";
    public const string ResultNotRealNumber = "RESULT_NOT_REAL_NUMBER";
    public const string MarkerOutOfRange = "Ptc.Plots.MARKER_OUT_OF_RANGE";
    public const string MarkersHasUnits = "Ptc.Plots.MARKERS_HAS_UNITS";
    public const string TickmarksHasUnits = "Ptc.Plots.TICKMARKS_HAS_UNITS";

    private bool _contentLoaded;

    public AxisLimitsErrorsDictionary()
    {
        InitializeComponent();
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
        Uri resourceLocator = new Uri("/Quire.Controls.Plots;component/axislimitserrorsdictionary.xaml", UriKind.Relative);
        Application.LoadComponent(this, resourceLocator);
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "10.0.11.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
        _contentLoaded = true;
    }
}
