#pragma once

#include "DepmanContracts.h"

namespace Quire { namespace depman {

    [Quire::Mirror::MirrorOfAttribute("depman.SafeRun, mcdepman")]
    ref class SafeRun
    {
    private:
        System::Threading::ThreadStart^ ts;
        Quire::Mcd::RuntimeOptions^ options;
        Quire::utl_mgd::InterruptControl^ interrupt_control;
        Quire::utl::InterruptControl* interrupt_control_umgd;
        System::Exception^ ex;

    public:
        SafeRun(System::Threading::ThreadStart^ _ts, Quire::Mcd::RuntimeOptions^ o, Quire::utl_mgd::InterruptControl^ c, Quire::utl::InterruptControl* c_umgd);
        void Run();
        void CheckErrors();
    };

}}
