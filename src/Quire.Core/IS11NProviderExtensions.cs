#nullable disable
using Quire.Mirror;
using Quire.Wpf;

namespace Quire.Serialization;

public static partial class IS11NProviderExtensions
{
    [MirrorVerified("Release IL")]
    public static string PackImageExt(this IS11NProvider provider, ImagePackagingInformation pictureInfo)
    {
        string result = string.Empty;
        if (provider == null)
        {
            DebugTools.DevAssert(false, "Missing S11NProvider initialization can't call PackImage");
        }
        else
        {
            result = provider.PackImage(pictureInfo);
        }
        return result;
    }

    [MirrorVerified("Release IL")]
    public static string PackExternalObjectExt(this IS11NProvider provider, string sourceFileName, string excelUniqueId, ExternalObjectType objType)
    {
        string result = string.Empty;
        if (provider == null)
        {
            DebugTools.DevAssert(false, "Missing S11NProvider initialization can't call PackImage");
        }
        else
        {
            result = provider.PackExternalObject(sourceFileName, excelUniqueId, objType);
        }
        return result;
    }
}
