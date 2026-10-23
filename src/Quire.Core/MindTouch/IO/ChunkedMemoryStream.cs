#nullable disable
using Quire.Mirror;
using Quire.Mirror;

namespace Quire.MindTouch.IO;

public partial class ChunkedMemoryStream
{
    public ChunkedMemoryStream(int capacity)
    {
        if (capacity < 0)
        {
            throw new ArgumentOutOfRangeException("capacity");
        }
        _buffer = new byte[capacity];
        _publiclyVisible = true;
    }

    protected override void Dispose(bool disposing)
    {
        try
        {
            if (disposing)
            {
                _buffer = null;
                _chunks = null;
                _closed = true;
            }
        }
        finally
        {
            base.Dispose(disposing);
        }
    }

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

    private void ConsolidateChunks(long capacity)
    {
        byte[] buffer = new byte[capacity];
        long position = _position;
        Position = 0L;
        Read(buffer, 0, (int)_length);
        Position = position;
        _buffer = buffer;
        _chunks = null;
    }

    private byte[] GetChunk(int index, out int count)
    {
        if (index == 0)
        {
            count = _bufferCount;
            return _buffer;
        }
        index--;
        if (_chunks == null)
        {
            int num;
            for (num = 1; num <= index; num *= 2)
            {
            }
            _chunks = new byte[num][];
        }
        else if (index >= _chunks.Length)
        {
            int num2;
            for (num2 = _chunks.Length * 2; num2 <= index; num2 *= 2)
            {
            }
            Array.Resize(ref _chunks, num2);
        }
        byte[] result = _chunks[index] ?? (_chunks[index] = new byte[ChunkSize]);
        count = ChunkSize;
        return result;
    }

    public override long Seek(long offset, System.IO.SeekOrigin origin)
    {
        EnsureOpen();
        long num;
        switch (origin)
        {
            case System.IO.SeekOrigin.Begin:
                num = offset;
                break;
            case System.IO.SeekOrigin.Current:
                num = _position + offset;
                break;
            case System.IO.SeekOrigin.End:
                num = _length + offset;
                break;
            default:
                throw new NotImplementedException(string.Format("unknown SeekOrigin.{0}", origin));
        }
        if (num < 0L)
        {
            throw new ArgumentException("final offset cannot be negative", "offset");
        }
        return _position = num;
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
