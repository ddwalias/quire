#nullable disable
using Quire.Mirror;

namespace Quire.Core;

public static partial class MaybeHasPointThatSnapsToPixelExtensions
{
    [MirrorVerified("Release IL")]
    public static bool HasPointThatSnapsToPixel(this IMaybeHasPointThatSnapsToPixel item) => item.SnappingHelper.HasPointThatSnapsToPixel;
}
