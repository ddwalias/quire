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
#pragma comment(linker, "/include:?Kill@ActionChain@depman@@QEAAXXZ")
#pragma comment(linker, "/include:?GetSharedThreadContext@ActionChain@depman@@$$FQEBMPE$AAVSharedThreadContext@2Quire@@XZ")
#pragma comment(linker, "/include:?value@ActionChain@depman@@$$FQEAMPE$AAVObject@System@@XZ")

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

System::Collections::Generic::Dictionary<CacheKey, __int64>^ ActionChainMgd::TreatAsReadyDeps(System::Collections::Generic::Dictionary<CacheKey, __int64>^ d)
{
    DepTrackingEnv^ env = gcnew DepTrackingEnv(loc());
    if (d == nullptr)
        d = gcnew System::Collections::Generic::Dictionary<CacheKey, __int64>();

    for each (CacheKey key in m_deps->Keys)
    {
        if (key.ds.full->StartsWith("%%children") || key.ds.full->StartsWith("value mc_"))
        {
            ::depman::Position* p = env->Lookup(key.ds.full, key.sm, key.dt, nullptr);
            d[key] = reinterpret_cast<__int64>(p);
        }
    }

    if (next() != nullptr)
    {
        return next()->TreatAsReadyDeps(d);
    }

    return d;
}

::depman::Position* DepTrackingEnv::Lookup(System::String^ /*name*/, ScopingMode /*scoping_mode*/, DependencyType /*deptype*/, OnDep^ /*on_dep_callback*/)
{
    throw Quire::Mirror::MirrorStub::NotImplemented();
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

void ActionChain::Kill()
{
    is_dead_ = true;
    if (next_)
        next_->Kill();
}

::Quire::depman::SharedThreadContext^ ActionChain::GetSharedThreadContext() const
{
    return loc_->env()->GetSharedThreadContext();
}

System::Object^ ActionChain::value()
{
    if (mgd_->env()->needs_check() && !locked_)
        return nullptr;
    return next_ ? next_->value() : mgd_->result();
}

System::Collections::Generic::Dictionary<::Quire::depman::CacheKey, __int64>^ ActionChain::TreatAsReadyDeps(System::Collections::Generic::Dictionary<::Quire::depman::CacheKey, __int64>^ d)
{
    return mgd_->TreatAsReadyDeps(d);
}

} // namespace depman
