#pragma once

void __cdecl operator delete(void* ptr, unsigned __int64 size) noexcept;

#include <map>
#include <set>
#include <string>
#include "../../Quire.Native.Utl/utl.h"

namespace Engine { namespace Unmanaged {
    template <template <typename> class Alloc = std::allocator, typename CharT = wchar_t>
    class label_set : public std::set<std::basic_string<CharT, std::char_traits<CharT>, Alloc<CharT>>, std::less<std::basic_string<CharT, std::char_traits<CharT>, Alloc<CharT>>>, Alloc<std::basic_string<CharT, std::char_traits<CharT>, Alloc<CharT>>>>
    {
    public:
        typedef std::basic_string<CharT, std::char_traits<CharT>, Alloc<CharT>> string_type;
        typedef std::set<string_type, std::less<string_type>, Alloc<string_type>> base_set;

        bool is_wild_;

        __forceinline label_set() : base_set(typename base_set::key_compare(), Alloc<string_type>()), is_wild_(false) {}
        __forceinline label_set(const label_set& other) : base_set(other), is_wild_(other.is_wild_) {}
        __forceinline ~label_set() = default;

        void add(const CharT* s, size_t len)
        {
            if (bool is_wild = (s[0] == 42 && s[1] == 0))
            {
                is_wild_ = true;
                return;
            }
            this->insert(string_type(s, len));
        }

        bool operator<(const label_set& other) const
        {
            if (other.is_wild_ && !is_wild_)
                return true;
            if (!other.is_wild_ && is_wild_)
                return false;
            return (const base_set&)*this < (const base_set&)other;
        }
    };

    struct std_label_set : public label_set<std::allocator, wchar_t> {
        __forceinline std_label_set() : label_set<std::allocator, wchar_t>() {}
        std_label_set(const std_label_set& other)
            : label_set<std::allocator, wchar_t>(other) {}
        ~std_label_set() = default;
    };
    static_assert(sizeof(std_label_set) == 24, "std_label_set size must be 24 bytes");

    namespace {
        template <typename Set>
        bool subset(const Set& a, const Set& b)
        {
            if (a.size() > b.size())
                return false;
            return std::includes(b.begin(), b.end(), a.begin(), a.end());
        }
    }

    template <typename Set>
    bool _unifies(const Set& a, const Set& b)
    {
        if (&a == &b)
            return true;
        if (a.is_wild_)
        {
            if (b.is_wild_)
                return true;
            return subset((const typename Set::base_set&)a, (const typename Set::base_set&)b);
        }
        if (b.is_wild_)
        {
            return subset((const typename Set::base_set&)b, (const typename Set::base_set&)a);
        }
        return (const typename Set::base_set&)a == (const typename Set::base_set&)b;
    }
}}

namespace Quire { namespace depman {
    [Quire::Mirror::MirrorOfAttribute("depman.ResolveMode, mcdepman")]
    public enum class ResolveMode
    {
        AT = 0,
        BEFORE = 1,
        AT_OR_BEFORE = 2
    };

    [Quire::Mirror::MirrorOfAttribute("depman.DSym, mcdepman")]
    public value struct DSym
    {
        System::String^ name;
        Quire::utl_mgd::LabelSet^ labels;

    private:
        System::String^ m_full;

    public:
        DSym(System::String^ symbol)
            : name(Quire::utl_mgd::LabelUtils::SplitName(symbol)),
              labels(Quire::utl_mgd::LabelTrie::InternLabels(symbol)) {}

        property bool wild
        {
            bool get() { return labels->Wild(); }
        }

        property System::String^ full
        {
            System::String^ get()
            {
                if (m_full == nullptr)
                    m_full = name + labels->ToString();

                return m_full;
            }
        }

        virtual int GetHashCode() override
        {
            return full->GetHashCode();
        }

        virtual bool Equals(System::Object^ other) override
        {
            DSym^ symbol = dynamic_cast<DSym^>(other);
            if (symbol == nullptr)
                return false;

            if (labels != symbol->labels)
                return false;

            // Release IL keeps the failed name comparison as the final branch.
            if (!name->Equals(symbol->name))
                return false;

            return true;
        }
    };
}}

