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

    private:
        void _bangSafeWaitHandle() { ::CloseHandle(static_cast<HANDLE>(m_handle)); GC::KeepAlive(this); }
        void _tildeSafeWaitHandle() { ::CloseHandle(static_cast<HANDLE>(m_handle)); GC::KeepAlive(this); }

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

    private:
        void _bangSafeMutex() { EnsureReleased(); }
        void _tildeSafeMutex() { EnsureReleased(); }

    public:
        __declspec(noinline) ~SafeMutex() { EnsureReleased(); }
        !SafeMutex() { EnsureReleased(); }
    };

    generic <typename T>
    [Quire::Mirror::MirrorOfAttribute("utl_mgd.Mailbox`1, utl-mgd")]
    public ref class Mailbox
    {
    private:
        SafeMutex^ m_mutex;
        SafeEvent^ m_nonempty;
        System::Collections::Generic::Queue<T>^ m_queue;

    public:
        Mailbox()
        {
            m_mutex = gcnew SafeMutex("mailbox mutex");
            m_nonempty = gcnew SafeEvent("mailbox non-empty");
            m_queue = gcnew System::Collections::Generic::Queue<T>();
            m_nonempty->Reset();
            GC::KeepAlive(this);
        }

        T GetMessage()
        {
            T message = T();
            GetMessage(message, reinterpret_cast<void*>(-1));
            return message;
        }

        Boolean GetMessage(T% message, void* timeout)
        {
            do
            {
                m_mutex->WaitOne();
                try
                {
                    if (m_queue->Count > 0)
                    {
                        message = m_queue->Dequeue();
                        if (m_queue->Count == 0) m_nonempty->Reset();
                        return true;
                    }
                }
                finally { m_mutex->EnsureReleased(1); }
            }
            while (m_nonempty->WaitTwoWithTimeout(timeout));
            return false;
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
            try { Int32 count = m_queue->Count; GC::KeepAlive(this); return count; }
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
            auto args = safe_cast<System::Tuple<WeakReference^, Data, Mailbox<Msg>^, OnInit^, OnMessage^, Int64>^>(obj);
            WeakReference^ ownerReference = args->Item1;
            Data data = args->Item2;
            Mailbox<Msg>^ mailbox = args->Item3;
            OnInit^ onInit = args->Item4;
            OnMessage^ onMessage = args->Item5;
            void* deathHandle = System::IntPtr(args->Item6).ToPointer();
            if (onInit != nullptr)
            {
                Object^ target = ownerReference->Target;
                if (target != nullptr) onInit(safe_cast<Owner>(target), data);
            }

            Msg message = Msg();
            Object^ target;
            do
            {
                message = Msg();
                if (!mailbox->GetMessage(message, deathHandle)) break;
                target = ownerReference->Target;
            }
            while (target != nullptr && onMessage(safe_cast<Owner>(target), data, message));

            if (deathHandle != reinterpret_cast<void*>(-1)) ::CloseHandle(static_cast<HANDLE>(deathHandle));
        }

        void Init(Int32 stack_size)
        {
            m_mailbox = gcnew Mailbox<Msg>();
            m_death_handle = reinterpret_cast<void*>(-1);
            m_stack_size = stack_size;
            m_thread = gcnew Thread(gcnew ParameterizedThreadStart(&MessageQueue::MsgLoop), stack_size);
            m_thread->IsBackground = true;
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

    private:
        void _bangMessageQueue() { Stop(); }
        void _tildeMessageQueue() { _bangMessageQueue(); }

    public:
        void Start(Owner owner, Data data, OnInit^ onInit, OnMessage^ onMessage, Boolean stopThreadOnFinalize)
        {
            if (m_thread->Name == nullptr)
                m_thread->Name = String::Concat("MsgLoop(", owner->ToString(), ")");
            if (stopThreadOnFinalize)
                m_death_handle = ::CreateEventW(nullptr, TRUE, FALSE, nullptr);

            Int64 handle = System::IntPtr(m_death_handle).ToInt64();
            auto parameter = gcnew System::Tuple<WeakReference^, Data, Mailbox<Msg>^, OnInit^, OnMessage^, Int64>(
                gcnew WeakReference(owner), data, m_mailbox, onInit, onMessage, handle);
            m_thread->Start(parameter);
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

        ~MessageQueue() { _tildeMessageQueue(); GC::SuppressFinalize(this); GC::KeepAlive(this); }
        !MessageQueue() { _bangMessageQueue(); }
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

#pragma warning(push)
#pragma warning(disable:4100 4702)
    [Quire::Mirror::MirrorOfAttribute("Engine.Misc.XmlUtils, mcmisc")]
    public ref class XmlUtils
    {
    private:
        static Boolean s_Debug;
        static String^ s_WhitepaceChars;
        static String^ s_TagBreakChars;
        static Char s_OpenAngle;
        static Char s_CloseAngle;
        static Char s_DoubleQuote;
        static Char s_SingleQuote;
        static Char s_Slash;
        static Char s_Excl;
        static Char s_Hyphen;
        static Char s_CloseBracket;
        static Char s_EqualSign;
        static String^ s_xmlns;

    protected:
        static Boolean s_TightenXmlnsAttributes;

    private:
        static Int32 s_TestsDone;
        static Int32 s_TestsPassed;
        static Int32 s_TestNum;
        static Int32 s_TestPos;

        static XmlUtils()
        {
            s_Debug = false;
            s_WhitepaceChars = " \t\f\n\r";
            s_TagBreakChars = " \t\f\n\r\"\'/>";
            s_OpenAngle = '<';
            s_CloseAngle = '>';
            s_DoubleQuote = '\"';
            s_SingleQuote = '\'';
            s_Slash = '/';
            s_Excl = '!';
            s_Hyphen = '-';
            s_CloseBracket = ']';
            s_EqualSign = '=';
            s_xmlns = "xmlns";
            s_TightenXmlnsAttributes = true;
            s_TestsDone = 0;
            s_TestsPassed = 0;
            s_TestNum = 0;
            s_TestPos = 0;
        }

        static void BadSyntax(String^ str, Int32 pos, String^ fmt, Object^ obj0)
        {
            BadSyntax(str, pos, String::Format(fmt, obj0));
        }

        static void BadSyntax(String^ str, Int32 pos, String^ fmt, Object^ obj0, Object^ obj1)
        {
            BadSyntax(str, pos, String::Format(fmt, obj0, obj1));
        }

        static void BadSyntax(String^ str, Int32 pos, String^ fmt, Object^ obj0, Object^ obj1, Object^ obj2)
        {
            BadSyntax(str, pos, String::Format(fmt, obj0, obj1, obj2));
        }

        static void Dbg(String^ str)
        {
            if (s_Debug)
                Console::WriteLine(str);
        }

        static void Dbg(String^ fmt, Object^ obj0)
        {
            Dbg(String::Format(fmt, obj0));
        }

        static void Dbg(String^ fmt, Object^ obj0, Object^ obj1)
        {
            Dbg(String::Format(fmt, obj0, obj1));
        }

        static void Dbg(String^ fmt, Object^ obj0, Object^ obj1, Object^ obj2)
        {
            Dbg(String::Format(fmt, obj0, obj1, obj2));
        }

        static Boolean FindCandidateXmlnsAttr(String^ str, Int32% scanPos)
        {
            Int32 num = scanPos;
            while (true)
            {
                Int32 num2 = str->IndexOf(s_xmlns, num);
                Int32 num3 = num2;
                if (num3 < 0)
                    return false;
                while (num3 > num)
                {
                    num3--;
                    if (str[num3].Equals(s_CloseAngle))
                        break;
                    if (str[num3].Equals(s_OpenAngle))
                    {
                        scanPos = num3;
                        return true;
                    }
                }
                num = num2 + s_xmlns->Length;
            }
        }
        static Boolean SkipWhitespace(String^ str, Int32% pos) { return false; }
        static Int32 BackOverWhitespace(String^ str, Int32 pos) { return 0; }

    public:
        static property Boolean TightenXmlnsAttributes
        {
            Boolean get() { return s_TightenXmlnsAttributes; }
            void set(Boolean value) { s_TightenXmlnsAttributes = value; }
        }

        static void BadSyntax(String^ str, Int32 pos, String^ msg)
        {
            throw gcnew Exception(String::Format("bad_syntax\t{0}\tpos={1}\ttext={2}", msg, pos, str));
        }
        static Boolean LookingAtText(String^ str, Int32 scanPos, String^ text) { return false; }
        static Boolean RequireText(String^ str, Int32% scanPos, String^ text)
        {
            if (!LookingAtText(str, scanPos, text))
            {
                Dbg("RequireText \"{0}\" failing @ {1} in {2}", text, scanPos, str);
                return false;
            }
            scanPos += text->Length;
            return true;
        }
        static Boolean FindElementStart(String^ str, Int32% startPos)
        {
            return FindElementStart(str, startPos, s_WhitepaceChars);
        }
        static Boolean FindElementStart(String^ str, Int32% startPos, String^ whitespace)
        {
            for (Int32 i = startPos; i < str->Length; ++i)
            {
                Char value = str[i];
                Int32 num = whitespace->IndexOf(value);
                Dbg(String::Concat("ch=", value.ToString(), " wspos=", num.ToString()));
                if (num == -1)
                {
                    num = s_WhitepaceChars->IndexOf(value);
                    if (num == -1)
                    {
                        startPos = i;
                        Dbg(String::Concat("findElementStart succeeded @ ", i.ToString()));
                        return true;
                    }
                    startPos = i;
                    Dbg(String::Concat("FindElementStart whitespace barrier @ ", i.ToString()));
                    return false;
                }
            }
            Dbg("FindElementStart failing");
            return false;
        }
        static Boolean ScanElement(String^ str, Int32% startPos, Int32% endPos) { return false; }
        static Boolean SkipElement(String^ str, Int32% scanPos)
        {
            if (!FindElementStart(str, scanPos))
                return false;
            Char c = str[scanPos];
            if (c.Equals(s_OpenAngle))
            {
                String^ elementName = nullptr;
                Boolean isEmpty = false;
                Boolean isClosing = false;
                Int32 startPos = scanPos;
                Int32 endPos = startPos;
                if (ScanTag(str, startPos, endPos, elementName, isEmpty, isClosing))
                {
                    startPos = endPos;
                    if (!isEmpty)
                    {
                        if (isClosing)
                            BadSyntax(str, startPos, "</{0}> seen out of context.", elementName);
                        if (!SkipMatchingTag(str, elementName, endPos))
                            BadSyntax(str, scanPos, "Unable to find matching tag.");
                    }
                }
                else if (!SkipBracketedSGML(str, endPos))
                    BadSyntax(str, scanPos, "Unable to scan tag.");
                scanPos = endPos;
                return true;
            }
            BadSyntax(str, scanPos, "Bad character where '<' expected.");
            return false;
        }
        static Boolean ScanTag(String^ str, Int32% startPos, Int32% endPos,
            String^% elementName, Boolean% isEmpty, Boolean% isClosing)
        {
            Boolean flag = false;
            Boolean flag2 = false;
            Int32 num = startPos;
            Int32 num2 = num;
            elementName = nullptr;
            endPos = startPos;
            isEmpty = false;
            isClosing = false;
            if (num2 == str->Length || !str[num2].Equals(s_OpenAngle))
                BadSyntax(str, startPos, "Tag must start with '<'");
            num2++;
            if (num2 == str->Length)
                BadSyntax(str, startPos, "Premature end of stream while reading tag.");
            if (str[num2].Equals(s_Slash))
            {
                num2++;
                flag2 = true;
            }
            else if (str[num2].Equals(s_Excl))
                return false;
            Int32 i;
            if (!ScanTagName(str, num2, i))
                BadSyntax(str, startPos, "Missing tag name.");
            String^ text = str->Substring(num2, i - num2);
            endPos = startPos;
            for (; i < str->Length; i++)
            {
                Char c = str[i];
                if (c.Equals(s_Slash))
                {
                    flag = true;
                    continue;
                }
                if (c.Equals(s_CloseAngle))
                {
                    if (text == nullptr)
                        BadSyntax(str, startPos, "Missing tag name.");
                    elementName = text;
                    isEmpty = flag;
                    isClosing = flag2;
                    startPos = num;
                    endPos = i + 1;
                    Dbg("Found tag {0} @ {1}", str->Substring(startPos, endPos - startPos), endPos);
                    return true;
                }
                flag = false;
                if (c.Equals(s_SingleQuote) || c.Equals(s_DoubleQuote))
                {
                    if (!ScanAttributeString(str, i))
                        BadSyntax(str, i, "Bad string literal.");
                    i--;
                }
            }
            BadSyntax(str, startPos, "Missing '>' in tag.");
            return false;
        }

        static Boolean ScanTagName(String^ str, Int32% startPos, Int32% endPos)
        {
            Int32 i;
            for (i = startPos; i < str->Length; i++)
                if (s_TagBreakChars->IndexOf(str[i]) < 0)
                    break;
            endPos = startPos;
            for (Int32 j = i; j < str->Length; j++)
            {
                if (s_TagBreakChars->IndexOf(str[j]) >= 0)
                {
                    if (j == startPos)
                        BadSyntax(str, startPos, "Missing tag name.");
                    startPos = i;
                    endPos = j;
                    Dbg("ScanTagName win @ {0}", endPos);
                    return true;
                }
            }
            BadSyntax(str, startPos, "No end to tag name.");
            return false;
        }

        static Boolean ScanAttributeString(String^ str, Int32% scanPos)
        {
            Dbg("Scanning attribute string @ {0}", scanPos);
            if (scanPos >= str->Length)
                BadSyntax(str, scanPos, "Scan string at end of file.");
            Int32 num = scanPos;
            Char obj = str[num];
            if (!obj.Equals(s_SingleQuote) && !obj.Equals(s_DoubleQuote))
                BadSyntax(str, scanPos, "Trying to ScanAttributeString on non-String");
            for (num++; num < str->Length; num++)
            {
                Char c = str[num];
                if (c.Equals(s_CloseAngle))
                {
                    BadSyntax(str, num, "Found '>' in attribute value. Should be &gt;.");
                    return false;
                }
                if (c.Equals(obj))
                {
                    scanPos = num + 1;
                    Dbg("ScanAttributeString win @ {0}", num);
                    return true;
                }
            }
            BadSyntax(str, scanPos, "String not closed.");
            return false;
        }

        static Boolean SkipMatchingTag(String^ str, String^ elementName, Int32% scanPos)
        {
            if (elementName == nullptr || elementName->Length == 0)
                throw gcnew Exception("Matching null tag?");
            Int32 num = 1;
            for (Int32 i = scanPos; i < str->Length; i++)
            {
                Char c = str[i];
                if (!c.Equals(s_OpenAngle))
                    continue;
                String^ elementName2 = nullptr;
                Int32 endPos;
                Int32 startPos = i;
                Boolean isEmpty;
                Boolean isClosing;
                if (ScanTag(str, startPos, endPos, elementName2, isEmpty, isClosing))
                {
                    if (elementName2 != nullptr && elementName2->Equals(elementName) && !isEmpty)
                    {
                        if (isClosing)
                        {
                            scanPos = endPos;
                            num--;
                            if (num < 1)
                            {
                                Dbg(String::Concat("Found ", elementName, " matching tag @ ", endPos.ToString()));
                                return true;
                            }
                        }
                        else
                            num++;
                    }
                }
                else if (SkipCDATA(str, i))
                    i--;
                else if (SkipSgmlComment(str, i))
                    i--;
                else
                    BadSyntax(str, scanPos, "Stray <");
            }
            BadSyntax(str, scanPos, "Can't find </{0}>", elementName);
            return false;
        }
        static Boolean SkipBracketedSGML(String^ str, Int32% scanPos) { return false; }
        static Boolean SkipSgmlComment(String^ str, Int32% scanPos)
        {
            Int32 i = scanPos;
            Dbg("SgmlComment entered, pos={0}", i);
            RequireText(str, i, "<!");
            Int32 num = 0;
            Boolean flag = false;
            for (; i < str->Length; i++)
            {
                Char value = str[i];
                if (value.Equals(s_Hyphen))
                {
                    num++;
                    if (num == 2)
                    {
                        num = 0;
                        flag = !flag;
                    }
                    continue;
                }
                num = 0;
                if (!flag)
                {
                    if (value.Equals(s_CloseAngle))
                    {
                        scanPos = i + 1;
                        Dbg("SgmlCommentEnd = {0}", scanPos);
                        return true;
                    }
                    if (s_WhitepaceChars->IndexOf(value) < 0)
                        BadSyntax(str, i, "Strange state of SGML comment.");
                }
            }
            return false;
        }
        static String^ StripXmlnsAttrs(String^ str)
        {
            if (str->IndexOf("xmlns") < 0)
                return str;
            System::Text::StringBuilder^ stringBuilder = gcnew System::Text::StringBuilder();
            System::Collections::Generic::Queue<Triple<Int32, Int32, Int32>^>^ skipAttrs;
            Int32 num = 0;
            Int32 num2 = num;
            while (FindCandidateXmlnsAttr(str, num2))
            {
                stringBuilder->Append(str->Substring(num, num2 - num));
                num = num2;
                if (!FindXmlnsAttrs(str, num2, skipAttrs))
                    return str;
                auto enumerator = skipAttrs->GetEnumerator();
                while (enumerator.MoveNext())
                {
                    Triple<Int32, Int32, Int32>^ item = enumerator.Current;
                    Int32 first = item->First;
                    GC::KeepAlive(item->Second);
                    Int32 third = item->Third;
                    stringBuilder->Append(str->Substring(num, first - num));
                    num = third;
                }
                num2 = num;
            }
            stringBuilder->Append(str->Substring(num2));
            return stringBuilder->ToString();
        }
        static Boolean EquivalentXml(String^ xml1, String^ xml2) { return false; }
        static String^ TightenXmlWhitespace(String^ rawstr)
        {
            String^ text = s_TightenXmlnsAttributes ? StripXmlnsAttrs(rawstr) : rawstr;
            Int32 i = 0;
            Int32 length = text->Length;
            Boolean flag = false;
            System::Text::StringBuilder^ stringBuilder = gcnew System::Text::StringBuilder();
            for (; i < length; i++)
            {
                Char value = text[i];
                Int32 num = i;
                Int32 scanPos = i;
                if (value.Equals(s_OpenAngle))
                {
                    flag = false;
                    if (LookingAtText(text, i, "<!") && SkipBracketedSGML(text, scanPos))
                    {
                        stringBuilder->Append(text, i, scanPos - i);
                        i = scanPos - 1;
                        continue;
                    }
                    num = i + 1;
                    scanPos = text->IndexOf(s_CloseAngle, num + 1);
                    if (scanPos < 0)
                        return text;
                    if (text->IndexOf("xml:space=\"preserve\"", num, scanPos - num) >= 0 ||
                        text->IndexOf("xml:space='preserve'", num, scanPos - num) >= 0)
                    {
                        num = i;
                        if (!ScanElement(text, num, scanPos))
                            return text;
                        stringBuilder->Append(text, i, scanPos - i);
                        i = scanPos - 1;
                    }
                    else
                        stringBuilder->Append(value);
                }
                else if (value.Equals(s_CloseAngle))
                {
                    flag = false;
                    stringBuilder->Append(value);
                    i++;
                    SkipWhitespace(text, i);
                    i--;
                }
                else if (s_WhitepaceChars->IndexOf(value) >= 0)
                {
                    flag = true;
                    i++;
                    SkipWhitespace(text, i);
                    i--;
                }
                else
                {
                    if (flag)
                        stringBuilder->Append(' ');
                    flag = false;
                    stringBuilder->Append(value);
                }
            }
            return stringBuilder->ToString();
        }
        static Boolean FindXmlnsAttrs(String^ str, Int32% scanPos,
            System::Collections::Generic::Queue<Triple<Int32, Int32, Int32>^>^% skipAttrs)
        {
            Int32 num = scanPos;
            Int32 i;
            Int32 num2 = -1;
            Int32 num3 = 0;
            String^ text;
            skipAttrs = gcnew System::Collections::Generic::Queue<Triple<Int32, Int32, Int32>^>();
            if (num == str->Length || !str[num].Equals(s_OpenAngle))
                BadSyntax(str, scanPos, "Tag must start with '<'");
            num++;
            if (num == str->Length)
                BadSyntax(str, scanPos, "Premature end of stream while reading tag.");
            if (str[num].Equals(s_Slash))
                num++;
            else if (str[num].Equals(s_Excl))
                return false;
            if (!ScanTagName(str, num, i))
                BadSyntax(str, scanPos, "Missing tag name.");
            text = str->Substring(num, i - num);
            for (; i < str->Length; i++)
            {
                Char value = str[i];
                if (value.Equals(s_Slash))
                {
                    num2 = -1;
                    continue;
                }
                if (s_WhitepaceChars->IndexOf(value) >= 0)
                {
                    num2 = -1;
                    continue;
                }
                if (value.Equals(s_EqualSign))
                {
                    num3 = i;
                    continue;
                }
                if (value.Equals(s_CloseAngle))
                {
                    num2 = -1;
                    if (text == nullptr)
                        BadSyntax(str, scanPos, "Missing tag name.");
                    scanPos = i + 1;
                    return true;
                }
                if (value.Equals(s_SingleQuote) || value.Equals(s_DoubleQuote))
                {
                    if (!ScanAttributeString(str, i))
                        BadSyntax(str, i, "Bad string literal.");
                    if (num2 >= 0)
                    {
                        String^ text2 = str->Substring(num2, num3 - num2);
                        if (text2->Equals("xmlns") || text2->StartsWith("xmlns:"))
                            skipAttrs->Enqueue(gcnew Triple<Int32, Int32, Int32>(BackOverWhitespace(str, num2), num2, i));
                    }
                    i--;
                }
                else if (num2 < 0)
                    num2 = i;
            }
            BadSyntax(str, scanPos, "Missing '>' in tag.");
            return false;
        }
        static Boolean SkipCDATA(String^ str, Int32% scanPos)
        {
            Int32 i = scanPos;
            Int32 num = 0;
            if (!RequireText(str, i, "<![CDATA["))
            {
                Dbg("CDATA failing @ {0} (no prefix), not updating scan pointer", i);
                return false;
            }
            for (; i < str->Length; i++)
            {
                Char c = str[i];
                if (c.Equals(s_CloseBracket))
                {
                    num++;
                    Dbg("CDATA bracket count = {0} @ pos {1}", num, i);
                    if (num > 2)
                    {
                        num = 2;
                        Dbg("CDATA Resetting bracket count to 2 @ pos {0}", i);
                    }
                }
                else
                {
                    if (num == 2 && c.Equals(s_CloseAngle))
                    {
                        i++;
                        scanPos = i;
                        Dbg("CDATA winning @ {0}, updating scan pointer", i);
                        return true;
                    }
                    num = 0;
                }
            }
            Dbg("CDATA failing @ {0}, not updating scan pointer", i);
            return false;
        }
        static Boolean UnorderedEqual(String^ actual, String^ expected) { return false; }
        static Boolean UnorderedEqual(String^ xml1, String^ xml2, Boolean exact)
        {
            if (xml1->Equals(xml2))
                return true;
            Int32 startPos = 0;
            Int32 startPos2 = 0;
            Int32 endPos;
            Int32 endPos2;
            String^ elementName = nullptr;
            String^ elementName2 = nullptr;
            Boolean isEmpty;
            Boolean isEmpty2;
            Boolean isClosing;
            Boolean isClosing2;
            if (!ScanTag(xml1, startPos, endPos, elementName, isEmpty, isClosing))
                return false;
            if (!ScanTag(xml2, startPos2, endPos2, elementName2, isEmpty2, isClosing2))
                return false;
            if (isClosing || isClosing2)
                return false;
            if (elementName != elementName2)
                return false;
            String^ text = OrderedArbitrarily(xml1);
            String^ text2 = OrderedArbitrarily(xml2);
            if (exact)
                return text == text2;
            return EquivalentXml(text, text2);
        }
        static String^ OrderedArbitrarily(String^ xml)
        {
            Int32 startPos = 0;
            Int32 endPos;
            String^ elementName = nullptr;
            Boolean isEmpty;
            Boolean isClosing;
            if (!ScanTag(xml, startPos, endPos, elementName, isEmpty, isClosing))
                BadSyntax(xml, startPos, "Element opening tag is ill-formed.");
            if (isEmpty)
                return String::Format("{0}></{1}>", xml->Substring(startPos, endPos - 2));
            if (isClosing)
                BadSyntax(xml, startPos, "Element starts with close tag?");
            Int32 num = xml->LastIndexOf('<');
            if (num < endPos)
                BadSyntax(xml, startPos, "Element in unexpected format.");
            System::Collections::SortedList^ sortedList = gcnew System::Collections::SortedList();
            Int32 startPos2 = endPos;
            Int32 endPos2;
            while (startPos2 < num && ScanElement(xml, startPos2, endPos2))
            {
                String^ text = xml->Substring(startPos2, endPos2 - startPos2);
                sortedList->Add(text, text);
                startPos2 = endPos2;
            }
            System::Text::StringBuilder^ stringBuilder = gcnew System::Text::StringBuilder();
            stringBuilder->Append(xml, 0, endPos);
            for each (String^ value in sortedList->GetValueList())
                stringBuilder->Append(value);
            stringBuilder->Append(xml, num, xml->Length - num);
            return stringBuilder->ToString();
        }
        static void TestInit() {}
        static void TestCheck(String^ str, Boolean result, Boolean expected_result, Int32 expected_pos)
        {
            if (result != expected_result)
                Console::WriteLine("Test #{0} failed. res={1}, expected={2}, pos={3}", s_TestNum, result, expected_result, s_TestPos);
            else if (!expected_result || s_TestPos != expected_pos)
                Console::WriteLine("Test #{0} failed. pos={1}, expected={2}", s_TestNum, s_TestPos, expected_pos);
            else
                s_TestsPassed++;
            BumpTest();
        }
        static void TestCheck(Boolean value) {}
        static void BumpTest() {}
        static void TestFinalize()
        {
            String^ text = s_TestsPassed == s_TestsDone ? "Passed." : "Failed.";
            Console::WriteLine("{0} {1}/{2} tests succeeded.", text, s_TestsPassed, s_TestsDone);
        }
        static void SelfTest(array<String^>^ ignored_data)
        {
            TestInit();
            String^ str;
            str = "<!--foo-->a"; TestCheck(str, SkipSgmlComment(str, s_TestPos), true, 10);
            str = "<!--foo>-->a"; TestCheck(str, SkipSgmlComment(str, s_TestPos), true, 11);
            str = "<!--foo<-->a"; TestCheck(str, SkipSgmlComment(str, s_TestPos), true, 11);
            str = "<![CDATA[foo]]>a"; TestCheck(str, SkipCDATA(str, s_TestPos), true, 15);
            str = "<![CDATA[foo]>]]>a"; TestCheck(str, SkipCDATA(str, s_TestPos), true, 17);
            str = "<![CDATA[foo]]]]>a"; TestCheck(str, SkipCDATA(str, s_TestPos), true, 17);
            str = "<![CDATA[foo]>]]]>a"; TestCheck(str, SkipCDATA(str, s_TestPos), true, 18);
            str = "<x>foo</x>a"; TestCheck(str, SkipElement(str, s_TestPos), true, 10);
            str = "<x>foo</x>a"; TestCheck(str, SkipElement(str, s_TestPos), true, 10);
            str = "<x>foo<y/></x>a"; TestCheck(str, SkipElement(str, s_TestPos), true, 14);
            str = "<x>foo<y></x>a"; TestCheck(str, SkipElement(str, s_TestPos), true, 13);
            str = "<x>foo<x/>bar</x>a"; TestCheck(str, SkipElement(str, s_TestPos), true, 17);
            str = "<x>foo<x>bar</x>baz</x>a"; TestCheck(str, SkipElement(str, s_TestPos), true, 23);
            str = "<x foo=\"xxx\"/>abc</x>"; TestCheck(str, SkipElement(str, s_TestPos), true, 14);
            str = "<x foo='xxx'/>abc</x>"; TestCheck(str, SkipElement(str, s_TestPos), true, 14);
            TestCheck(EquivalentXml("<foo x='3'   y='4' />", "<foo x='3' y='4'   />"));
            TestCheck(EquivalentXml("<foo  x='3'  >  <bar xml:space=\"preserve\">  abc </bar> </foo>",
                "<foo x='3'><bar xml:space=\"preserve\">  abc </bar></foo>"));
            TestCheck(TightenXmlWhitespace("<foo x='3'   y='4' />") == "<foo x='3' y='4' />");
            TestCheck(TightenXmlWhitespace("<foo  x='3'  >  <bar xml:space=\"preserve\">  abc </bar> </foo>") ==
                "<foo x='3'><bar xml:space=\"preserve\">  abc </bar></foo>");
            TestCheck(TightenXmlWhitespace("<foo  x='3'  >  <bar >  abc\n   def   </bar> </foo>") ==
                "<foo x='3'><bar>abc def</bar></foo>");
            TestFinalize();
            String^ text = "<x y=\"alpha\">a xmlns=\"gack\" or xmlns:zowie=\"grumble\" is cool.<q/></x>";
            String^ text2 = "<x xmlns=\"foo\" y=\"alpha\" xmlns:zip=\"xyz\">a xmlns=\"gack\" or xmlns:zowie=\"grumble\" is cool.<q xmlns:zing=\"hm\"/></x>";
            Console::WriteLine("Trying Xmlns tests...");
            String^ text3 = StripXmlnsAttrs(text2);
            if (text3->Equals(text))
                Console::WriteLine("OK: {0} ==> {1}", text2, text3);
            else
                Console::WriteLine("FAILED: {0} != {1}", text3, text);
        }
        XmlUtils() {}
    };
#pragma warning(pop)
}}}

