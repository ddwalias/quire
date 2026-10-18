#nullable disable
using Quire.Mirror;

namespace Quire.MindTouch.IO;

public partial class ChunkedMemoryStream
{
    [MirrorVerified("MindTouchIOChunkedMemoryStreamMechanicalDifferentialTests")]
    public ChunkedMemoryStream() : this(0) { }

    [MirrorVerified("MindTouchIOChunkedMemoryStreamMechanicalDifferentialTests")]
    public ChunkedMemoryStream(byte[] buffer) : this(buffer, true) { }

    [MirrorVerified("MindTouchIOChunkedMemoryStreamMechanicalDifferentialTests")]
    public ChunkedMemoryStream(byte[] buffer, int index, int count) : this(buffer, index, count, true, false) { }

    [MirrorVerified("MindTouchIOChunkedMemoryStreamMechanicalDifferentialTests")]
    public ChunkedMemoryStream(byte[] buffer, int index, int count, bool writable) : this(buffer, index, count, writable, false) { }

    [MirrorVerified("MindTouchIOChunkedMemoryStreamMechanicalDifferentialTests")]
    public override void Flush() => EnsureOpen();

    [MirrorVerified("MindTouchIOChunkedMemoryStreamMechanicalDifferentialTests")]
    public static int ChunkSize { get; set; }
}
