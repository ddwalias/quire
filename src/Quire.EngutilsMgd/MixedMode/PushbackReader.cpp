#include "EngutilsMgd.h"

namespace Quire { namespace Native { namespace EngutilsMgd {

    void PushbackReader::Unread(int c)
    {
        m_stack->Push(c);
        GC::KeepAlive(this);
    }

    int PushbackReader::Read()
    {
        if (m_stack->Count == 0)
        {
            int result = m_reader->Read();
            GC::KeepAlive(this);
            return result;
        }
        int result2 = (int)m_stack->Pop();
        GC::KeepAlive(this);
        return result2;
    }

}}}
