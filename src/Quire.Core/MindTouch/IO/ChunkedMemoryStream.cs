#nullable disable
using Quire.Mirror;
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

    [MirrorVerified("Release IL")]
    private bool _closed;

    [MirrorVerified("Release IL")]
    public override bool CanRead => !_closed;

    [MirrorVerified("Release IL")]
    public override bool CanSeek => !_closed;

    [MirrorVerified("Release IL")]
    public override bool CanWrite
    {
        get
        {
            if (!_closed)
            {
                return !_readonly;
            }
            return false;
        }
    }

    [MirrorVerified("Release IL")]
    public override long Length
    {
        get
        {
            EnsureOpen();
            return _length;
        }
    }

    [MirrorVerified("Release IL")]
    public override long Position
    {
        get
        {
            EnsureOpen();
            return _position;
        }
        set
        {
            EnsureOpen();
            if (value < 0L)
            {
                throw new ArgumentOutOfRangeException("value");
            }
            _position = value;
        }
    }

}
