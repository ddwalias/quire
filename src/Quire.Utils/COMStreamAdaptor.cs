#nullable disable
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace Quire.Utils;

public partial class COMStreamAdaptor
{
    private IStream m_COMStream;

    public override bool CanRead => true;

    public override bool CanSeek => true;

    public override bool CanWrite => true;

    public override long Length
    {
        get
        {
            CheckDisposed();
            m_COMStream.Stat(out var pstatstg, 1);
            return pstatstg.cbSize;
        }
    }

    public override long Position
    {
        get => Seek(0L, SeekOrigin.Current);
        set => Seek(value, SeekOrigin.Begin);
    }

    public COMStreamAdaptor(IStream COMStream)
    {
        if (COMStream == null)
        {
            throw new ArgumentNullException();
        }
        m_COMStream = COMStream;
    }

    protected void CheckDisposed()
    {
        if (m_COMStream == null)
        {
            throw new ArgumentNullException();
        }
    }

    public unsafe override int Read(byte[] buffer, int offset, int count)
    {
        CheckDisposed();
        int num;
        int* ptr = &num;
        if (offset != 0)
        {
            byte[] array = new byte[count];
            m_COMStream.Read(array, count, new IntPtr(ptr));
            Array.Copy(array, 0, buffer, offset, num);
        }
        else
        {
            m_COMStream.Read(buffer, count, new IntPtr(ptr));
        }
        return num;
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        CheckDisposed();
        if (offset != 0)
        {
            int num = buffer.Length - offset;
            byte[] array = new byte[num];
            Array.Copy(buffer, offset, array, 0, num);
            IntPtr pcbWritten = new IntPtr(0);
            m_COMStream.Write(array, num, pcbWritten);
        }
        else
        {
            m_COMStream.Write(buffer, count, IntPtr.Zero);
        }
    }

    public unsafe override long Seek(long offset, SeekOrigin origin)
    {
        CheckDisposed();
        long result = 0L;
        long* ptr = &result;
        m_COMStream.Seek(offset, (int)origin, new IntPtr(ptr));
        return result;
    }

    public override void SetLength(long length)
    {
        CheckDisposed();
        m_COMStream.SetSize(length);
    }

    public override void Flush()
    {
        CheckDisposed();
        Commit();
    }

    public new void Dispose()
    {
        CleanUp();
        base.Dispose();
    }

    protected void CleanUp()
    {
        if (m_COMStream != null)
        {
            Commit();
            Marshal.ReleaseComObject(m_COMStream);
            m_COMStream = null;
        }
    }

    protected virtual void Commit()
    {
        m_COMStream.Commit(0);
    }

    public override void Close()
    {
        CleanUp();
        GC.SuppressFinalize(this);
    }
}
