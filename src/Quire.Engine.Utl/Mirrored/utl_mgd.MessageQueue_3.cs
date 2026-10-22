#nullable disable
#pragma warning disable

namespace Quire.utl_mgd;

partial class MessageQueue<Owner, Data, Msg>
{
    private global::Quire.utl_mgd.Mailbox<Msg> m_mailbox;
    private global::System.Threading.Thread m_thread;
    private int m_stack_size;
    private unsafe void* m_death_handle;
}
