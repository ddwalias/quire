using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Threading;
using Quire.Wpf;

namespace Quire;

public partial class AddonsCollection
{
    public void CallAddonsAtQuit()
    {
        if (_idleTimer != null)
        {
            _idleTimer.Stop();
        }

        foreach (Addon addon in _addonsCollection)
        {
            addon.UnloadAddon();
        }

        _addonsCollection.Clear();
        _documentsOpenedInSession.Clear();
    }

    public void CallAddonsToEnablePtcPlaces(bool enable)
    {
        if (!_isPtcPlacesForced)
        {
            AddonsProperties.SetPtcPlacesConnectionStatus(enable);
            foreach (Addon addon in _addonsCollection)
            {
                addon.CallAddonToEnablePtcPlaces(enable);
            }
        }
        else
        {
            _isPtcPlacesForced = false;
        }
    }

    public void CallAddonsOnCommand(int cmdID, string addonID)
    {
        foreach (Addon addon in _addonsCollection)
        {
            if (addonID.Equals(addon.AddOnId))
            {
                addon.CallAddonOnCommand(cmdID);
                return;
            }
        }
    }

    public bool CallAddonsUpdateMenuItem(int cmdID, string addonID)
    {
        foreach (Addon addon in _addonsCollection)
        {
            if (addonID.Equals(addon.AddOnId))
            {
                return addon.CallAddonUpdateMenuItem(cmdID);
            }
        }

        return false;
    }

    private bool IsAddonsCollectionNotEmpty()
    {
        return _addonsCollection.Count > 0;
    }

    public void CallAddonsWhenDocumentSaving(string documentPath)
    {
        if (!string.IsNullOrEmpty(documentPath))
        {
            foreach (Addon addon in _addonsCollection)
            {
                addon.CallAddonWhenDocumentSaving(documentPath);
            }
        }
    }

    public void CallAddonsWhenDocumentSaved(string documentPath)
    {
        foreach (Addon addon in _addonsCollection)
        {
            addon.CallAddonWhenDocumentSaved(documentPath);
        }
    }

    public void CallAddonsOnDocumentModified(string documentPath)
    {
    }

    public string CallAddonsToGetResolvedFileNameForSave(string savedFileName)
    {
        return savedFileName;
    }

    public bool CallAddonsSaveToPtcServer(string currentDocumentPath, string newDocumentPath)
    {
        bool saved = false;
        foreach (Addon addon in _addonsCollection)
        {
            saved = addon.CallAddonSaveToPtcServer(currentDocumentPath, newDocumentPath);
            if (saved)
            {
                break;
            }
        }

        return saved;
    }

    public bool CallAddonsToCheckIfCwdShouldBeUpdated(string directoryPath)
    {
        bool shouldBeUpdated = true;
        if (IsAddonsCollectionNotEmpty())
        {
            try
            {
                string defaultDirectory = CallAddonsToGetDefaultDirectory(0);
                if (!string.IsNullOrEmpty(defaultDirectory))
                {
                    shouldBeUpdated = !Path.GetFullPath(defaultDirectory).TrimEnd('\\').Equals(Path.GetFullPath(directoryPath).TrimEnd('\\'), StringComparison.InvariantCultureIgnoreCase);
                }
            }
            catch
            {
            }
        }

        return shouldBeUpdated;
    }

    private string GetActiveAddOnId()
    {
        string addOnId = string.Empty;
        foreach (Addon addon in _addonsCollection)
        {
            if (addon.CallAddonToCheckIsAddOnActive())
            {
                addOnId = addon.AddOnId;
                break;
            }
        }

        return addOnId;
    }

    public void UpdateRibbonMenuItems(object sender, RoutedEventArgs e)
    {
        if (IsAddonsCollectionNotEmpty())
        {
            if (GetActiveAddOnId() == AddonsProperties.IdWwgmAddon)
            {
                SetAddonsMenuItemsStatus(true);
                ForcePtcPlacesEnableIfNeeds();
                return;
            }

            DisableInRibbonAllAddonsMenuItems();
        }
    }

    private void DisableInRibbonAllAddonsMenuItems()
    {
        SetAddonsMenuItemsStatus(false);
    }

    private void SetAddonsMenuItemsStatus(bool wcAddonEnable)
    {
        _spiritMainWindow.IntegrationCommands.WcEnable = wcAddonEnable;
    }

    private void ForcePtcPlacesEnableIfNeeds()
    {
        if (!_spiritMainWindow.IntegrationCommands.PTCPlacesEnable)
        {
            _isPtcPlacesForced = true;
            _spiritMainWindow.IntegrationCommands.PTCPlacesEnable = true;
        }
    }

    public string CallAddonsToGetResolvedFileNameForOpen(string openedFileName)
    {
        if (!IsAddonsCollectionNotEmpty())
        {
            return openedFileName;
        }

        string resolvedFileName = string.Empty;
        if (!string.IsNullOrEmpty(openedFileName))
        {
            bool isResolved = false;
            string addonFileName = null;
            foreach (Addon addon in _addonsCollection)
            {
                addonFileName = addon.CallAddonToGetResolvedFileNameForOpenAlloc(openedFileName);
                isResolved = !string.IsNullOrEmpty(addonFileName);
                break;
            }

            resolvedFileName = addonFileName ?? (File.Exists(openedFileName) ? openedFileName : string.Empty);
            if (!_documentsOpenedInSession.Keys.Contains(resolvedFileName))
            {
                _documentsOpenedInSession.Add(resolvedFileName, isResolved);
            }
        }

        return resolvedFileName;
    }

