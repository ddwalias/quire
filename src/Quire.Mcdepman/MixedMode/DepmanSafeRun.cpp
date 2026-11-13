#include "DepmanSafeRun.h"

namespace Quire { namespace depman {

    SafeRun::SafeRun(System::Threading::ThreadStart^ _ts, Quire::Mcd::RuntimeOptions^ o, Quire::utl_mgd::InterruptControl^ c, Quire::utl::InterruptControl* c_umgd)
        : ts(_ts),
          options(o),
          interrupt_control(c),
          interrupt_control_umgd(c_umgd)
    {
    }

    void SafeRun::Run()
    {
        try
        {
            Quire::Mpl::Run::Rts::RegisterThreadForCancellationChecking(interrupt_control->Cts);
            options->SyncToThread();
            Quire::utl_mgd::InterruptControl::ThreadInitialize(interrupt_control);
            Quire::utl::InterruptControl::ThreadInitialize(interrupt_control_umgd);
            ts();
        }
        catch (System::Exception^ ex)
        {
            this->ex = ex;
        }
    }

    void SafeRun::CheckErrors()
    {
        if (ex == nullptr)
            return;

        for (System::Exception^ inner = ex; inner != nullptr; inner = inner->InnerException)
        {
            if (dynamic_cast<System::Threading::ThreadAbortException^>(inner) != nullptr)
                throw SafeAbortException::Instance();
        }

        throw ex;
    }

}}
