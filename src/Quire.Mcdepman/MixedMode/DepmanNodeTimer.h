#pragma once

#define WIN32_LEAN_AND_MEAN
#define NOMINMAX
#include <sdkddkver.h>
#include "../../Quire.Native.Utl/utl.h"

namespace Quire { namespace depman {

    [Quire::Mirror::MirrorOfAttribute("depman.NodeTimer, mcdepman")]
    public ref class NodeTimer : System::IDisposable
    {
    public:
        ::utl::PerformanceTimer* m_timer;

        NodeTimer();
        void Start();
        void Stop();
        double GetElapsed();

    private:
        ~NodeTimer();
        !NodeTimer();
    };

}}
