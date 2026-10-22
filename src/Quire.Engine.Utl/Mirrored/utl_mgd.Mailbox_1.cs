#nullable disable
#pragma warning disable

namespace Quire.utl_mgd;

partial class Mailbox<T>
{
    private global::Quire.utl_mgd.SafeMutex m_mutex;
    private global::Quire.utl_mgd.SafeEvent m_nonempty;
    private global::System.Collections.Generic.Queue<T> m_queue;
}
