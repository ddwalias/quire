#nullable disable
#pragma warning disable

using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using Quire.Controls.Core;
using Quire.Controls.Plots.Base3D.ViewModel;
using Quire.Controls.Plots.Presenter;
using Quire.Wpf;

namespace Quire.Controls.Plots.Plot3D.View;

public partial class PlotAxesWidget3D
{
    public static class PropertyNames
    {
        public const string ActiveAxis = "ActiveAxis";

        public const string HighlightedAxis = "HighlightedAxis";
    }

    private AxesType _activeAxisType;

    private AxesType? _highlightedAxis;

    private readonly AxesType[] _axisTypesOrderedByXyz = new AxesType[3]
    {
        AxesType.X,
        AxesType.Y,
        AxesType.Z
    };

    private AxisSelectorWidget AxisSelector { get; set; }

    internal PlotGraphAxis XAxis => _xAxis;

    internal PlotGraphAxis YAxis => _yAxis;

    internal PlotGraphAxis ZAxis => _zAxis;

    internal Plot3DLinearAxis ActiveAxis => GetAxis(ActiveAxisType);

    public PlotAxesWidget3D(Base3DAxisViewModel xAxisViewModel, Base3DAxisViewModel yAxisViewModel, Base3DAxisViewModel zAxisViewModel)
    {
        InitializeComponent();
        _xAxis.ViewModel = xAxisViewModel;
        _yAxis.ViewModel = yAxisViewModel;
        _zAxis.ViewModel = zAxisViewModel;
        DataContext = (_axisSelectorControl.DataContext = new PlotAxesWidget3DViewModel(xAxisViewModel, yAxisViewModel, zAxisViewModel));
        ActivateAxisInternal(AxesType.Z);
        HighlightedAxis = null;
        CommandBindings.Add(new CommandBinding(EquationCommands.Traverse, OnTraverseForward, OnCanTraverseForward));
        CommandBindings.Add(new CommandBinding(EquationCommands.BackwardsTraverse, OnTraverseBackward, OnCanTraverseBackwards));
        IsVisibleChanged += OnIsVisibleChanged;
    }

