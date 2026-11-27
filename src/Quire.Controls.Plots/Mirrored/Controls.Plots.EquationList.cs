#nullable disable
#pragma warning disable

using System;
using System.Collections.Generic;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Xml;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Input;
using System.Windows.Threading;
using Quire;
using Quire.Controls.Core;
using Quire.Controls.Plots.Presenter;
using Quire.Controls.Plots.ViewModel;
using Quire.Undo;
using Quire.Wpf;

namespace Quire.Controls.Plots;

public partial class EquationList
{

    private static void HandleItemsMarginChanged(DependencyObject dObj, DependencyPropertyChangedEventArgs eArgs)
    {
        ((EquationList)(object)dObj).SetMinMaxWidth();
    }
    static EquationList()
    {
        _typeOfThis = typeof(EquationList);
        ItemMarginProperty = DependencyProperty.Register("ItemMargin", typeof(uint), _typeOfThis, new PropertyMetadata(20u, HandleItemsMarginChanged));
        InfiniteSize = new Size(double.PositiveInfinity, double.PositiveInfinity);
        FrameworkElement.WidthProperty.OverrideMetadata(typeof(EquationList), new FrameworkPropertyMetadata(double.NaN, WidthListChanged));
    }
    private static readonly global::System.Type _typeOfThis;
    private const uint ItemMarginDefault = 20u;
    private global::Quire.Controls.Plots.IEquationListViewModel _viewModel;
    private readonly global::System.Windows.RoutedEventHandler _focusChangeEventHandler;
    public static readonly global::System.Windows.DependencyProperty ItemMarginProperty;
    private static readonly global::System.Windows.Size InfiniteSize;
    internal global::Quire.Controls.Plots.EquationListContents EquationListGrid;
    private bool _contentLoaded;
    public IEquationListViewModel ViewModel
    {
        get
        {
            return _viewModel;
        }
        set
        {
            if (_viewModel == value)
            {
                return;
            }
            if (_viewModel != null)
            {
                RequestAddEquation -= _viewModel.HandleRequestAddEquation;
                EquationRemoved -= _viewModel.HandleEquationRemoved;
                RemoveHandler(UIElement.GotFocusEvent, _focusChangeEventHandler);
                RemoveHandler(UIElement.LostFocusEvent, _focusChangeEventHandler);
            }
            _viewModel = value;
            if (_viewModel != null)
            {
                RequestAddEquation += _viewModel.HandleRequestAddEquation;
                EquationRemoved += _viewModel.HandleEquationRemoved;
                AddHandler(UIElement.GotFocusEvent, _focusChangeEventHandler, handledEventsToo: true);
                AddHandler(UIElement.LostFocusEvent, _focusChangeEventHandler, handledEventsToo: true);
            }
        }
    }

    public uint ItemMargin
    {
        get
        {
            return (uint)((DependencyObject)this).GetValue(ItemMarginProperty);
        }
        set
        {
            ((DependencyObject)this).SetValue(ItemMarginProperty, (object)value);
        }
    }
    public event global::System.Action<int, int> RequestAddEquation;
    public event global::System.Action<global::Quire.Controls.Plots.Presenter.IPlotEquation, int, int, global::Quire.Direction> EquationRemoved;
    public EquationList()
    {
        _focusChangeEventHandler = (object sender, RoutedEventArgs routedEventArgs) =>
        {
            ViewModel.EquationListHasFocus = this.IsLogicalFocusWithin();
        };
        RightDraggerVisibility = Visibility.Visible;
        InitializeComponent();
        BindCommands();
    }

    private void BindCommands()
    {
        CommandBindings.Add(new CommandBinding(EquationCommands.InsertRow, HandleInsertItemToLegendCommand));
        CommandBindings.Add(new CommandBinding(EquationCommands.NavigateLeft, OnNavigateLeft, CanNavigateLeft));
        CommandBindings.Add(new CommandBinding(EquationCommands.NavigateRight, OnNavigateRight, CanNavigateRight));
        CommandBindings.Add(new CommandBinding(EquationCommands.NavigateUp, OnNavigateUp, CanNavigateUp));
        CommandBindings.Add(new CommandBinding(EquationCommands.NavigateDown, OnNavigateDown, CanNavigateDown));
    }

