#nullable disable
#pragma warning disable

namespace Quire.Core;

partial interface IMaybeHasPointThatSnapsToPixel
{
    global::Quire.Core.ISnapToPixelHelper SnappingHelper { get; }
}
