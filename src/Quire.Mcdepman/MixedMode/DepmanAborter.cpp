#include "DepmanAborter.h"
#include "DepmanSafeRun.cpp"

namespace Quire { namespace depman {

    Aborter::Aborter(System::Threading::Thread^ t, Quire::utl_mgd::InterruptControl^ c, Quire::utl::InterruptControl* uc, ThreadData^ td)
        : thread(t),
          interrupt_control(c),
          interrupt_control_umgd(uc),
          threadData(td),
          cts(c->Cts)
    {
    }

}}
