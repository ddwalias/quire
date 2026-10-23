#define WIN32_LEAN_AND_MEAN
#define NOMINMAX
#include <sdkddkver.h>
#if defined(_M_AMD64) && !defined(_AMD64_)
#define _AMD64_
#endif
#include <minwindef.h>
#include <winnt.h>
#include <minwinbase.h>
#include <handleapi.h>
#include <synchapi.h>
#ifndef INFINITE
#define INFINITE 0xffffffffu
#endif
#ifndef WAIT_OBJECT_0
#define WAIT_OBJECT_0 0u
#endif
#ifndef WAIT_TIMEOUT
#define WAIT_TIMEOUT 258u
#endif
#include <cstdlib>
#include <iostream>
#include <new>
#include <ostream>
#include <string>
#include <vcclr.h>
#ifdef IsLoggingEnabled
#undef IsLoggingEnabled
#endif

using namespace System;
using namespace System::Collections;
using namespace System::Diagnostics;
using namespace System::Reflection;
using namespace System::Runtime::InteropServices;
using namespace System::Text;
using namespace System::Threading;

namespace ms
{
    template <typename T>
    struct Range
    {
        T* first;
        T* last;
    };
}

template <typename T>
class Substring : public ms::Range<T>
{
public:
    Substring(T* p, size_t n) : ms::Range<T>{p, p + n} {}
    Substring(T* s) : Substring(s, std::char_traits<std::remove_const_t<T>>::length(s)) {}
    Substring(const std::basic_string<std::remove_const_t<T>>& s) : Substring{s.c_str(), s.size()} {}
    Substring(const ms::Range<T>& r) : ms::Range<T>(r) {}

    T* begin() const { return this->first; }
    T* end() const { return this->last; }
    size_t size() const { return this->last - this->first; }
};

namespace ms
{
    // Rabin-Karp search with a shift-and-add rolling hash.
    template <typename T>
    T* str_str(Substring<T> ss, Substring<T> pp)
    {
        if (pp.size() <= ss.size())
        {
            size_t n = pp.size();
            unsigned msb = (n <= 32) << (n - 1);
            unsigned hs = 0, hp = 0;
            T* s = ss.begin();
            T* e = s;
            for (T* p = pp.begin(); p != pp.end(); ++e, ++p)
            {
                hs = hs * 2 + *e;
                hp = hp * 2 + *p;
            }
            for (;;)
            {
                if (hs == hp && std::char_traits<std::remove_const_t<T>>::compare(pp.begin(), s, n) == 0)
                    return s;
                if (e == ss.end())
                    break;
                hs -= *s++ * msb;
                hs = hs * 2 + *e++;
            }
        }
        return nullptr;
    }

    template <typename C>
    std::basic_string<C> str_rpl(Substring<const C> ss, Substring<const C> tt, Substring<const C> rr)
    {
        std::basic_string<C> result;
        result.reserve(ss.size());
        while (const C* p = str_str(ss, tt))
        {
            result.append(ss.begin(), p);
            result.append(rr.begin(), rr.end());
            ss = Range<const C>{p + tt.size(), ss.end()};
        }
        result.append(ss.begin(), ss.end());
        return result;
    }
}

std::wstring ToWStrng(Object^ o)
{
    pin_ptr<const wchar_t> p = PtrToStringChars(o->ToString());
    return std::wstring(p);
}

std::wstring strrpl(Substring<const wchar_t> s, Substring<const wchar_t> t, Substring<const wchar_t> r)
{
    return ms::str_rpl(s, t, r);
}

namespace utl
{
    std::string ToStrng(Object^ o)
    {
        if (o == nullptr)
            return "<null>";
        String^ s = o->ToString();
        if (s->Length > 0)
        {
            pin_ptr<Byte> p = &Encoding::UTF8->GetBytes(s)[0];
            return reinterpret_cast<const char*>(p);
        }
        return "";
    }

    __declspec(dllimport) std::ostream& __cdecl Log(Substring<const char> name);

    __declspec(dllimport) Boolean __cdecl IsLoggingEnabled();
}

[assembly:AssemblyVersionAttribute("0.1.0.0")];

namespace Quire { namespace utl_mgd {

    [Quire::Mirror::MirrorOfAttribute("utl_mgd.SafeWaitHandle, utl-mgd")]
    public ref class SafeWaitHandle abstract : IDisposable
    {
    protected:
        String^ m_name;
        void* m_handle;

        SafeWaitHandle(String^ name, void* handle) : m_name(name), m_handle(handle) {}

    public:
        void WaitOne()
        {
            ::WaitForSingleObject(static_cast<HANDLE>(m_handle), INFINITE);
            GC::KeepAlive(this);
        }

        Boolean WaitOneWithTimeout(Int32 timeout)
        {
            if (timeout == -1)
            {
                ::WaitForSingleObject(static_cast<HANDLE>(m_handle), INFINITE);
                GC::KeepAlive(this);
                return true;
            }

            Boolean result = ::WaitForSingleObject(static_cast<HANDLE>(m_handle), static_cast<DWORD>(timeout)) != WAIT_TIMEOUT;
            GC::KeepAlive(this);
            return result;
        }

        Boolean WaitTwoWithTimeout(void* other)
        {
            if (other == reinterpret_cast<void*>(-1))
            {
                ::WaitForSingleObject(static_cast<HANDLE>(m_handle), INFINITE);
                GC::KeepAlive(this);
                return true;
            }

            HANDLE handles[] = { static_cast<HANDLE>(m_handle), static_cast<HANDLE>(other) };
            DWORD result = ::WaitForMultipleObjects(2, handles, FALSE, INFINITE);
            if (result == WAIT_OBJECT_0)
            {
                GC::KeepAlive(this);
                return true;
            }
            return false;
        }

        void Free()
        {
            delete this;
            GC::KeepAlive(this);
        }

    public:
        void* GetUnmanagedHandle() { return m_handle; }
        virtual String^ ToString() override { return m_name; }

        __declspec(noinline) ~SafeWaitHandle()
        {
            ::CloseHandle(static_cast<HANDLE>(m_handle));
            GC::KeepAlive(this);
        }

        !SafeWaitHandle()
        {
            ::CloseHandle(static_cast<HANDLE>(m_handle));
            GC::KeepAlive(this);
        }
    };

