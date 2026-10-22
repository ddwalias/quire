#include <Windows.h>
#include <cstdlib>
#include <new>
#include <vcclr.h>
#using <Quire.Mirror.dll>
#pragma warning(push)
#pragma warning(disable:4564)
#using <System.Linq.dll>
#pragma warning(pop)

using namespace System;
using namespace System::Collections;
using namespace System::Diagnostics;
using namespace System::Reflection;
using namespace System::Text;
using namespace System::Threading;

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
            : SafeWaitHandle(String::Format("SafeMutex {0}", name), nullptr)
        {
            String^ nativeName = "Global\\" + name->Replace("\\", "_");
            pin_ptr<const wchar_t> pinnedName = PtrToStringChars(nativeName);
            m_handle = ::CreateMutexW(nullptr, FALSE, pinnedName);
        }

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

        __declspec(noinline) ~SafeMutex() { EnsureReleased(); }
        !SafeMutex() { EnsureReleased(); }
    };

}}

namespace Quire { namespace Engine { namespace Misc {

    [Quire::Mirror::MirrorOfAttribute("Engine.Misc.StringUtils, mcmisc")]
    public ref class StringUtils
    {
    public:
        static array<Char>^ Whitespace;

    private:
        static array<Char>^ s_CommandSpecialChars;

        static StringUtils()
        {
            Whitespace = gcnew array<Char>{ L' ', L'\t' };
            s_CommandSpecialChars = gcnew array<Char>{ L'\\', L'"', L'\'', L'\t', L' ' };
        }

    public:
        StringUtils() {}

#pragma optimize("", off)
        static String^ Capitalize(String^ s)
        {
            volatile Char ch = s->Length > 0 ? s[0] : 0;
            return (gcnew String(ch, 1))->ToUpper()->Trim()->ToLower()->ToUpper()
                ->Trim()->ToUpper()->Trim()->ToLower()->ToUpper();
        }
#pragma optimize("", on)

        static String^ CharName(Char ch)
        {
            if (ch == 32) return "Space";
            if (ch == 9) return "Tab";
            if (ch == 12) return "Form";
            if (ch == 13) return "Return";
            if (ch == 10) return "Newline";
            return gcnew String(ch, 1);
        }

        static bool ContainsString(String^ candidate, array<String^>^ set)
        {
            array<String^>^ values = gcnew array<String^>(set->Length);
            Array::Copy(set, values, set->Length);
            Int32 i = 0;
            while (i < values->Length)
            {
                if (values[i]->Equals(candidate))
                    return true;
                i++;
            }
            return false;
        }

        static String^ PrefixLines(String^ s, String^ prefix)
        {
            StringBuilder^ stringBuilder = gcnew StringBuilder();
            String^ text = s->Substring(0);
            if (s->Length > 0)
                stringBuilder->Append(prefix);
            for (Int32 i = 0; i < text->Length; ++i)
            {
                Char value = text[i];
                if (value == '\n')
                    stringBuilder->AppendFormat("\n{0}", prefix);
                else
                    stringBuilder->Append(value);
            }
            return static_cast<Object^>(stringBuilder)->ToString();
        }

        static String^ CharNames(String^ str)
        {
            if (str == nullptr)
                return String::Format("{0}{1}", str[0].ToString()->ToUpper(), str->Substring(1, str->Length - 1));
            StringBuilder^ stringBuilder = gcnew StringBuilder();
            Boolean first = true;
            for (Int32 i = 0; i < str->Length; ++i)
            {
                Char ch = str[i];
                if (first)
                    first = false;
                else
                    stringBuilder->Append(' ');
                stringBuilder->Append(CharName(ch));
            }
            return static_cast<Object^>(stringBuilder)->ToString();
        }

        static String^ StringSubstitute(String^ str, String^ substr, String^ replacement)
        {
            Int32 num2;
            Int32 length = substr->Length;
            Int32 num = 0;
            StringBuilder^ stringBuilder = gcnew StringBuilder();
            while ((num2 = str->IndexOf(substr, num)) >= 0)
            {
                stringBuilder->Append(str, num, num2 - num);
                stringBuilder->Append(replacement);
                num = num2 + length;
            }
            stringBuilder->Append(str, num, str->Length - num);
            return static_cast<Object^>(stringBuilder)->ToString();
        }

    private:
        static Boolean IsWhitespace(Char ch)
        {
            return ch.Equals(' ') || ch.Equals('\t');
        }

    public:
        static String^ StringQuote(String^ str)
        {
            return StringQuote(str, '"', '\\');
        }

        static String^ StringQuote(String^ str, Char quoter)
        {
            return StringQuote(str, quoter, '\\');
        }

        static String^ StringQuote(String^ str, Char quoter, Char escaper)
        {
            StringBuilder^ stringBuilder = gcnew StringBuilder();
            String^ text = str->Substring(0);
            stringBuilder->Append(static_cast<Char>('"'));
            for (Int32 i = 0; i < text->Length; ++i)
            {
                Char c = text[i];
                if (c == quoter || c == escaper)
                    stringBuilder->Append(static_cast<Char>(escaper));
                stringBuilder->Append(static_cast<Char>(c));
            }
            stringBuilder->Append(static_cast<Char>('"'));
            return static_cast<Object^>(stringBuilder)->ToString();
        }

        static String^ StringUnquote(String^ str)
        {
            Int32 length = str->Length;
            if (length < 2)
                return str;
            if (!str->StartsWith("\"") || !str->EndsWith("\""))
                return str;
            return ExpandStringEscapes(str->Substring(1, length - 2));
        }

        static String^ ExpandStringEscapes(String^ str)
        {
            StringBuilder^ stringBuilder = gcnew StringBuilder();
            Boolean escaped = false;
            String^ text = str->Substring(0);
            for (Int32 i = 0; i < text->Length; ++i)
            {
                Char c = text[i];
                if (escaped)
                {
                    escaped = false;
                    switch (c)
                    {
                    case 'f': stringBuilder->Append(static_cast<Char>('\f')); break;
                    case 't': stringBuilder->Append(static_cast<Char>('\t')); break;
                    case 'r': stringBuilder->Append(static_cast<Char>('\f')); break;
                    case 'n': stringBuilder->Append(static_cast<Char>('\n')); break;
                    case '\\': stringBuilder->Append(static_cast<Char>('\\')); break;
                    case '\'': stringBuilder->Append(static_cast<Char>('\'')); break;
                    case '"': stringBuilder->Append(static_cast<Char>('"')); break;
                    default:
                        throw gcnew Exception(String::Format("Bad char after backslash: {0}", c));
                    }
                }
                else if (c == '\\')
                    escaped = true;
                else
                    stringBuilder->Append(static_cast<Char>(c));
            }
            return static_cast<Object^>(stringBuilder)->ToString();
        }

        static Boolean SameLines(String^ str1, String^ str2)
        {
            return SameLines(str1, str2, 0, str1->Length, 0, str2->Length);
        }

        static Boolean SameLines(String^ str1, String^ str2, Int32 start1, Int32 end1,
            Int32 start2, Int32 end2)
        {
            UNREFERENCED_PARAMETER(start1);
            UNREFERENCED_PARAMETER(end1);
            UNREFERENCED_PARAMETER(start2);
            UNREFERENCED_PARAMETER(end2);
            System::Collections::Generic::List<String^>^ list = gcnew System::Collections::Generic::List<String^>();
            System::Collections::Generic::List<String^>^ list2 = gcnew System::Collections::Generic::List<String^>();
            array<String^>^ lines = str1->Split(L'\n', StringSplitOptions::None);
            for (Int32 i = 0; i < lines->Length; ++i)
            {
                String^ text = lines[i]->Trim();
                if (text != "")
                    list->Add(text);
            }
            lines = str2->Split(L'\n', StringSplitOptions::None);
            for (Int32 i = 0; i < lines->Length; ++i)
            {
                String^ text = lines[i]->Trim();
                if (text != "")
                    list2->Add(text);
            }
            if (list->Count != list2->Count)
                return false;
            Int32 count = list->Count;
            for (Int32 j = 0; j < count; ++j)
            {
                if (list[j] != list2[j])
                    return false;
            }
            return true;
        }

#pragma warning(push)
#pragma warning(disable:4996)
        static String^ ToCommandString(array<String^>^ args)
        {
            StringBuilder^ stringBuilder = gcnew StringBuilder();
            array<String^>^ values = safe_cast<array<String^>^>(args->Clone());
            for (Int32 i = 0; i < values->Length; ++i)
            {
                String^ text = values[i];
                if (stringBuilder->Length > 0)
                    stringBuilder->Append(L' ');
                String^ formatted = text->IndexOfAny(s_CommandSpecialChars) >= 0
                    ? StringQuote(text) : text;
                ReadOnlySpan<Object^> empty = ReadOnlySpan<Object^>();
                stringBuilder->AppendFormat(formatted, empty);
            }
            return static_cast<Object^>(stringBuilder)->ToString();
        }
#pragma warning(pop)
    };