    public void CallAddonsOnOpenDialogOpening(ref global::Microsoft.Win32.OpenFileDialog ofd)
    {
        if (!IsAddonsCollectionNotEmpty())
        {
            return;
        }

        bool checkFileExist = false;
        ofd.InitialDirectory = GetInitialDirectoryOnOpen(ref checkFileExist);
        ofd.CheckFileExists = checkFileExist;
    }

    public void CallAddonsOnOpenDialogOpening(ref System.Windows.Forms.OpenFileDialog ofd)
    {
        if (!IsAddonsCollectionNotEmpty())
        {
            return;
        }

        bool checkFileExist = false;
        ofd.InitialDirectory = GetInitialDirectoryOnOpen(ref checkFileExist);
        ofd.CheckFileExists = checkFileExist;
    }

    private string GetInitialDirectoryOnOpen(ref bool checkFileExist)
    {
        string initialDirectory = CallAddonsToGetDefaultDirectory((int)AddonsProperties.FileDlgType.Open);
        if (string.IsNullOrEmpty(initialDirectory))
        {
            initialDirectory = Properties.Settings.Default.CWD;
            checkFileExist = true;
        }

        return initialDirectory;
    }

    public void CallAddonsOnOpenDialogClosing()
    {
    }

    public void CallAddonsOnSaveDialogOpening(ref System.Windows.Forms.SaveFileDialog sfd)
    {
        if (!IsAddonsCollectionNotEmpty())
        {
            return;
        }

        bool checkPathExists = false;
        sfd.InitialDirectory = GetInitialDirectoryOnSave(ref checkPathExists);
        sfd.CheckPathExists = checkPathExists;
    }

    private string GetInitialDirectoryOnSave(ref bool checkPathExists)
    {
        string initialDirectory = CallAddonsToGetDefaultDirectory((int)AddonsProperties.FileDlgType.Save);
        if (string.IsNullOrEmpty(initialDirectory))
        {
            initialDirectory = Properties.Settings.Default.CWD;
            checkPathExists = true;
        }

        return initialDirectory;
    }

    private string CallAddonsToGetDefaultDirectory(int dlgType)
    {
        string defaultDirectory = string.Empty;
        foreach (Addon addon in _addonsCollection)
        {
            defaultDirectory = addon.CallAddonToGetDefaultDirectory(dlgType);
            return defaultDirectory ?? string.Empty;
        }

        return defaultDirectory;
    }

    internal string CallAddonsToOpenFromWindchill()
    {
        string windchillLocation = string.Empty;
        foreach (Addon addon in _addonsCollection)
        {
            windchillLocation = addon.CallAddonToOpenFromWindchill();
            return windchillLocation ?? string.Empty;
        }

        return windchillLocation;
    }

    public AddonsCollection(SpiritMainWindow mainWindow)
    {
        _spiritMainWindow = mainWindow;
        _addonsCollection = new Collection<Addon>();
        _documentsOpenedInSession = new Dictionary<string, bool>();
        _isPtcPlacesForced = false;
        if (LoadAddonsAndAddToCollection())
        {
            new AddonsControls(_spiritMainWindow.CommandBindings);
            StartOnIdleTimeProcessing();
            return;
        }

        DisableInRibbonAllAddonsMenuItems();
    }

    private bool LoadAddonsAndAddToCollection()
    {
        AddonsProperties.AddonsData[] addonsData = AddonsProperties.GetAddonsProperties(_spiritMainWindow.IsComInstance);
        if (addonsData.Length != 0)
        {
            _mathcadPrimeApplicationPtr = new CMathcadPrimeApplication(false);
            for (int i = 0; i < addonsData.Length; i++)
            {
                Addon addon = new Addon(addonsData[i].ID);
                if (addon.LoadAddon(addonsData[i].Path, _mathcadPrimeApplicationPtr))
                {
                    _addonsCollection.Add(addon);
                }
            }
        }

        return IsAddonsCollectionNotEmpty();
    }

    private void StartOnIdleTimeProcessing()
    {
        _idleTimer = new DispatcherTimer(new TimeSpan(0, 0, 0, 0, 100), DispatcherPriority.ApplicationIdle, new EventHandler(OnIdleTimeProcessing), Dispatcher.CurrentDispatcher);
        _idleTimer.Start();
    }

    private void OnIdleTimeProcessing(object sender, EventArgs e)
    {
        foreach (Addon addon in _addonsCollection)
        {
            addon.CallAddonOnIdle();
        }
    }

    public string GetDisplayNameForDocName(string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
        {
            return null;
        }

        return FilePathToNameConverter.Convert(CallAddonsToGetDisplayName(filePath), null);
    }


    private string CallAddonsToGetDisplayName(string fileName)
    {
        return string.Empty;
    }
}