namespace Quire { namespace utl_mgd {

}}
namespace Quire { namespace Engine { namespace Misc {

#pragma warning(push)
#pragma warning(disable:4100 4702)
    [Quire::Mirror::MirrorOfAttribute("Engine.Misc.LispUtils, mcmisc")]
    public ref class LispUtils
    {
    private:
        static Boolean s_Debug;
        static String^ s_LispWhitepaceChars;
        static String^ s_LispHorizontalWhitespaceChars;
        static String^ s_LispVerticalWhitespaceChars;
        static String^ s_LispBreakChars;
        static Char s_EscapeChar;
        static Char s_OpenParen;
        static Char s_CloseParen;
        static Char s_StringQuote;
        static Char s_CommentChar;
        static array<Char>^ s_SymbolSeps;
        static array<String^>^ s_Definers;

    public:
        ref class LinePartition;

    private:
        static LispUtils()
        {
            s_Debug = false;
            s_LispWhitepaceChars = " \t\f\n\r";
            s_LispHorizontalWhitespaceChars = " \t";
            s_LispVerticalWhitespaceChars = "\f\n\r";
            s_LispBreakChars = " \t\f\n\r()\";";
            s_EscapeChar = '\\'; s_OpenParen = '('; s_CloseParen = ')';
            s_StringQuote = '"'; s_CommentChar = ';';
            s_SymbolSeps = gcnew array<Char>{ '{', ':' };
            s_Definers = gcnew array<String^>{ "define", "global", "define-eval", "lambda" };
        }

    public:
        static void badSyntax(String^ msg, String^ str, Int32 pos)
        {
            auto nearby = gcnew System::Text::StringBuilder();
            Int32 lineNumber = 1, index = 0, limit = Math::Min(Math::Max(pos, 0), str->Length);
            while (index < limit)
            {
                Char c = str[index++];
                if (c == '\n') { lineNumber++; nearby->Length = 0; }
                else nearby->Append(c);
            }
            while (index < str->Length && str[index] != '\n' && str[index] != '\r') nearby->Append(str[index++]);
            Console::WriteLine("Bad syntax ({0}) @ pos={1} line={2}\n Context (line {2}): {3}",
                gcnew array<Object^>{ msg, pos, lineNumber, nearby->ToString() });
            throw gcnew Exception("bad_syntax");
        }

    private:
        static void Dbg(String^ str) { if (s_Debug) Console::WriteLine(str); }

    public:
        static Boolean scanExpression(String^ str, Int32% startPos, Int32% endPos)
        {
            Int32 start = startPos, end = startPos;
            endPos = startPos;
            if (!findExpressionStart(str, start) || !skipExpression(str, start)) return false;
            end = start;
            start = startPos;
            if (!findExpressionStart(str, start)) return false;
            endPos = end; startPos = start; return true;
        }
        static Boolean skipExpression(String^ str, Int32% startPos)
        {
            Dbg("str=" + str + " startPos=" + startPos);
            if (!findExpressionStart(str, startPos)) { Dbg("skipExpression found no start."); return false; }
            return str[startPos] == s_OpenParen ? skipList(str, startPos) : skipAtom(str, startPos);
        }
        static Boolean skipList(String^ str, Int32% startPos)
        {
            Boolean escaped = false; Int32 pos = startPos, level = 0;
            Dbg("skipList entered, level=0, pos=" + startPos);
            if (!findExpressionStart(str, pos)) { Dbg("skipList found no start."); return false; }
            if (str[pos] != s_OpenParen) badSyntax("missing open paren at start of skipList", str, pos);
            level++;
            for (pos++; pos < str->Length; pos++)
            {
                Char c = str[pos];
                if (escaped) escaped = false;
                else if (c == s_EscapeChar) escaped = true;
                else if (c == s_CommentChar)
                {
                    if (!skipLine(str, pos)) break;
                    pos--;
                }
                else if (c == s_StringQuote)
                {
                    if (!skipString(str, pos)) break;
                    pos--;
                }
                else if (c == s_OpenParen) level++;
                else if (c == s_CloseParen && --level == 0) { startPos = pos + 1; return true; }
            }
            return false;
        }
        static Boolean skipAtom(String^ str, Int32% startPos)
        {
            Boolean escaped = false; Int32 pos = startPos;
            if (!findExpressionStart(str, pos)) return false;
            if (str[pos] == s_StringQuote)
            {
                if (!skipString(str, pos)) return false;
                startPos = pos; return true;
            }
            if (s_LispBreakChars->IndexOf(str[pos]) >= 0) badSyntax("skipAtom found no start of atom", str, pos);
            for (; pos < str->Length; pos++)
            {
                Char c = str[pos];
                if (escaped) escaped = false;
                else if (c == s_EscapeChar) escaped = true;
                else if (s_LispBreakChars->IndexOf(c) >= 0) { startPos = pos; return true; }
            }
            if (!escaped && pos != startPos) { startPos = pos; return true; }
            return false;
        }
        static Boolean findExpressionStart(String^ str, Int32% startPos) { return findExpressionStart(str, startPos, s_LispWhitepaceChars); }
        static Boolean findExpressionStart(String^ str, Int32% startPos, String^ whitespace)
        {
            for (Int32 pos = startPos; pos < str->Length; pos++)
            {
                Char c = str[pos];
                if (c == s_CommentChar) { if (!skipLine(str, pos)) break; pos--; continue; }
                if (whitespace->IndexOf(c) < 0)
                {
                    if (s_LispWhitepaceChars->IndexOf(c) >= 0) { startPos = pos; return false; }
                    startPos = pos; return true;
                }
            }
            return false;
        }
        static Boolean skipLine(String^ str, Int32% startPos)
        {
            Int32 pos = startPos;
            for (; pos < str->Length; pos++) if (str[pos] == '\n') { startPos = pos + 1; return true; }
            startPos = pos; return false;
        }
        static Boolean skipString(String^ str, Int32% startPos)
        {
            if (str[startPos] != s_StringQuote) badSyntax("missing string quote in skipString", str, startPos);
            for (Int32 pos = startPos + 1; pos < str->Length; pos++)
            {
                Char c = str[pos];
                if (c == s_EscapeChar) pos++;
                else if (c == s_StringQuote) { startPos = pos + 1; return true; }
            }
            return false;
        }
        static System::Collections::Generic::Queue<String^>^ scanExpressionLine(String^ s) { Int32 cursor = 0; return scanExpressionLine(s, cursor); }
        static System::Collections::Generic::Queue<String^>^ scanExpressionLine(String^ s, Int32% cursor)
        {
            auto queue = gcnew System::Collections::Generic::Queue<String^>();
            Int32 start = cursor, end = cursor;
            if (!findExpressionStart(s, start)) return nullptr;
            while (findExpressionStart(s, start, s_LispHorizontalWhitespaceChars))
            {
                end = start;
                if (s_LispVerticalWhitespaceChars->IndexOf(s[start]) >= 0) { skipLine(s, start); break; }
                if (!scanExpression(s, start, end)) badSyntax("ill-formed expression", s, start);
                queue->Enqueue(s->Substring(start, end - start)); start = end;
            }
            cursor = end; return queue;
        }
        static System::Collections::Generic::Queue<String^>^ scanExpressionLine(String^ s, Int32% cursor, Boolean permitXML) { return nullptr; }
        static LinePartition^ partitionExpressionLine(String^ s, Int32% cursor, Boolean permitXML) { return nullptr; }
        static System::Collections::Generic::Queue<String^>^ scanExpressions(String^ s) { return scanExpressions(s, 0); }
        static System::Collections::Generic::Queue<String^>^ scanExpressions(String^ s, Int32 startPos)
        {
            auto queue = gcnew System::Collections::Generic::Queue<String^>(); Int32 start = startPos, end = startPos;
            while (scanExpression(s, start, end)) { queue->Enqueue(s->Substring(start, end - start)); start = end; }
            return queue;
        }

    private:
        static void PrettyPrint(String^ exp) { }

    public:
        static String^ PrettyString(String^ exp) { return nullptr; }
        static String^ PrettyStringTop(String^ exp, Int32 target_indentation) { return nullptr; }
        static Boolean IsDefinerName(String^ op_name) { return false; }
        static String^ IdentifierName(String^ op_name) { return nullptr; }

    private:
        static void PrettyStringWorker(String^ exp, Boolean top, Int32% spos, Int32% epos,
            Int32 indent, System::IO::TextWriter^ writer) { }

    public:
        static void IndentTo(Int32 n, System::IO::TextWriter^ writer) { }
        static String^ SelfTest(array<String^>^ args) { return nullptr; }
        LispUtils() {}

        [Quire::Mirror::MirrorOfAttribute("Engine.Misc.LispUtils+LinePartition, mcmisc")]
        ref class LinePartition
        {
        private:
            String^ m_Line;
            System::Collections::Generic::Queue<Quire::Engine::Misc::Pair<Int32, Int32>^>^ m_Locs;

        public:
            LinePartition(String^ line) { m_Line = line; m_Locs = gcnew System::Collections::Generic::Queue<Quire::Engine::Misc::Pair<Int32, Int32>^>(); }
            property String^ Line { String^ get() { return m_Line; } }
            property Int32 Count { Int32 get() { return m_Locs->Count; } }
            void PutSegment(Int32 startPos, Int32 endPos) { m_Locs->Enqueue(gcnew Quire::Engine::Misc::Pair<Int32, Int32>(startPos, endPos)); }
            String^ PeekSegment() { auto pair = m_Locs->Peek(); return m_Line->Substring(pair->First, pair->Second - pair->First); }
            String^ TakeSegment() { auto pair = m_Locs->Dequeue(); return m_Line->Substring(pair->First, pair->Second - pair->First); }
            System::Collections::Generic::Queue<String^>^ Segments() { return SegmentsFrom(0); }
            System::Collections::Generic::Queue<String^>^ SegmentsFrom(Int32 n)
            {
                auto result = gcnew System::Collections::Generic::Queue<String^>(); Int32 i = 0;
                for each (Quire::Engine::Misc::Pair<Int32, Int32>^ pair in m_Locs)
                    if (i >= n) result->Enqueue(m_Line->Substring(pair->First, pair->Second - pair->First));
                return result;
            }
            String^ JoinedText() { return JoinedTextFrom(0); }
            String^ JoinedTextFrom(Int32 n)
            {
                Int32 start = m_Line->Length, end = m_Line->Length;
                if (n >= m_Locs->Count) return "";
                if (n < 0) n = 0;
                Int32 i = -1;
                for each (Quire::Engine::Misc::Pair<Int32, Int32>^ pair in m_Locs)
                {
                    if (++i == n) start = pair->First;
                    end = pair->Second;
                }
                return m_Line->Substring(start, end - start);
            }
        };
    };