    [Quire::Mirror::MirrorOfAttribute("utl_mgd.SafeEvent, utl-mgd")]
    public ref class SafeEvent : SafeWaitHandle
    {
    public:
        SafeEvent(String^ name)
            : SafeWaitHandle(String::Format("SafeEvent {0}", name), ::CreateEventW(nullptr, TRUE, FALSE, nullptr)) {}

        void Set()
        {
            ::SetEvent(static_cast<HANDLE>(m_handle));
            GC::KeepAlive(this);
        }

        void Reset()
        {
            ::ResetEvent(static_cast<HANDLE>(m_handle));
            GC::KeepAlive(this);
        }
    };

    [Quire::Mirror::MirrorOfAttribute("utl_mgd.SafeMutex, utl-mgd")]
    public ref class SafeMutex : SafeWaitHandle
    {
    public:
        SafeMutex(String^ name, Int32)
            : SafeWaitHandle(String::Format("SafeMutex {0}", name),
                ::CreateMutexW(nullptr, FALSE, (std::wstring(L"Global\\") + strrpl(ToWStrng(name), L"\\", L"_")).c_str())) {}

        SafeMutex(String^ name)
            : SafeWaitHandle(String::Format("SafeMutex {0}", name), ::CreateMutexW(nullptr, FALSE, nullptr)) {}

        void ReleaseMutex()
        {
            ::ReleaseMutex(static_cast<HANDLE>(m_handle));
            GC::KeepAlive(this);
        }

        void EnsureReleased(UInt32 max_tries)
        {
            UInt32 count = 0;
            if (0 == max_tries)
                goto done;
            do
            {
                if (!::ReleaseMutex(static_cast<HANDLE>(m_handle)))
                    break;
                ++count;
            }
            while (count != max_tries);
        done:
            GC::KeepAlive(this);
        }

        void EnsureReleased()
        {
            UInt32 count = 0;
            do
            {
                if (!::ReleaseMutex(static_cast<HANDLE>(m_handle)))
                    break;
                ++count;
            }
            while (count != 1000);
            GC::KeepAlive(this);
        }

    public:
        __declspec(noinline) ~SafeMutex() { EnsureReleased(); }
        __declspec(noinline) !SafeMutex() { EnsureReleased(); }
    };

    [Quire::Mirror::MirrorOfAttribute("utl_mgd.Logging, utl-mgd")]
    public ref class Logging
    {
    public:
        Logging() {}

        static Boolean IsLoggingEnabled()
        {
            return utl::IsLoggingEnabled();
        }

        static void Log(String^ logname, String^ message)
        {
            utl::Log(utl::ToStrng(logname)) << utl::ToStrng(message);
        }

        static void LogLine(String^ logname, String^ message)
        {
            utl::Log(utl::ToStrng(logname)) << utl::ToStrng(message) << std::endl;
        }

    };

    generic <typename T>
    [Quire::Mirror::MirrorOfAttribute("utl_mgd.Mailbox`1, utl-mgd")]
    public ref class Mailbox
    {
    private:
        SafeMutex^ m_mutex = gcnew SafeMutex("mailbox mutex");
        SafeEvent^ m_nonempty = gcnew SafeEvent("mailbox non-empty");
        System::Collections::Generic::Queue<T>^ m_queue = gcnew System::Collections::Generic::Queue<T>();

    public:
        Mailbox()
        {
            m_nonempty->Reset();
            GC::KeepAlive(this);
        }

        T GetMessage()
        {
            T msg;
            GetMessage(msg, reinterpret_cast<void*>(-1));
            return msg;
        }

        Boolean GetMessage(T% msg, void* timeout)
        {
            for (;;)
            {
                m_mutex->WaitOne();
                try
                {
                    if (m_queue->Count > 0)
                    {
                        msg = m_queue->Dequeue();
                        if (m_queue->Count == 0) m_nonempty->Reset();
                        return true;
                    }
                }
                finally { m_mutex->EnsureReleased(1); }
                if (!m_nonempty->WaitTwoWithTimeout(timeout))
                    return false;
            }
        }


        void PutMessage(T message)
        {
            m_mutex->WaitOne();
            try { m_queue->Enqueue(message); m_nonempty->Set(); }
            finally { m_mutex->EnsureReleased(1); }
            GC::KeepAlive(this);
        }

        Int32 Count()
        {
            m_mutex->WaitOne();
            try { return m_queue->Count; }
            finally { m_mutex->EnsureReleased(1); }
        }
    };

