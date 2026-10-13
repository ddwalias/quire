using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using Quire.Controls.Worksheet;
using Quire.Core;
using Quire.EquationEditor.Tree.Transform;
using Quire.PersistentDataObjects;
using Quire.Controls.Core;

namespace Quire;

public static class Commands
{
    public static readonly RoutedUICommand CalculateNow = new("Calculate now", nameof(CalculateNow), typeof(Commands));
    public static readonly RoutedUICommand RecalculateAll = new("Recalculate worksheet", nameof(RecalculateAll), typeof(Commands));
    public static readonly RoutedUICommand InsertMath = new("Math region", nameof(InsertMath), typeof(Commands));
    public static readonly RoutedUICommand InsertText = new("Text region", nameof(InsertText), typeof(Commands));
    public static readonly RoutedUICommand InsertPlot = new("XY plot", nameof(InsertPlot), typeof(Commands));
}

public partial class MainWindow : Window
{
    private WorksheetControl _worksheet = null!;
    private string? _path;
    private bool _dirty, _syncingUi;
    private ToggleButton? _openPalette;

    public MainWindow()
    {
        InitializeComponent();
        CommandBindings.Add(new CommandBinding(ApplicationCommands.New, (_, _) => New()));
        CommandBindings.Add(new CommandBinding(ApplicationCommands.Open, (_, _) => Open()));
        CommandBindings.Add(new CommandBinding(ApplicationCommands.Save, (_, _) => Save()));
        CommandBindings.Add(new CommandBinding(ApplicationCommands.SaveAs, (_, _) => SaveAs()));
        CommandBindings.Add(new CommandBinding(ApplicationCommands.Help, (_, _) => new QuickReferenceWindow { Owner = this }.Show()));
        CommandBindings.Add(new CommandBinding(Commands.CalculateNow, (_, _) => _worksheet.CalculateNow()));
        CommandBindings.Add(new CommandBinding(Commands.RecalculateAll, (_, _) => _worksheet.RecalculateAll()));
        CommandBindings.Add(new CommandBinding(Commands.InsertMath, (_, _) => _worksheet.InsertMath()));
        CommandBindings.Add(new CommandBinding(Commands.InsertText, (_, _) => _worksheet.InsertText()));
        CommandBindings.Add(new CommandBinding(Commands.InsertPlot, (_, _) => _worksheet.InsertXYPlot()));

        foreach (var (toggle, build) in new (ToggleButton, Action<Panel, Action<string>>)[]
                 {
                     (OperatorsToggle, Palettes.BuildOperators), (GreekToggle, Palettes.BuildGreek),
                     (UnitsToggle, Palettes.BuildUnits), (FunctionsToggle, Palettes.BuildFunctions),
                 })
        {
            toggle.Checked += (_, _) => ShowPalette(toggle, build);
            toggle.Unchecked += (_, _) => { if (_openPalette == toggle) PalettePopup.IsOpen = false; };
        }

        PreviewKeyDown += HandleWindowKeyDown;
        ReplaceWorksheet(new WorksheetDocument());
    }

    // ---------------------------------------------------------------- worksheet lifecycle

    private void ReplaceWorksheet(WorksheetDocument doc)
    {
        (_worksheet as IDisposable)?.Dispose();
        _worksheet = new WorksheetControl();
        _worksheet.DocumentChanged += (_, _) => { _dirty = true; UpdateTitle(); };
        _worksheet.StatusChanged += (_, _) => UpdateStatus();
        WorksheetHost.Content = _worksheet;
        _worksheet.Load(doc);
        _worksheet.Zoom = ZoomSlider.Value;
        SyncSettingsUi();
        UpdateStatus();
    }

    public void LoadSample()
    {
        ReplaceWorksheet(SampleWorksheet.Create());
        _path = null;
        _dirty = false;
        UpdateTitle("Getting started");
    }

