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

    public ChunkedMemoryStream(byte[] buffer, bool writable)
    {
        if (buffer == null)
        {
            throw new ArgumentNullException("buffer");
        }
        _buffer = buffer;
        _bufferIndex = 0;
        _bufferCount = buffer.Length;
        _length = _bufferCount;
        _fixed = true;
        _readonly = !writable;
    }

    public ChunkedMemoryStream(byte[] buffer, int index, int count, bool writable, bool publiclyVisible)
    {
        if (buffer == null)
        {
            throw new ArgumentNullException("buffer");
        }
        if (index < 0 || index >= buffer.Length)
        {
            throw new ArgumentOutOfRangeException("index");
        }
        if (count < 0 || index >= buffer.Length - count)
        {
            throw new ArgumentOutOfRangeException("count");
        }
        _buffer = buffer;
        _bufferIndex = index;
        _bufferCount = count;
        _length = _bufferCount - _bufferIndex;
        _fixed = true;
        _readonly = !writable;
        _publiclyVisible = publiclyVisible;
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

    public override int Read(byte[] buffer, int offset, int count)
    {
        EnsureOpen();
        if (buffer == null)
        {
            throw new ArgumentNullException("buffer");
        }
        if (offset < 0)
        {
            throw new ArgumentOutOfRangeException("offset");
        }
        if (count < 0)
        {
            throw new ArgumentOutOfRangeException("count");
        }
        if (count > buffer.Length - offset)
        {
            throw new ArgumentException("count exceeds remaining bytes in buffer", "count");
        }
        if (_position >= _length || count == 0)
        {
            return 0;
        }
        int offset2;
        int chunkIndex = GetChunkIndex(_position, out offset2);
        int result = (count = Math.Min(count, (int)(_length - _position)));
        _position += count;
        while (count > 0)
        {
            byte[] chunk = GetChunk(chunkIndex++, out int count2);
            int num = Math.Min(count2 - offset2, count);
            Array.Copy(chunk, offset2, buffer, offset, num);
            count -= num;
            offset += num;
            offset2 = 0;
        }
        return result;
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        EnsureOpen();
        if (_readonly)
        {
            throw new NotSupportedException("Stream is not writable");
        }
        if (buffer == null)
        {
            throw new ArgumentNullException("buffer");
        }
        if (offset < 0)
        {
            throw new ArgumentOutOfRangeException("offset");
        }
        if (count < 0)
        {
            throw new ArgumentOutOfRangeException("count");
        }
        if (count > buffer.Length - offset)
        {
            throw new ArgumentException("count exceeds remaining bytes in buffer", "count");
        }
        EnsureCapacity(_position + count);
        int offset2;
        int chunkIndex = GetChunkIndex(_position, out offset2);
        _position += count;
        _length = Math.Max(_length, _position);
        while (count > 0)
        {
            int count2;
            byte[] chunk = GetChunk(chunkIndex++, out count2);
            int num = Math.Min(count2 - offset2, count);
            Array.Copy(buffer, offset, chunk, offset2, num);
            count -= num;
            offset += num;
            offset2 = 0;
        }
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

    private void EnsureCapacity(long requested)
    {
        long capacity = Capacity;
        if (requested <= capacity)
        {
            return;
        }
        if (_fixed)
        {
            throw new NotSupportedException("Cannot expand this ChunkedMemoryStream");
        }
        if (requested < 256)
        {
            requested = 256L;
        }
        if (requested < capacity * 2)
        {
            requested = capacity * 2;
        }
        if (requested > ChunkSize)
        {
            requested = ChunkSize;
        }
        if (_buffer.Length != requested)
        {
            Array.Resize(ref _buffer, (int)requested);
            _bufferCount = _buffer.Length - _bufferIndex;
        }
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

    public override void SetLength(long value)
    {
        EnsureOpen();
        if (_fixed && value > Capacity)
        {
            throw new NotSupportedException("Expanding this ChunkedMemoryStream is not supported");
        }
        if (_readonly)
        {
            throw new NotSupportedException("Cannot write to this ChunkedMemoryStream");
        }
        if (value < 0)
        {
            throw new ArgumentOutOfRangeException("value");
        }
        EnsureCapacity(value);
        int offset;
        int num = GetChunkIndex(value, out offset);
        if (_chunks != null && num < _chunks.Length)
        {
            if (offset > 0)
            {
                byte[] array = _chunks[num];
                if (array != null)
                {
                    Array.Clear(array, offset, ChunkSize - offset);
                }
                num++;
            }
            for (int i = num; i < _chunks.Length; i++)
            {
                _chunks[i] = null;
            }
        }
        _length = value;
        _position = Math.Min(_position, _length);
    }

    public void WriteTo(System.IO.Stream stream)
    {
        long num = _length;
        int num2 = 0;
        while (num > 0)
        {
            int count;
            byte[] chunk = GetChunk(num2++, out count);
            int num3 = (int)Math.Min(num, count);
            stream.Write(chunk, 0, num3);
            num -= num3;
        }
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
