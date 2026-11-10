#nullable disable
using Quire.Calculation.Core;
using Quire.Controls.Core;
using Quire.EquationEditor.SpecTable;
using Quire.Controls.Core.IORegions;
using Quire.Matrices;
using Quire.PersistentData;
using System.Windows;
using Quire.EquationEditor.SpecTable.Model;
using System.Windows.Input;
using System.Windows.Media;
using System;

namespace Quire.Controls;

public partial class SpecTableControl
{
    public bool CalcIsDisabled
    {
        get => (bool)GetValue(CalcIsDisabledProperty);
        set => SetValue(CalcIsDisabledProperty, value);
    }

    public bool IsEmpty => false;

    public bool IsActive
    {
        get;
        set;
    }

    public SynchronizationStatus SynchronizationStatus
    {
        get => (SynchronizationStatus)GetValue(SynchronizationStatusProperty);
        set => SetValue(SynchronizationStatusProperty, value);
    }

    public CalculatingStatus CalculatingStatus
    {
        get => (CalculatingStatus)GetValue(CalculatingStatusProperty);
        set => SetValue(CalculatingStatusProperty, value);
    }

    public bool CanHideFromCalculationContext => true;

    public MathFormat MathFormat
    {
        get;
        set;
    }

    public void SimulateMouseClick(System.Windows.Point location, int numberOfClicks)
    {
        Activate(true);
    }

    private void CanNavigateDown(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = Model.CanNavigationBetweenCells(Map.GetAssociatedModel(ActiveCell), Direction.Down, false);
    }

    private void CanNavigateUp(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = Model.CanNavigationBetweenCells(Map.GetAssociatedModel(ActiveCell), Direction.Up, false);
    }

    private void CanNavigateRight(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = Model.CanNavigationBetweenCells(Map.GetAssociatedModel(ActiveCell), Direction.Right, false);
    }

    private void CanNavigateLeft(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = Model.CanNavigationBetweenCells(Map.GetAssociatedModel(ActiveCell), Direction.Left, false);
    }

    private void OnNavigateRightByTerm(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
    {
        Model.HandleNavigationBetweenCells(Map.GetAssociatedModel(ActiveCell), Direction.Right, true);
    }

    private void OnNavigateLeftBuyTerm(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
    {
        Model.HandleNavigationBetweenCells(Map.GetAssociatedModel(ActiveCell), Direction.Left, true);
    }

    private void OnNavigateDown(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
    {
        Model.HandleNavigationBetweenCells(Map.GetAssociatedModel(ActiveCell), Direction.Down, false);
    }

    private void OnNavigateUp(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
    {
        Model.HandleNavigationBetweenCells(Map.GetAssociatedModel(ActiveCell), Direction.Up, false);
    }

    private void OnNavigateRight(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
    {
        Model.HandleNavigationBetweenCells(Map.GetAssociatedModel(ActiveCell), Direction.Right, false);
    }

    private void OnNavigateLeft(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
    {
        Model.HandleNavigationBetweenCells(Map.GetAssociatedModel(ActiveCell), Direction.Left, false);
    }

    private void Commit()
    {
        Model.Commit();
    }

    private void HandleRequestEvaluation(Quire.Calculation.Core.ICalculatable calculatable)
    {
        Commit();
    }

    private void FireChangedEvent()
    {
        RaiseEvent(new RoutedEventArgs(SpecTableControl.ChangedEvent, this));
    }

    public Quire.EquationEditor.SpecTable.Model.ISpecTableColumn GetColumnFromIndex(int index)
    {
        return Model.GetColumn(index);
    }

    public void FireContentChangedEvents()
    {
        FireContentAndRegionChangedEvents(IORegionChangeType.Content);
    }

    public bool SubstituteAndCommitInputValue(string sExpr, string valueSExpr, string scalingSExpr)
    {
        return Model.SubstituteAndCommitInputValue(sExpr, valueSExpr, scalingSExpr, IsActive);
    }

    internal Quire.Controls.EquationControl GetCellAt(int row, int column)
    {
        return View.GetCellAt(row, column);
    }

    public double VerticalAlignmentOffset => View.VerticalAlignmentOffset;

    public int NumberOfColumns => Model.ColumnCount;

    public int Columns => Model.ColumnCount;

    public string RegionSExpression => Model.SExpression;

    private static void HandleCalcIsDisabledChanged(System.Windows.DependencyObject d, System.Windows.DependencyPropertyChangedEventArgs e)
    {
        SpecTableControl table = d as SpecTableControl;
        if (table != null)
        {
            table.FireContentAndRegionChangedEvents(IORegionChangeType.CalcIsDisabledStatus);
        }
    }

    private void ExecuteClearMathFormat(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
    {
        MathFormat.ClearFormat();
    }

    private void ExecuteIncreaseFontSize(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
    {
        MathFormat.IncreaseFontSize();
    }

    private void ExecuteDecreaseFontSize(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
    {
        MathFormat.DecreaseFontSize();
    }

    private void ExecuteChangeFontColor(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
    {
        MathFormat.ChangeFontColor((Color)e.Parameter);
    }

    private void ExecuteChangeHighlightColor(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
    {
        MathFormat.ChangeHighlightColor((Color?)e.Parameter);
    }

    public static Quire.Controls.SpecTableControl DefinitionListToSpecTable(System.Collections.Generic.List<Quire.EquationEditor.Tree.MathNode> definionsList)
    {
        ISpecTableModel model = SpecTableModel.DefinitionsListToSpecTableModel(definionsList);
        if (model == null)
        {
            return null;
        }

        return new SpecTableControl(model);
    }

    public bool HighlightAll(string text, FindOptions options)
    {
        throw new NotImplementedException();
    }

    public override bool ShouldSerializeContent()
    {
        return false;
    }

    private void FireCalculatableRemoved(ICalculatable wrapper)
    {
        if (CalculatableRemoved != null)
        {
            CalculatableRemoved.Invoke(wrapper);
        }
    }

    protected override void OnMouseDown(MouseButtonEventArgs e)
    {
        DelegateFocus();
    }

    public bool FirstPropertyForXamlSerialization => true;

    public bool IsCalculating
    {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }

    public MathFormat CurrentMathFormat
    {
        get => MathFormat;
        set => throw new NotImplementedException();
    }
}
