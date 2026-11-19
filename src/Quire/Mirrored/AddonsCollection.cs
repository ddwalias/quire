using System.IO;

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
}
