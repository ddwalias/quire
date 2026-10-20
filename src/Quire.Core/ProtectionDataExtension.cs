#nullable disable
using Quire.Mirror;

namespace Quire;

public static partial class ProtectionDataExtension
{
    [MirrorVerified("Release IL")]
    public static ProtectionLevel ProtectionLevel(this ProtectionData protectionData)
    {
        if (protectionData != null)
        {
            return protectionData.ProtectionLevel;
        }
        return global::Quire.ProtectionLevel.None;
    }

    [MirrorVerified("Release IL")]
    public static bool IsProtected(this ProtectionLevel protectionLevel)
    {
        return protectionLevel != global::Quire.ProtectionLevel.None;
    }

    [MirrorVerified("Release IL")]
    public static bool IsActivationProtected(this ProtectionLevel protectionLevel)
    {
        return protectionLevel.HasFlag(global::Quire.ProtectionLevel.Activation);
    }

    [MirrorVerified("Release IL")]
    public static bool IsSelectionProtected(this ProtectionLevel protectionLevel)
    {
        return protectionLevel.HasFlag(global::Quire.ProtectionLevel.Selection);
    }

    [MirrorVerified("Release IL")]
    public static bool IsDisplayStateProtected(this ProtectionLevel protectionLevel)
    {
        return protectionLevel.HasFlag(global::Quire.ProtectionLevel.DisplayState);
    }
}
