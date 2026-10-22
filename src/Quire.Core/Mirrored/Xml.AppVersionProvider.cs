#nullable disable
#pragma warning disable

namespace Quire.Xml;

static partial class AppVersionProvider
{
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
