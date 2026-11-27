#nullable disable

using Quire.MessageBoxes;
using Quire.Controls.Core;
using Quire.Wpf;

namespace Quire;

public partial class SpiritMainWindow
{
    private static void OnCWDChanged(string directory)
    {
        if (Addons?.CallAddonsToCheckIfCwdShouldBeUpdated(directory) ?? true)
        {
            global::Quire.Properties.Settings.Default.CWD = directory;
            global::Quire.Properties.Settings.Default.Save();
        }
    }

    private string AskForSaveFileName(
        global::Quire.Controls.Core.IEngineeringDocument doc,
        global::System.Collections.Generic.IList<global::Quire.Controls.Core.FileDialogFilterPair> filterList,
        string defaultFileExtension)
    {
        if (MessageBoxManager.IsDisabled)
        {
            return null;
        }

        global::System.Windows.Forms.SaveFileDialog sfd = new global::System.Windows.Forms.SaveFileDialog();
        if (defaultFileExtension.Equals(".mctx", global::System.StringComparison.OrdinalIgnoreCase))
        {
            string myTemplatesLocation = GetMyTemplatesLocation();
            if (!global::System.IO.Directory.Exists(myTemplatesLocation))
            {
                if (!IsDirectoryOneOfDefaultTemplateLocations(myTemplatesLocation))
                {
                    SetAndDisplayWarning(TryFindResource("Application.DirectoryDoesNotExist") as string, myTemplatesLocation);
                    return null;
                }

                if (global::Quire.Services.AppServiceRegistry.GetService<global::Quire.IO.IFileOperationsProvider>().CreateDirectory(myTemplatesLocation) != null)
                {
                    SetAndDisplayWarning(TryFindResource("Application.UnableCreateDirectory") as string, myTemplatesLocation);
                    return null;
                }
            }

            sfd.InitialDirectory = myTemplatesLocation;
        }
        else if (global::System.IO.Directory.Exists(doc.CWD))
        {
            sfd.InitialDirectory = doc.CWD;
        }
        else if (global::System.IO.Directory.Exists(global::Quire.Properties.Settings.Default.CWD))
        {
            sfd.InitialDirectory = global::Quire.Properties.Settings.Default.CWD;
        }

        Addons.CallAddonsOnSaveDialogOpening(ref sfd);
        if (filterList == null)
        {
            return null;
        }

        sfd.Filter = filterList.ToFilterString();
        sfd.FilterIndex = GetFilterIdxForExtensionOrDefault(filterList, defaultFileExtension);
        sfd.FileName = FilePathToNameConverter.Convert(doc.DisplayFilePath, doc.DocumentName);
        try
        {
            if (DialogManager.ShowDialog(sfd) == global::System.Windows.Forms.DialogResult.OK)
            {
                return ReviseFileExtension(sfd.FileName, filterList[sfd.FilterIndex - 1].Extension);
            }
        }
        catch (global::System.IO.PathTooLongException ex)
        {
            global::System.Windows.MessageBox.Show(
                this,
                ex.Message,
                ex.Source,
                global::System.Windows.MessageBoxButton.OK,
                global::System.Windows.MessageBoxImage.Exclamation,
                global::System.Windows.MessageBoxResult.OK,
                global::System.Windows.MessageBoxOptions.None);
        }

        return null;
    }
}
