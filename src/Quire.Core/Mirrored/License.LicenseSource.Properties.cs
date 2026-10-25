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
}