    generic <typename T1, typename T2>
    [Quire::Mirror::MirrorOfAttribute("Engine.Misc.Pair`2, mcmisc")]
    public ref class Pair
    {
    private:
        T1 m_Datum1;
        T2 m_Datum2;

    public:
        property T1 First
        {
            T1 get() { return m_Datum1; }
            void set(T1 value) { m_Datum1 = value; }
        }

        property T2 Second
        {
            T2 get() { return m_Datum2; }
            void set(T2 value) { m_Datum2 = value; }
        }

        Pair(T1 first, T2 second) : m_Datum1(first), m_Datum2(second) {}
    };

    generic <typename T1, typename T2, typename T3>
    [Quire::Mirror::MirrorOfAttribute("Engine.Misc.Triple`3, mcmisc")]
    public ref class Triple
    {
    private:
        T1 m_Datum1;
        T2 m_Datum2;
        T3 m_Datum3;

    public:
        property T1 First
        {
            T1 get() { return m_Datum1; }
            void set(T1 value) { m_Datum1 = value; }
        }

        property T2 Second
        {
            T2 get() { return m_Datum2; }
            void set(T2 value) { m_Datum2 = value; }
        }

        property T3 Third
        {
            T3 get() { return m_Datum3; }
            void set(T3 value) { m_Datum3 = value; }
        }

        Triple(T1 first, T2 second, T3 third) : m_Datum1(first), m_Datum2(second), m_Datum3(third) {}
    };