template <typename T>
struct value_less {
    bool operator()(const T* a, const T* b) const {
        return a->ord_val_ < b->ord_val_;
    }
};

template <typename Map>
typename Map::mapped_type lookup(const typename Map::key_type& k, Map& m)
{
    auto it = m.find(k);
    if (it == m.end())
        return nullptr;
    return it->second;
}

template <typename Map>
typename Map::iterator lower_bound_ne(const typename Map::key_type& key, Map& m)
{
    if (m.empty())
        return m.end();

    auto it = m.lower_bound(key);
    if (it == m.end())
    {
        auto last = m.end();
        --last;
        return last;
    }
    if (it != m.begin())
    {
        --it;
        return it;
    }
    return m.end();
}

namespace depman {
    class ActionChain;
    class Position;

    enum DefinitionType {
        DEF = 0,
        UNDEF = 1
    };

    enum DependencyType {
        DEP_NORMAL = 0
    };

    struct SDef {
        int scope_id;
        std::wstring name;
        Engine::Unmanaged::std_label_set labels;

        SDef(int s, const std::wstring& n, const Engine::Unmanaged::std_label_set& l)
            : scope_id(s), name(n), labels(l) {}

        SDef(const SDef& other)
            : scope_id(other.scope_id), name(other.name), labels(other.labels) {}

        bool operator<(const SDef& other) const
        {
            if (scope_id < other.scope_id)
                return true;
            if (scope_id > other.scope_id)
                return false;
            if (name < other.name)
                return true;
            if (other.name < name)
                return false;
            if (labels < other.labels)
                return true;
            return false;
        }
    };

    static_assert(sizeof(SDef) == 64, "SDef size must be 64 bytes");

    typedef std::map<Position*, DefinitionType, value_less<Position>> DefMap;
    typedef std::map<SDef, DefMap*> DefTable;

    typedef std::map<Position*, std::pair<ActionChain*, DependencyType>, value_less<Position>> ForwardDepMap;
    typedef std::map<SDef, ForwardDepMap*> ForwardDepTable;

    void AddLabels(::Quire::depman::DSym ds, Engine::Unmanaged::std_label_set& lb);

    class DepManager {
    public:
        virtual ~DepManager();
        void depctl(int cmd);
        DefMap::iterator find_def(DefMap* defs, Position* pos, ::Quire::depman::ResolveMode mode);
        Position* find_def_nodead(DefMap* defs, Position* r, ::Quire::depman::ResolveMode m);
        Position* find_next_def(const SDef& name, Position* r);
        int is_bound(int scope, ::Quire::depman::DSym ds, Position* position, ::Quire::depman::ResolveMode mode);
        Position* resolve(int scope, ::Quire::depman::DSym ds, Position* position, ::Quire::depman::ResolveMode mode);

    private:
        DefTable def_table_;
        ForwardDepTable forward_deps_;
        bool enabled_;             // 0x28 (offset 40)
        unsigned char pad_29_[15]; // 0x29..0x38 (to 56 bytes)
    };

    static_assert(sizeof(DepManager) == 56, "DepManager size must be 56 bytes");
}

namespace Quire { namespace utl {
    using ::utl::InterruptControl;
}}

namespace Quire { namespace depman {

    [Quire::Mirror::MirrorOfAttribute("depman.ScopingMode, mcdepman")]
    public enum class ScopingMode
    {
        NORMAL = 0,
        CHILD_ONLY = 1,
        EXCLUDE_CHILD = 2
    };

    [Quire::Mirror::MirrorOfAttribute("depman.Scope, mcdepman")]
    public ref class Scope abstract
    {
    public:
        property Scope^ Parent { virtual Scope^ get() abstract; }
        property int Id { virtual int get() abstract; }

        virtual int GetHashCode() override
        {
            return Id;
        }

        virtual bool Equals(System::Object^ other) override
        {
            Scope^ scope = dynamic_cast<Scope^>(other);
            if (scope == nullptr)
                return false;

            return Id == scope->Id;
        }

        Scope() {}
    };