    generic <typename Owner, typename Data, typename Msg>
    where Owner : ref class
    [Quire::Mirror::MirrorOfAttribute("utl_mgd.MessageQueue`3, utl-mgd")]
    public ref class MessageQueue : System::Collections::Queue, IDisposable
    {
    private:
        Mailbox<Msg>^ m_mailbox;
        Thread^ m_thread;
        Int32 m_stack_size;
        void* m_death_handle;

    public:
        [Quire::Mirror::MirrorOfAttribute("utl_mgd.MessageQueue`3+OnInit, utl-mgd")]
        delegate void OnInit(Owner owner, Data data);

        [Quire::Mirror::MirrorOfAttribute("utl_mgd.MessageQueue`3+OnMessage, utl-mgd")]
        delegate Boolean OnMessage(Owner owner, Data data, Msg message);

    private:
        static void MsgLoop(Object^ obj)
        {
            System::Tuple<WeakReference^, Data, Mailbox<Msg>^, OnInit^, OnMessage^, Int64>^ args =
                dynamic_cast<System::Tuple<WeakReference^, Data, Mailbox<Msg>^, OnInit^, OnMessage^, Int64>^>(obj);
            WeakReference^ owner = args->Item1;
            Data data = args->Item2;
            Mailbox<Msg>^ mailbox = args->Item3;
            OnInit^ oi = args->Item4;
            OnMessage^ om = args->Item5;
            void* death_handle = (void*)args->Item6;
            if (oi != nullptr)
            {
                Object^ target = owner->Target;
                if (target != nullptr)
                    oi((Owner)target, data);
            }

            Msg msg;
            Object^ target;
            bool more;
            do
            {
                msg = Msg();
                if (!mailbox->GetMessage(msg, death_handle))
                    break;
                target = owner->Target;
                if (target == nullptr)
                    break;
                more = om((Owner)target, data, msg);
            }
            while (more);

            if (death_handle != (void*)-1)
                ::CloseHandle(death_handle);
        }

        void Init(Int32 stack_size)
        {
            m_mailbox = gcnew Mailbox<Msg>();
            m_death_handle = (void*)-1;
            Thread^ thread = gcnew Thread(gcnew ParameterizedThreadStart(&MessageQueue::MsgLoop), stack_size);
            thread->IsBackground = true;
            m_thread = thread;
        }

    public:
        MessageQueue(Owner owner, Data data, OnInit^ onInit, OnMessage^ onMessage)
        {
            Init(1048576); Start(owner, data, onInit, onMessage, true);
        }

        MessageQueue(Owner owner, Data data, OnInit^ onInit, OnMessage^ onMessage, Int32 stack_size)
        {
            Init(stack_size); Start(owner, data, onInit, onMessage, true);
        }

        MessageQueue(Int32 stack_size) { Init(stack_size); }

    public:
        void Start(Owner owner, Data data, OnInit^ onInit, OnMessage^ onMessage, Boolean stopThreadOnFinalize)
        {
            if (m_thread->Name == nullptr)
                m_thread->Name = String::Concat(String::Concat("MsgLoop(", owner->ToString()), ")");
            if (stopThreadOnFinalize)
                m_death_handle = ::CreateEventW(nullptr, TRUE, FALSE, nullptr);

            m_thread->Start(System::Tuple::Create<WeakReference^, Data, Mailbox<Msg>^, OnInit^, OnMessage^, Int64>(
                gcnew WeakReference(owner), data, m_mailbox, onInit, onMessage, (Int64)m_death_handle));
            GC::KeepAlive(this);
        }

        void Stop()
        {
            if (m_death_handle != reinterpret_cast<void*>(-1))
            {
                ::SetEvent(static_cast<HANDLE>(m_death_handle));
                m_death_handle = reinterpret_cast<void*>(-1);
            }
        }

        Mailbox<Msg>^ GetMailbox() { return m_mailbox; }
        Thread^ GetThread() { return m_thread; }

        ~MessageQueue() { this->!MessageQueue(); }
        !MessageQueue() { Stop(); }
    };

}}

namespace Quire { namespace utl_mgd {

    [Quire::Mirror::MirrorOfAttribute("utl_mgd.InterruptControl, utl-mgd")]
    public ref class InterruptControl
    {
    private:
        [ThreadStatic]
        static InterruptControl^ s_interrupt_control;
        SafeMutex^ m_request_mutex = gcnew SafeMutex("InterruptControl request mutex");
        SafeMutex^ m_abort_mutex = gcnew SafeMutex("InterruptControl abort mutex");
        Boolean m_interrupt_requested;
        CancellationTokenSource^ m_cts = gcnew CancellationTokenSource();

    public:
        property CancellationTokenSource^ Cts
        {
            CancellationTokenSource^ get() { return m_cts; }
        }

        void BeginRequestInterrupt()
        {
            m_request_mutex->WaitOne();
            m_abort_mutex->WaitOne();
            m_interrupt_requested = true;
            m_request_mutex->ReleaseMutex();
            GC::KeepAlive(this);
        }

        void EndRequestInterrupt()
        {
            m_abort_mutex->ReleaseMutex();
            m_request_mutex->WaitOne();
            m_interrupt_requested = false;
            m_request_mutex->ReleaseMutex();
            GC::KeepAlive(this);
        }

        static void ThreadInitialize(InterruptControl^ control)
        {
            s_interrupt_control = control;
        }

        static void ProhibitInterruption()
        {
            InterruptControl^ control = s_interrupt_control;
            if (control == nullptr)
                return;

            try
            {
                control->m_request_mutex->WaitOne();
                if (control->m_interrupt_requested)
                {
                    control->m_request_mutex->ReleaseMutex();
                    throw gcnew Exception("abort current thread");
                }

                control->m_abort_mutex->WaitOne();
            }
            finally
            {
                control->m_request_mutex->EnsureReleased(1);
            }
        }

        static void AllowInterruption()
        {
            InterruptControl^ control = s_interrupt_control;
            if (control != nullptr)
                control->m_abort_mutex->EnsureReleased(1);
        }

        static Int32 ProhibitInterruption_(Int32)
        {
            ProhibitInterruption();
            return 0;
        }

        static Int32 AllowInterruption_(Int32)
        {
            InterruptControl^ control = s_interrupt_control;
            if (control != nullptr)
                control->m_abort_mutex->EnsureReleased(1);
            return 0;
        }
    };
}}

namespace Quire { namespace utl_mgd {

    [Quire::Mirror::MirrorOfAttribute("utl_mgd.SafeReadWriteLock, utl-mgd")]
    public ref class SafeReadWriteLock
    {
    private:
        SafeMutex^ m_write_lock = gcnew SafeMutex("SafeReadWriteLock:write");
        SafeMutex^ m_reader_count_lock = gcnew SafeMutex("SafeReadWriteLock:rcount");
        Int32 m_reader_count = 0;
        SafeEvent^ m_no_readers = gcnew SafeEvent("SafeReadWriteLock:no readers");

    public:
        SafeReadWriteLock()
        {
            m_no_readers->Set();
            GC::KeepAlive(this);
        }

        void WaitReadLock()
        {
            m_write_lock->WaitOne();
            m_reader_count_lock->WaitOne();
            try
            {
                Int32 count = ++m_reader_count;
                if (count == 1)
                    m_no_readers->Reset();
            }
            finally
            {
                m_reader_count_lock->ReleaseMutex();
                m_write_lock->ReleaseMutex();
            }
            GC::KeepAlive(this);
        }

        void ReleaseReadLock()
        {
            m_reader_count_lock->WaitOne();
            try
            {
                Int32 count = m_reader_count + -1;
                m_reader_count = count;
                if (count == 0)
                    m_no_readers->Set();
            }
            finally
            {
                m_reader_count_lock->ReleaseMutex();
            }
            GC::KeepAlive(this);
        }

        void WaitWriteLock()
        {
            m_write_lock->WaitOne();
            m_no_readers->WaitOne();
            GC::KeepAlive(this);
        }

        void ReleaseWriteLock()
        {
            m_write_lock->ReleaseMutex();
            GC::KeepAlive(this);
        }
    };
}}

