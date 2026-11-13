#include "DepmanNodeTimer.h"

namespace Quire { namespace depman {

    NodeTimer::NodeTimer()
    {
        m_timer = new ::utl::PerformanceTimer();
    }

    NodeTimer::~NodeTimer()
    {
        this->!NodeTimer();
    }

    NodeTimer::!NodeTimer()
    {
        delete m_timer;
    }

    void NodeTimer::Start()
    {
        m_timer->Start();
    }

    void NodeTimer::Stop()
    {
        m_timer->Stop();
    }

    double NodeTimer::GetElapsed()
    {
        return m_timer->GetElapsed();
    }

}}
