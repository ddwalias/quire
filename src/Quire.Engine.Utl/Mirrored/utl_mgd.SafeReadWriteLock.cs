#nullable disable
#pragma warning disable

namespace Quire.utl_mgd;

partial class SafeReadWriteLock
{
    private global::Quire.utl_mgd.SafeMutex m_write_lock;
    private global::Quire.utl_mgd.SafeMutex m_reader_count_lock;
    private int m_reader_count;
    private global::Quire.utl_mgd.SafeEvent m_no_readers;
}
