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

    [MirrorVerified("Release IL")]
    private void EnsureOpen()
    {
        if (_closed)
        {
            throw new ObjectDisposedException("stream has already been closed or disposed");
        }
    }

    [MirrorVerified("Release IL")]
    public byte[] ToArray()
    {
        EnsureOpen();
        byte[] array = new byte[Length];
        long position = _position;
        Position = 0L;
        Read(array, 0, array.Length);
        Position = position;
        return array;
    }

    [MirrorVerified("Release IL")]
    public byte[] GetBuffer()
    {
        if (!_publiclyVisible)
        {
            throw new UnauthorizedAccessException();
        }
        if (_chunks != null)
        {
            ConsolidateChunks(Capacity);
        }
        return _buffer;
    }

    private int GetChunkIndex(long position, out int offset)
    {
        if (position < _buffer.Length - _bufferIndex)
        {
            offset = (int)position + _bufferIndex;
            return 0;
        }
        position -= _buffer.Length;
        int num = (int)(position / ChunkSize);
        offset = (int)(position % ChunkSize);
        return num + 1;
    }

    [MirrorVerified("Release IL")]
    public long Capacity
    {
        get
        {
            int capacity = _bufferCount;
            if (_chunks != null)
            {
                capacity += _chunks.Length * ChunkSize;
            }
            return capacity;
        }
        set
        {
            if (value == Capacity)
            {
                return;
            }
            if (_fixed)
            {
                throw new NotSupportedException("Cannot expand this ChunkedMemoryStream");
            }
            if (value < _length)
            {
                throw new ArgumentOutOfRangeException("value", string.Format("New capacity cannot be negative or less than the current length {0} {1}", value, _length));
            }
            ConsolidateChunks(value);
        }
    }

}