    private void CanNavigateDown(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = CanNavigate(SelectedItem.Equation, Direction.Down);
    }

    private void CanNavigateUp(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = CanNavigate(SelectedItem.Equation, Direction.Up);
    }

    private void CanNavigateRight(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = CanNavigate(SelectedItem.Equation, Direction.Right);
    }

    private void CanNavigateLeft(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = CanNavigate(SelectedItem.Equation, Direction.Left);
    }

    private void OnNavigateDown(object sender, ExecutedRoutedEventArgs e)
    {
        EquationControlNavigateOffOfEquation(Direction.Down, isJumpNavigation: false);
    }

    private void OnNavigateUp(object sender, ExecutedRoutedEventArgs e)
    {
        EquationControlNavigateOffOfEquation(Direction.Up, isJumpNavigation: false);
    }

    private void OnNavigateRight(object sender, ExecutedRoutedEventArgs e)
    {
        EquationControlNavigateOffOfEquation(Direction.Right, isJumpNavigation: false);
    }

    private void OnNavigateLeft(object sender, ExecutedRoutedEventArgs e)
    {
        EquationControlNavigateOffOfEquation(Direction.Left, isJumpNavigation: false);
    }

    public EquationListItem GetItem(int index)
    {
        return EquationListGrid.Children[index] as EquationListItem;
    }

    public int EquationCount => EquationListGrid.Children.Count;
    public global::System.Collections.Generic.IEnumerable<global::Quire.Controls.Plots.Presenter.IPlotEquation> Equations => from EquationListItem item in EquationListGrid.Children select item.Equation;

    internal int Columns
    {
        get
        {
            return EquationListGrid.Columns;
        }
        set
        {
            if (EquationListGrid.Columns != value)
            {
                EquationListGrid.Columns = value;
                EquationListGrid.InvalidateMeasure();
                SetMinMaxWidth();
            }
        }
    }

    private void MoveToEquation(int i, bool shouldMoveToEnd)
    {
        this[i].DelegateFocus();
        if (shouldMoveToEnd)
        {
            EquationCommands.NavigateToEquationEnd.Execute(null, this[i]);
        }
        else
        {
            EquationCommands.NavigateToEquationStart.Execute(null, this[i]);
        }
        ChangeSelectedItem(EquationListGrid.Children[SelectedItemIndex] as EquationListItem);
    }

    public void MoveToEquationStart(int i)
    {
        MoveToEquation(i, shouldMoveToEnd: false);
    }

    public void MoveToEquationEnd(int i)
    {
        MoveToEquation(i, shouldMoveToEnd: true);
    }

    public bool IsItemActive(int indexToCheck)
    {
        return SelectedItemIndex == indexToCheck;
    }

    private static void WidthListChanged(DependencyObject dObj, DependencyPropertyChangedEventArgs eArgs)
    {
        EquationList equationList = dObj as EquationList;
        double newWidth = (double)eArgs.NewValue;
        if (equationList == null)
        {
            return;
        }
        if (equationList.MaxListWidth <= 0.0)
        {
            equationList.SetMinMaxWidth();
            if (equationList.Visibility == Visibility.Collapsed)
            {
                ForceMeasure(equationList.EquationListGrid);
            }
        }
        if (equationList.MaxListWidth <= 0.0)
        {
            return;
        }
        equationList.Columns = equationList.EquationListGrid.AdjustNumberOfColumnsToWidth(newWidth);
    }

    private EquationListItem AddItem(int addedItemIndex)
    {
        FireRequestAddEquation(addedItemIndex);
        return SelectedItem;
    }