    [Quire::Mirror::MirrorOfAttribute("Engine.Misc.FileUtils, mcmisc")]
    public ref class FileUtils
    {
    private:
        static String^ s_Char_Alpha;
        static String^ s_Char_alpha;
        static String^ s_Char_Beta;
        static String^ s_Char_beta;
        static String^ s_Char_Gamma;
        static String^ s_Char_gamma;
        static String^ s_Char_Delta;
        static String^ s_Char_delta;
        static String^ s_Char_Epsilon;
        static String^ s_Char_epsilon;
        static String^ s_Char_Zeta;
        static String^ s_Char_zeta;
        static String^ s_Char_Eta;
        static String^ s_Char_eta;
        static String^ s_Char_Theta;
        static String^ s_Char_theta;
        static String^ s_Char_Iota;
        static String^ s_Char_iota;
        static String^ s_Char_Kappa;
        static String^ s_Char_kappa;
        static String^ s_Char_Lambda;
        static String^ s_Char_lambda;
        static String^ s_Char_Mu;
        static String^ s_Char_mu;
        static String^ s_Char_Nu;
        static String^ s_Char_nu;
        static String^ s_Char_Xi;
        static String^ s_Char_xi;
        static String^ s_Char_Omicron;
        static String^ s_Char_omicron;
        static String^ s_Char_Pi;
        static String^ s_Char_pi;
        static String^ s_Char_Rho;
        static String^ s_Char_rho;
        static String^ s_Char_Sigma;
        static String^ s_Char_sigma;
        static String^ s_Char_Tau;
        static String^ s_Char_tau;
        static String^ s_Char_Upsilon;
        static String^ s_Char_upsilon;
        static String^ s_Char_Phi;
        static String^ s_Char_phi;
        static String^ s_Char_Chi;
        static String^ s_Char_chi;
        static String^ s_Char_Psi;
        static String^ s_Char_psi;
        static String^ s_Char_Omega;
        static String^ s_Char_omega;
        static String^ s_Char_phi_alt;
        static String^ s_Char_pi_alt;
        static String^ s_Char_sigma_alt;
        static String^ s_Char_theta_alt;
        static String^ s_Char_bottom;
        static String^ s_Char_ohm;
        static String^ s_Char_currency;
        static String^ s_Char_yen;
        static String^ s_Char_deg;
        static String^ s_Char_hbar;
        static String^ s_Char_infinity;
        static String^ s_Char_element_of;
        static String^ s_Char_hooked_f;
        static String^ s_Char_right_arrow;
        static String^ s_Char_left_arrow;
        static String^ s_Char_copyright;
        static String^ s_Char_trademark;
        static String^ s_Char_registered;
        static String^ s_Char_DoubleStruckC;
        static String^ s_Char_DoubleStruckQ;
        static String^ s_Char_DoubleStruckR;
        static String^ s_Char_DoubleStruckZ;
        static String^ s_Start_Subscript;
        static String^ s_End_Subscript;
        static String^ s_Start_Superscript;
        static String^ s_End_Superscript;
        static array<Char>^ s_CharMappings;
        static System::Collections::Generic::Dictionary<Char, Char>^ s_AsciiToUnicodeTable;
        static System::Collections::Generic::Dictionary<Char, Char>^ s_UnicodeToAsciiTable;
        static initonly array<String^>^ s_NameMappings;
        static System::Collections::Generic::Dictionary<String^, String^>^ s_CharNameToUnicodeStringTable;
        static System::Collections::Generic::Dictionary<Char, String^>^ s_UnicodeToCharNameTable;
        static System::Collections::Generic::List<Quire::Engine::Misc::Pair<String^, Char>^>^ s_LongUnicodeToShortcutKeyTable;
        static System::Collections::Generic::Dictionary<Char, Char>^ s_GreekShortcutKeyToUnicodeTable;
        static System::Collections::Generic::Dictionary<Char, Char>^ s_UnicodeToGreekShortcutKeyTable;
        static Boolean s_bNamedCharMapInitialized;

        static FileUtils()
        {
            s_Char_Alpha = gcnew String(L"\u0391");
            s_Char_alpha = gcnew String(L"\u03b1");
            s_Char_Beta = gcnew String(L"\u0392");
            s_Char_beta = gcnew String(L"\u03b2");
            s_Char_Gamma = gcnew String(L"\u0393");
            s_Char_gamma = gcnew String(L"\u03b3");
            s_Char_Delta = gcnew String(L"\u0394");
            s_Char_delta = gcnew String(L"\u03b4");
            s_Char_Epsilon = gcnew String(L"\u0395");
            s_Char_epsilon = gcnew String(L"\u03b5");
            s_Char_Zeta = gcnew String(L"\u0396");
            s_Char_zeta = gcnew String(L"\u03b6");
            s_Char_Eta = gcnew String(L"\u0397");
            s_Char_eta = gcnew String(L"\u03b7");
            s_Char_Theta = gcnew String(L"\u0398");
            s_Char_theta = gcnew String(L"\u03b8");
            s_Char_Iota = gcnew String(L"\u0399");
            s_Char_iota = gcnew String(L"\u03b9");
            s_Char_Kappa = gcnew String(L"\u039a");
            s_Char_kappa = gcnew String(L"\u03ba");
            s_Char_Lambda = gcnew String(L"\u039b");
            s_Char_lambda = gcnew String(L"\u03bb");
            s_Char_Mu = gcnew String(L"\u039c");
            s_Char_mu = gcnew String(L"\u03bc");
            s_Char_Nu = gcnew String(L"\u039d");
            s_Char_nu = gcnew String(L"\u03bd");
            s_Char_Xi = gcnew String(L"\u039e");
            s_Char_xi = gcnew String(L"\u03be");
            s_Char_Omicron = gcnew String(L"\u039f");
            s_Char_omicron = gcnew String(L"\u03bf");
            s_Char_Pi = gcnew String(L"\u03a0");
            s_Char_pi = gcnew String(L"\u03c0");
            s_Char_Rho = gcnew String(L"\u03a1");
            s_Char_rho = gcnew String(L"\u03c1");
            s_Char_Sigma = gcnew String(L"\u03a3");
            s_Char_sigma = gcnew String(L"\u03c3");
            s_Char_Tau = gcnew String(L"\u03a4");
            s_Char_tau = gcnew String(L"\u03c4");
            s_Char_Upsilon = gcnew String(L"\u03a5");
            s_Char_upsilon = gcnew String(L"\u03c5");
            s_Char_Phi = gcnew String(L"\u03a6");
            s_Char_phi = gcnew String(L"\u03c6");
            s_Char_Chi = gcnew String(L"\u03a7");
            s_Char_chi = gcnew String(L"\u03c7");
            s_Char_Psi = gcnew String(L"\u03a8");
            s_Char_psi = gcnew String(L"\u03c8");
            s_Char_Omega = gcnew String(L"\u03a9");
            s_Char_omega = gcnew String(L"\u03c9");
            s_Char_phi_alt = gcnew String(L"\u03d5");
            s_Char_pi_alt = gcnew String(L"\u03d6");
            s_Char_sigma_alt = gcnew String(L"\u03c2");
            s_Char_theta_alt = gcnew String(L"\u03d1");
            s_Char_bottom = gcnew String(L"\u22a5");
            s_Char_ohm = gcnew String(L"\u2126");
            s_Char_currency = gcnew String(L"\u00a4");
            s_Char_yen = gcnew String(L"\u00a5");
            s_Char_deg = gcnew String(L"\u00b0");
            s_Char_hbar = gcnew String(L"\u210f");
            s_Char_infinity = gcnew String(L"\u221e");
            s_Char_element_of = gcnew String(L"\u2208");
            s_Char_hooked_f = gcnew String(L"\u0192");
            s_Char_right_arrow = gcnew String(L"\u2192");
            s_Char_left_arrow = gcnew String(L"\u2190");
            s_Char_copyright = gcnew String(L"\u00a9");
            s_Char_trademark = gcnew String(L"\u2122");
            s_Char_registered = gcnew String(L"\u00ae");
            s_Char_DoubleStruckC = gcnew String(L"\u2102");
            s_Char_DoubleStruckQ = gcnew String(L"\u211a");
            s_Char_DoubleStruckR = gcnew String(L"\u211d");
            s_Char_DoubleStruckZ = gcnew String(L"\u2124");
            s_Start_Subscript = gcnew String(L"\u0086\u208a");
            s_End_Subscript = gcnew String(L"\u0087\u208a");
            s_Start_Superscript = gcnew String(L"\u0086\u208a");
            s_End_Superscript = gcnew String(L"\u0087\u208a");
            s_CharMappings = gcnew array<Char>{
            L'\u0065', L'\u2208', L'\u0068', L'\u210f', L'\u006f', L'\u00b0', L'\u0070', L'\u03c0',
            L'\u0077', L'\u03c9', L'\u0041', L'\u2200', L'\u0043', L'\u2102', L'\u0045', L'\u2203',
            L'\u004f', L'\u2126', L'\u0051', L'\u211a', L'\u0052', L'\u211d', L'\u0056', L'\u2207',
            L'\u0059', L'\u00a5', L'\u005a', L'\u2124', L'\u0024', L'\u00a4', L'\u0038', L'\u221e',
            L'\u0023', L'\u2260', L'\u003c', L'\u2264', L'\u003e', L'\u2265', L'\u005e', L'\u2227',
            L'\u0076', L'\u2228', L'\u0021', L'\u00ac', L'\u007e', L'\u2248', L'\u003d', L'\u2261'
            };
            s_NameMappings = gcnew array<String^>{
            gcnew String(L"\u0061"), nullptr, s_Char_alpha, gcnew String(L"\u0061\u006c\u0070\u0068\u0061"),
            gcnew String(L"\u0041"), nullptr, s_Char_Alpha, gcnew String(L"\u0041\u006c\u0070\u0068\u0061"),
            gcnew String(L"\u0062"), nullptr, s_Char_beta, gcnew String(L"\u0062\u0065\u0074\u0061"),
            gcnew String(L"\u0042"), nullptr, s_Char_Beta, gcnew String(L"\u0042\u0065\u0074\u0061"),
            gcnew String(L"\u0063"), nullptr, s_Char_chi, gcnew String(L"\u0063\u0068\u0069"),
            gcnew String(L"\u0043"), nullptr, s_Char_Chi, gcnew String(L"\u0043\u0068\u0069"),
            gcnew String(L"\u0064"), nullptr, s_Char_delta, gcnew String(L"\u0064\u0065\u006c\u0074\u0061"),
            gcnew String(L"\u0044"), nullptr, s_Char_Delta, gcnew String(L"\u0044\u0065\u006c\u0074\u0061"),
            gcnew String(L"\u0065"), nullptr, s_Char_epsilon, gcnew String(L"\u0065\u0070\u0073\u0069\u006c\u006f\u006e"),
            gcnew String(L"\u0045"), nullptr, s_Char_Epsilon, gcnew String(L"\u0045\u0070\u0073\u0069\u006c\u006f\u006e"),
            gcnew String(L"\u0066"), nullptr, s_Char_phi, gcnew String(L"\u0070\u0068\u0069"),
            gcnew String(L"\u0046"), nullptr, s_Char_Phi, gcnew String(L"\u0050\u0068\u0069"),
            gcnew String(L"\u0067"), nullptr, s_Char_gamma, gcnew String(L"\u0067\u0061\u006d\u006d\u0061"),
            gcnew String(L"\u0047"), nullptr, s_Char_Gamma, gcnew String(L"\u0047\u0061\u006d\u006d\u0061"),
            gcnew String(L"\u0068"), nullptr, s_Char_eta, gcnew String(L"\u0065\u0074\u0061"),
            gcnew String(L"\u0048"), nullptr, s_Char_Eta, gcnew String(L"\u0045\u0074\u0061"),
            gcnew String(L"\u0069"), nullptr, s_Char_iota, gcnew String(L"\u0069\u006f\u0074\u0061"),
            gcnew String(L"\u0049"), nullptr, s_Char_Iota, gcnew String(L"\u0049\u006f\u0074\u0061"),
            gcnew String(L"\u006a"), nullptr, s_Char_phi_alt, gcnew String(L"\u0070\u0068\u0069\u005f\u0061\u006c\u0074"),
            gcnew String(L"\u004a"), nullptr, s_Char_theta_alt, gcnew String(L"\u0074\u0068\u0065\u0074\u0061\u005f\u0061\u006c\u0074"),
            gcnew String(L"\u006b"), nullptr, s_Char_kappa, gcnew String(L"\u006b\u0061\u0070\u0070\u0061"),
            gcnew String(L"\u004b"), nullptr, s_Char_Kappa, gcnew String(L"\u004b\u0061\u0070\u0070\u0061"),
            gcnew String(L"\u006c"), nullptr, s_Char_lambda, gcnew String(L"\u006c\u0061\u006d\u0062\u0064\u0061"),
            gcnew String(L"\u004c"), nullptr, s_Char_Lambda, gcnew String(L"\u004c\u0061\u006d\u0062\u0064\u0061"),
            gcnew String(L"\u006d"), nullptr, s_Char_mu, gcnew String(L"\u006d\u0075"),
            gcnew String(L"\u004d"), nullptr, s_Char_Mu, gcnew String(L"\u004d\u0075"),
            gcnew String(L"\u006e"), nullptr, s_Char_nu, gcnew String(L"\u006e\u0075"),
            gcnew String(L"\u004e"), nullptr, s_Char_Nu, gcnew String(L"\u004e\u0075"),
            gcnew String(L"\u006f"), nullptr, s_Char_omicron, gcnew String(L"\u006f\u006d\u0069\u0063\u0072\u006f\u006e"),
            gcnew String(L"\u004f"), nullptr, s_Char_Omicron, gcnew String(L"\u004f\u006d\u0069\u0063\u0072\u006f\u006e"),
            gcnew String(L"\u0070"), nullptr, s_Char_pi, gcnew String(L"\u0070\u0069"),
            gcnew String(L"\u0050"), nullptr, s_Char_Pi, gcnew String(L"\u0050\u0069"),
            gcnew String(L"\u0071"), nullptr, s_Char_theta, gcnew String(L"\u0074\u0068\u0065\u0074\u0061"),
            gcnew String(L"\u0051"), nullptr, s_Char_Theta, gcnew String(L"\u0054\u0068\u0065\u0074\u0061"),
            gcnew String(L"\u0072"), nullptr, s_Char_rho, gcnew String(L"\u0072\u0068\u006f"),
            gcnew String(L"\u0052"), nullptr, s_Char_Rho, gcnew String(L"\u0052\u0068\u006f"),
            gcnew String(L"\u0073"), nullptr, s_Char_sigma, gcnew String(L"\u0073\u0069\u0067\u006d\u0061"),
            gcnew String(L"\u0053"), nullptr, s_Char_Sigma, gcnew String(L"\u0053\u0069\u0067\u006d\u0061"),
            gcnew String(L"\u0074"), nullptr, s_Char_tau, gcnew String(L"\u0074\u0061\u0075"),
            gcnew String(L"\u0054"), nullptr, s_Char_Tau, gcnew String(L"\u0054\u0061\u0075"),
            gcnew String(L"\u0075"), nullptr, s_Char_upsilon, gcnew String(L"\u0075\u0070\u0073\u0069\u006c\u006f\u006e"),
            gcnew String(L"\u0055"), nullptr, s_Char_Upsilon, gcnew String(L"\u0055\u0070\u0073\u0069\u006c\u006f\u006e"),
            gcnew String(L"\u0076"), nullptr, s_Char_pi_alt, gcnew String(L"\u0070\u0069\u005f\u0061\u006c\u0074"),
            gcnew String(L"\u0056"), nullptr, s_Char_sigma_alt, gcnew String(L"\u0073\u0069\u0067\u006d\u0061\u005f\u0061\u006c\u0074"),
            gcnew String(L"\u0077"), nullptr, s_Char_omega, gcnew String(L"\u006f\u006d\u0065\u0067\u0061"),
            gcnew String(L"\u0057"), nullptr, s_Char_Omega, gcnew String(L"\u004f\u006d\u0065\u0067\u0061"),
            gcnew String(L"\u0078"), nullptr, s_Char_xi, gcnew String(L"\u0078\u0069"),
            gcnew String(L"\u0058"), nullptr, s_Char_Xi, gcnew String(L"\u0058\u0069"),
            gcnew String(L"\u0079"), nullptr, s_Char_psi, gcnew String(L"\u0070\u0073\u0069"),
            gcnew String(L"\u0059"), nullptr, s_Char_Psi, gcnew String(L"\u0050\u0073\u0069"),
            gcnew String(L"\u007a"), nullptr, s_Char_zeta, gcnew String(L"\u007a\u0065\u0074\u0061"),
            gcnew String(L"\u005a"), nullptr, s_Char_Zeta, gcnew String(L"\u005a\u0065\u0074\u0061"),
            nullptr, nullptr, s_Char_currency, gcnew String(L"\u0063\u0075\u0072\u0072\u0065\u006e\u0063\u0079"),
            nullptr, nullptr, s_Char_ohm, gcnew String(L"\u006f\u0068\u006d"),
            nullptr, nullptr, s_Char_yen, gcnew String(L"\u0079\u0065\u006e"),
            nullptr, nullptr, s_Char_deg, gcnew String(L"\u0064\u0065\u0067"),
            nullptr, nullptr, s_Char_infinity, gcnew String(L"\u0069\u006e\u0066\u0069\u006e\u0069\u0074\u0079"),
            nullptr, nullptr, s_Char_hbar, gcnew String(L"\u0068\u0062\u0061\u0072"),
            nullptr, nullptr, s_Char_element_of, gcnew String(L"\u0065\u006c\u0065\u006d\u0065\u006e\u0074\u005f\u006f\u0066"),
            nullptr, nullptr, s_Char_bottom, gcnew String(L"\u0062\u006f\u0074\u0074\u006f\u006d"),
            nullptr, nullptr, s_Char_right_arrow, gcnew String(L"\u0072\u0069\u0067\u0068\u0074\u005f\u0061\u0072\u0072\u006f\u0077"),
            nullptr, nullptr, s_Char_left_arrow, gcnew String(L"\u006c\u0065\u0066\u0074\u005f\u0061\u0072\u0072\u006f\u0077"),
            nullptr, nullptr, s_Char_copyright, gcnew String(L"\u0063\u006f\u0070\u0079\u0072\u0069\u0067\u0068\u0074"),
            nullptr, nullptr, s_Char_trademark, gcnew String(L"\u0074\u0072\u0061\u0064\u0065\u006d\u0061\u0072\u006b"),
            nullptr, nullptr, s_Char_registered, gcnew String(L"\u0072\u0065\u0067\u0069\u0073\u0074\u0065\u0072\u0065\u0064"),
            nullptr, nullptr, s_Char_hooked_f, gcnew String(L"\u0068\u006f\u006f\u006b\u0065\u0064\u005f\u0066"),
            nullptr, nullptr, s_Char_DoubleStruckC, gcnew String(L"\u0044\u006f\u0075\u0062\u006c\u0065\u0053\u0074\u0072\u0075\u0063\u006b\u0043"),
            nullptr, nullptr, s_Char_DoubleStruckQ, gcnew String(L"\u0044\u006f\u0075\u0062\u006c\u0065\u0053\u0074\u0072\u0075\u0063\u006b\u0051"),
            nullptr, nullptr, s_Char_DoubleStruckR, gcnew String(L"\u0044\u006f\u0075\u0062\u006c\u0065\u0053\u0074\u0072\u0075\u0063\u006b\u0052"),
            nullptr, nullptr, s_Char_DoubleStruckZ, gcnew String(L"\u0044\u006f\u0075\u0062\u006c\u0065\u0053\u0074\u0072\u0075\u0063\u006b\u005a"),
            nullptr, gcnew String(L"\u005b"), s_Start_Subscript, gcnew String(L"\u0053\u0074\u0061\u0072\u0074\u0053\u0075\u0062\u0073\u0063\u0072\u0069\u0070\u0074"),
            nullptr, gcnew String(L"\u005d"), s_End_Subscript, gcnew String(L"\u0045\u006e\u0064\u0053\u0075\u0062\u0073\u0063\u0072\u0069\u0070\u0074"),
            nullptr, gcnew String(L"\u0028"), s_Start_Superscript, gcnew String(L"\u0053\u0074\u0061\u0072\u0074\u0053\u0075\u0070\u0065\u0072\u0073\u0063\u0072\u0069\u0070\u0074"),
            nullptr, gcnew String(L"\u0029"), s_End_Superscript, gcnew String(L"\u0045\u006e\u0064\u0053\u0075\u0070\u0065\u0072\u0073\u0063\u0072\u0069\u0070\u0074")
            };
            s_AsciiToUnicodeTable = gcnew System::Collections::Generic::Dictionary<Char, Char>();
            s_UnicodeToAsciiTable = gcnew System::Collections::Generic::Dictionary<Char, Char>();
            s_CharNameToUnicodeStringTable = gcnew System::Collections::Generic::Dictionary<String^, String^>();
            s_UnicodeToCharNameTable = gcnew System::Collections::Generic::Dictionary<Char, String^>();
            s_LongUnicodeToShortcutKeyTable = gcnew System::Collections::Generic::List<Quire::Engine::Misc::Pair<String^, Char>^>();
            s_GreekShortcutKeyToUnicodeTable = gcnew System::Collections::Generic::Dictionary<Char, Char>();
            s_UnicodeToGreekShortcutKeyTable = gcnew System::Collections::Generic::Dictionary<Char, Char>();
            s_bNamedCharMapInitialized = false;
        }

    public:
        static System::IO::StreamReader^ StreamToPath(String^ path)
        {
            return gcnew System::IO::StreamReader(path, System::Text::Encoding::UTF8, true);
        }

        static void ReadExpanded(String^ path, System::IO::TextWriter^ output)
        {
            auto readers = gcnew System::Collections::Generic::Stack<System::IO::StreamReader^>();
            auto paths = gcnew System::Collections::Generic::Stack<String^>();
            auto references = gcnew System::Collections::Generic::Stack<Boolean>();
            try
            {
                auto reader = StreamToPath(path);
                Boolean isReference = false;
                Int32 commentDepth = 0;
                readers->Push(reader); paths->Push(path); references->Push(isReference);
                while (reader != nullptr)
                {
                    String^ line = reader->ReadLine();
                    if (line != nullptr)
                    {
                        if (line->IndexOf("/*") >= 0 && line->Trim() == "/*") commentDepth++;
                        else if (line->IndexOf("*/") >= 0 && line->Trim() == "*/") commentDepth--;
                        else
                        {
                            if (commentDepth != 0) continue;
                            Boolean includeReference = false;
                            String^ includePath = ProcessInclude(line, includeReference);
                            if (includePath != nullptr)
                            {
                                String^ directory = System::IO::Path::GetDirectoryName(path);
                                if (directory == nullptr) continue;
                                String^ childPath = System::IO::Path::Combine(directory, includePath);
                                reader = StreamToPath(childPath); path = childPath; isReference = includeReference;
                                readers->Push(reader); paths->Push(childPath); references->Push(isReference);
                                if (isReference) SkipPast(reader, "### END PREAMBLE ###");
                            }
                            else if (isReference && line->StartsWith("### BEGIN TEST CASE"))
                                SkipPast(reader, "### END TEST CASE");
                            else
                                output->WriteLine(line);
                        }
                    }
                    else
                    {
                        if (commentDepth > 0) throw gcnew System::Exception("End of file within comment.");
                        readers->Pop(); paths->Pop(); references->Pop(); reader->Close();
                        if (readers->Count > 0)
                        {
                            reader = readers->Peek(); path = paths->Peek(); isReference = references->Peek();
                        }
                        else
                        {
                            reader = nullptr; path = nullptr; isReference = false;
                        }
                    }
                }
            }
            finally
            {
                while (readers->Count > 0) readers->Pop()->Close();
            }
        }

        static String^ ReadExpanded(String^ path)
        {
            auto writer = gcnew System::IO::StringWriter();
            ReadExpanded(path, writer);
            safe_cast<System::IO::TextWriter^>(writer)->Close();
            return writer->ToString();
        }

        static String^ ContractUnicode(String^ str)
        {
            auto result = gcnew StringBuilder();
            for (Int32 i = 0; i < str->Length; ++i)
            {
                Char c = str[i], ascii = 0;
                String^ charName = nullptr;
                UInt32 value = c;
                if (value < 128) result->Append(c);
                else if (UnicodeToAscii(c, ascii)) result->AppendFormat(L"\\{0}", ascii);
                else if (UnicodeToGreekShortcutKey(c, ascii)) result->AppendFormat(L"\\g{0}", ascii);
                else if (LongUnicodeToShortcutKey(str, i, ascii)) result->AppendFormat(L"\\{0}", ascii);
                else if (UnicodeToCharName(c, charName)) result->AppendFormat(L"\\&{0};", charName);
                else result->AppendFormat(L"\\u{0:x4}", value);
            }
            return result->ToString();
        }
        static String^ ExpandUnicode(String^ str)
        {
            auto result = gcnew StringBuilder();
            auto buffer = gcnew StringBuilder();
            Boolean escape = false, greek = false, unicode = false, named = false;
            Int32 pos = 0;
            while (pos < str->Length)
            {
                Char c = str[pos++], mapped = 0;
                if (escape)
                {
                    escape = false;
                    if (AsciiToUnicode(c, mapped)) { result->Append(mapped); continue; }
                    switch (c)
                    {
                    case 'f': result->Append('\f'); break;
                    case 't': result->Append('\t'); break;
                    case 'r': result->Append('\r'); break;
                    case 'n': result->Append('\n'); break;
                    case '\r': if (pos < str->Length && str[pos] == '\n') ++pos; break;
                    case '\t':
                    case ' ':
                    {
                        Int32 next = pos;
                        while (next < str->Length)
                        {
                            Char space = str[next++];
                            if (space == '\r')
                            {
                                if (next < str->Length && str[next] == '\n') ++next;
                                continue;
                            }
                            if (space == '\t' || space == ' ') continue;
                            if (space == '\n') break;
                            throw gcnew Exception(String::Format(L"Bad escape sequence: {0}",
                                StringUtils::CharNames(str->Substring(pos - 2, next - pos + 2))));
                        }
                        pos = next; break;
                    }
                    case '[': result->Append(L'\u0086'); result->Append(L'\u208a'); break;
                    case ']': result->Append(L'\u0087'); result->Append(L'\u208a'); break;
                    case '(': result->Append(L'\u0086'); result->Append(L'\u207a'); break;
                    case ')': result->Append(L'\u0087'); result->Append(L'\u207a'); break;
                    case 'g': greek = true; break;
                    case '&': named = true; break;
                    case 'u': unicode = true; break;
                    case '\n': break;
                    default: result->Append(L'\\'); result->Append(c); break;
                    }
                }
                else if (greek)
                {
                    greek = false;
                    if (GreekShortcutKeyToUnicode(c, mapped)) result->Append(mapped);
                    else { --pos; result->AppendFormat(L"\\g{0}", c); }
                }
                else if (unicode)
                {
                    Boolean complete = false;
                    if ((c >= '0' && c <= '9') || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F'))
                    {
                        buffer->Append(c);
                        complete = buffer->Length == 4;
                    }
                    else
                    {
                        --pos; result->AppendFormat(L"\\u{0}", buffer); buffer->Length = 0; unicode = false;
                    }
                    if (complete)
                    {
                        result->Append(static_cast<Char>(Int32::Parse(buffer->ToString(), System::Globalization::NumberStyles::AllowHexSpecifier)));
                        buffer->Length = 0; unicode = false;
                    }
                }
                else if (named)
                {
                    if ((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || c == '_')
                    {
                        buffer->Append(c); continue;
                    }
                    String^ name = buffer->ToString(); buffer->Length = 0; named = false;
                    if (c == ';')
                    {
                        String^ value = nullptr;
                        if (CharNameToUnicodeString(name, value)) result->Append(value);
                        else result->AppendFormat(L"\\&{0};", name);
                    }
                    else
                    {
                        --pos; result->AppendFormat(L"\\&{0}", name);
                    }
                }
                else if (c == L'\\') escape = true;
                else result->Append(c);
            }
            return result->ToString();
        }
        static Boolean UnicodeToAscii(Char unicode, Char% ascii)
        {
            if (s_UnicodeToAsciiTable->Count == 0) InitCharMappings();
            if (!s_UnicodeToAsciiTable->ContainsKey(unicode)) return false;
            ascii = s_UnicodeToAsciiTable[unicode]; return true;
        }
        static Boolean AsciiToUnicode(Char ascii, Char% unicode)
        {
            if (s_AsciiToUnicodeTable->Count == 0) InitCharMappings();
            if (!s_AsciiToUnicodeTable->ContainsKey(ascii)) return false;
            unicode = s_AsciiToUnicodeTable[ascii]; return true;
        }
        static Boolean CharNameToUnicodeString(String^ name, String^% unicode)
        {
            if (!s_bNamedCharMapInitialized) InitNamedCharMap();
            if (!s_CharNameToUnicodeStringTable->ContainsKey(name)) return false;
            unicode = s_CharNameToUnicodeStringTable[name]; return true;
        }
        static Boolean UnicodeToCharName(Char unicode, String^% charName)
        {
            if (!s_bNamedCharMapInitialized) InitNamedCharMap();
            if (!s_UnicodeToCharNameTable->ContainsKey(unicode)) return false;
            charName = s_UnicodeToCharNameTable[unicode]; return true;
        }
        static Boolean GreekShortcutKeyToUnicode(Char greekShortcut, Char% unicode)
        {
            if (!s_bNamedCharMapInitialized) InitNamedCharMap();
            if (!s_GreekShortcutKeyToUnicodeTable->ContainsKey(greekShortcut)) return false;
            unicode = s_GreekShortcutKeyToUnicodeTable[greekShortcut]; return true;
        }
        static Boolean LongUnicodeToShortcutKey(String^ str, Int32% pos, Char% shortcutKey)
        {
            if (!s_bNamedCharMapInitialized) InitNamedCharMap();
            for (Int32 n = 0; n < s_LongUnicodeToShortcutKeyTable->Count; ++n)
            {
                Quire::Engine::Misc::Pair<String^, Char>^ item = s_LongUnicodeToShortcutKeyTable[n];
                String^ value = item->First;
                if (str->Length < pos + value->Length) continue;
                Boolean equal = true;
                for (Int32 i = 0; i < value->Length; ++i)
                    if (str[pos + i] != value[i]) { equal = false; break; }
                if (equal) { shortcutKey = item->Second; pos += value->Length - 1; return true; }
            }
            return false;
        }
        static Boolean UnicodeToGreekShortcutKey(Char unicode, Char% greekShortcut)
        {
            if (!s_bNamedCharMapInitialized) InitNamedCharMap();
            if (!s_UnicodeToGreekShortcutKeyTable->ContainsKey(unicode)) return false;
            greekShortcut = s_UnicodeToGreekShortcutKeyTable[unicode]; return true;
        }

    private:
        static String^ ProcessInclude(String^ line, Boolean% isReference)
        {
            Int32 i = 0;
            for (; i < line->Length && (line[i] == ' ' || line[i] == '\t'); i++) { }
            String^ text = i > 0 ? line->Substring(i) : line;
            if (text->StartsWith("#include") || text->StartsWith("#reference"))
            {
                isReference = text->StartsWith("#reference");
                Int32 first = text->IndexOf('"'), last = text->LastIndexOf('"');
                if (first < 0 || last - first < 2) throw gcnew System::Exception("bad #include directive");
                return text->Substring(first + 1, last - first - 1);
            }
            isReference = false; return nullptr;
        }

        static void SkipPast(System::IO::StreamReader^ reader, String^ seek)
        {
            String^ line;
            System::IO::TextReader^ textReader = reader;
            do { line = textReader->ReadLine(); } while (line != nullptr && !line->StartsWith(seek));
        }

        static void InitCharMappings()
        {
            for (Int32 i = 0; i < s_CharMappings->Length; i += 2)
            {
                Char ascii = s_CharMappings[i], unicode = s_CharMappings[i + 1];
                s_UnicodeToAsciiTable[unicode] = ascii;
                s_AsciiToUnicodeTable[ascii] = unicode;
            }
        }
        static void InitNamedCharMap()
        {
            for (Int32 i = 0; i < s_NameMappings->Length; i += 4)
            {
                String^ shortcut = s_NameMappings[i];
                String^ longShortcut = s_NameMappings[i + 1];
                String^ value = s_NameMappings[i + 2];
                String^ name = s_NameMappings[i + 3];
                if (name != nullptr) s_CharNameToUnicodeStringTable[name] = value;
                if (value != nullptr && value->Length == 1) s_UnicodeToCharNameTable[value[0]] = name;
                if (longShortcut != nullptr)
                {
                    if (longShortcut->Length != 1)
                        throw gcnew Exception(String::Format(L"Greek Name Table init failed for char \"{0}\": \"{1}\".Length != 1", name, longShortcut));
                    if (value == nullptr) throw gcnew ArgumentNullException("unicode_string", "Input shouldn't be null.");
                    s_LongUnicodeToShortcutKeyTable->Add(
                        gcnew Quire::Engine::Misc::Pair<String^, Char>(value, longShortcut[0]));
                }
                if (shortcut != nullptr)
                {
                    if (shortcut->Length != 1) throw gcnew Exception("Greek Name Table init failed(2).");
                    if (value != nullptr && value->Length != 1)
                        throw gcnew Exception(String::Format(L"Greek Name Table init failed for char \"{0}\": \"{1}\".Length != 1", name, value));
                    if (value == nullptr) throw gcnew ArgumentNullException("unicode_string", "Input shouldn't be null.");
                    Char key = shortcut[0], mapped = value[0];
                    s_GreekShortcutKeyToUnicodeTable[key] = mapped;
                    s_UnicodeToGreekShortcutKeyTable[mapped] = key;
                }
            }
            s_bNamedCharMapInitialized = true;
        }

    public:
        static Int32 ComputeLineNumber(String^ str, Int32 pos)
        {
            String^ nearby; return ComputeLineNumber(str, pos, nearby);
        }

        static Int32 ComputeLineNumber(String^ str, Int32 pos, String^% nearby_text)
        {
            String^ pointer; return ComputeLineNumber(str, pos, nearby_text, pointer);
        }

        static Int32 ComputeLineNumber(String^ str, Int32 pos, String^% nearby_text, String^% pointer_text)
        {
            auto line = gcnew System::Text::StringBuilder();
            auto pointer = gcnew System::Text::StringBuilder();
            Int32 lineNumber = 1, index = 0, limit = pos;
            if (limit < 0) limit = 0;
            if (limit > str->Length) limit = str->Length;
            while (index < limit)
            {
                Char c = str[index];
                if (c == '\n') { lineNumber++; line->Length = 0; pointer->Length = 0; }
                else { pointer->Append(' '); line->Append(c); }
                index++;
            }
            pointer->Append("^");
            while (index < str->Length)
            {
                Char c = str[index];
                if (c == '\n' || c == '\r') break;
                line->Append(c); index++;
            }
            nearby_text = line->ToString(); pointer_text = pointer->ToString(); return lineNumber;
        }

        FileUtils() {}
    };
#pragma warning(pop)

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

    [Quire::Mirror::MirrorOfAttribute("utl_mgd.TaskQueue, utl-mgd")]
    public ref class TaskQueue : MessageQueue<TaskQueue^, Int32, Function^>
    {
    private:
        SafeMutex^ m_empty_mutex;
        SafeEvent^ m_empty;

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
        TaskQueue() : MessageQueue<TaskQueue^, Int32, Function^>(1048576)
        {
            m_empty_mutex = gcnew SafeMutex("task queue empty mutex");
            m_empty = gcnew SafeEvent("task queue empty");
            MessageQueue<TaskQueue^, Int32, Function^>::OnMessage^ callback =
                gcnew MessageQueue<TaskQueue^, Int32, Function^>::OnMessage(&TaskQueue::ExecuteTask);
            Start(this, 0, nullptr, callback, true);
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
