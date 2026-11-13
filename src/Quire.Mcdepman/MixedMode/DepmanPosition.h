#pragma once

class ActionChain;

template <typename T>
class OrderedList;

template <typename T>
class OrderedListNode;

namespace depman {

class DepManager;
class Position;

// Layout from the original's accessors. Size: 216 bytes (0xD8).
class PositionBase {
public:
    virtual ~PositionBase();

private:
    DepManager* dep_manager_;                      // 0x08
    Position* position_;                          // 0x10 (offset 16)
    int handle_;                                  // 0x18 (offset 24)
    unsigned char pad_1c_[0x04];                  // 0x1C
    ActionChain* action_chain_;                   // 0x20 (offset 32)
    void* env_;                                   // 0x28 (offset 40, gcroot<depman::SEnvironment^>)
    void* static_deps_;                           // 0x30 (offset 48, gcroot<System::Collections::ICollection^>)
    void* scope_;                                 // 0x38 (offset 56, gcroot<depman::Scope^>)
    void* env_scope_;                             // 0x40 (offset 64, gcroot<depman::Scope^>)
    void* labeled_dictionary_;                    // 0x48 (offset 72, gcroot<utl_mgd::LabeledDictionary^>)
    void* pos_callback_;                          // 0x50 (offset 80, gcroot<depman::PosCallback^>)
    void* pos_queue_callback_;                    // 0x58 (offset 88, gcroot<depman::PosQueueCallback^>)
    void* interrupt_callback_;                    // 0x60 (offset 96, gcroot<depman::InterruptCallback^>)
    void* treated_ready_invalid_callback_;        // 0x68 (offset 104, gcroot<depman::TreatedReadyInvalidCallback^>)
    void* object_;                                // 0x70 (offset 112, gcroot<System::Object^>)
    int client_handle_;                           // 0x78 (offset 120)
    unsigned char pad_7c_[0x04];                  // 0x7C
    void* dparent_;                               // 0x80 (offset 128)
    unsigned char dependents_[0x10];              // 0x88 (offset 136..151, std::set<depman::Position*>)
    int lock_count_;                              // 0x98 (offset 152)
    bool send_messages_;                          // 0x9C (offset 156)
    unsigned char pad_9d_[0x03];                  // 0x9D
    int base_priority_;                           // 0xA0 (offset 160)
    int priority_;                                // 0xA4 (offset 164)
    void* not_busy_event_;                        // 0xA8 (offset 168, gcroot<utl_mgd::SafeEvent^>)
    void* ready_event_;                           // 0xB0 (offset 176, gcroot<utl_mgd::SafeEvent^>)
    bool is_dead_;                                // 0xB8 (offset 184)
    unsigned char pad_b9_[0x03];                  // 0xB9
    int state_;                                   // 0xBC (offset 188)
    bool is_busy_;                                // 0xC0 (offset 192)
    bool invalidated_;                            // 0xC1 (offset 193)
    bool treated_ready_;                          // 0xC2 (offset 194)
    bool computing_;                              // 0xC3 (offset 195)
    int busy_thread_id_;                          // 0xC4 (offset 196)
    void* timer_;                                 // 0xC8 (offset 200, gcroot<depman::NodeTimer^>)
    bool notify_computing_;                       // 0xD0 (offset 208)
    unsigned char pad_d1_[0x07];                  // 0xD1
};

static_assert(sizeof(PositionBase) == 216, "PositionBase size must be 216 bytes");

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
