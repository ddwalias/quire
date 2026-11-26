#nullable disable
#pragma warning disable

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using Quire;
using Quire.Controls.Core;
using Quire.Controls.Plots.Presenter;

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
}
