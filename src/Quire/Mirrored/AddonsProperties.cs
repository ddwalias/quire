using System.Collections.Generic;
using Microsoft.Win32;

namespace Quire;

internal static partial class AddonsProperties
{
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
}