    public void OpenFile(string path)
    {
        try
        {
            var ext = Path.GetExtension(path).ToLowerInvariant();
            if (ext is ".mcdx" or ".mctx")
            {
                var result = WorksheetConverter.ImportMcdx(path);
                ReplaceWorksheet(result.Document);
                _path = null;
                _dirty = true;
                UpdateTitle(Path.GetFileNameWithoutExtension(path) + " (imported)");
                if (result.Warnings.Count > 0)
                    MessageBox.Show(this, "Some content could not be imported:\n\n• " + string.Join("\n• ", result.Warnings.Take(15)),
                        "Import Mathcad Prime worksheet", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            ReplaceWorksheet(WorksheetPackage.Load(path));
            _path = path;
            _dirty = false;
            UpdateTitle();
        }
        catch (Exception ex) when (ex is not OutOfMemoryException)
        {
            MessageBox.Show(this, $"Could not open {Path.GetFileName(path)}:\n{ex.Message}", "Quire", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private bool ConfirmDiscard()
    {
        _worksheet.EditingRegion?.EndEdit(commit: true);
        if (!_dirty) return true;
        var answer = MessageBox.Show(this, "Save changes to this worksheet?", "Quire", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
        return answer switch
        {
            MessageBoxResult.Yes => Save(),
            MessageBoxResult.No => true,
            _ => false,
        };
    }

    private void New()
    {
        if (!ConfirmDiscard()) return;
        ReplaceWorksheet(new WorksheetDocument());
        _path = null;
        _dirty = false;
        UpdateTitle();
    }

    private void Open()
    {
        if (!ConfirmDiscard()) return;
        var dlg = new OpenFileDialog
        {
            Filter = "All worksheets (*.qwx;*.mcdx;*.mctx)|*.qwx;*.mcdx;*.mctx|Quire worksheets (*.qwx)|*.qwx|Mathcad Prime worksheets (*.mcdx;*.mctx)|*.mcdx;*.mctx",
        };
        if (dlg.ShowDialog(this) == true) OpenFile(dlg.FileName);
    }

    private bool Save()
    {
        if (_path == null) return SaveAs();
        try
        {
            _worksheet.EditingRegion?.EndEdit(commit: true);
            WorksheetPackage.Save(_worksheet.ToDocument(), _path);
            _dirty = false;
            UpdateTitle();
            return true;
        }
        catch (Exception ex) when (ex is not OutOfMemoryException)
        {
            MessageBox.Show(this, $"Could not save:\n{ex.Message}", "Quire", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }
    }

    private bool SaveAs()
    {
        var dlg = new SaveFileDialog { Filter = "Quire worksheet (*.qwx)|*.qwx", DefaultExt = WorksheetPackage.Extension, FileName = Path.GetFileName(_path ?? "Worksheet1.qwx") };
        if (dlg.ShowDialog(this) != true) return false;
        _path = dlg.FileName;
        return Save();
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        if (!ConfirmDiscard()) e.Cancel = true;
        base.OnClosing(e);
    }

    protected override void OnClosed(EventArgs e)
    {
        (_worksheet as IDisposable)?.Dispose();
        base.OnClosed(e);
    }

    private string? _untitledName;

    private void UpdateTitle(string? untitledName = null)
    {
        if (untitledName != null) _untitledName = untitledName;
        var name = _path != null ? Path.GetFileName(_path) : _untitledName ?? "Untitled";
        Title = $"{name}{(_dirty ? " •" : "")} — Quire";
    }

    private void UpdateStatus()
    {
        CalcStatus.Text = _worksheet.IsCalculating ? "Calculating…" : _worksheet.Model.IsAutoCalcOn ? "Ready" : "Manual calculation (F9)";
        RegionStatus.Text = $"{_worksheet.RegionCount} region{(_worksheet.RegionCount == 1 ? "" : "s")}";
        int errors = _worksheet.ErrorCount;
        ErrorStatus.Text = errors == 0 ? "No errors" : $"{errors} error{(errors == 1 ? "" : "s")}";
        ErrorStatus.Foreground = errors == 0 ? Brushes.DimGray : Brushes.Firebrick;
        ZoomText.Text = $"{_worksheet.Zoom * 100:0}%";
    }

    private void SyncSettingsUi()
    {
        _syncingUi = true;
        var m = _worksheet.Model;
        AutoCalcCheck.IsChecked = AutoCalcMenu.IsChecked = m.IsAutoCalcOn;
        UnitSiMenu.IsChecked = m.UnitSystem == "si";
        UnitUsMenu.IsChecked = m.UnitSystem == "uscs";
        UnitCgsMenu.IsChecked = m.UnitSystem == "cgs";
        UnitSystemCombo.SelectedItem = UnitSystemCombo.Items.Cast<ComboBoxItem>().FirstOrDefault(i => (string)i.Tag == m.UnitSystem);
        TrailingZerosMenu.IsChecked = m.ResultFormat.ShowTrailingZeros;
        _syncingUi = false;
    }

    // ---------------------------------------------------------------- keyboard routing

    private void HandleWindowKeyDown(object sender, KeyEventArgs e)
    {
        if (_worksheet.EditingRegion != null || Keyboard.Modifiers != ModifierKeys.Control) return;
        switch (e.Key)
        {
            case Key.Z: _worksheet.Undo(); break;
            case Key.Y: _worksheet.Redo(); break;
            case Key.C: _worksheet.Copy(); break;
            case Key.X: _worksheet.Cut(); break;
            case Key.V: _worksheet.Paste(); break;
            default: return;
        }
        e.Handled = true;
    }

    // ---------------------------------------------------------------- palettes

    private void ShowPalette(ToggleButton toggle, Action<Panel, Action<string>> build)
    {
        if (_openPalette != null && _openPalette != toggle) _openPalette.IsChecked = false;
        _openPalette = toggle;
        PaletteContent.Children.Clear();
        build(PaletteContent, text => _worksheet.InsertIntoEditor(text));
        PalettePopup.PlacementTarget = toggle;
        PalettePopup.IsOpen = true;
    }

    private void PalettePopup_Closed(object? sender, EventArgs e)
    {
        if (_openPalette != null && !_openPalette.IsMouseOver) _openPalette.IsChecked = false;
    }

    // ---------------------------------------------------------------- menu handlers

    private void ImportMcdx_Click(object sender, RoutedEventArgs e)
    {
        if (!ConfirmDiscard()) return;
        var dlg = new OpenFileDialog { Filter = "Mathcad Prime worksheets (*.mcdx;*.mctx)|*.mcdx;*.mctx" };
        if (dlg.ShowDialog(this) == true) OpenFile(dlg.FileName);
    }

    private void Sample_Click(object sender, RoutedEventArgs e)
    {
        if (ConfirmDiscard()) LoadSample();
    }

    private void Exit_Click(object sender, RoutedEventArgs e) => Close();
    private void Undo_Click(object sender, RoutedEventArgs e) => _worksheet.Undo();
    private void Redo_Click(object sender, RoutedEventArgs e) => _worksheet.Redo();
    private void Cut_Click(object sender, RoutedEventArgs e) => _worksheet.Cut();
    private void Copy_Click(object sender, RoutedEventArgs e) => _worksheet.Copy();
    private void Paste_Click(object sender, RoutedEventArgs e) => _worksheet.Paste();
    private void Delete_Click(object sender, RoutedEventArgs e) => _worksheet.DeleteSelection();
    private void SelectAll_Click(object sender, RoutedEventArgs e) => _worksheet.SelectAll();
    private void InsertPolar_Click(object sender, RoutedEventArgs e) => _worksheet.InsertPolarPlot();
    private void InsertSolveBlock_Click(object sender, RoutedEventArgs e) => _worksheet.InsertSolveBlock();

    private void AutoCalcMenu_Click(object sender, RoutedEventArgs e) => SetAutoCalc(AutoCalcMenu.IsChecked);
    private void AutoCalcCheck_Click(object sender, RoutedEventArgs e) => SetAutoCalc(AutoCalcCheck.IsChecked == true);

    private void SetAutoCalc(bool on)
    {
        _worksheet.SetAutoCalc(on);
        SyncSettingsUi();
        UpdateStatus();
    }

    private void UnitSystemMenu_Click(object sender, RoutedEventArgs e) => SetUnitSystem((string)((MenuItem)sender).Tag);

    private void UnitSystemCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (!_syncingUi && UnitSystemCombo.SelectedItem is ComboBoxItem item) SetUnitSystem((string)item.Tag);
    }

    private void SetUnitSystem(string name)
    {
        _worksheet.SetUnitSystem(name);
        SyncSettingsUi();
    }

    private void Decimals_Click(object sender, RoutedEventArgs e) =>
        _worksheet.SetResultFormat(_worksheet.Model.ResultFormat with { Decimals = int.Parse((string)((MenuItem)sender).Tag) });

    private void Threshold_Click(object sender, RoutedEventArgs e) =>
        _worksheet.SetResultFormat(_worksheet.Model.ResultFormat with { ExponentialThreshold = int.Parse((string)((MenuItem)sender).Tag) });

    private void TrailingZeros_Click(object sender, RoutedEventArgs e) =>
        _worksheet.SetResultFormat(_worksheet.Model.ResultFormat with { ShowTrailingZeros = TrailingZerosMenu.IsChecked });

    private void ZoomSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (_worksheet != null) _worksheet.Zoom = e.NewValue;
    }

    private void About_Click(object sender, RoutedEventArgs e) =>
        MessageBox.Show(this,
            "Quire — an engineering calculation worksheet.\n\n" +
            "Live math with units, functions, ranges, matrices, solve blocks and plots, " +
            "laid out freely on the page and calculated top to bottom, left to right.\n\nVersion 0.1",
            "About Quire", MessageBoxButton.OK, MessageBoxImage.Information);

    // ---------------------------------------------------------------- headless rendering (used for visual checks)

    public async Task RenderToFileAndExitAsync(string path)
    {
        await _worksheet.WhenCalculated();
        await Dispatcher.InvokeAsync(() => { }, System.Windows.Threading.DispatcherPriority.ApplicationIdle);
        await Task.Delay(300);
        await Dispatcher.InvokeAsync(() => { }, System.Windows.Threading.DispatcherPriority.ApplicationIdle);
        RenderElement((FrameworkElement)Content, path);
        RenderElement(_worksheet.PagesElement, Path.ChangeExtension(path, null) + "-full.png");
        _dirty = false;
        Close();
    }

    private void RenderElement(FrameworkElement element, string path)
    {
        var dpi = VisualTreeHelper.GetDpi(this);
        var bmp = new RenderTargetBitmap((int)(element.ActualWidth * dpi.DpiScaleX), (int)(element.ActualHeight * dpi.DpiScaleY),
            dpi.PixelsPerInchX, dpi.PixelsPerInchY, PixelFormats.Pbgra32);
        bmp.Render(element);
        var encoder = new PngBitmapEncoder();
        encoder.Frames.Add(BitmapFrame.Create(bmp));
        using var fs = File.Create(path);
        encoder.Save(fs);
    }
}