namespace Quire { namespace utl_mgd {

    [Quire::Mirror::MirrorOfAttribute("utl_mgd.SortedStringSet, utl-mgd")]
    public ref class SortedStringSet : IEnumerable
    {
    private:
        SortedList^ m_strings;

    public:
        SortedStringSet(array<String^>^ strings) : m_strings(gcnew SortedList())
        {
            Int32 num = 0;
            if (0 < strings->Length)
            {
                do
                {
                    m_strings->Add(strings[num], true);
                    num++;
                }
                while (num < strings->Length);
            }
            GC::KeepAlive(this);
        }

        SortedStringSet(SortedStringSet^ other) : m_strings(gcnew SortedList(other->m_strings))
        {
        }

        SortedStringSet() : m_strings(gcnew SortedList())
        {
        }

        property Int32 Count
        {
            Int32 get()
            {
                Int32 count = m_strings->Count;
                GC::KeepAlive(this);
                return count;
            }
        }

        virtual IEnumerator^ GetEnumerator()
        {
            IEnumerator^ enumerator = m_strings->Keys->GetEnumerator();
            GC::KeepAlive(this);
            return enumerator;
        }

        Boolean IsSubsetOf(SortedStringSet^ other)
        {
            IEnumerator^ enumerator = m_strings->Keys->GetEnumerator();
            try
            {
                while (enumerator->MoveNext())
                {
                    String^ key = safe_cast<String^>(enumerator->Current);
                    if (!other->m_strings->Contains(key))
                    {
                        return false;
                    }
                }
            }
            finally
            {
                IDisposable^ disposable = dynamic_cast<IDisposable^>(enumerator);
                if (disposable != nullptr)
                    delete disposable;
            }

            GC::KeepAlive(this);
            return true;
        }

        Boolean SetEquals(SortedStringSet^ other)
        {
            Int32 count = m_strings->Count;
            GC::KeepAlive(this);
            Int32 count2 = other->m_strings->Count;
            GC::KeepAlive(other);
            if (count != count2)
            {
                GC::KeepAlive(this);
                return false;
            }
            return IsSubsetOf(other);
        }

        void CopyTo(array<String^>^ a)
        {
            m_strings->Keys->CopyTo(a, 0);
            GC::KeepAlive(this);
        }

        void Add(String^ s)
        {
            m_strings->Add(s, true);
            GC::KeepAlive(this);
        }

        Boolean Contains(String^ s)
        {
            Boolean result = m_strings->Contains(s);
            GC::KeepAlive(this);
            return result;
        }
    };
}}

namespace Quire { namespace utl_mgd {

    [Quire::Mirror::MirrorOfAttribute("utl_mgd.Function, utl-mgd")]
    public interface class Function
    {
        Object^ Invoke(array<Object^>^ args);
        Object^ Invoke0();
        Object^ Invoke1(Object^ a0);
        Object^ Invoke2(Object^ a0, Object^ a1);
        Object^ Invoke3(Object^ a0, Object^ a1, Object^ a2);
        Object^ Invoke4(Object^ a0, Object^ a1, Object^ a2, Object^ a3);
        Object^ Invoke5(Object^ a0, Object^ a1, Object^ a2, Object^ a3, Object^ a4);
        Object^ Invoke6(Object^ a0, Object^ a1, Object^ a2, Object^ a3, Object^ a4, Object^ a5);
        Object^ Invoke7(Object^ a0, Object^ a1, Object^ a2, Object^ a3, Object^ a4, Object^ a5, Object^ a6);
    };

    [Serializable]
    [Quire::Mirror::MirrorOfAttribute("utl_mgd.FunctionBase, utl-mgd")]
    public ref class FunctionBase abstract : Function
    {
    public:
        virtual Object^ Invoke(array<Object^>^ args) abstract;

        FunctionBase() {}

        virtual Object^ Invoke0() { return Invoke(nullptr); }

        virtual Object^ Invoke1(Object^ a0)
        {
            array<Object^>^ args = gcnew array<Object^>(1);
            args[0] = a0;
            return Invoke(args);
        }

        virtual Object^ Invoke2(Object^ a0, Object^ a1)
        {
            array<Object^>^ args = gcnew array<Object^>(2);
            args[0] = a0;
            args[1] = a1;
            return Invoke(args);
        }

        virtual Object^ Invoke3(Object^ a0, Object^ a1, Object^ a2)
        {
            array<Object^>^ args = gcnew array<Object^>(3);
            args[0] = a0;
            args[1] = a1;
            args[2] = a2;
            return Invoke(args);
        }

        virtual Object^ Invoke4(Object^ a0, Object^ a1, Object^ a2, Object^ a3)
        {
            array<Object^>^ args = gcnew array<Object^>(4);
            args[0] = a0;
            args[1] = a1;
            args[2] = a2;
            args[3] = a3;
            return Invoke(args);
        }

        virtual Object^ Invoke5(Object^ a0, Object^ a1, Object^ a2, Object^ a3, Object^ a4)
        {
            array<Object^>^ args = gcnew array<Object^>(5);
            args[0] = a0;
            args[1] = a1;
            args[2] = a2;
            args[3] = a3;
            args[4] = a4;
            return Invoke(args);
        }

        virtual Object^ Invoke6(Object^ a0, Object^ a1, Object^ a2, Object^ a3, Object^ a4, Object^ a5)
        {
            array<Object^>^ args = gcnew array<Object^>(6);
            args[0] = a0;
            args[1] = a1;
            args[2] = a2;
            args[3] = a3;
            args[4] = a4;
            args[5] = a5;
            return Invoke(args);
        }

        virtual Object^ Invoke7(Object^ a0, Object^ a1, Object^ a2, Object^ a3, Object^ a4, Object^ a5, Object^ a6)
        {
            array<Object^>^ args = gcnew array<Object^>(7);
            args[0] = a0;
            args[1] = a1;
            args[2] = a2;
            args[3] = a3;
            args[4] = a4;
            args[5] = a5;
            args[6] = a6;
            return Invoke(args);
        }
    };