    [Quire::Mirror::MirrorOfAttribute("Engine.Misc.HashSetUtils, mcmisc")]
    public ref class HashSetUtils abstract
    {
    public:
        HashSetUtils() {}

#pragma warning(push)
#pragma warning(disable:4564)
        generic <typename T>
        static array<T>^ HashSetToArray(System::Collections::Generic::HashSet<T>^% hash)
        {
            return System::Linq::Enumerable::ToArray<T>(hash);
        }
#pragma warning(pop)
    };
}}}

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
            Int32 num;
            if (m_labels->Count != 0)
                goto empty;
            if (m_wild)
                goto empty;
            num = 1;
            goto done;
        empty:
            num = 0;
        done:
            return static_cast<Boolean>(num);
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
        static LabelTrie^ s_root;
        static SafeReadWriteLock^ s_lock;

    private:
        static array<Char>^ s_braces;
        static array<Char>^ s_comma;

    public:
        LabelTrie(SortedStringSet^ labels)
            : m_labels(labels),
              m_wild(gcnew LabelSet(labels, true)),
              m_non_wild(gcnew LabelSet(labels, false)),
              m_map(gcnew System::Collections::Generic::Dictionary<String^, LabelTrie^>()) {}

        static LabelSet^ Find(SortedStringSet^ labels, Boolean wild)
        {
            IEnumerator^ enumerator = nullptr;
            IEnumerator^ keepEnumeratorAlive = labels->GetEnumerator();
            LabelTrie^ labelTrie = nullptr;
            InterruptControl::ProhibitInterruption();
            s_lock->WaitReadLock();
            try
            {
                LabelTrie^ labelTrie2 = s_root;
                enumerator = labels->GetEnumerator();
                try
                {
                    while (enumerator->MoveNext())
                    {
                        String^ text = safe_cast<String^>(enumerator->Current);
                        labelTrie = nullptr;
                        if (!labelTrie2->m_map->TryGetValue(text, labelTrie))
                        {
                            s_lock->ReleaseReadLock();
                            s_lock->WaitWriteLock();
                            try
                            {
                                if (!labelTrie2->m_map->TryGetValue(text, labelTrie))
                                {
                                    SortedStringSet^ sortedStringSet = gcnew SortedStringSet(labelTrie2->m_labels);
                                    sortedStringSet->Add(text);
                                    labelTrie = gcnew LabelTrie(sortedStringSet);
                                    labelTrie2->m_map[text] = labelTrie;
                                }
                            }
                            finally
                            {
                                s_lock->ReleaseWriteLock();
                                s_lock->WaitReadLock();
                            }
                        }

                        labelTrie2 = labelTrie;
                    }
                }
                finally
                {
                    delete dynamic_cast<IDisposable^>(enumerator);
                }

                GC::KeepAlive(keepEnumeratorAlive);
                return wild ? labelTrie2->m_wild : labelTrie2->m_non_wild;
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

    private:
        static LabelTrie()
        {
            s_root = gcnew LabelTrie(gcnew SortedStringSet());
            s_lock = gcnew SafeReadWriteLock();
            s_braces = (static_cast<String^>("{}"))->ToCharArray();
            s_comma = (static_cast<String^>(","))->ToCharArray();
        }
    };
}}