    private void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs args)
    {
        if (AxisSelector != null)
        {
            AxisSelector.Dispose();
            AxisSelector = null;
        }
        if (IsVisible)
        {
            AxisSelector = new AxisSelectorWidget(this, _axisSelectorControl);
        }
    }

    private void OnCanTraverseForward(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = CanTraverse;
    }

    private void OnTraverseForward(object sender, ExecutedRoutedEventArgs e)
    {
        OnTraverse(FocusNavigationDirection.Next);
    }

    private void OnCanTraverseBackwards(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = CanTraverse;
    }

    private void OnTraverseBackward(object sender, ExecutedRoutedEventArgs e)
    {
        OnTraverse(FocusNavigationDirection.Previous);
    }

    private bool CanTraverse => IsKeyboardFocusWithin;

    private void OnTraverse(FocusNavigationDirection direction)
    {
        EquationControl nextEquation = GetNextTraversalItemOnAxis(direction);
        if (nextEquation == null)
        {
            AxesType[] orderedAxes = ((direction == FocusNavigationDirection.Next) ? _axisTypesOrderedByXyz : _axisTypesOrderedByXyz.Reverse().ToArray());
            int nextIndex = (Array.IndexOf(orderedAxes, ActiveAxisType) + 1) % orderedAxes.Length;
            AxesType axisTypeToActivate = orderedAxes[nextIndex];
            ActivateAxisInternal(axisTypeToActivate);
            nextEquation = ((direction == FocusNavigationDirection.Next) ? ActiveAxis.StartExpressionEqn : ActiveAxis.EndExpressionEqn);
        }
        ActivateAxisEquation(nextEquation, direction);
    }

    private static void ActivateAxisEquation(EquationControl nextEquation, FocusNavigationDirection? direction)
    {
        DebugTools.DevAssert(nextEquation.IsLoaded, null);
        Func<bool> focusAction = delegate
        {
            if (nextEquation.IsKeyboardFocusWithin)
            {
                return true;
            }
            if (!nextEquation.IsMeasureValid || !nextEquation.IsArrangeValid)
            {
                return false;
            }
            nextEquation.Focus();
            if (direction.HasValue)
            {
                nextEquation.RaiseEvent(new CustomRoutedEventArgs<FocusNavigationDirection>(CommonRoutedEvents.TraversedFocusEvent, direction.Value));
            }
            return true;
        };
        if (!focusAction())
        {
            EventHandler focusOnLayoutUpdated = null;
            focusOnLayoutUpdated = delegate
            {
                DebugTools.DevAssert(focusAction(), null);
                nextEquation.LayoutUpdated -= focusOnLayoutUpdated;
            };
            nextEquation.LayoutUpdated += focusOnLayoutUpdated;
        }
    }

    private EquationControl GetNextTraversalItemOnAxis(FocusNavigationDirection direction)
    {
        List<EquationControl> list = CollectAxesEquations(new Plot3DLinearAxis[1] { ActiveAxis });
        if (direction == FocusNavigationDirection.Previous)
        {
            list.Reverse();
        }
        int nextIndex = 1 + GetCurrentFocusedEquationIndex(list);
        if (nextIndex >= list.Count)
        {
            return null;
        }
        return list[nextIndex];
    }

    private static int GetCurrentFocusedEquationIndex(List<EquationControl> equationsList)
    {
        return equationsList.FindIndex((EquationControl equation) => equation.IsKeyboardFocusWithin);
    }

    private static List<EquationControl> CollectAxesEquations(IEnumerable<IPlotAxisView> axes)
    {
        List<EquationControl> editableAxisRelatedElements = new List<EquationControl>();
        axes.Each(delegate(IPlotAxisView axis)
        {
            editableAxisRelatedElements.AddRange(axis.CollectElementsForTraversal());
        });
        return editableAxisRelatedElements;
    }

    internal AxesType ActiveAxisType
    {
        get
        {
            return _activeAxisType;
        }
        private set
        {
            if (value != _activeAxisType)
            {
                _activeAxisType = value;
                FirePropertyChanged("ActiveAxis");
            }
        }
    }

    internal AxesType? HighlightedAxis
    {
        get
        {
            return _highlightedAxis;
        }
        set
        {
            if (value != _highlightedAxis)
            {
                _highlightedAxis = value;
                FirePropertyChanged("HighlightedAxis");
            }
        }
    }

    internal Plot3DLinearAxis GetAxis(AxesType axisType)
    {
        switch (axisType)
        {
        case AxesType.X:
            return _xAxis;
        case AxesType.Y:
            return _yAxis;
        case AxesType.Z:
            return _zAxis;
        default:
            return null;
        }
    }

    private IEnumerable<Plot3DLinearAxis> AllAxes()
    {
        yield return _xAxis;
        yield return _yAxis;
        yield return _zAxis;
    }

    internal void ActivateAxis(AxesType axisTypeToActivate, FocusNavigationDirection? direction = FocusNavigationDirection.Next)
    {
        List<EquationControl> equationsList = CollectAxesEquations(new Plot3DLinearAxis[1] { ActiveAxis });
        int index = GetCurrentFocusedEquationIndex(equationsList);
        if (index == -1)
        {
            index = 0;
        }
        ActivateAxisInternal(axisTypeToActivate);
        equationsList = CollectAxesEquations(new Plot3DLinearAxis[1] { ActiveAxis });
        if ((AssertionE)DebugTools.DevAssert(equationsList.Count > index, null) == AssertionE.Failed)
        {
            index = equationsList.Count - 1;
        }
        ActivateAxisEquation(equationsList[index], direction);
    }

    private void ActivateAxisInternal(AxesType axisTypeToActivate)
    {
        if (ActiveAxisType == axisTypeToActivate)
        {
            return;
        }
        Plot3DLinearAxis newActiveAxis = GetAxis(axisTypeToActivate);
        AllAxes().Each(delegate(Plot3DLinearAxis axis)
        {
            axis.Visibility = ((axis != newActiveAxis) ? Visibility.Collapsed : Visibility.Visible);
        });
        ActiveAxisType = axisTypeToActivate;
    }

    public event PropertyChangedEventHandler PropertyChanged;

    private void FirePropertyChanged(string propertyName)
    {
        if (PropertyChanged != null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    private static AxesType? AxisForSender(object sender)
    {
        FrameworkElement frameworkElement = sender as FrameworkElement;
        if (!DebugTools.AssertNotNull(frameworkElement, null).Passed)
        {
            return null;
        }
        return (AxesType)frameworkElement.Tag;
    }

    private void AxisButtonClick(object sender, RoutedEventArgs args)
    {
        AxesType axisTypeToActivate = AxisForSender(sender) ?? AxesType.Z;
        ActivateAxis(axisTypeToActivate, FocusNavigationDirection.Next);
    }

    private void AxisButtonMouseEnter(object sender, MouseEventArgs args)
    {
        HighlightedAxis = AxisForSender(sender);
    }

    private void AxisButtonMouseLeave(object sender, MouseEventArgs args)
    {
        DebugTools.DevAssert(AxisForSender(sender) == HighlightedAxis, null);
        HighlightedAxis = null;
    }

    private Grid _axisSelectorControl;

    private Plot3DLinearAxis _xAxis;

    private Plot3DLinearAxis _yAxis;

    private Plot3DLinearAxis _zAxis;

    private bool _contentLoaded;

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "10.0.11.0")]
    public void InitializeComponent()
    {
        if (_contentLoaded)
        {
            return;
        }
        _contentLoaded = true;
        Uri resourceLocator = new Uri("/Quire.Controls.Plots;component/plot3d/view/plotaxeswidget3d.xaml", UriKind.Relative);
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
        switch (connectionId)
        {
        case 1:
            _axisSelectorControl = (Grid)target;
            break;
        case 2:
            ((Button)target).Click += AxisButtonClick;
            ((Button)target).MouseEnter += AxisButtonMouseEnter;
            ((Button)target).MouseLeave += AxisButtonMouseLeave;
            break;
        case 3:
            ((Button)target).Click += AxisButtonClick;
            ((Button)target).MouseEnter += AxisButtonMouseEnter;
            ((Button)target).MouseLeave += AxisButtonMouseLeave;
            break;
        case 4:
            ((Button)target).Click += AxisButtonClick;
            ((Button)target).MouseEnter += AxisButtonMouseEnter;
            ((Button)target).MouseLeave += AxisButtonMouseLeave;
            break;
        case 5:
            _xAxis = (Plot3DLinearAxis)target;
            break;
        case 6:
            _yAxis = (Plot3DLinearAxis)target;
            break;
        case 7:
            _zAxis = (Plot3DLinearAxis)target;
            break;
        default:
            _contentLoaded = true;
            break;
        }
    }
}