    [Quire::Mirror::MirrorOfAttribute("utl_mgd.TaskQueue, utl-mgd")]
    public ref class TaskQueue : MessageQueue<TaskQueue^, Int32, Function^>
    {
    private:
        SafeMutex^ m_empty_mutex = gcnew SafeMutex("task queue empty mutex");
        SafeEvent^ m_empty = gcnew SafeEvent("task queue empty");

        static Boolean ExecuteTask(TaskQueue^ queue, Int32, Function^ function)
        {
            function->Invoke0(); queue->OnTaskExecuted(); return true;
        }

    protected:
        void OnTaskExecuted()
        {
            m_empty_mutex->WaitOne();
            try { if (GetMailbox()->Count() == 0) m_empty->Set(); }
            finally { m_empty_mutex->EnsureReleased(1); }
            GC::KeepAlive(this);
        }

    public:
        TaskQueue() : MessageQueue<TaskQueue^, Int32, Function^>(this, 0, nullptr,
            gcnew MessageQueue<TaskQueue^, Int32, Function^>::OnMessage(&TaskQueue::ExecuteTask))
        {
            m_empty->Set(); GC::KeepAlive(this);
        }

        void EnqueueTask(Function^ function)
        {
            m_empty_mutex->WaitOne();
            try { m_empty->Reset(); GetMailbox()->PutMessage(function); }
            finally { m_empty_mutex->EnsureReleased(1); }
            GC::KeepAlive(this);
        }

        void WaitEmpty() { m_empty->WaitOne(); GC::KeepAlive(this); }
        Int32 Count() { Int32 count = GetMailbox()->Count(); GC::KeepAlive(this); return count; }
    };

    generic <typename T>
    [Quire::Mirror::MirrorOfAttribute("utl_mgd.UnaryVoidWrapper`1, utl-mgd")]
    public ref class UnaryVoidWrapper
    {
    private:
        Function^ m_f;

    public:
        UnaryVoidWrapper(Function^ f) : m_f(f) {}

        void Invoke(T a1)
        {
            m_f->Invoke1(a1);
            GC::KeepAlive(this);
        }
    };
}}

namespace Quire { namespace utl_mgd {

    [Quire::Mirror::MirrorOfAttribute("utl_mgd.LabelSet, utl-mgd")]
    public ref class LabelSet
    {
    private:
        SortedStringSet^ m_labels;
        Boolean m_wild;
        String^ m_cached_name;

    public:
        LabelSet(SortedStringSet^ labels, Boolean wild)
            : m_labels(labels), m_wild(wild) {}

        Boolean Unifies(LabelSet^ other)
        {
            if (this == other)
                return true;

            if (m_wild)
            {
                if (other->m_wild)
                    return true;
                Boolean result = m_labels->IsSubsetOf(other->m_labels);
                GC::KeepAlive(this);
                return result;
            }

            if (other->m_wild)
            {
                Boolean result2 = other->m_labels->IsSubsetOf(m_labels);
                GC::KeepAlive(this);
                return result2;
            }

            Boolean result3 = m_labels->SetEquals(other->m_labels);
            GC::KeepAlive(this);
            return result3;
        }

        static Boolean _unifies(LabelSet^ a, LabelSet^ b)
        {
            return a->Unifies(b);
        }

        SortedStringSet^ Labels() { return m_labels; }

        Boolean Wild() { return m_wild; }

        Boolean Empty()
        {
            return m_labels->Count == 0 && !m_wild;
        }

        virtual String^ ToString() override
        {
            if (m_cached_name == nullptr)
            {
                StringBuilder^ stringBuilder = gcnew StringBuilder();
                stringBuilder->Append(static_cast<Char>('{'));
                EmitLabels(stringBuilder, ',');
                stringBuilder->Append(static_cast<Char>('}'));
                m_cached_name = stringBuilder->ToString();
            }

            GC::KeepAlive(this);
            return m_cached_name;
        }

        void EmitLabels(StringBuilder^ builder, Char delimiter)
        {
            Boolean flag = true;
            IEnumerator^ enumerator = m_labels->GetEnumerator();
            try
            {
                while (enumerator->MoveNext())
                {
                    String^ label = safe_cast<String^>(enumerator->Current);
                    if (!flag)
                    {
                        builder->Append(delimiter);
                    }
                    else
                    {
                        flag = false;
                    }

                    builder->Append(label);
                }
            }
            finally
            {
                IDisposable^ disposable = dynamic_cast<IDisposable^>(enumerator);
                if (disposable != nullptr)
                    delete disposable;
            }

            if (m_wild)
            {
                if (!flag)
                    builder->Append(delimiter);
                builder->Append(static_cast<Char>('*'));
            }

            GC::KeepAlive(this);
        }

        array<String^>^ ToVector()
        {
            array<String^>^ result = gcnew array<String^>(m_labels->Count);
            m_labels->CopyTo(result);
            GC::KeepAlive(this);
            return result;
        }
    };
}}

namespace Quire { namespace utl_mgd {

