#pragma once

void __cdecl operator delete(void* ptr, unsigned __int64 size) noexcept;

namespace Quire { namespace utl {
    struct InterruptControl
    {
        bool interrupted;
        void RequestInterrupt(bool requested);
    };
}}

namespace Quire { namespace depman {

    [Quire::Mirror::MirrorOfAttribute("depman.ScopingMode, mcdepman")]
    public enum class ScopingMode
    {
        NORMAL = 0,
        CHILD_ONLY = 1,
        EXCLUDE_CHILD = 2
    };

    [Quire::Mirror::MirrorOfAttribute("depman.ResolveMode, mcdepman")]
    public enum class ResolveMode
    {
        AT = 0,
        BEFORE = 1,
        AT_OR_BEFORE = 2
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

    [Quire::Mirror::MirrorOfAttribute("depman.BooleanMediator, mcdepman")]
    public interface class BooleanMediator
    {
        void AddCallback(Quire::Engine::BooleanCallback^ b);
        void RemoveCallback(Quire::Engine::BooleanCallback^ b);
        void WaitValueTrue(int timeout);
        property bool Value { bool get(); }
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

}}
