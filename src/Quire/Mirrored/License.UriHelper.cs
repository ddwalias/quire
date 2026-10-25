#nullable disable

namespace Quire.License;

public static partial class UriHelper
{
    public static global::System.Uri GetUriFromString(string uriString)
    {
        if (global::System.Uri.TryCreate(uriString, global::System.UriKind.Absolute, out global::System.Uri result))
        {
            return result;
        }

        return null;
    }
}