    [Quire::Mirror::MirrorOfAttribute("utl_mgd.LabelTrie, utl-mgd")]
    public ref class LabelTrie
    {
    public:
        SortedStringSet^ m_labels;
        LabelSet^ m_wild;
        LabelSet^ m_non_wild;
        System::Collections::Generic::Dictionary<String^, LabelTrie^>^ m_map;
        // Static initializers, not a static constructor: a .cctor the author wrote would drop Mathcad's beforefieldinit.
        static LabelTrie^ s_root = gcnew LabelTrie(gcnew SortedStringSet());
        static SafeReadWriteLock^ s_lock = gcnew SafeReadWriteLock();

    private:
        static array<Char>^ s_braces = (static_cast<String^>("{}"))->ToCharArray();
        static array<Char>^ s_comma = (static_cast<String^>(","))->ToCharArray();

    public:
        LabelTrie(SortedStringSet^ labels)
            : m_labels(labels),
              m_wild(gcnew LabelSet(labels, true)),
              m_non_wild(gcnew LabelSet(labels, false)),
              m_map(gcnew System::Collections::Generic::Dictionary<String^, LabelTrie^>()) {}

        static LabelSet^ Find(SortedStringSet^ labels, Boolean wild)
        {
            LabelTrie^ next = nullptr;
            InterruptControl::ProhibitInterruption();
            s_lock->WaitReadLock();
            try
            {
                LabelTrie^ trie = s_root;
                for each (String^ label in labels)
                {
                    next = nullptr;
                    if (!trie->m_map->TryGetValue(label, next))
                    {
                        s_lock->ReleaseReadLock();
                        s_lock->WaitWriteLock();
                        try
                        {
                            if (!trie->m_map->TryGetValue(label, next))
                            {
                                SortedStringSet^ set = gcnew SortedStringSet(trie->m_labels);
                                set->Add(label);
                                next = gcnew LabelTrie(set);
                                trie->m_map[label] = next;
                            }
                        }
                        finally
                        {
                            s_lock->ReleaseWriteLock();
                            s_lock->WaitReadLock();
                        }
                    }
                    trie = next;
                }
                return wild ? trie->m_wild : trie->m_non_wild;
            }
            finally
            {
                s_lock->ReleaseReadLock();
                InterruptControl::AllowInterruption();
            }
        }

        static LabelSet^ InternLabels(String^ s)
        {
            Boolean wild = false;
            SortedStringSet^ sortedStringSet = gcnew SortedStringSet();
            Int32 num = s->IndexOf('{');
            if (num == -1)
                return s_root->m_non_wild;

            s = s->Substring(num + 1);
            s = s->Trim(s_braces);
            array<String^>^ labels = s->Split(s_comma);
            Int32 num2 = labels->Length;
            Int32 num3 = 0;
            if (0 < num2)
            {
                do
                {
                    String^ text = labels[num3]->Trim();
                    labels[num3] = text;
                    if (labels[num3]->Length != 0)
                    {
                        if (labels[num3]->Equals("*"))
                            wild = true;
                        else
                            sortedStringSet->Add(labels[num3]);
                    }
                    num3++;
                }
                while (num3 < num2);
            }

            return Find(sortedStringSet, wild);
        }

        static LabelSet^ InternLabels(array<String^>^ labels, Boolean wild)
        {
            return Find(gcnew SortedStringSet(labels), wild);
        }

        static LabelSet^ Empty()
        {
            return s_root->m_non_wild;
        }

        static LabelSet^ WithWild(LabelSet^ labelSet, Boolean wild)
        {
            if (labelSet->Wild() == wild)
                return labelSet;
            return Find(labelSet->Labels(), wild);
        }

    };
}}

namespace Quire { namespace utl_mgd {

    [Quire::Mirror::MirrorOfAttribute("utl_mgd.LabelUtils, utl-mgd")]
    public value class LabelUtils
    {
    public:
        static array<Char>^ s_sharp = (static_cast<String^>("#"))->ToCharArray();

        static String^ SplitName(String^ s)
        {
            Int32 num = s->IndexOf('{');
            return (num == -1) ? s : s->Substring(0, num);
        }

        static LabelSet^ SplitLabels(String^ s)
        {
            return LabelTrie::InternLabels(s);
        }

        static String^ SplitMangledName(String^ s)
        {
            Int32 num = s->IndexOf('#');
            return (num == -1) ? s : s->Substring(0, num);
        }

        static LabelSet^ SplitMangledLabels(String^ s)
        {
            Boolean wild = false;
            Int32 i = s->IndexOf('#');
            if (i == -1)
                return LabelTrie::Empty();

            s = s->Substring(i + 1);
            array<String^>^ labels = s->Split(s_sharp);
            Int32 n = labels->Length;
            if (labels[n - 1]->Equals("wild"))
            {
                wild = true;
                array<String^>^ tame = gcnew array<String^>(n - 1);
                for (Int32 j = 0; j < n - 1; ++j)
                    tame[j] = labels[j];
                labels = tame;
            }
            return LabelTrie::InternLabels(labels, wild);
        }

        static String^ Unmangle(String^ mangled)
        {
            String^ text = SplitMangledLabels(mangled)->ToString();
            Int32 num = mangled->IndexOf('#');
            return ((num == -1) ? mangled : mangled->Substring(0, num)) + text;
        }

        static String^ Canonicalize(String^ s, Boolean% wild)
        {
            LabelSet^ labelSet = LabelTrie::InternLabels(s);
            wild = labelSet->Wild();
            String^ text = labelSet->ToString();
            Int32 num = s->IndexOf('{');
            return ((num == -1) ? s : s->Substring(0, num)) + text;
        }

        static String^ Canonicalize(String^ s)
        {
            String^ text = LabelTrie::InternLabels(s)->ToString();
            Int32 num = s->IndexOf('{');
            return ((num == -1) ? s : s->Substring(0, num)) + text;
        }

        static String^ ForceWild(String^ s)
        {
            LabelSet^ labelSet = LabelTrie::InternLabels(s);
            labelSet = LabelTrie::WithWild(labelSet, true);
            String^ text = labelSet->ToString();
            Int32 num = s->IndexOf('{');
            return ((num == -1) ? s : s->Substring(0, num)) + text;
        }

        static Boolean Unifies(String^ c, String^ c_non_wild)
        {
            Int32 num = c->IndexOf('{');
            String^ obj = ((num == -1) ? c : c->Substring(0, num));
            Int32 num2 = c_non_wild->IndexOf('{');
            String^ value = ((num2 == -1) ? c_non_wild : c_non_wild->Substring(0, num2));
            if (!obj->Equals(value))
                return false;

            LabelSet^ labelSet = LabelTrie::InternLabels(c);
            LabelSet^ other = LabelTrie::InternLabels(c_non_wild);
            return labelSet->Unifies(other);
        }

        static Object^ LookupPossiblyWild(String^ c, IDictionary^ d)
        {
            LabelSet^ labels = LabelTrie::InternLabels(c);
            if (!labels->Wild())
                return d[c];

            Int32 i = c->IndexOf('{');
            String^ name = i == -1 ? c : c->Substring(0, i);
            for each (DictionaryEntry entry in d)
            {
                String^ key = dynamic_cast<String^>(entry.Key);
                String^ kname = SplitName(key);
                if (!name->Equals(kname))
                    continue;
                LabelSet^ klabels = SplitLabels(key);
                if (labels->Unifies(klabels))
                    return entry.Value;
            }
            return nullptr;
        }

        static String^ FindBinding(String^ s, IDictionary^ d)
        {
            Int32 i = s->IndexOf('{');
            String^ name = i == -1 ? s : s->Substring(0, i);
            LabelSet^ labels = LabelTrie::InternLabels(s);
            for each (DictionaryEntry entry in d)
            {
                String^ key = dynamic_cast<String^>(entry.Key);
                String^ kname = SplitName(key);
                if (!name->Equals(kname))
                    continue;
                LabelSet^ klabels = SplitLabels(key);
                if (labels->Unifies(klabels))
                    return key;
            }
            return nullptr;
        }

    };