    internal double MaxListWidth { get; set; }
    internal double MaxListHeight { get; set; }
    public int SelectedItemIndex { get; private set; }
    public EquationListItem SelectedItem => EquationListGrid.Children[SelectedItemIndex] as EquationListItem;
    internal global::Quire.Controls.Plots.Presenter.IEquationListItem AddItemToLegend(int index, global::Quire.Controls.Plots.Presenter.IEquationListItem equationListItem, bool focus) {
		if (equationListItem == null)
		{
			equationListItem = new EquationListItem
			{
				Equation = GetNewEquation()
			};
		}
		if (focus)
		{
			((DispatcherObject)this).Dispatcher.BeginInvoke((DispatcherPriority)6, (Delegate)(Action)(() =>
			{
				equationListItem.Equation.DelegateFocus();
			}));
		}
		RegisterToEvents(equationListItem);
		EquationListGrid.Children.Insert(index, (UIElement)equationListItem);
		SetMinMaxWidth();
		ChangeSelectedItem((EquationListItem)equationListItem);
		return equationListItem;
	}
    private global::Quire.Controls.Plots.Presenter.IPlotEquation GetNewEquation() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public bool InCustomInitialization { get; set; }
    public bool EquationItemWOUnit { get; set; }

    private void RegisterToEvents(IEquationListItem equationListItem)
    {
        IPlotEquation equation = equationListItem.Equation;
        equationListItem.ItemChange += EquationListItemItemChange;
        if (equation != null)
        {
            equation.RequestRemove += EquationControlRequestRemove;
            if (equation is IPlotEquationScaled plotEquationScaled)
            {
                plotEquationScaled.ScalingChanged += ScalingChanged;
            }
        }
        if (equationListItem.Equation is FrameworkElement frameworkElement)
        {
            frameworkElement.SizeChanged += EquationControlSizeChanged;
            frameworkElement.IsKeyboardFocusWithinChanged += OnIsKeyboardFocusWithinChanged;
        }
    }

    private void UnregisterFromEvents(IEquationListItem equationListItem)
    {
        IPlotEquation equation = equationListItem.Equation;
        equationListItem.ItemChange -= EquationListItemItemChange;
        if (equation != null)
        {
            equation.RequestRemove -= EquationControlRequestRemove;
            if (equation is IPlotEquationScaled plotEquationScaled)
            {
                plotEquationScaled.ScalingChanged -= ScalingChanged;
            }
        }
        if (equationListItem.Equation is FrameworkElement frameworkElement)
        {
            frameworkElement.SizeChanged -= EquationControlSizeChanged;
            frameworkElement.IsKeyboardFocusWithinChanged -= OnIsKeyboardFocusWithinChanged;
        }
    }

    private void ScalingChanged(IPlotEquationScaled eq)
    {
        eq?.ShowOrHideScaling(IsKeyboardFocusWithin);
    }

    private void OnIsKeyboardFocusWithinChanged(object sender, global::System.Windows.DependencyPropertyChangedEventArgs e) {
		EquationListItemsEnumerable.Each((EquationListItem item) =>
		{
			if (item.Equation is PlotEquation plotEquation)
			{
				plotEquation.ShowOrHideScaling(IsKeyboardFocusWithin);
			}
		});
		if (IsKeyboardFocusWithin)
		{
			FireActiveEquationChanged(sender as IPlotEquation);
		}
	}

    private void FireActiveEquationChanged(IPlotEquation equationControl)
    {
        if (ActiveEquationChanged != null)
        {
            ActiveEquationChanged(equationControl);
        }
    }

    private void AddItem(string sExpression)
    {
        EquationListItem equationListItem = AddEquationByPosition();
        if (!string.IsNullOrEmpty(sExpression))
        {
            equationListItem.Equation.EquationSExpression = sExpression;
        }
    }

    private IPlotEquation AddItem(XmlNode xmlNode)
    {
        EquationListItem equationListItem = AddItem(EquationCount);
        if (xmlNode != null)
        {
            equationListItem.Equation.SetRegionXmlNode(xmlNode);
        }
        return equationListItem.Equation;
    }

    private void FireRequestAddEquation(int index)
    {
        if (RequestAddEquation != null)
        {
            RequestAddEquation(index, SelectedItemIndex);
        }
    }

    private void EquationListItemItemChange(EquationListItem equationListItem)
    {
        ChangeSelectedItem(equationListItem);
    }

    private void ChangeSelectedItem(EquationListItem equationListItem)
    {
        SelectedItemIndex = EquationListGrid.Children.IndexOf(equationListItem);
    }

