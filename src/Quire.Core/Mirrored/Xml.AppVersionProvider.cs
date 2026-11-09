#nullable disable
#pragma warning disable

namespace Quire.Xml;

static partial class AppVersionProvider
{
    private static global::Quire.Xml.AppVersionData _versionData;
    private static readonly object Lock = new object();
    private const string ResourceName = "Ptc.Core.AppVersion.json";

    public static global::Quire.Xml.AppVersionData VersionData
    {
        get
        {
            if (_versionData != null)
            {
                return _versionData;
            }
            lock (Lock)
            {
                if (_versionData == null)
                {
                    _versionData = LoadVersionData();
                }
            }
            return _versionData;
        }
    }
}