    [Quire::Mirror::MirrorOfAttribute("depman.DepNodeCallback, mcdepman")]
    public delegate void DepNodeCallback(long client_handle, long h, Quire::Engine::NodeStatus s);

    [Quire::Mirror::MirrorOfAttribute("depman.PosCallback, mcdepman")]
    delegate void PosCallback(::depman::Position* h, Quire::Engine::NodeStatus s);

    interface class Action;

    [Quire::Mirror::MirrorOfAttribute("depman.ActionResult, mcdepman")]
    public value struct ActionResult
    {
        System::Object^ result;
        Action^ next;
        System::Object^ state;

        ActionResult(Action^ action, System::Object^ actionState)
            : result(nullptr), next(action), state(actionState) {}

        ActionResult(System::Object^ data, System::Object^ actionState)
            : result(data), next(nullptr), state(actionState) {}

        System::Object^ GetResult() { return result; }
        Action^ GetNext() { return next; }
        System::Object^ GetState() { return state; }
    };

    [Quire::Mirror::MirrorOfAttribute("depman.Environment, mcdepman")]
    public interface class Environment
    {
        System::Collections::IDictionary^ Lookup(System::String^ name);
        System::Collections::IDictionary^ LookupEx(System::String^ name, ScopingMode m);
        void ExecuteInterruptibly(System::Threading::ThreadStart^ ts, bool bCoopOnly);
        int GetRegionId();
    };

    [Quire::Mirror::MirrorOfAttribute("depman.Action, mcdepman")]
    public interface class Action : System::IDisposable
    {
        ActionResult Invoke(Environment^ env, System::Object^ state);
        System::Collections::ICollection^ GetDefs();
        bool IsUndefine();
        bool IsChildOnly();
    };

    ref class DepTrackingEnv;

    ref class ActionChainMgd;
    ref class SharedThreadContext;

    [Quire::Mirror::MirrorOfAttribute("depman.CachingEnv, mcdepman")]
    ref class CachingEnv
    {
        System::Collections::Hashtable^ m_table;
        bool m_needs_check;
        DepTrackingEnv^ m_env;
        ActionChainMgd^ m_chain;

    public:
        CachingEnv(DepTrackingEnv^ e, ActionChainMgd^ c)
            : m_table(gcnew System::Collections::Hashtable()),
              m_needs_check(false),
              m_env(e),
              m_chain(c)
        {
        }

        void set_needs_check(bool b)
        {
            m_needs_check = b;
        }

        bool needs_check()
        {
            return m_needs_check;
        }
    };

    [Quire::Mirror::MirrorOfAttribute("depman.BooleanMediator, mcdepman")]
    public interface class BooleanMediator
    {
        void AddCallback(Quire::Engine::BooleanCallback^ b);
        void RemoveCallback(Quire::Engine::BooleanCallback^ b);
        void WaitValueTrue(int timeout);
        property bool Value { bool get(); }
    };

    [Quire::Mirror::MirrorOfAttribute("depman.DependencyType, mcdepman")]
    public enum class DependencyType
    {
        Static = 0,
        Dynamic = 1,
        All = 2
    };

    [Quire::Mirror::MirrorOfAttribute("depman.OnDep, mcdepman")]
    public delegate void OnDep(Scope^ scope, DSym sym, ScopingMode sm, DependencyType dt);

    [Quire::Mirror::MirrorOfAttribute("depman.DepTrackingEnv, mcdepman")]
    public ref class DepTrackingEnv
    {
        ::depman::Position* m_loc;

    public:
        DepTrackingEnv(::depman::Position* p)
            : m_loc(p)
        {
        }

        ::depman::Position* loc()
        {
            return m_loc;
        }

        [Quire::Mirror::MirrorStubAttribute]
        virtual ::depman::Position* Lookup(System::String^ name, ScopingMode scoping_mode, DependencyType deptype, OnDep^ on_dep_callback);
    };

