#nullable disable
#pragma warning disable

using System;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Microsoft.Win32;
using Quire.Controls.Core.Excel;

namespace Quire;

public partial class ImportExcel
{
    public static readonly global::System.Windows.DependencyProperty SelectedRangeProperty = global::System.Windows.DependencyProperty.Register("SelectedRange", typeof(string), typeof(global::Quire.ImportExcel));
    public static readonly global::System.Windows.DependencyProperty FileNameProperty = global::System.Windows.DependencyProperty.Register("FileName", typeof(string), typeof(global::Quire.ImportExcel));
    public static readonly global::System.Windows.DependencyProperty SelectedColumnsProperty = global::System.Windows.DependencyProperty.Register("SelectedColumns", typeof(string), typeof(global::Quire.ImportExcel));
    public static readonly global::System.Windows.DependencyProperty SelectedRowsProperty = global::System.Windows.DependencyProperty.Register("SelectedRows", typeof(string), typeof(global::Quire.ImportExcel));
    public static readonly global::System.Windows.DependencyProperty ZoomProperty = global::System.Windows.DependencyProperty.Register("Zoom", typeof(double), typeof(global::Quire.ImportExcel), new global::System.Windows.FrameworkPropertyMetadata(1.0));
    public static readonly global::System.Windows.DependencyProperty SelectionValidProperty = global::System.Windows.DependencyProperty.Register("SelectionValid", typeof(bool), typeof(global::Quire.ImportExcel));
    public static readonly global::System.Windows.DependencyProperty FileLoadedProperty = global::System.Windows.DependencyProperty.Register("FileLoaded", typeof(bool), typeof(global::Quire.ImportExcel), new global::System.Windows.FrameworkPropertyMetadata(false));
    public static readonly global::System.Windows.DependencyProperty WorksheetHasDataProperty = global::System.Windows.DependencyProperty.Register("WorksheetHasData", typeof(bool), typeof(global::Quire.ImportExcel), new global::System.Windows.FrameworkPropertyMetadata(false));
    private Thread _loadExcelThread;

    public string SelectedRange
    {
        get => (string)GetValue(SelectedRangeProperty);
        set => SetValue(SelectedRangeProperty, value);
    }

    public string FileName
    {
        get => (string)GetValue(FileNameProperty);
        set => SetValue(FileNameProperty, value);
    }

    public string SelectedColumns
    {
        get => (string)GetValue(SelectedColumnsProperty);
        set => SetValue(SelectedColumnsProperty, value);
    }

    public string SelectedRows
    {
        get => (string)GetValue(SelectedRowsProperty);
        set => SetValue(SelectedRowsProperty, value);
    }

    public double Zoom
    {
        get => (double)GetValue(ZoomProperty);
        set => SetValue(ZoomProperty, value);
    }

    public bool SelectionValid
    {
        get => (bool)GetValue(SelectionValidProperty);
        set => SetValue(SelectionValidProperty, value);
    }

    public bool FileLoaded
    {
        get => (bool)GetValue(FileLoadedProperty);
        set => SetValue(FileLoadedProperty, value);
    }

    public bool WorksheetHasData
    {
        get => (bool)GetValue(WorksheetHasDataProperty);
        set => SetValue(WorksheetHasDataProperty, value);
    }

    public ImportExcel()
    {
        InitializeComponent();
    }

    private void OnBrowseClick(object sender, RoutedEventArgs e)
    {
        OpenFileDialog ofd = new OpenFileDialog
        {
            Title = Resources["ExcelOpenFileDialogTitle"] as string,
            Filter = Resources["ExcelOpenFileDialogFilter"] as string
        };
        if (!string.IsNullOrEmpty(txtFileName.Text))
        {
            ofd.FileName = Path.GetFileName(txtFileName.Text);
            ofd.InitialDirectory = Path.GetDirectoryName(txtFileName.Text);
        }
        SpiritMainWindow.Addons.CallAddonsOnOpenDialogOpening(ref ofd);
        bool? dialogResult = ofd.ShowDialog();
        SpiritMainWindow.Addons.CallAddonsOnOpenDialogClosing();
        if (dialogResult == true)
        {
            string resolvedFileName = SpiritMainWindow.Addons.CallAddonsToGetResolvedFileNameForOpen(ofd.FileName);
            Mouse.OverrideCursor = Cursors.AppStarting;
            _mainGrid.IsEnabled = false;
            _loadExcelThread = new Thread(LoadExcel)
            {
                IsBackground = true
            };
            _loadExcelThread.Start(resolvedFileName);
        }
    }

