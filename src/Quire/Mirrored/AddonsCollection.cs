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
}