    public int IndexOf(IPlotEquation plotEquation)
    {
        UIElementCollection children = EquationListGrid.Children;
        int count = children.Count;
        for (int num = 0; num < count; num++)
        {
            if (children[num] is IEquationListItem equationListItem && equationListItem.Equation == plotEquation)
            {
                return num;
            }
        }
        return -1;
    }

    internal void RequestRemoveItem(EquationListItem equationListItem, Direction direction)
    {
        if (EquationListGrid.Children.Count != 1)
        {
            int num = EquationListGrid.Children.IndexOf(equationListItem);
            if ((direction == Direction.Left || num == EquationListGrid.Children.Count - 1) && SelectedItemIndex != 0)
            {
                FireEquationRemoved(equationListItem, num, num - 1, direction);
            }
            else
            {
                FireEquationRemoved(equationListItem, num, num, direction);
            }
        }
        else if (!equationListItem.Equation.IsPlotEquationEmpty)
        {
            CommandBuffer.Execute(new ClearPlotEquationCommand(equationListItem.Equation));
        }
    }

    private void EquationControlRequestRemove(object sender, Direction direction)
    {
        if (!(sender is IPlotEquation))
        {
            return;
        }
        EquationListItem equationListItem = GetEquationListItem((IPlotEquation)sender);
        RequestRemoveItem(equationListItem, direction);
    }

    internal EquationListItem RemoveItemAt(int itemToDeleteIndex, int indexOfPreviousItem, Direction direction)
    {
        EquationListItem item = GetItem(itemToDeleteIndex);
        UnregisterFromEvents(item);
        EquationListGrid.Children.RemoveAt(itemToDeleteIndex);
        SetMinMaxWidth();
        ChangeSelectedItem(GetItem(indexOfPreviousItem));
        this[indexOfPreviousItem].DelegateFocus();
        if (direction == Direction.Left)
        {
            EquationCommands.NavigateToEquationEnd.Execute(Direction.Right, this[SelectedItemIndex]);
        }
        if (direction == Direction.Right)
        {
            EquationCommands.NavigateToEquationStart.Execute(null, this[SelectedItemIndex]);
        }
        return item;
    }

    private void FireEquationRemoved(EquationListItem equation, int deletedItemIndex, int priviousItemIndex, Direction direction)
    {
        if (EquationRemoved != null)
        {
            EquationRemoved(equation.Equation, deletedItemIndex, priviousItemIndex, direction);
        }
    }

    private void EquationControlSizeChanged(object sender, SizeChangedEventArgs e)
    {
        SetMinMaxWidth(e.PreviousSize.Width, e.NewSize.Width);
    }

    private void SetMinMaxWidth()
    {
        SetMinMaxWidth(MaxListWidth, 0.0);
    }
    public void ArrangeEquationsHorizontally() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void SetMinMaxWidth(double previousWidth, double newWidth) {
		if (EquationListGrid.Children.OfType<EquationListItem>().IsNullOrEmpty())
		{
			return;
		}
		if ((previousWidth == MaxListWidth && newWidth < MaxListWidth) || MaxListWidth == 0.0)
		{
			MaxListWidth = EquationListGrid.Children.OfType<EquationListItem>().Max((EquationListItem item) => GetEffectiveEquationWidth(item.Equation));
			MaxWidth = (double)EquationCount * (MaxListWidth + (double)ItemMargin);
			EquationListGrid.MaxWidth = MaxWidth;
			if (double.IsNaN(Width))
			{
				Width = (double)Columns * (MaxListWidth + (double)ItemMargin);
			}
			MinWidth = MaxListWidth + (double)ItemMargin;
		}
		if (newWidth > MaxListWidth)
		{
			MaxListWidth = newWidth;
			if (double.IsNaN(Width))
			{
				Width = (double)Columns * (MaxListWidth + (double)ItemMargin);
			}
			MaxWidth = (double)EquationCount * (MaxListWidth + (double)ItemMargin);
			EquationListGrid.MaxWidth = MaxWidth;
			MinWidth = MaxListWidth + (double)ItemMargin;
		}
	}

    private double GetEffectiveEquationWidth(IPlotEquation equation)
    {
        FrameworkElement frameworkElement = (FrameworkElement)equation;
        if (Visibility != Visibility.Collapsed)
        {
            return frameworkElement.ActualWidth;
        }
        ForceMeasure(frameworkElement);
        Size desiredSize = frameworkElement.DesiredSize;
        return desiredSize.Width;
    }