    private void BeginInvoke(Action action)
    {
        Dispatcher.BeginInvoke(DispatcherPriority.Loaded, action);
    }

    private void BeginInvoke<T1, T2>(Action<T1, T2> action, T1 param1, T2 param2)
    {
        Dispatcher.BeginInvoke(DispatcherPriority.Loaded, action, param1, param2);
    }

    private void LoadExcel(object parameter)
    {
        try
        {
            string fileName = parameter as string;
            if (AssertionE.Failed == DebugTools.DevAssert(fileName != null, null))
            {
                return;
            }
            ExcelReader excelReader = new ExcelReader();
            excelReader.LoadExcelBook(fileName);
            BeginInvoke(() =>
            {
                Mouse.OverrideCursor = Cursors.Wait;
                _cancelButton.IsEnabled = false;
            });
            BeginInvoke(OnExcelLoaded, fileName, excelReader);
            BeginInvoke(() =>
            {
                Mouse.OverrideCursor = null;
                _cancelButton.IsEnabled = true;
            });
        }
        catch (ThreadAbortException)
        {
            Thread.ResetAbort();
            BeginInvoke(() =>
            {
                Mouse.OverrideCursor = null;
            });
        }
        catch (Exception ex)
        {
            BeginInvoke(() =>
            {
                MessageBox.Show(ex.Message, Title, MessageBoxButton.OK, MessageBoxImage.Hand);
            });
        }
    }

    private void OnExcelLoaded(string resolvedFileName, ExcelReader er)
    {
        FileName = resolvedFileName;
        preview.ItemsSource = er.WorkBook.Sheets;
        preview.SelectedIndex = 0;
        FileLoaded = true;
        _mainGrid.IsEnabled = true;
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        base.OnClosing(e);
        if (_loadExcelThread == null || _loadExcelThread.ThreadState != ThreadState.Background)
        {
            return;
        }
        _loadExcelThread.Abort();
        _loadExcelThread.Join();
        _loadExcelThread = null;
    }

    private void OnInsertClick(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
    }

    internal ImportExcel importExcel;
    internal Grid _mainGrid;
    internal TextBlock txtFileName;
    internal TextBox txtColumns;
    internal TextBox txtRows;
    internal TextBlock txtRange;
    internal global::Quire.Zoom zoom;
    internal TabControl preview;
    internal Button _cancelButton;
    private bool _contentLoaded;

    [global::System.Diagnostics.DebuggerNonUserCode]
    public void InitializeComponent()
    {
        if (_contentLoaded)
        {
            return;
        }
        _contentLoaded = true;
        global::System.Uri resourceLocator = new global::System.Uri("/Quire;component/importexcel.xaml", global::System.UriKind.Relative);
        global::System.Windows.Application.LoadComponent(this, resourceLocator);
    }

    [global::System.Diagnostics.DebuggerNonUserCode]
    internal global::System.Delegate _CreateDelegate(global::System.Type delegateType, string handler)
    {
        return global::System.Delegate.CreateDelegate(delegateType, this, handler);
    }

    [global::System.Diagnostics.DebuggerNonUserCode]
    [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    void global::System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target)
    {
        switch (connectionId)
        {
            case 1:
                importExcel = (global::Quire.ImportExcel)target;
                return;
            case 2:
                _mainGrid = (global::System.Windows.Controls.Grid)target;
                return;
            case 3:
                txtFileName = (global::System.Windows.Controls.TextBlock)target;
                return;
            case 4:
                ((global::System.Windows.Controls.Button)target).Click += OnBrowseClick;
                return;
            case 5:
                txtColumns = (global::System.Windows.Controls.TextBox)target;
                return;
            case 6:
                txtRows = (global::System.Windows.Controls.TextBox)target;
                return;
            case 7:
                txtRange = (global::System.Windows.Controls.TextBlock)target;
                return;
            case 8:
                zoom = (global::Quire.Zoom)target;
                return;
            case 9:
                preview = (global::System.Windows.Controls.TabControl)target;
                return;
            case 10:
                ((global::System.Windows.Controls.Button)target).Click += OnInsertClick;
                return;
            case 11:
                _cancelButton = (global::System.Windows.Controls.Button)target;
                return;
        }
        _contentLoaded = true;
    }
}
