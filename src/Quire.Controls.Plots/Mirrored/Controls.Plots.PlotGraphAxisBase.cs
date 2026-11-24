#nullable disable
#pragma warning disable

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Quire.Controls;
using Quire.Controls.Core;
using Quire.Controls.Plots.ViewModel;

namespace Quire.Controls.Plots;

public abstract partial class PlotGraphAxisBase
{
    static PlotGraphAxisBase()
    {
        _typeOfThis = typeof(PlotGraphAxisBase);
        NumbersBrush = new SolidColorBrush(Color.FromRgb(112, 112, 112));
        LineColorProperty = DependencyProperty.Register("LineColor", typeof(Color), _typeOfThis, new FrameworkPropertyMetadata(Color.FromRgb(112, 112, 112), FrameworkPropertyMetadataOptions.AffectsRender, LineColorChanged));
        ShowGridLinesProperty = DependencyProperty.Register("ShowGridLines", typeof(bool), _typeOfThis, new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender, ShowGridLinesChanged));
        GridLineColorProperty = DependencyProperty.Register("GridLineColor", typeof(Color), _typeOfThis, new FrameworkPropertyMetadata(PlotCommonConstants.DefaultGridLineColor, FrameworkPropertyMetadataOptions.AffectsRender, GridLineColorChanged));
        TickMarksProperty = DependencyProperty.Register("TickMarks", typeof(int), _typeOfThis, CreateTickmarksPropertyMetadata(11));
    }

    public Color LineColor
    {
        get
        {
            return (Color)GetValue(LineColorProperty);
        }
        set
        {
            SetValue(LineColorProperty, value);
        }
    }

    public IAxisViewModel ViewModel
    {
        get
        {
            return _viewModel;
        }
        set
        {
            if (_viewModel != null)
            {
                UnhookFromViewModel();
            }
            DataContext = (_viewModel = value);
            if (_viewModel != null)
            {
                HookToViewModel();
            }
        }
    }

    public EquationControl StartExpressionEqn => _startEquation.Control;

    public EquationControl SecondTickExpressionEqn => _secondTickEquation.Control;

    public EquationControl EndExpressionEqn => _endEquation.Control;

    public bool ShowGridLines
    {
        get
        {
            return (bool)GetValue(ShowGridLinesProperty);
        }
        set
        {
            SetValue(ShowGridLinesProperty, value);
        }
    }

    public Color GridLineColor
    {
        get
        {
            return (Color)GetValue(GridLineColorProperty);
        }
        set
        {
            SetValue(GridLineColorProperty, value);
        }
    }

    private static void LineColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is PlotGraphAxisBase plotGraphAxisBase)
        {
            plotGraphAxisBase.LineBrush.Color = plotGraphAxisBase.LineColor;
        }
        else
        {
            DebugTools.DevAssert(false, null);
        }
    }

    private void InitializeLimitEquation(EquationControl limitEquation)
    {
        limitEquation.Style = new Style(typeof(EquationControl));
        HideLimit(limitEquation);
        limitEquation.IsKeyboardFocusWithinChanged += LimitEditorLostFocus;
        limitEquation.SExpressionChanging += LimitEquationSExpressionChanging;
        VisualChildren.Add(limitEquation);
    }

    private void LimitEquationSExpressionChanging(string sExpression)
    {
        LimitSExpressionChanging = true;
    }

    private void LimitEditorLostFocus(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (StartExpressionEqn.IsKeyboardFocusWithin || EndExpressionEqn.IsKeyboardFocusWithin || SecondTickExpressionEqn.IsKeyboardFocusWithin)
        {
            ShowLimitsExpressions();
        }
        else
        {
            HideLimitsExpressions();
        }
    }

    protected static void ShowLimit(UIElement limit)
    {
        limit.Opacity = 1.0;
        limit.IsHitTestVisible = true;
    }

    private static void HideLimit(EquationControl limit)
    {
        if (!limit.IsKeyboardFocusWithin && limit.ErrorCount == 0)
        {
            limit.Opacity = 0.0;
            limit.IsHitTestVisible = false;
        }
    }
}
