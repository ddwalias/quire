using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using Quire.Controls.Core.Excel;

namespace Quire.Controls;

public partial class ExcelGrid
{
    private bool _userSelection;
    internal global::System.Windows.Controls.DataGrid dataGrid1;

    public IEnumerable<ExcelRow> Rows
    {
        get
        {
            return (IEnumerable<ExcelRow>)GetValue(RowsProperty);
        }
        private set
        {
            SetValue(RowsPropertyKey, value);
        }
    }

    public IEnumerable<string> Columns
    {
        get
        {
            return (IEnumerable<string>)GetValue(ColumnsProperty);
        }
        private set
        {
            SetValue(ColumnsPropertyKey, value);
        }
    }

    public string SheetName
    {
        get
        {
            return (string)GetValue(SheetNameProperty);
        }
        private set
        {
            SetValue(SheetNamePropertyKey, value);
        }
    }

    public string SelectedRange
    {
        get
        {
            return (string)GetValue(SelectedRangeProperty);
        }
        set
        {
            SetValue(SelectedRangeProperty, value);
        }
    }

    public string SelectedColumns
    {
        get
        {
            return (string)GetValue(SelectedColumnsProperty);
        }
        set
        {
            SetValue(SelectedColumnsProperty, value);
        }
    }

    public string SelectedRows
    {
        get
        {
            return (string)GetValue(SelectedRowsProperty);
        }
        set
        {
            SetValue(SelectedRowsProperty, value);
        }
    }

    public bool SelectionValid
    {
        get
        {
            return (bool)GetValue(SelectionValidProperty);
        }
        set
        {
            SetValue(SelectionValidProperty, value);
        }
    }

    public double Zoom
    {
        get
        {
            return (double)GetValue(ZoomProperty);
        }
        set
        {
            SetValue(ZoomProperty, value);
        }
    }

    public bool WorksheetHasData
    {
        get
        {
            return (bool)GetValue(WorksheetHasDataProperty);
        }
        set
        {
            SetValue(WorksheetHasDataProperty, value);
        }
    }

    private static void SelectedRangeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ExcelGrid excelGrid = (ExcelGrid)d;
        if (excelGrid.IsLoaded)
        {
            excelGrid.SelectCells();
        }
        else
        {
            excelGrid.Dispatcher.BeginInvoke(new Action(excelGrid.SelectCells), DispatcherPriority.Loaded);
        }
    }

    private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (!(DataContext is ExcelSheet excelSheet))
        {
            return;
        }

        dataGrid1.Columns.Clear();
        foreach (string column in excelSheet.Columns)
        {
            Binding binding = new Binding("Cells")
            {
                Converter = new GetCellValueAtColumnConverter(),
                ConverterParameter = column
            };
            DataGridTextColumn item = new DataGridTextColumn
            {
                Header = column,
                Binding = binding
            };
            dataGrid1.Columns.Add(item);
        }

        Rows = excelSheet.Rows;
        Columns = excelSheet.Columns;
        SheetName = excelSheet.Name;
        if (dataGrid1.IsLoaded)
        {
            SelectAllDataIfNotEmpty();
        }
        else
        {
            dataGrid1.Dispatcher.BeginInvoke(new Action(SelectAllDataIfNotEmpty), DispatcherPriority.Loaded);
        }
    }

    private void SelectAllDataIfNotEmpty()
    {
        WorksheetHasData = Rows.Count() > 0;
        if (WorksheetHasData)
        {
            dataGrid1.SelectAllCells();
        }
        else
        {
            ClearSelection();
        }
    }

    private void OnSelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
    {
        HandleSelectedCellsChanged();
    }

    private void HandleSelectedCellsChanged()
    {
        IList<DataGridCellInfo> selectedCells = dataGrid1.SelectedCells;
        if (selectedCells == null || selectedCells.Count <= 0)
        {
            ClearSelection();
            return;
        }

        DataGridCellInfo first = selectedCells.First();
        DataGridCellInfo last = selectedCells.Last();
        string firstColumn = first.Column.Header.ToString();
        string lastColumn = last.Column.Header.ToString();
        int firstRow = ((ExcelRow)first.Item).Row;
        int lastRow = ((ExcelRow)last.Item).Row;
        _userSelection = true;
        SelectedColumns = string.Format("{0} - {1}", firstColumn, lastColumn);
        SelectedRows = string.Format("{0} - {1}", firstRow, lastRow);
        _userSelection = false;
        SetSelectedRange(firstRow, lastRow, firstColumn, lastColumn);
        SelectionValid = true;
    }

    private void ClearSelection()
    {
        _userSelection = true;
        SelectedColumns = string.Empty;
        SelectedRows = string.Empty;
        SelectedRange = string.Empty;
        _userSelection = false;
        SelectionValid = false;
    }

    private void dataGrid1_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
        {
            e.Handled = true;
        }
    }

    internal static void InvokeAddRegion(DataGrid dataGrid, int firstRow, int lastRow, int firstColumn, int lastColumn)
    {
        dataGrid.SelectedCells.GetType().GetMethod("AddRegion", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(dataGrid.SelectedCells, new object[4]
        {
            firstRow - 1,
            firstColumn,
            lastRow - firstRow + 1,
            lastColumn - firstColumn + 1
        });
    }

    private void SetSelectedRange(int firstRow, int lastRow, string firstColumn, string lastColumn)
    {
        SelectedRange = string.Format("{0}!{1}{2}:{3}{4}", SheetName, firstColumn, firstRow, lastColumn, lastRow);
    }
}
