#define WIN32_LEAN_AND_MEAN
#define NOMINMAX
#include <sdkddkver.h>
#include "DepmanNodeTimer.h"
#include "DepmanPosition.h"

#include <algorithm>

// Uncalled native member functions are stripped by /OPT:REF unless referenced.
#pragma comment(linker, "/include:?GetHandle@PositionBase@depman@@QEBAJXZ")
#pragma comment(linker, "/include:?GetPriority@PositionBase@depman@@QEBAHXZ")
#pragma comment(linker, "/include:?IsDead@PositionBase@depman@@QEAA_NXZ")
#pragma comment(linker, "/include:?GetClientHandle@PositionBase@depman@@QEBAJXZ")
#pragma comment(linker, "/include:?GetBasePriority@PositionBase@depman@@QEBAHXZ")
#pragma comment(linker, "/include:?Invalidated@PositionBase@depman@@QEBA_NXZ")
#pragma comment(linker, "/include:?TreatedReady@PositionBase@depman@@QEBA_NXZ")
#pragma comment(linker, "/include:?IsBusy@PositionBase@depman@@QEAA_NXZ")
#pragma comment(linker, "/include:?IsReady@PositionBase@depman@@QEAA_NXZ")
#pragma comment(linker, "/include:?SetNotifyComputing@PositionBase@depman@@QEAAX_N@Z")
#pragma comment(linker, "/include:?ResetPriority@PositionBase@depman@@QEAAXXZ")
#pragma comment(linker, "/include:?ClearDParent@PositionBase@depman@@QEAAXXZ")
#pragma comment(linker, "/include:?SetBasePriority@PositionBase@depman@@QEAAXH@Z")
#pragma comment(linker, "/include:?Lock@PositionBase@depman@@QEAAXXZ")
#pragma comment(linker, "/include:?UnLock@PositionBase@depman@@QEAAXXZ")
#pragma comment(linker, "/include:?BeginMove@PositionBase@depman@@QEAAXXZ")
#pragma comment(linker, "/include:?SendMessage@PositionBase@depman@@$$FQEAMXW4NodeStatus@Engine@Quire@@@Z")
#pragma comment(linker, "/include:?SetTimer@PositionBase@depman@@$$FQEAMXPE$AAUNodeTimer@2Quire@@@Z")
#pragma comment(linker, "/include:?IsLocked@PositionBase@depman@@QEBA_NXZ")
#pragma comment(linker, "/include:?IsUndefine@PositionBase@depman@@QEAA_NXZ")
#pragma comment(linker, "/include:?IsChildOnly@PositionBase@depman@@QEAA_NXZ")
#pragma comment(linker, "/include:?GetDefs@PositionBase@depman@@QEAMPE$AAUICollection@Collections@System@@XZ")
#pragma comment(linker, "/include:?GetDependents@PositionBase@depman@@QEAAAEAV?$set@PEAVPosition@depman@@U?$less@PEAVPosition@depman@@@std@@V?$allocator@PEAVPosition@depman@@@4@@std@@XZ")
#pragma comment(linker, "/include:?GetScope@PositionBase@depman@@QEAMPE$AAVScope@2Quire@@XZ")
#pragma comment(linker, "/include:?SetScope@PositionBase@depman@@QEAMXPE$AAVScope@2Quire@@@Z")
#pragma comment(linker, "/include:?GetEnvScope@PositionBase@depman@@QEAMPE$AAVScope@2Quire@@XZ")
#pragma comment(linker, "/include:?env@PositionBase@depman@@$$FQEBMPE$AAUSEnvironment@2Quire@@XZ")
#pragma comment(linker, "/include:?GetSharedThreadContext@PositionBase@depman@@$$FQEBMPE$AAVSharedThreadContext@2Quire@@XZ")
#pragma comment(linker, "/include:?StartTiming@PositionBase@depman@@QEAAXXZ")
#pragma comment(linker, "/include:?Kill@PositionBase@depman@@QEAAXXZ")
#pragma comment(linker, "/include:?TreatAsReady@PositionBase@depman@@QEAAX_N@Z")
#pragma comment(linker, "/include:?OnDep@PositionBase@depman@@QEAAXPEAVPosition@2@@Z")
#pragma comment(linker, "/include:?depctl@DepManager@depman@@QEAAXH@Z")
#pragma comment(linker, "/include:?EndMove@PositionBase@depman@@QEAA_NXZ")
#pragma comment(linker, "/include:?find_def@DepManager@depman@@QEAM?AV?$_Tree_iterator@V?$_Tree_val@U?$_Tree_simple_types@U?$pair@QEAVPosition@depman@@W4DefinitionType@2@@std@@@std@@@std@@@std@@PEAV?$map@PEAVPosition@depman@@W4DefinitionType@2@U?$value_less@VPosition@depman@@@@V?$allocator@U?$pair@QEAVPosition@depman@@W4DefinitionType@2@@std@@@std@@@4@PEAVPosition@2@W4ResolveMode@2Quire@@@Z")
#pragma comment(linker, "/include:?find_def_nodead@DepManager@depman@@QEAMPEAVPosition@2@PEAV?$map@PEAVPosition@depman@@W4DefinitionType@2@U?$value_less@VPosition@depman@@@@V?$allocator@U?$pair@QEAVPosition@depman@@W4DefinitionType@2@@std@@@std@@@std@@PEAV32@W4ResolveMode@2Quire@@@Z")
#pragma comment(linker, "/include:?find_next_def@DepManager@depman@@QEAAPEAVPosition@2@AEBUSDef@2@PEAV32@@Z")
// Pragmas for methods blocked on cross-assembly gcroot mapping in MethodAudit (SafeEvent, LabeledDictionary).
// Source implementations below are exact and ready to be un-commented once MethodAudit supports cross-assembly gcroot:
// #pragma comment(linker, "/include:?SetBusy@PositionBase@depman@@QEAAX_N@Z")
// #pragma comment(linker, "/include:?WaitReady@PositionBase@depman@@QEAAXXZ")
// #pragma comment(linker, "/include:?PrepareKill@PositionBase@depman@@QEAAXXZ")
// #pragma comment(linker, "/include:?ready@PositionBase@depman@@$$FQEAMPE$AAVLabeledDictionary@utl_mgd@Quire@@XZ")
// Uncalled native member functions are stripped by /OPT:REF unless referenced.
// When referenced by callers in Depman, they will be emitted with exact IL.
// #pragma comment(linker, "/include:?previous@Position@depman@@$$FQEAAPEAV12@XZ")
// #pragma comment(linker, "/include:?next@Position@depman@@$$FQEAAPEAV12@XZ")