    [Quire::Mirror::MirrorOfAttribute("depman.CacheKey, mcdepman")]
    public value struct CacheKey
    {
    public:
        DSym ds;
        ScopingMode sm;
        DependencyType dt;

        CacheKey(DSym _ds, ScopingMode _sm, DependencyType _dt)
        {
            ds = _ds;
            sm = _sm;
            dt = _dt;
        }

        property System::String^ name
        {
            System::String^ get()
            {
                return ds.full;
            }
        }

        virtual int GetHashCode() override
        {
            int symbol_hash = ds.GetHashCode();
            return (int)sm ^ (int)dt ^ symbol_hash;
        }

        virtual bool Equals(System::Object^ other) override
        {
            CacheKey^ key = dynamic_cast<CacheKey^>(other);
            if (key == nullptr)
                return false;

            return ds.Equals(key->ds) && sm == key->sm && dt == key->dt;
        }
    };

    [Quire::Mirror::MirrorOfAttribute("depman.ActionChainMgd, mcdepman")]
    ref class ActionChainMgd
    {
        ::depman::ActionChain* m_chain_umgd;
        Action^ m_action;
        CachingEnv^ m_env;
        System::Object^ m_state;
        System::Object^ m_result;
        System::Collections::Generic::Dictionary<CacheKey, System::Collections::Generic::HashSet<Scope^>^>^ m_deps;

        ::depman::DepManager* depman();
        ::depman::Position* loc();
        ::depman::ActionChain* next();

    public:
        ActionChainMgd(::depman::ActionChain* chain, Action^ action, DepTrackingEnv^ env, System::Object^ state)
            : m_chain_umgd(chain),
              m_action(action),
              m_env(gcnew CachingEnv(env, this)),
              m_state(state),
              m_deps(gcnew System::Collections::Generic::Dictionary<CacheKey, System::Collections::Generic::HashSet<Scope^>^>())
        {
        }

        Action^ action()
        {
            return m_action;
        }

        CachingEnv^ env()
        {
            return m_env;
        }

        System::Object^ state()
        {
            return m_state;
        }

        System::Object^ result()
        {
            return m_result;
        }

        bool IsDead();
        SharedThreadContext^ GetSharedThreadContext();
        System::Collections::Generic::Dictionary<CacheKey, __int64>^ TreatAsReadyDeps(System::Collections::Generic::Dictionary<CacheKey, __int64>^ d);

        void set_result(System::Object^ r)
        {
            m_result = r;
        }
    };

    [Quire::Mirror::MirrorOfAttribute("depman.DepException, mcdepman")]
    public ref class DepException abstract : System::Exception
    {
    public:
        DepException() {}
    };

    [Quire::Mirror::MirrorOfAttribute("depman.SafeAbortException, mcdepman")]
    public ref class SafeAbortException : DepException
    {
    private:
        SafeAbortException() {}

    public:
        static SafeAbortException^ Instance()
        {
            return gcnew SafeAbortException();
        }
    };

    [Quire::Mirror::MirrorOfAttribute("depman.ThreadData, mcdepman")]
    ref class ThreadData
    {
    public:
        System::Threading::Thread^ interruptible_task;
        bool coop_interrupt_only;
        Quire::utl_mgd::InterruptControl^ interrupt_control;
        Quire::utl::InterruptControl* interrupt_control_umgd;
        bool interrupt_requested;
        Quire::utl_mgd::SafeMutex^ interrupt_mutex;

        ThreadData()
            : interrupt_control(gcnew Quire::utl_mgd::InterruptControl()),
              interrupt_control_umgd(new Quire::utl::InterruptControl()),
              interrupt_mutex(gcnew Quire::utl_mgd::SafeMutex("ThreadData::interrupt_mutex"))
        {
        }

        ~ThreadData()
        {
            ::operator delete(interrupt_control_umgd, sizeof(Quire::utl::InterruptControl));
            System::GC::KeepAlive(this);
        }

        !ThreadData()
        {
            ::operator delete(interrupt_control_umgd, sizeof(Quire::utl::InterruptControl));
            System::GC::KeepAlive(this);
        }
    };

