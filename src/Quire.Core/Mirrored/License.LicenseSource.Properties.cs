#nullable disable

namespace Quire.License;

public partial class LicenseSource
{
    public LicenseUriType Type { get; private set; }

    private string _filePath;
    private string _host;
    private uint? _port;
    private string _sourceList;

    public string FilePath
    {
        get => _filePath;
        set
        {
            _filePath = value;
            Type = LicenseUriType.FilePath;
        }
    }

    public string Host
    {
        get => _host;
        set
        {
            _host = value;
            Type = LicenseUriType.ServerAndPort;
        }
    }

    public uint? Port
    {
        get => _port;
        set
        {
            _port = value;
            Type = LicenseUriType.ServerAndPort;
        }
    }

    public string SourceList
    {
        get => _sourceList;
        set
        {
            _sourceList = value;
            Type = LicenseUriType.SourceList;
        }
    }

    public void Clear()
    {
        Host = null;
        FilePath = null;
        SourceList = null;
        Port = null;
        Type = LicenseUriType.None;
    }

    public static void Save(LicenseType licenseType, LicenseUriType licensePathForm, string licensePath)
    {
        Properties.Settings.Default.LicenseType = licenseType;
        Properties.Settings.Default.LicensePathForm = licensePathForm;
        Properties.Settings.Default.LicensePath = licensePath;
        Properties.Settings.Default.Save();
    }

    public void Save(LicenseType licenseType)
    {
        if (Properties.Settings.Default.LicenseType == licenseType
            && Properties.Settings.Default.LicensePathForm == Type
            && Properties.Settings.Default.LicensePath == ToString())
        {
            return;
        }

        Save(licenseType, Type, ToString());
    }

    public override string ToString()
    {
        string value = "";
        LicenseUriType type = Type;
        switch (type)
        {
            case LicenseUriType.FilePath:
                value = FilePath;
                break;
            case LicenseUriType.ServerAndPort:
                value = string.Format("{0}@{1}", Port, Host);
                break;
            case LicenseUriType.SourceList:
                value = _sourceList;
                break;
        }

        return value;
    }

    private static bool GetHostAndPortFromLicensePathSetting(string licensePathSetting, out string licenseHost, out uint licensePort)
    {
        bool parsed = false;
        licenseHost = string.Empty;
        licensePort = 0;
        string[] parts = licensePathSetting.Split('@', global::System.StringSplitOptions.None);
        if (parts.Length == 2)
        {
            if (uint.TryParse(parts[0], out uint port))
            {
                parsed = true;
                licensePort = port;
            }

            licenseHost = parts[1];
        }

        return parsed;
    }

    public LicenseSource()
    {
    }
}