namespace depman {

System::Collections::ICollection^ PositionBase::GetDefs()
{
    return static_deps_;
}

std::set<Position*>& PositionBase::GetDependents()
{
    return dependents_;
}

::Quire::depman::Scope^ PositionBase::GetScope() { return scope_; }

void PositionBase::SetScope(::Quire::depman::Scope^ scope)
{
    scope_ = scope;
}

::Quire::depman::Scope^ PositionBase::GetEnvScope() { return env_scope_; }

::Quire::depman::SEnvironment^ PositionBase::env() const { return env_; }

::Quire::depman::SharedThreadContext^ PositionBase::GetSharedThreadContext() const
{
    return static_cast<::Quire::depman::SEnvironment^>(env_)->GetSharedThreadContext();
}

long PositionBase::GetHandle() const
{
    return handle_;
}

int PositionBase::GetPriority() const
{
    return priority_;
}

bool PositionBase::IsDead()
{
    return is_dead_;
}

long PositionBase::GetClientHandle() const
{
    return client_handle_;
}

int PositionBase::GetBasePriority() const
{
    return base_priority_;
}

bool PositionBase::Invalidated() const
{
    return invalidated_;
}

bool PositionBase::TreatedReady() const
{
    return treated_ready_;
}

bool PositionBase::IsLocked() const
{
    return action_chain_ && action_chain_->IsLocked();
}

bool PositionBase::IsUndefine()
{
    return action_chain_ && action_chain_->IsUndefine();
}

bool PositionBase::IsChildOnly()
{
    return action_chain_ && action_chain_->IsChildOnly();
}

bool PositionBase::IsBusy()
{
    return is_busy_;
}

bool PositionBase::IsReady()
{
    if (!action_chain_)
        return true;
    if (!is_busy_ && (action_chain_ ? action_chain_->value() : nullptr))
        return true;
    return false;
}

void PositionBase::SetBusy(bool busy)
{
    is_busy_ = busy;
    if (busy)
    {
        busy_thread_id_ = System::Threading::Thread::CurrentThread->ManagedThreadId;
        not_busy_event_->Reset();
    }
    else
    {
        not_busy_event_->Set();
    }
}

void PositionBase::StartTiming()
{
    if (timer_)
        timer_->Start();
}

void PositionBase::WaitReady()
{
    ::Quire::utl_mgd::SafeEvent^ ev = *reinterpret_cast<gcroot<::Quire::utl_mgd::SafeEvent^>*>(&ready_event_);
    if (ev != nullptr)
        ev->WaitOne();
}

void PositionBase::PrepareKill()
{
    send_messages_ = false;
    auto& ev_ref = *reinterpret_cast<gcroot<::Quire::utl_mgd::SafeEvent^>*>(&ready_event_);
    ::Quire::utl_mgd::SafeEvent^ ev = ev_ref;
    ev_ref = nullptr;
    ev->Set();
}

void PositionBase::Kill()
{
    is_dead_ = true;
    if (dparent_ != nullptr)
    {
        auto parent = dparent_;
        Position* pos = position_;
        pos->dparent_ = nullptr;
        parent->dependents_.erase(pos);
    }
}

void PositionBase::TreatAsReady(bool b)
{
    treated_ready_ = b;
    if (b)
    {
        invalidated_ = false;
        if (action_chain_ != nullptr)
        {
            __int64 chain = reinterpret_cast<__int64>(action_chain_);
            object_ = reinterpret_cast<ActionChain*>(chain)->TreatAsReadyDeps(nullptr);
        }
        if (dparent_ != nullptr)
        {
            auto parent = dparent_;
            Position* pos = position_;
            pos->dparent_ = nullptr;
            parent->dependents_.erase(pos);
        }
    }
    else
    {
        object_ = nullptr;
    }
}

void PositionBase::OnDep(Position* p)
{
    TreatAsReady(false);
    const int priority = p->priority_;
    priority_ = std::max(priority_, priority);
    if (p->dparent_ == nullptr)
    {
        dependents_.insert(p);
        p->dparent_ = position_;
    }
}

::Quire::utl_mgd::LabeledDictionary^ PositionBase::ready()
{
    if (!is_busy_ && (action_chain_ ? action_chain_->value() : nullptr))
        return *reinterpret_cast<gcroot<::Quire::utl_mgd::LabeledDictionary^>*>(&labeled_dictionary_);
    return nullptr;
}

void PositionBase::SetNotifyComputing(bool notify)
{
    notify_computing_ = notify;
}

void PositionBase::ResetPriority()
{
    priority_ = base_priority_;
}

void PositionBase::ClearDParent()
{
    dparent_ = nullptr;
}

void PositionBase::SetBasePriority(int priority)
{
    base_priority_ = priority;
    priority_ = std::max(priority_, base_priority_);
}

void PositionBase::SendMessage(::Quire::Engine::NodeStatus status)
{
    if (send_messages_)
    {
        if (pos_callback_)
            pos_callback_->Invoke(position_, status);
    }
}

void PositionBase::SetTimer(::Quire::depman::NodeTimer^ timer)
{
    timer_ = timer;
}

void PositionBase::Lock()
{
    if (action_chain_)
        action_chain_->Lock();
}

void PositionBase::UnLock()
{
    if (action_chain_)
        action_chain_->UnLock();
    invalidated_ = false;
}

void PositionBase::BeginMove()
{
}

bool PositionBase::EndMove()
{
    if (!action_chain_)
        return false;

    auto old_deps = dynamic_cast<System::Collections::Generic::Dictionary<::Quire::depman::CacheKey, __int64>^>(static_cast<System::Object^>(object_));
    auto new_deps = action_chain_->TreatAsReadyDeps(nullptr);
    if (!old_deps)
        return new_deps != nullptr;
    if (!new_deps)
        return true;

    for each (auto key in old_deps->Keys)
    {
        __int64 val;
        if (!new_deps->TryGetValue(key, val))
            return true;
        if (val != old_deps[key])
            return true;
    }
    return false;
}

Position* Position::previous()
{
    return dynamic_cast<Position*>(prev_);
}

Position* Position::next()
{
    return dynamic_cast<Position*>(next_);
}

void DepManager::depctl(int cmd)
{
    enabled_ = (cmd == 0);
}

DefMap::iterator DepManager::find_def(DefMap* defs, Position* pos, ::Quire::depman::ResolveMode mode)
{
    auto it = defs->end();
    if (mode != ::Quire::depman::ResolveMode::BEFORE)
        it = defs->find(pos);

    if (mode != ::Quire::depman::ResolveMode::AT && it == defs->end())
        it = lower_bound_ne(pos, *defs);

    return it;
}

Position* DepManager::find_def_nodead(DefMap* defs, Position* r, ::Quire::depman::ResolveMode m)
{
    if (!defs)
        return nullptr;

    auto it = find_def(defs, r, m);
    if (it != defs->end())
    {
        auto pos = it->first;
        while (pos->IsDead())
        {
            auto cur = pos;
            auto next = find_def(defs, cur, ::Quire::depman::ResolveMode::BEFORE);
            if (next == defs->end())
                return nullptr;
            pos = next->first;
        }
        return pos;
    }
    return nullptr;
}

Position* DepManager::find_next_def(const SDef& name, Position* r)
{
    auto defs = lookup(name, def_table_);
    if (defs)
    {
        auto it = defs->upper_bound(r);
        if (it != defs->end() && it->second != DefinitionType::UNDEF)
            return it->first;
    }
    return nullptr;
}

} // namespace depman