    [Quire::Mirror::MirrorOfAttribute("depman.SharedThreadContext, mcdepman")]
    ref class SharedThreadContext
    {
    private:
        System::Collections::Hashtable^ m_thread_map;
        Quire::utl_mgd::SafeMutex^ m_mutex;
        Quire::utl_mgd::SafeMutex^ m_data_mutex;
        bool m_abort_requested;
        Quire::Mcd::RuntimeOptions^ m_runtime_options;
        int m_worker_stack_size;

        SharedThreadContext^ GetSharedThreadContext() { return this; }

        void check_interrupt()
        {
            ThreadData^ threadData = get_thread_data(System::Threading::Thread::CurrentThread->ManagedThreadId);
            System::GC::KeepAlive(this);
            if (threadData->interrupt_requested)
                throw SafeAbortException::Instance();

            System::GC::KeepAlive(this);
        }

        ThreadData^ get_thread_data()
        {
            ThreadData^ result = get_thread_data(System::Threading::Thread::CurrentThread->ManagedThreadId);
            System::GC::KeepAlive(this);
            return result;
        }

        ThreadData^ get_thread_data(int threadId)
        {
            m_data_mutex->WaitOne();
            try
            {
                return dynamic_cast<ThreadData^>(m_thread_map[threadId]);
            }
            finally
            {
                m_data_mutex->ReleaseMutex();
            }
        }

        void clear_thread_data(int threadId)
        {
            m_data_mutex->WaitOne();
            try
            {
                m_thread_map->Remove(threadId);
            }
            finally
            {
                m_data_mutex->ReleaseMutex();
            }
            System::GC::KeepAlive(this);
        }

    public:
        SharedThreadContext(int workerStackSize)
            : m_thread_map(gcnew System::Collections::Hashtable()),
              m_mutex(gcnew Quire::utl_mgd::SafeMutex("DependencyMutex")),
              m_data_mutex(gcnew Quire::utl_mgd::SafeMutex("ThreadDataMutex")),
              m_runtime_options(gcnew Quire::Mcd::RuntimeOptions()),
              m_worker_stack_size(workerStackSize)
        {
            ++Quire::Mcd::RuntimeOptions::s_ro_counter;
        }

        property Quire::utl_mgd::SafeMutex^ Mutex
        {
            Quire::utl_mgd::SafeMutex^ get() { return m_mutex; }
        }

        property Quire::Mcd::RuntimeOptions^ RuntimeOptions
        {
            Quire::Mcd::RuntimeOptions^ get() { return m_runtime_options; }
        }

        void ObtainMutex(bool bCheckInterrupt)
        {
            m_mutex->WaitOne();
            if (bCheckInterrupt)
                check_interrupt();
        }

        void ReleaseMutex(bool bCheckInterrupt)
        {
            m_mutex->EnsureReleased(1);
            if (bCheckInterrupt)
                check_interrupt();
        }

        void CheckInterrupt()
        {
            check_interrupt();
        }

        void RegisterThread(int threadId)
        {
            ThreadData^ value = gcnew ThreadData();
            m_thread_map[threadId] = value;
            System::GC::KeepAlive(this);
        }

        void UnregisterThread(int threadId)
        {
            clear_thread_data(threadId);
        }

        void SyncToThread()
        {
            m_runtime_options->SyncToThread();
            System::GC::KeepAlive(this);
        }

        void RemoveUnitSystem()
        {
            m_runtime_options->RemoveUnitSystem();
            System::GC::KeepAlive(this);
        }
    };

    [Quire::Mirror::MirrorOfAttribute("depman.SEnvironment, mcdepman")]
    interface class SEnvironment
    {
        ::depman::Position* Lookup(Scope^ s, DSym sym);
        SharedThreadContext^ GetSharedThreadContext();
        int GetRegionId();
    };

}}
