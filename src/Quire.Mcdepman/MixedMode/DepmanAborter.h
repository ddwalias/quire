#pragma once

#include "DepmanContracts.h"

namespace Quire { namespace depman {

    [Quire::Mirror::MirrorOfAttribute("depman.Aborter, mcdepman")]
    ref class Aborter
    {
    private:
        System::Threading::Thread^ thread;
        Quire::utl_mgd::InterruptControl^ interrupt_control;
        Quire::utl::InterruptControl* interrupt_control_umgd;
        ThreadData^ threadData;
        System::Threading::CancellationTokenSource^ cts;

    public:
        Aborter(System::Threading::Thread^ t, Quire::utl_mgd::InterruptControl^ c, Quire::utl::InterruptControl* uc, ThreadData^ td);
    };

}}
