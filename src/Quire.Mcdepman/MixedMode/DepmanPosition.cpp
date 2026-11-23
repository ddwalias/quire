#define WIN32_LEAN_AND_MEAN
#define NOMINMAX
#include <sdkddkver.h>
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
#pragma comment(linker, "/include:?SetNotifyComputing@PositionBase@depman@@QEAAX_N@Z")
#pragma comment(linker, "/include:?ResetPriority@PositionBase@depman@@QEAAXXZ")
#pragma comment(linker, "/include:?ClearDParent@PositionBase@depman@@QEAAXXZ")
#pragma comment(linker, "/include:?SetBasePriority@PositionBase@depman@@QEAAXH@Z")
#pragma comment(linker, "/include:?IsLocked@PositionBase@depman@@QEBA_NXZ")
#pragma comment(linker, "/include:?IsUndefine@PositionBase@depman@@QEAA_NXZ")
#pragma comment(linker, "/include:?IsChildOnly@PositionBase@depman@@QEAA_NXZ")
#pragma comment(linker, "/include:?GetScope@PositionBase@depman@@QEAMPE$AAVScope@2Quire@@XZ")
#pragma comment(linker, "/include:?GetEnvScope@PositionBase@depman@@QEAMPE$AAVScope@2Quire@@XZ")
#pragma comment(linker, "/include:?env@PositionBase@depman@@$$FQEBMPE$AAUSEnvironment@2Quire@@XZ")
// Uncalled native member functions are stripped by /OPT:REF unless referenced.
// When referenced by callers in Depman, they will be emitted with exact IL.
// #pragma comment(linker, "/include:?previous@Position@depman@@$$FQEAAPEAV12@XZ")
// #pragma comment(linker, "/include:?next@Position@depman@@$$FQEAAPEAV12@XZ")

namespace depman {

::Quire::depman::Scope^ PositionBase::GetScope() { return scope_; }

::Quire::depman::Scope^ PositionBase::GetEnvScope() { return env_scope_; }

::Quire::depman::SEnvironment^ PositionBase::env() const { return env_; }

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

Position* Position::previous()
{
    return dynamic_cast<Position*>(prev_);
}

Position* Position::next()
{
    return dynamic_cast<Position*>(next_);
}

} // namespace depman