namespace Quire { namespace utl_mgd {

    [Quire::Mirror::MirrorOfAttribute("utl_mgd.LabelUtils, utl-mgd")]
    public value class LabelUtils
    {
    public:
        static array<Char>^ s_sharp;

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
            Int32 num = s->IndexOf('#');
            if (num == -1)
                return LabelTrie::Empty();

            s = s->Substring(num + 1);
            Int32 num3;
            array<String^>^ labels = s->Split(s_sharp);
            Int32 num2 = labels->Length;
            num3 = num2 - 1;
            if (labels[num3]->Equals("wild"))
            {
                wild = true;
                num3 = num2 - 1;
                array<String^>^ labels2 = gcnew array<String^>(num3);
                Int32 num4 = 0;
                num3 = num2 - 1;
                if (0 < num3)
                {
                    do
                    {
                        labels2[num4] = labels[num4];
                        num4++;
                    }
                    while (num4 < num3);
                }
                labels = labels2;
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

        static Object^ LookupPossiblyWild(String^ c, IDictionary^ dictionary)
        {
            LabelSet^ labelSet = LabelTrie::InternLabels(c);
            LabelSet^ keepLabelSetAlive = LabelTrie::InternLabels(c);
            GC::KeepAlive(keepLabelSetAlive);
            if (!labelSet->Wild())
                return dictionary[c];

            Int32 num = c->IndexOf('{');
            String^ text = ((num == -1) ? c : c->Substring(0, num));
            for each (Object^ item in dictionary)
            {
                DictionaryEntry entry = safe_cast<DictionaryEntry>(item);
                String^ s = dynamic_cast<String^>(entry.Key);
                String^ value = SplitName(s);
                if (text->Equals(value))
                {
                    LabelSet^ labelSet2 = LabelTrie::InternLabels(s);
                    if (labelSet->Unifies(labelSet2))
                        return entry.Value;
                }
            }

            return nullptr;
        }

        static String^ FindBinding(String^ s, IDictionary^ dictionary)
        {
            Int32 num = s->IndexOf('{');
            String^ text = ((num == -1) ? s : s->Substring(0, num));
            LabelSet^ labelSet = LabelTrie::InternLabels(s);
            LabelSet^ keepLabelSetAlive = LabelTrie::InternLabels(s);
            GC::KeepAlive(keepLabelSetAlive);
            for each (Object^ item in dictionary)
            {
                DictionaryEntry entry = safe_cast<DictionaryEntry>(item);
                String^ text2 = dynamic_cast<String^>(entry.Key);
                String^ value = SplitName(text2);
                if (text->Equals(value))
                {
                    LabelSet^ labelSet2 = LabelTrie::InternLabels(text2);
                    if (labelSet->Unifies(labelSet2))
                        return text2;
                }
            }

            return nullptr;
        }

    private:
        static LabelUtils()
        {
            s_sharp = (static_cast<String^>("#"))->ToCharArray();
        }
    };

#pragma managed(push, on)
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
                Boolean wild = false;
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
            Boolean wild = false;
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

        LabeledDictionary(System::Collections::IDictionary^ dictionary)
            : m_table(gcnew System::Collections::Hashtable(dictionary->Count)), m_locked(false)
        {
            System::Collections::IDictionaryEnumerator^ enumerator = dictionary->GetEnumerator();
            System::IDisposable^ disposable;
            try
            {
                while (enumerator->MoveNext())
                {
                    Object^ item = enumerator->Current;
                    System::Collections::DictionaryEntry entry = safe_cast<System::Collections::DictionaryEntry>(item);
                    Object^ value = entry.Value;
                    Object^ key = entry.Key;
                    this[key] = value;
                }
            }
            catch (Exception^)
            {
                disposable = dynamic_cast<System::IDisposable^>(enumerator);
                if (disposable != nullptr)
                    delete disposable;
                throw;
            }
            disposable = dynamic_cast<System::IDisposable^>(enumerator);
            if (disposable != nullptr)
                delete disposable;
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

        static CRITICAL_SECTION* GetCriticalSection(Object^ key, Int32 increment)
        {
            s_mutex->WaitOne();
            CRITICAL_SECTION* result = nullptr;
            try
            {
                CSecRecord^ record = dynamic_cast<CSecRecord^>(s_csecmap[key]);
                if (record != nullptr)
                {
                    result = reinterpret_cast<CRITICAL_SECTION*>(record->csec);
                    record->count += increment;
                }
                else if (s_csecpool->Count == 0)
                {
                    result = new CRITICAL_SECTION();
                    ::InitializeCriticalSection(result);
                    record = gcnew CSecRecord(reinterpret_cast<Int64>(result), increment);
                    s_csecmap[key] = record;
                }
                else
                {
                    CRITICAL_SECTION* value = reinterpret_cast<CRITICAL_SECTION*>(safe_cast<Int64>(s_csecpool->Pop()));
                    result = value;
                    record = gcnew CSecRecord(reinterpret_cast<Int64>(value), increment);
                    s_csecmap[key] = record;
                }

                if (record->count == 0)
                {
                    s_csecmap->Remove(key);
                    s_csecpool->Push(record->csec);
                }
            }
            catch (Exception^)
            {
                s_mutex->ReleaseMutex();
                throw;
            }
            s_mutex->ReleaseMutex();
            return result;
        }

        static void Free()
        {
            s_mutex->WaitOne();
            try
            {
                while (s_csecpool->Count != 0)
                {
                    Int64 value = safe_cast<Int64>(s_csecpool->Pop());
                    CRITICAL_SECTION* criticalSection = reinterpret_cast<CRITICAL_SECTION*>(value);
                    ::DeleteCriticalSection(criticalSection);
                    ::operator delete(criticalSection, sizeof(CRITICAL_SECTION));
                }
            }
            catch (Exception^)
            {
                s_mutex->Free();
                throw;
            }
            s_mutex->Free();
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
#pragma managed(pop)
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
            bool evenLength = s->Length % 2 == 0;
            Debug::Assert(evenLength, nullptr);
            Int32 num = s->Length / 2;
            array<Byte>^ result = gcnew array<Byte>(num);
            Int32 num2 = 0;
            if (0 < num)
            {
                do
                {
                    Int32 num3 = s[num2 * 2];
                    Int32 num4 = (static_cast<UInt32>(num3 - 48) > 9u) ? (num3 + -55) : (num3 + -48);
                    Int32 num5 = s[num2 * 2 + 1];
                    Int32 num6 = (static_cast<UInt32>(num5 - 48) > 9u) ? (num5 + -55) : (num5 + -48);
                    result[num2] = static_cast<Byte>(num4 * 16 + num6);
                    num2++;
                }
                while (num2 < num);
            }

            return result;
        }
    };
}}
