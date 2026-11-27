#nullable disable

using System;
using System.Reflection;
using Microsoft.Win32;
using Quire.IO;
using Quire.Controls.Core;

namespace Quire;

public partial class SpiritMainWindow
{
    [global::System.Runtime.InteropServices.DllImport("user32.dll")]
    private static extern bool SetForegroundWindow(nint hWnd);

    private void DisplayCriticalWarningWithLogFile(string displayedMessage, string logFile)
    {
        DisplayCriticalWarning(string.Format(
            "{0}{1}{2}{1}{3}",
            displayedMessage,
            Environment.NewLine,
            (string)FindResource("Application.LogFileIsLocatedAt"),
            logFile));
    }

    private static string GetEngNbInstallDirectoryLocalPath()
    {
        try
        {
            string version = Assembly.GetExecutingAssembly().GetName().Version?.ToString();
            if (string.IsNullOrEmpty(version))
            {
                return string.Empty;
            }

            string keyName = "SOFTWARE\\PTC\\EngineeringNotebookHelp\\" + version;
            using RegistryKey key = Registry.LocalMachine.OpenSubKey(keyName);
            string installLocation = key?.GetValue("InstallLocation") as string;
            return string.IsNullOrEmpty(installLocation) ? string.Empty : installLocation;
        }
        catch (Exception)
        {
            return string.Empty;
        }
    }

