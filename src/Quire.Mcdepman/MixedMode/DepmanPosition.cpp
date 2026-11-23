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
#pragma comment(linker, "/include:?SetTimer@PositionBase@depman@@$$FQEAMXPE$AAVNodeTimer@2Quire@@@Z")
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
    return *reinterpret_cast<std::set<Position*>*>(dependents_);
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

Position* Position::previous()
{
    return dynamic_cast<Position*>(prev_);
}

Position* Position::next()
{
    return dynamic_cast<Position*>(next_);
}

} // namespace depman