    private static void ForceMeasure(FrameworkElement element)
    {
        element.Measure(InfiniteSize);
    }

    private bool CanNavigate(IPlotEquation equation, Direction direction)
    {
        if (equation.Equals(this[0]) && (direction == Direction.Left || direction == Direction.Up))
        {
            return false;
        }
        if (equation.Equals(this[EquationCount - 1]) && (direction == Direction.Right || direction == Direction.Down))
        {
            return false;
        }
        return true;
    }

    private void EquationControlNavigateOffOfEquation(Direction direction, bool isJumpNavigation)
    {
        if (!isJumpNavigation)
        {
            if (direction == Direction.Right && SelectedItemIndex != EquationCount - 1)
            {
                MoveToEquationStart(SelectedItemIndex + 1);
            }
            else if (direction == Direction.Left && SelectedItemIndex != 0)
            {
                MoveToEquationEnd(SelectedItemIndex - 1);
            }
            else if (direction == Direction.Up && SelectedItemIndex - Columns >= 0)
            {
                MoveToEquationStart(SelectedItemIndex - Columns);
            }
            else if (direction == Direction.Down && SelectedItemIndex + Columns <= EquationCount - 1)
            {
                MoveToEquationStart(SelectedItemIndex + Columns);
            }
        }
    }

    public IPlotEquation this[int itemIndex] => ((EquationListItem)EquationListGrid.Children[itemIndex]).Equation;

    private IEnumerable<EquationListItem> EquationListItemsEnumerable => EquationListGrid.Children.Cast<EquationListItem>();
    private global::Quire.Controls.Plots.EquationListItem GetEquationListItem(global::Quire.Controls.Plots.Presenter.IPlotEquation plotEquation) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public void AddTraceToEquation(IPlotTrace trace, IPlotEquation plotEquation)
    {
        GetEquationListItem(plotEquation).AddTrace(trace);
    }

    public void AddTraceToEquation(IPlotTrace trace, IPlotEquation plotEquation, int index)
    {
        GetEquationListItem(plotEquation).AddTrace(trace, index);
    }

    public void RemoveTraceFromEquation(int traceIndex, IPlotEquation plotEquation)
    {
        GetEquationListItem(plotEquation).RemoveTrace(traceIndex);
    }

    public void AddEquation(string expression)
    {
        AddItem(expression);
    }

    public IPlotEquation AddEquation(XmlNode xmlNode)
    {
        return AddItem(xmlNode);
    }

    public IEquationListItem AddEquation(int index)
    {
        return AddItemToLegend(index, null, focus: false);
    }

    public EquationListItem AddEquationByPosition()
    {
        if (EquationCount == 0)
        {
            return AddItem(0);
        }
        if (this[SelectedItemIndex].AtLeftEdge && this[SelectedItemIndex].Expression.EquationSExpression != "@PLACEHOLDER")
        {
            return AddItem(SelectedItemIndex);
        }
        return AddItem(SelectedItemIndex + 1);
    }

    private bool ContainsLabels
    {
        get
        {
            bool result = false;
            foreach (UIElement child in EquationListGrid.Children)
            {
                if (child is EquationListItem)
                {
                    result = (child as EquationListItem).ContainsLabels;
                }
            }
            return result;
        }
    }
    private void HandleInsertItemToLegendCommand(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "10.0.11.0")]
    public void InitializeComponent()
    {
        if (_contentLoaded)
        {
            return;
        }
        _contentLoaded = true;
        Uri resourceLocator = new Uri("/Quire.Controls.Plots;component/equationlist.xaml", UriKind.Relative);
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
            EquationListGrid = (EquationListContents)target;
        }
        else
        {
            _contentLoaded = true;
        }
    }
    public event global::System.Action<global::Quire.Controls.Plots.Presenter.IPlotEquation> ActiveEquationChanged;

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IMathFormat DeserializeMathFormat { get; set; }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IDictionary<IdLabels, ExtendedFont> DeserializeLabeledIdFormat { get; set; }
}
