#pragma once

#include <gcroot.h>
#include <set>
#include "DepmanContracts.h"

template <typename T>
class OrderedList;

template <typename T>
class OrderedListNode;

namespace Quire { namespace depman {
    ref class ActionChainMgd;
    ref struct NodeTimer;
}}

namespace depman {

class ActionChain;
class DepManager;
class Position;

// Layout from the original's accessors. Size: 216 bytes (0xD8).
class PositionBase {
public:
    virtual ~PositionBase();

    long GetHandle() const;
    int GetPriority() const;
    __declspec(noinline) bool IsDead();
    bool IsLocked() const;
    __declspec(noinline) bool IsUndefine();
    bool IsChildOnly();
    long GetClientHandle() const;
    int GetBasePriority() const;
    bool Invalidated() const;
    bool TreatedReady() const;
    bool IsBusy();
    bool IsReady();
    void SetBusy(bool busy);
    void StartTiming();
    void WaitReady();
    void PrepareKill();
    void Kill();
    void TreatAsReady(bool b);
    void OnDep(Position* p);
    ::Quire::utl_mgd::LabeledDictionary^ ready();
    void SetNotifyComputing(bool notify);
    void ResetPriority();
    void ClearDParent();
    void SetBasePriority(int priority);
    void SendMessage(::Quire::Engine::NodeStatus status);
    void SetTimer(::Quire::depman::NodeTimer^ timer);
    void SetScope(::Quire::depman::Scope^ scope);
    void Lock();
    void UnLock();
    void BeginMove();
    bool EndMove();
    System::Collections::ICollection^ GetDefs();
    std::set<Position*>& GetDependents();
    ::Quire::depman::Scope^ GetScope();
    ::Quire::depman::Scope^ GetEnvScope();
    ::Quire::depman::SEnvironment^ env() const;
    ::Quire::depman::SharedThreadContext^ GetSharedThreadContext() const;

private:
    DepManager* dep_manager_;                      // 0x08
    Position* position_;                          // 0x10 (offset 16)
    int handle_;                                  // 0x18 (offset 24)
    unsigned char pad_1c_[0x04];                  // 0x1C
    ActionChain* action_chain_;                   // 0x20 (offset 32)
    gcroot<::Quire::depman::SEnvironment^> env_;    // 0x28 (offset 40)
    gcroot<System::Collections::ICollection^> static_deps_; // 0x30 (offset 48)
    gcroot<::Quire::depman::Scope^> scope_;         // 0x38 (offset 56)
    gcroot<::Quire::depman::Scope^> env_scope_;     // 0x40 (offset 64)
    void* labeled_dictionary_;                    // 0x48 (offset 72, gcroot<utl_mgd::LabeledDictionary^>)
    gcroot<::Quire::depman::PosCallback^> pos_callback_; // 0x50 (offset 80)
    void* pos_queue_callback_;                    // 0x58 (offset 88, gcroot<depman::PosQueueCallback^>)
    void* interrupt_callback_;                    // 0x60 (offset 96, gcroot<depman::InterruptCallback^>)
    void* treated_ready_invalid_callback_;        // 0x68 (offset 104, gcroot<depman::TreatedReadyInvalidCallback^>)
    gcroot<System::Object^> object_;              // 0x70 (offset 112, gcroot<System::Object^>)
    int client_handle_;                           // 0x78 (offset 120)
    unsigned char pad_7c_[0x04];                  // 0x7C
    PositionBase* dparent_;                       // 0x80 (offset 128)
    std::set<Position*> dependents_;              // 0x88 (offset 136..151, std::set<depman::Position*>)
    int lock_count_;                              // 0x98 (offset 152)
    bool send_messages_;                          // 0x9C (offset 156)
    unsigned char pad_9d_[0x03];                  // 0x9D
    int base_priority_;                           // 0xA0 (offset 160)
    int priority_;                                // 0xA4 (offset 164)
    gcroot<::Quire::utl_mgd::SafeEvent^> not_busy_event_; // 0xA8 (offset 168)
    void* ready_event_;                           // 0xB0 (offset 176, gcroot<utl_mgd::SafeEvent^>)
    bool is_dead_;                                // 0xB8 (offset 184)
    unsigned char pad_b9_[0x03];                  // 0xB9
    int state_;                                   // 0xBC (offset 188)
    bool is_busy_;                                // 0xC0 (offset 192)
    bool invalidated_;                            // 0xC1 (offset 193)
    bool treated_ready_;                          // 0xC2 (offset 194)
    bool computing_;                              // 0xC3 (offset 195)
    int busy_thread_id_;                          // 0xC4 (offset 196)
    gcroot<::Quire::depman::NodeTimer^> timer_;    // 0xC8 (offset 200)
    bool notify_computing_;                       // 0xD0 (offset 208)
    unsigned char pad_d1_[0x07];                  // 0xD1
};

static_assert(sizeof(PositionBase) == 216, "PositionBase size must be 216 bytes");

// ActionChainMgd IL accessors establish the pointer, lock flag, and dead flag offsets below.
class ActionChain {
private:
    gcroot<::Quire::depman::ActionChainMgd^> mgd_;   // 0x00 (offset 0)

public:
    bool IsLocked() const;
    bool IsUndefine();
    bool IsChildOnly();
    void Lock();
    void UnLock();
    void SetNeedsCheck();
    void Kill();
    System::Object^ value();
    System::Collections::Generic::Dictionary<::Quire::depman::CacheKey, __int64>^ TreatAsReadyDeps(System::Collections::Generic::Dictionary<::Quire::depman::CacheKey, __int64>^ d);
    ::Quire::depman::SharedThreadContext^ GetSharedThreadContext() const;

    // ActionChainMgd's reference IL reads these pointer fields at offsets 8, 16, and 24.
    DepManager* dep_manager_;                       // 0x08 (offset 8)
    Position* loc_;                                 // 0x10 (offset 16)
    ActionChain* next_;                             // 0x18 (offset 24)

private:
    bool locked_;                                    // 0x20 (offset 32)

public:
    bool is_dead_;                                   // 0x21 (offset 33)
};

static_assert(sizeof(ActionChain) == 40, "ActionChain layout must remain 40 bytes");

} // namespace depman

// Template for ordered list nodes. Size with PositionBase: 248 bytes (0xF8).
template <typename T>
class OrderedListNode : public T {
public:
    virtual ~OrderedListNode();

    OrderedList<T>* list_;                        // 0xD8 (offset 216)
    OrderedListNode<T>* prev_;                    // 0xE0 (offset 224)
    OrderedListNode<T>* next_;                    // 0xE8 (offset 232)
    double ord_val_;                              // 0xF0 (offset 240)
};

namespace depman {

// Recovered native depman::Position layout. Size: 248 bytes (0xF8).
class Position : public ::OrderedListNode<PositionBase> {
public:
    Position* previous();
    Position* next();
};

static_assert(sizeof(Position) == 248, "Position size must be 248 bytes");

} // namespace depman
