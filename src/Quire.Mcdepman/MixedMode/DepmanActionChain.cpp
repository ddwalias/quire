#define WIN32_LEAN_AND_MEAN
#define NOMINMAX
#include <sdkddkver.h>
#include "DepmanContracts.h"
#include "DepmanPosition.h"

// Defined in its own translation unit so that PositionBase::IsLocked calls it, as the original does.
#pragma comment(linker, "/include:?IsLocked@ActionChain@depman@@QEBA_NXZ")
#pragma comment(linker, "/include:?IsUndefine@ActionChain@depman@@QEAA_NXZ")
#pragma comment(linker, "/include:?IsChildOnly@ActionChain@depman@@QEAA_NXZ")
#pragma comment(linker, "/include:?Lock@ActionChain@depman@@QEAAXXZ")
#pragma comment(linker, "/include:?UnLock@ActionChain@depman@@QEAAXXZ")
#pragma comment(linker, "/include:?SetNeedsCheck@ActionChain@depman@@QEAAXXZ")

namespace Quire { namespace depman {

::depman::DepManager* ActionChainMgd::depman()
{
    auto* result = m_chain_umgd->dep_manager_;
    System::GC::KeepAlive(this);
    return result;
}

::depman::Position* ActionChainMgd::loc()
{
    auto* result = m_chain_umgd->loc_;
    System::GC::KeepAlive(this);
    return result;
}

::depman::ActionChain* ActionChainMgd::next()
{
    auto* result = m_chain_umgd->next_;
    System::GC::KeepAlive(this);
    return result;
}

bool ActionChainMgd::IsDead()
{
    bool result = m_chain_umgd->is_dead_;
    System::GC::KeepAlive(this);
    return result;
}

SharedThreadContext^ ActionChainMgd::GetSharedThreadContext()
{
    SharedThreadContext^ result = m_chain_umgd->loc_->env()->GetSharedThreadContext();
    System::GC::KeepAlive(this);
    return result;
}

}} // namespace Quire::depman

namespace depman {

bool ActionChain::IsLocked() const
{
    return locked_;
}

bool ActionChain::IsUndefine()
{
    return mgd_->action()->IsUndefine();
}

bool ActionChain::IsChildOnly()
{
    return mgd_->action()->IsChildOnly();
}

void ActionChain::Lock()
{
    locked_ = true;
    if (next_)
        next_->Lock();
}

void ActionChain::UnLock()
{
    locked_ = false;
    if (next_)
        next_->UnLock();
}

void ActionChain::SetNeedsCheck()
{
    loc_->env()->GetSharedThreadContext()->ObtainMutex(false);
    try
    {
        mgd_->env()->set_needs_check(true);
        if (next_)
            next_->SetNeedsCheck();
    }
    finally
    {
        loc_->env()->GetSharedThreadContext()->ReleaseMutex(false);
    }
}

} // namespace depman
