#include "EngutilsMgd.h"

namespace Quire { namespace Engine {

    UnmanagedMemoryStream::UnmanagedMemoryStream(unsigned char* memory, int length, int capacity, bool canWrite)
    {
        _mem = memory;
        _length = length;
        _capacity = capacity;
        _writable = canWrite;
        _isOpen = true;
    }

    bool UnmanagedMemoryStream::CanRead::get()
    {
        return _isOpen;
    }

    bool UnmanagedMemoryStream::CanSeek::get()
    {
        return _isOpen;
    }

    bool UnmanagedMemoryStream::CanWrite::get()
    {
        return _writable;
    }

    void UnmanagedMemoryStream::Close()
    {
        _isOpen = false;
        _writable = false;
        _mem = nullptr;
    }

    void UnmanagedMemoryStream::Flush()
    {
    }

    long long UnmanagedMemoryStream::Length::get()
    {
        if (!_isOpen)
        {
            throw gcnew Exception("StreamIsClosed");
        }
        return _length;
    }

    long long UnmanagedMemoryStream::Position::get()
    {
        if (!_isOpen)
        {
            throw gcnew Exception("StreamIsClosed");
        }
        return _position;
    }

    void UnmanagedMemoryStream::Position::set(long long value)
    {
        if (!_isOpen)
        {
            throw gcnew Exception("StreamIsClosed");
        }
        if (value < 0)
        {
            throw gcnew ArgumentOutOfRangeException("value");
        }
        _position = (int)value;
    }

    unsigned char* UnmanagedMemoryStream::GetBytePtr()
    {
        return _mem + _position;
    }

    int UnmanagedMemoryStream::Read(array<unsigned char>^ buffer, int offset, int count)
    {
        if (!_isOpen)
        {
            throw gcnew Exception("StreamIsClosed");
        }
        if (buffer == nullptr)
        {
            throw gcnew ArgumentNullException("buffer");
        }
        if (offset < 0)
        {
            throw gcnew ArgumentOutOfRangeException("offset");
        }
        if (count < 0)
        {
            throw gcnew ArgumentOutOfRangeException("count");
        }
        if (buffer->Length - offset < count)
        {
            throw gcnew ArgumentException();
        }
        int position = _position;
        int num = _length - position;
        if (num > count)
        {
            num = count;
        }
        if (num < 0)
        {
            num = 0;
        }
        memcpy(_mem, position, buffer, offset, num);
        _position = num + position;
        return num;
    }

    int UnmanagedMemoryStream::ReadByte()
    {
        if (!_isOpen)
        {
            throw gcnew Exception("StreamIsClosed");
        }
        int position = _position;
        if (position >= _length)
        {
            return -1;
        }
        _position = position + 1;
        return _mem[position];
    }

    long long UnmanagedMemoryStream::Seek(long long offset, SeekOrigin loc)
    {
        if (!_isOpen)
        {
            throw gcnew Exception("StreamIsClosed");
        }
        if (offset > MemStreamMaxLength)
        {
            throw gcnew ArgumentOutOfRangeException("offset");
        }
        switch (loc)
        {
        case SeekOrigin::Begin:
            if (offset < 0)
            {
                throw gcnew IOException("IO.IO_SeekBeforeBegin");
            }
            _position = (int)offset;
            break;
        case SeekOrigin::Current:
            if (_position + offset < 0)
            {
                throw gcnew IOException("IO.IO_SeekBeforeBegin");
            }
            _position += (int)offset;
            break;
        case SeekOrigin::End:
            if (_length + offset < 0)
            {
                throw gcnew IOException("IO.IO_SeekBeforeBegin");
            }
            _position = _length + (int)offset;
            break;
        default:
            throw gcnew ArgumentException("Argument_InvalidSeekOrigin");
        }
        return _position;
    }

    void UnmanagedMemoryStream::SetLength(long long value)
    {
        if (!_writable)
        {
            throw gcnew Exception("WriteNotSupported");
        }
        if (value > _capacity)
        {
            throw gcnew IOException("IO.IO_FixedCapacity");
        }
        _length = (int)value;
        if (_position > value)
        {
            _position = (int)value;
        }
    }

    void UnmanagedMemoryStream::Write(array<unsigned char>^ buffer, int offset, int count)
    {
        if (!_isOpen)
        {
            throw gcnew Exception("StreamIsClosed");
        }
        if (!_writable)
        {
            throw gcnew Exception("WriteNotSupported");
        }
        if (buffer == nullptr)
        {
            throw gcnew ArgumentNullException("buffer");
        }
        if (offset < 0)
        {
            throw gcnew ArgumentOutOfRangeException("offset");
        }
        if (count < 0)
        {
            throw gcnew ArgumentOutOfRangeException("count");
        }
        if (buffer->Length - offset < count)
        {
            throw gcnew ArgumentException("Argument_InvalidOffLen");
        }
        int position = _position;
        int num = position + count;
        if (num < 0)
        {
            throw gcnew IOException("IO.IO_StreamTooLong");
        }
        if (num > _length)
        {
            if (num > _capacity)
            {
                throw gcnew IOException("IO.IO_FixedCapacity");
            }
            _length = num;
        }
        memcpy(buffer, offset, _mem, position, count);
        _position = num;
    }

    void UnmanagedMemoryStream::WriteByte(unsigned char value)
    {
        if (!_isOpen)
        {
            throw gcnew Exception("StreamIsClosed");
        }
        if (!_writable)
        {
            throw gcnew Exception("WriteNotSupported");
        }
        int position = _position;
        if (position == _length)
        {
            if (position + 1 > _capacity)
            {
                throw gcnew IOException("IO.IO_FixedCapacity");
            }
            _length++;
        }
        _mem[position] = value;
        _position = position + 1;
    }

    void UnmanagedMemoryStream::memcpy(array<unsigned char>^ src, int srcIndex, unsigned char* dest, int destIndex, int len)
    {
        if (len == 0)
        {
            return;
        }
        for (int i = 0; i < len; i++)
        {
            (dest + i)[destIndex] = src[i + srcIndex];
        }
    }

    void UnmanagedMemoryStream::memcpy(unsigned char* src, int srcIndex, array<unsigned char>^ dest, int destIndex, int len)
    {
        if (len == 0)
        {
            return;
        }
        for (int i = 0; i < len; i++)
        {
            dest[i + destIndex] = (src + i)[srcIndex];
        }
    }

}}
