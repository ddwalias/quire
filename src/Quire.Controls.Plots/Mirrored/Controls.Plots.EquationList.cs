#nullable disable
#pragma warning disable

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Quire;
using Quire.Controls.Core;
using Quire.Controls.Plots.Presenter;
using Quire.Controls.Plots.ViewModel;

namespace Quire.Controls.Plots;

public partial class EquationList
{
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

    public int EquationCount => EquationListGrid.Children.Count;

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

    internal double MaxListWidth { get; set; }

    internal double MaxListHeight { get; set; }

    public int SelectedItemIndex { get; private set; }

    public EquationListItem SelectedItem => EquationListGrid.Children[SelectedItemIndex] as EquationListItem;

    public bool InCustomInitialization { get; set; }

    public bool EquationItemWOUnit { get; set; }

    public IPlotEquation this[int itemIndex] => ((EquationListItem)EquationListGrid.Children[itemIndex]).Equation;

    private IEnumerable<EquationListItem> EquationListItemsEnumerable => EquationListGrid.Children.Cast<EquationListItem>();

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

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IMathFormat DeserializeMathFormat { get; set; }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IDictionary<IdLabels, ExtendedFont> DeserializeLabeledIdFormat { get; set; }

    private static void HandleItemsMarginChanged(DependencyObject dObj, DependencyPropertyChangedEventArgs eArgs)
    {
        ((EquationList)(object)dObj).SetMinMaxWidth();
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
}
