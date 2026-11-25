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
#pragma comment(linker, "/include:?clear_result@ActionChain@depman@@QEAAXXZ")
#pragma comment(linker, "/include:??1ActionChain@depman@@QEAA@XZ")
#pragma comment(linker, "/include:?OnUnload@ActionChain@depman@@QEAAXXZ")
#pragma comment(linker, "/include:?add_env_scope@ActionChain@depman@@$$FQEAMXPE$AAVScope@2Quire@@@Z")
#pragma comment(linker, "/include:?remove_env_scope@ActionChain@depman@@$$FQEAMXPE$AAVScope@2Quire@@@Z")
#pragma comment(linker, "/include:?UnregisterDeps@ActionChain@depman@@QEAAXXZ")
#pragma comment(linker, "/include:?RegisterDeps@ActionChain@depman@@QEAAXXZ")
#pragma comment(linker, "/include:?GetDeps@ActionChain@depman@@$$FQEAMXPE$AAVStringVoid@2Quire@@@Z")
#pragma comment(linker, "/include:?AddStaticDep@ActionChain@depman@@$$FQEAMXPE$AAVString@System@@@Z")
#pragma comment(linker, "/include:?ClearDeps@ActionChain@depman@@QEAAXXZ")
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

void ActionChainMgd::OnUnload()
{
    m_deps->Clear();
    if (next() != nullptr)
        next()->OnUnload();
    System::GC::KeepAlive(this);
}

void ActionChainMgd::add_dep(Scope^ s, DSym sym, ScopingMode sm, DependencyType dt)
{
    CacheKey key(sym, sm, dt);
    System::Collections::Generic::HashSet<Scope^>^ set = nullptr;
    if (!m_deps->TryGetValue(key, set))
    {
        set = gcnew System::Collections::Generic::HashSet<Scope^>();
        m_deps->Add(key, set);
    }
    if (s != nullptr && !set->Contains(s))
    {
        set->Add(s);
        depman()->add_dep(s->Id, sym, loc(), m_chain_umgd, (::depman::DependencyType)dt);
    }
    System::GC::KeepAlive(this);
}

OnDep^ ActionChainMgd::on_dep()
{
    return gcnew OnDep(this, &ActionChainMgd::add_dep);
}

void ActionChainMgd::add_env_scope(Scope^ s)
{
    for each (CacheKey key in m_deps->Keys)
    {
        add_dep(s, key.ds, key.sm, key.dt);
    }
}

void ActionChainMgd::remove_env_scope(Scope^ s)
{
    for each (System::Collections::Generic::KeyValuePair<CacheKey, System::Collections::Generic::HashSet<Scope^>^> pair in m_deps)
    {
        CacheKey key = pair.Key;
        pair.Value->Remove(s);
        depman()->remove_dep(s->Id, key.ds, loc(), m_chain_umgd);
    }
    System::GC::KeepAlive(this);
}

void ActionChainMgd::UnregisterDeps()
{
    for each (System::Collections::Generic::KeyValuePair<CacheKey, System::Collections::Generic::HashSet<Scope^>^> pair in m_deps)
    {
        CacheKey key = pair.Key;
        for each (Scope^ s in pair.Value)
        {
            depman()->remove_dep(s->Id, key.ds, loc(), m_chain_umgd);
        }
    }
    if (next() != nullptr)
        next()->UnregisterDeps();
    System::GC::KeepAlive(this);
}

void ActionChainMgd::RegisterDeps()
{
    auto deps = m_deps;
    m_deps = gcnew System::Collections::Generic::Dictionary<CacheKey, System::Collections::Generic::HashSet<Scope^>^>();
    for each (CacheKey key in deps->Keys)
    {
        DepTrackingEnv^ env = m_env->GetDepTrackingEnv();
        OnDep^ cb = gcnew OnDep(this, &ActionChainMgd::add_dep);
        env->Lookup(key.ds.full, key.sm, key.dt, cb);
    }
    if (next() != nullptr)
        next()->RegisterDeps();
    System::GC::KeepAlive(this);
}

void ActionChainMgd::GetDeps(StringVoid^ f)
{
    auto enumerator = m_deps->Keys->GetEnumerator();
    while (enumerator.MoveNext())
    {
        System::String^ full = ((CacheKey)(System::Object^)enumerator.Current).ds.full;
        f(full);
    }
    if (next() != nullptr)
        next()->GetDeps(f);
    System::GC::KeepAlive(this);
}

void ActionChainMgd::AddStaticDep(System::String^ name)
{
    ScopingMode sm = ScopingMode::NORMAL;
    if (name->StartsWith("CHILD_ONLY:"))
    {
        sm = ScopingMode::CHILD_ONLY;
        name = name->Substring(11);
    }
    else if (name->StartsWith("EXCLUDE_CHILD:"))
    {
        sm = ScopingMode::EXCLUDE_CHILD;
        name = name->Substring(14);
    }
    DepTrackingEnv^ env = m_env->GetDepTrackingEnv();
    OnDep^ cb = gcnew OnDep(this, &ActionChainMgd::add_dep);
    env->Lookup(name, sm, DependencyType::Static, cb);
    System::GC::KeepAlive(this);
}

void ActionChainMgd::ClearDeps()
{
    UnregisterDeps();
    m_deps->Clear();
    System::GC::KeepAlive(this);
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

ActionChain::~ActionChain()
{
    mgd_->ClearDeps();
    delete next_;
    delete (::Quire::depman::ActionChainMgd^)mgd_;
}

void ActionChain::clear_result()
{
    loc_->env()->GetSharedThreadContext()->ObtainMutex(false);
    try
    {
        delete next_;
        next_ = nullptr;
        mgd_->set_result(nullptr);
    }
    finally
    {
        loc_->env()->GetSharedThreadContext()->ReleaseMutex(false);
    }
}

void ActionChain::OnUnload()
{
    mgd_->OnUnload();
}

void ActionChain::add_env_scope(::Quire::depman::Scope^ s)
{
    mgd_->add_env_scope(s);
}

void ActionChain::remove_env_scope(::Quire::depman::Scope^ s)
{
    mgd_->remove_env_scope(s);
}

void ActionChain::UnregisterDeps()
{
    mgd_->UnregisterDeps();
}

void ActionChain::RegisterDeps()
{
    mgd_->RegisterDeps();
}

void ActionChain::GetDeps(::Quire::depman::StringVoid^ f)
{
    mgd_->GetDeps(f);
}

void ActionChain::AddStaticDep(System::String^ name)
{
    mgd_->AddStaticDep(name);
}

void ActionChain::ClearDeps()
{
    mgd_->ClearDeps();
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