    [Quire::Mirror::MirrorOfAttribute("utl_mgd.LabeledDictionary, utl-mgd")]
    public ref class LabeledDictionary : System::Collections::IDictionary
    {
    private:
        System::Collections::Hashtable^ m_table;
        Boolean m_locked;

    public:
        virtual LabeledDictionary^ Lock() { m_locked = true; return this; }
        virtual void OnChange() {}

        virtual property Boolean IsFixedSize { Boolean get() { return false; } }
        virtual property Boolean IsReadOnly { Boolean get() { return m_locked; } }

        virtual property Object^ default[Object^]
        {
            Object^ get(Object^ key)
            {
                Boolean wild;
                String^ text = LabelUtils::Canonicalize(key->ToString(), wild);
                if (!wild)
                {
                    Object^ result = m_table[text];
                    GC::KeepAlive(this);
                    return result;
                }
                Object^ result2 = LabelUtils::LookupPossiblyWild(text, m_table);
                GC::KeepAlive(this);
                return result2;
            }
            void set(Object^ key, Object^ value)
            {
                OnChange();
                System::Collections::Hashtable^ table = m_table;
                String^ canonicalKey = LabelUtils::Canonicalize(key->ToString());
                table[canonicalKey] = value;
                GC::KeepAlive(this);
            }
        }

        virtual property System::Collections::ICollection^ Keys
        {
            System::Collections::ICollection^ get()
            {
                System::Collections::ICollection^ keys = m_table->Keys;
                GC::KeepAlive(this);
                return keys;
            }
        }

        virtual property System::Collections::ICollection^ Values
        {
            System::Collections::ICollection^ get()
            {
                System::Collections::ICollection^ values = m_table->Values;
                GC::KeepAlive(this);
                return values;
            }
        }

        virtual void Add(Object^ key, Object^ value)
        {
            OnChange();
            System::Collections::Hashtable^ table = m_table;
            String^ canonicalKey = LabelUtils::Canonicalize(key->ToString());
            table->Add(canonicalKey, value);
            GC::KeepAlive(this);
        }

        virtual void Clear() { m_table->Clear(); GC::KeepAlive(this); }

        virtual Boolean Contains(Object^ key)
        {
            Boolean wild;
            String^ text = LabelUtils::Canonicalize(key->ToString(), wild);
            if (!wild)
            {
                Boolean result = m_table->Contains(text);
                GC::KeepAlive(this);
                return result;
            }
            Boolean result2 = LabelUtils::LookupPossiblyWild(text, m_table) != nullptr;
            GC::KeepAlive(this);
            return result2;
        }

        Int32 Contains1(String^ text) { return Contains(text) ? 1 : 0; }

        virtual void Remove(Object^ key)
        {
            OnChange();
            System::Collections::Hashtable^ table = m_table;
            String^ canonicalKey = LabelUtils::Canonicalize(key->ToString());
            table->Remove(canonicalKey);
            GC::KeepAlive(this);
        }

        virtual System::Collections::IDictionaryEnumerator^ _e1()
            = System::Collections::IDictionary::GetEnumerator
        {
            System::Collections::IDictionaryEnumerator^ enumerator = m_table->GetEnumerator();
            GC::KeepAlive(this);
            return enumerator;
        }

        virtual System::Collections::IEnumerator^ _e2()
            = System::Collections::IEnumerable::GetEnumerator
        {
            return _e1();
        }

        virtual property Int32 Count
        {
            Int32 get() { Int32 count = m_table->Count; GC::KeepAlive(this); return count; }
        }

        virtual property Boolean IsSynchronized
        {
            Boolean get() { Boolean value = m_table->IsSynchronized; GC::KeepAlive(this); return value; }
        }

        virtual property Object^ SyncRoot
        {
            Object^ get() { Object^ value = m_table->SyncRoot; GC::KeepAlive(this); return value; }
        }

        virtual void CopyTo(System::Array^ array, Int32 index)
        {
            m_table->CopyTo(array, index);
            GC::KeepAlive(this);
        }

        LabeledDictionary^ Copy() { return gcnew LabeledDictionary(this); }

        LabeledDictionary^ MergeResult(LabeledDictionary^ dictionary)
        {
            if (Contains("%errors")) return this;
            if (dictionary->Contains("%errors"))
            {
                GC::KeepAlive(this);
                return dictionary;
            }
            LabeledDictionary^ result = gcnew LabeledDictionary(this);
            Object^ value = dictionary["%result"];
            result["%result"] = value;
            GC::KeepAlive(this);
            return result;
        }

        LabeledDictionary(System::Collections::IDictionary^ d)
            : m_table(gcnew System::Collections::Hashtable(d->Count)), m_locked(false)
        {
            for each (System::Collections::DictionaryEntry entry in d)
                this[entry.Key] = entry.Value;
            GC::KeepAlive(this);
        }

        LabeledDictionary(Int32 capacity)
            : m_table(gcnew System::Collections::Hashtable(capacity)), m_locked(false) {}

        LabeledDictionary()
            : m_table(gcnew System::Collections::Hashtable()), m_locked(false) {}
    };