    private void WorksheetPropertyChanged(object sender, global::System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == "IsHeaderActive" || e.PropertyName == "IsFooterActive")
        {
            FindProperties.Status = global::Quire.Controls.Core.FindReplaceStatus.Empty;
        }
    }

    protected override void OnDrop(global::System.Windows.DragEventArgs e)
    {
        if (e.Data.GetDataPresent(global::System.Windows.DataFormats.FileDrop))
        {
            if (e.Data.GetData(global::System.Windows.DataFormats.FileDrop) is string[] files && files.Length == 1)
            {
                AppCommands.OpenEngineeringDocument.Execute(files[0], ActiveDocument as global::System.Windows.IInputElement);
            }
        }
        else
        {
            base.OnDrop(e);
        }
    }

    private static void InsertReadExcel(
        global::Quire.Controls.Core.IEngineeringDocument document,
        string fileName,
        string range,
        global::System.Windows.IInputElement originalSource)
    {
        string path = FileUtils.EvaluateRelativePath(
            document.DocCalculationVariables.CurrentWorkingDirectory,
            fileName,
            stripSingleLeadingDot: false);
        string[] args = new string[2] { path, range };
        global::Quire.Controls.EquationCommands.InsertFunction.Execute(
            new global::Quire.Controls.InsertFunctionCommandParameter("READEXCEL", args),
            originalSource);
    }

    private bool HandleFindNext()
    {
        return Find(global::System.Windows.Documents.LogicalDirection.Forward);
    }

    private void HandleFindNext(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e)
    {
        HandleFindNext();
    }

    private void HandleFindPrev(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e)
    {
        Find(global::System.Windows.Documents.LogicalDirection.Backward);
    }

    private void IsVisibleChangedEventHandler(
        object sender,
        global::System.Windows.DependencyPropertyChangedEventArgs e)
    {
        if ((bool)e.NewValue)
        {
            foreach (global::System.Collections.Generic.KeyValuePair<
                         global::System.Windows.Window,
                         global::System.Windows.Visibility> item in _visibilityDictionary)
            {
                item.Key.Visibility = item.Value;
            }
        }
        else
        {
            _visibilityDictionary.Clear();
            foreach (global::System.Windows.Window ownedWindow in OwnedWindows)
            {
                _visibilityDictionary[ownedWindow] = ownedWindow.Visibility;
                ownedWindow.Visibility = global::System.Windows.Visibility.Hidden;
            }
        }
    }

    internal static void SetupMainWindowDimensions(global::System.Windows.Window window)
    {
        global::Quire.Properties.Settings settings = global::Quire.Properties.Settings.Default;
        if (settings.WinHeight.IsValid())
        {
            window.Height = settings.WinHeight;
        }

        if (settings.WinWidth.IsValid())
        {
            window.Width = settings.WinWidth;
        }

        if (settings.WinLeft.IsValid() && settings.WinLeft + window.Width > 10.0)
        {
            window.Left = settings.WinLeft;
            if (window.Left > global::System.Windows.SystemParameters.VirtualScreenWidth)
            {
                window.WindowStartupLocation = global::System.Windows.WindowStartupLocation.CenterScreen;
            }
        }

        if (settings.WinTop.IsValid() && settings.WinTop + window.Height > 10.0)
        {
            window.Top = settings.WinTop;
        }

        if (settings.WinMaximized)
        {
            window.WindowState = global::System.Windows.WindowState.Maximized;
        }
    }

    private void LaunchImportExcel(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e)
    {
        global::Quire.ImportExcel importExcel = new global::Quire.ImportExcel
        {
            Owner = this
        };
        bool? result = importExcel.ShowDialog();
        if (result.HasValue && result.Value)
        {
            InsertReadExcel(
                ActiveDocument,
                importExcel.FileName,
                importExcel.SelectedRange,
                e.OriginalSource as global::System.Windows.IInputElement);
        }
    }

    private void HandlePrintDocument(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e)
    {
        ActiveEngineeringDoc.Print(AskCancelPrintInProgressDialog);
    }

    private string GetTemplateFileName(
        global::System.Windows.Input.ExecutedRoutedEventArgs args,
        bool isNewFromTemplate = false)
    {
        string parameter = args.Parameter as string;
        string fileName = string.Empty;
        if (!string.IsNullOrEmpty(parameter))
        {
            fileName = TemplatesFolderLocations.ContainsKey(parameter)
                ? LaunchOpenFileDialogAndGetFileNameForTemplates(
                    TemplatesFolderLocations[parameter],
                    isNewFromTemplate)
                : parameter;
        }

        return fileName;
    }

    private string LaunchOpenFileDialogAndGetFileName(string commandName)
    {
        if (commandName.Equals("OpenFromWC"))
        {
            return Addons.CallAddonsToOpenFromWindchill();
        }

        string fileName = string.Empty;
        string filter = GetFileSaveOpenFilters(null, save: false).ToFilterString();
        global::System.Windows.Forms.OpenFileDialog dialog = new global::System.Windows.Forms.OpenFileDialog();
        Addons.CallAddonsOnOpenDialogOpening(ref dialog);
        dialog.Filter = filter;
        if (DialogManager.ShowDialog(dialog) == global::System.Windows.Forms.DialogResult.OK)
        {
            fileName = dialog.FileName;
        }

        return fileName;
    }

    private void HandleOpenDocument(
        object sender,
        global::System.Windows.Input.ExecutedRoutedEventArgs args)
    {
        string fileName;
        string tag = string.Empty;
        global::Quire.Controls.Core.DocumentReadonlyOptions readonlyOptions = null;
        if (args.Parameter is global::Quire.OpenEngineeringDocumentParameter documentParameter)
        {
            fileName = documentParameter.FilePath;
            readonlyOptions = documentParameter.ReadonlyOptions;
        }
        else
        {
            fileName = args.Parameter as string;
        }

        if (UrlUtils.HasTag(fileName, out string pathWithoutTag, out string tagName))
        {
            tag = tagName;
            fileName = pathWithoutTag;
        }

        if (!UseDocumentTabs && !AskToSaveDocument(ActiveDocument))
        {
            return;
        }

        if (string.IsNullOrEmpty(fileName))
        {
            fileName = LaunchOpenFileDialogAndGetFileName(((global::System.Windows.Input.RoutedCommand)args.Command).Name);
            Addons.CallAddonsOnOpenDialogClosing();
        }

        fileName = Addons.CallAddonsToGetResolvedFileNameForOpen(fileName);
        if (!string.IsNullOrEmpty(fileName))
        {
            if (!global::System.IO.File.Exists(fileName))
            {
                OpenFileExceptionHandler(fileName, new global::System.IO.FileNotFoundException(string.Empty, fileName));
            }
            else
            {
                OpenFile(fileName, readonlyOptions, tag);
            }
        }
    }

    private void ScrollToTagInDoc(global::Quire.Controls.Core.IEngineeringDocument document, string tag)
    {
        if (tag.IsNotEmpty())
        {
            (document.Worksheet as global::Quire.Controls.Worksheet.WorksheetControl)?.BringFirstTagItemIntoView(tag);
        }
    }
}
