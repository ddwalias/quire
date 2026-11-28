using System;
using System.Collections.Generic;
using System.Windows;
using Microsoft.Win32;

namespace Quire;

internal static partial class AddonsProperties
{
    public const string CmdNameOpenFromWc = "OpenFromWC";

    public static readonly string cmdTextOpenFromWC = ReadOpenFromWcCmdText();

    public const string IdWwgmAddon = "WWGM";

    private static readonly string[][] _addonRegKeyProperties = new string[1][] { new string[2] { "WWGM", "HKEY_CURRENT_USER" } };

    private const string HKeyCurrentUser = "HKEY_CURRENT_USER";
    private const string HKeyLocalMachine = "HKEY_LOCAL_MACHINE";
    private const string LoadAddOnsOnDemandRegKeyValue = "LoadAddonsOnDemand";
    private const string EndOfRegKey = "_Addon_Path";
    private const string AddonRegKey = "Software\\PTC\\Mathcad Prime\\Mathcad Prime 1.0\\Addons";
    private const string PtcPlacesStatusRegKeyValue = "PTC Places";
    private const int PtcPlacesEnable = 1;
    private const int PtcPlacesDisable = 0;

    private static string ReadOpenFromWcCmdText()
    {
        ResourceDictionary resourceDictionary = null;
        string text = string.Empty;
        try
        {
            resourceDictionary = new ResourceDictionary
            {
                Source = new Uri("pack://application:,,,/Quire;component/ApplicationResources.xaml")
            };
        }
        catch (UriFormatException)
        {
        }

        if (resourceDictionary != null)
        {
            text = resourceDictionary["Application.OpenFromWC"] as string;
        }

        return text;
    }

    public partial struct AddonsData
    {
        private readonly string _path;
        private readonly string _id;

        public AddonsData(string path, string id)
        {
            _path = path;
            _id = id;
        }

        public string Path => _path;

        public string ID => _id;
    }

    private static string GetAddonPath(string[] value)
    {
        string path = string.Empty;
        RegistryKey addonKey = OpenRegKeyByBaseKey(value[1]);
        if (addonKey != null)
        {
            path = addonKey.GetValue(value[0] + EndOfRegKey, string.Empty).ToString();
            addonKey.Close();
        }

        return path;
    }

    private static bool CheckIfAddonsShouldBeLoaded(bool isComInstance)
    {
        bool shouldBeLoaded = true;
        RegistryKey addonsKey = OpenAddonsRegKey(false);
        try
        {
            if (addonsKey != null && addonsKey.GetValue(LoadAddOnsOnDemandRegKeyValue) != null)
            {
                int loadOnDemand = (int)addonsKey.GetValue(LoadAddOnsOnDemandRegKeyValue);
                if (loadOnDemand == 2 || loadOnDemand == 1 & isComInstance)
                {
                    shouldBeLoaded = false;
                }
            }
        }
        catch
        {
        }

        return shouldBeLoaded;
    }

    public static AddonsData[] GetAddonsProperties(bool isComInstance)
    {
        List<AddonsData> addons = new List<AddonsData>();
        if (CheckIfAddonsShouldBeLoaded(isComInstance))
        {
            for (int i = 0; i < _addonRegKeyProperties.Length; i++)
            {
                string path = GetAddonPath(_addonRegKeyProperties[i]);
                if (!string.IsNullOrEmpty(path))
                {
                    addons.Add(new AddonsData(path, _addonRegKeyProperties[i][0]));
                }
            }
        }

        return addons.ToArray();
    }

    public static bool GetPtcPlacesConnectionStatus()
    {
        bool isConnected = false;
        RegistryKey addonsKey = OpenAddonsRegKey();
        try
        {
            if (addonsKey != null)
            {
                if (addonsKey.GetValue(PtcPlacesStatusRegKeyValue) == null)
                {
                    addonsKey.SetValue(PtcPlacesStatusRegKeyValue, 0);
                }
                else
                {
                    isConnected = (int)addonsKey.GetValue(PtcPlacesStatusRegKeyValue) == 1;
                }

                addonsKey.Close();
            }
        }
        catch
        {
        }

        return isConnected;
    }

    public static void SetPtcPlacesConnectionStatus(bool enable)
    {
        int status = 0;
        RegistryKey addonsKey = OpenAddonsRegKey();
        try
        {
            if (addonsKey != null)
            {
                if (enable)
                {
                    status = 1;
                }

                addonsKey.SetValue(PtcPlacesStatusRegKeyValue, status);
                addonsKey.Close();
            }
        }
        catch
        {
        }
    }

    private static RegistryKey OpenAddonsRegKey(bool writable = true)
    {
        RegistryKey addonsKey = null;
        try
        {
            addonsKey = Registry.CurrentUser.OpenSubKey(AddonRegKey, writable);
        }
        catch
        {
        }

        return addonsKey ?? CreateAddonsRegKey();
    }

    private static RegistryKey CreateAddonsRegKey()
    {
        RegistryKey addonsKey = null;
        try
        {
            addonsKey = Registry.CurrentUser.CreateSubKey(AddonRegKey, RegistryKeyPermissionCheck.ReadWriteSubTree);
        }
        catch
        {
        }

        return addonsKey;
    }

    private static RegistryKey OpenRegKeyByBaseKey(string baseKey)
    {
        RegistryKey addonsKey = null;
        try
        {
            switch (baseKey)
            {
                case HKeyCurrentUser:
                    addonsKey = Registry.CurrentUser.OpenSubKey(AddonRegKey, writable: false);
                    break;
                case HKeyLocalMachine:
                    addonsKey = Registry.LocalMachine.OpenSubKey(AddonRegKey, writable: false);
                    break;
            }
        }
        catch
        {
        }

        return addonsKey;
    }
}