    [Quire::Mirror::MirrorOfAttribute("utl_mgd.SafeMonitor, utl-mgd")]
    public ref class SafeMonitor
    {
    private:
        [Quire::Mirror::MirrorOfAttribute("utl_mgd.SafeMonitor+CSecRecord, utl-mgd")]
        ref class CSecRecord
        {
        public:
            Int64 csec;
            Int32 count;
            CSecRecord(Int64 value, Int32 references) : csec(value), count(references) {}
        };

        static System::Collections::Hashtable^ s_csecmap;
        static System::Collections::Stack^ s_csecpool;
        static SafeMutex^ s_mutex;

        static CRITICAL_SECTION* GetCriticalSection(Object^ o, Int32 inc)
        {
            s_mutex->WaitOne();
            try
            {
                CRITICAL_SECTION* cs = nullptr;
                CSecRecord^ r = dynamic_cast<CSecRecord^>(s_csecmap[o]);
                if (r != nullptr)
                {
                    cs = (CRITICAL_SECTION*)r->csec;
                    r->count += inc;
                }
                else if (s_csecpool->Count == 0)
                {
                    cs = new CRITICAL_SECTION;
                    ::InitializeCriticalSection(cs);
                    r = gcnew CSecRecord((Int64)cs, inc);
                    s_csecmap[o] = r;
                }
                else
                {
                    cs = (CRITICAL_SECTION*)(Int64)s_csecpool->Pop();
                    r = gcnew CSecRecord((Int64)cs, inc);
                    s_csecmap[o] = r;
                }

                if (r->count == 0)
                {
                    s_csecmap->Remove(o);
                    s_csecpool->Push(r->csec);
                }
                return cs;
            }
            finally
            {
                s_mutex->ReleaseMutex();
            }
        }

        static void Free()
        {
            s_mutex->WaitOne();
            try
            {
                while (s_csecpool->Count != 0)
                {
                    CRITICAL_SECTION* cs = (CRITICAL_SECTION*)(Int64)s_csecpool->Pop();
                    ::DeleteCriticalSection(cs);
                    delete cs;
                }
            }
            finally
            {
                s_mutex->Free();
            }
        }

        static Int32 on_exit()
        {
            Free();
            return 0;
        }

        static SafeMonitor()
        {
            s_csecmap = gcnew System::Collections::Hashtable();
            s_csecpool = gcnew System::Collections::Stack();
            s_mutex = gcnew SafeMutex("SafeMonitor");
            _onexit_m(on_exit);
        }

    public:
        SafeMonitor() {}

        static void Enter(Object^ key)
        {
            ::EnterCriticalSection(GetCriticalSection(key, 1));
        }

        static Boolean TryEnter(Object^ key)
        {
            if (::TryEnterCriticalSection(GetCriticalSection(key, 1)))
                return true;
            GetCriticalSection(key, -1);
            return false;
        }

        static void Exit(Object^ key)
        {
            ::LeaveCriticalSection(GetCriticalSection(key, -1));
        }
    };
}}

namespace Quire { namespace utl_mgd {

    [Quire::Mirror::MirrorOfAttribute("utl_mgd.VFunc, utl-mgd")]
    ref class VFunc
    {
    public:
        Function^ m_function;

        VFunc(Function^ f) : m_function(f) {}

        void Invoke()
        {
            m_function->Invoke0();
            GC::KeepAlive(this);
        }
    };

    [Quire::Mirror::MirrorOfAttribute("utl_mgd.ThreadingUtils, utl-mgd")]
    public ref class ThreadingUtils abstract
    {
    public:
        static void Spawn(Function^ f)
        {
            Thread^ thread = gcnew Thread(gcnew ThreadStart(gcnew VFunc(f), &VFunc::Invoke), 4194304);
            thread->IsBackground = true;
            thread->Start();
        }

        ThreadingUtils() {}
    };

    [Quire::Mirror::MirrorOfAttribute("utl_mgd.Str, utl-mgd")]
    public ref class Str
    {
    private:
        static Predicate<Char>^ s_is_white = gcnew Predicate<Char>(&Char::IsWhiteSpace);

        static String^ remove_if(Predicate<Char>^ p, String^ s)
        {
            Int32 length = s->Length;
            Int32 num = 0;
            if (0 < length)
            {
                do
                {
                    if (!p(s[num]))
                    {
                        num++;
                        continue;
                    }

                    StringBuilder^ stringBuilder = gcnew StringBuilder(length);
                    Int32 num2 = 0;
                    do
                    {
                        if (!p(s[num2]))
                            stringBuilder->Append(s[num2]);
                        num2++;
                    }
                    while (num2 < length);

                    return stringBuilder->ToString();
                }
                while (num < length);
            }

            return s;
        }

    public:
        Str() {}

        static String^ RemoveWhite(String^ s)
        {
            return remove_if(s_is_white, s);
        }
    };
}}

namespace Quire { namespace utl_mgd {

    [Quire::Mirror::MirrorOfAttribute("utl_mgd._Hex, utl-mgd")]
    public ref class _Hex
    {
    private:
        static Byte hexdig(Int32 hd)
        {
            if (static_cast<UInt32>(hd - 48) <= 9u)
                return static_cast<Byte>(hd + -48);
            return static_cast<Byte>(hd + -55);
        }

        static Int32 dighex(Byte dh)
        {
            if (static_cast<UInt32>(dh) < 10u)
                return dh + 48;
            return dh + 55;
        }

    public:
        _Hex() {}

        static String^ ToHex(array<Byte>^ b)
        {
            StringBuilder^ stringBuilder = gcnew StringBuilder(b->Length * 2);
            Int32 num = 0;
            if (0 < b->Length)
            {
                do
                {
                    Byte b2 = static_cast<Byte>(b[num] >> 4);
                    Int32 num2;
                    if (static_cast<UInt32>(b2) < 10u)
                        num2 = b2 + 48;
                    else
                        num2 = b2 + 55;
                    stringBuilder->Append(static_cast<Char>(num2));
                    Byte b3 = static_cast<Byte>(b[num] % 16);
                    Int32 num3;
                    if (static_cast<UInt32>(b3) < 10u)
                        num3 = b3 + 48;
                    else
                        num3 = b3 + 55;
                    stringBuilder->Append(static_cast<Char>(num3));
                    num++;
                }
                while (num < b->Length);
            }

            return stringBuilder->ToString();
        }

        static array<Byte>^ FromHex(String^ s)
        {
            Debug::Assert(s->Length % 2 == 0, nullptr);
            Int32 n = s->Length / 2;
            array<Byte>^ b = gcnew array<Byte>(n);
            for (Int32 i = 0; i < n; ++i)
                b[i] = (Byte)(hexdig(s[i * 2]) * 16 + hexdig(s[i * 2 + 1]));
            return b;
        }

    };
}}
