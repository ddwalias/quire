#include <Windows.h>
#include <vcclr.h>
#using <Quire.Mirror.dll>

#pragma managed(push, off)
namespace
{
    HANDLE NativeCreateEvent(const wchar_t* name)
    {
        return ::CreateEventW(nullptr, TRUE, FALSE, name);
    }

    HANDLE NativeCreateMutex(const wchar_t* name)
    {
        return ::CreateMutexW(nullptr, FALSE, name);
    }

    BOOL NativeSetEvent(HANDLE handle) { return ::SetEvent(handle); }
    BOOL NativeResetEvent(HANDLE handle) { return ::ResetEvent(handle); }
    BOOL NativeCloseHandle(HANDLE handle) { return ::CloseHandle(handle); }
    BOOL NativeReleaseMutex(HANDLE handle) { return ::ReleaseMutex(handle); }
    DWORD NativeWaitOne(HANDLE handle, DWORD timeout) { return ::WaitForSingleObject(handle, timeout); }
    DWORD NativeWaitTwo(HANDLE first, HANDLE second)
    {
        HANDLE handles[] = { first, second };
        return ::WaitForMultipleObjects(2, handles, FALSE, INFINITE);
    }
}
#pragma managed(pop)

using namespace System;
using namespace System::Diagnostics;
using namespace System::Reflection;
using namespace System::Text;

[assembly:AssemblyVersionAttribute("0.1.0.0")];

namespace Quire { namespace utl_mgd {

    [Quire::Mirror::MirrorOfAttribute("utl_mgd.SafeWaitHandle, utl-mgd")]
    public ref class SafeWaitHandle abstract : IDisposable
    {
    protected:
        String^ m_name;
        void* m_handle;

        SafeWaitHandle(String^ name, void* handle) : m_name(name), m_handle(handle) {}

        virtual void DisposeCore(Boolean disposing)
        {
            (void)disposing;
            if (m_handle != nullptr)
            {
                NativeCloseHandle(static_cast<HANDLE>(m_handle));
                m_handle = nullptr;
            }
        }

    private:
        void CloseFromFinalizer()
        {
            if (m_handle != nullptr)
            {
                NativeCloseHandle(static_cast<HANDLE>(m_handle));
                m_handle = nullptr;
            }
        }

    public:
        void WaitOne()
        {
            NativeWaitOne(static_cast<HANDLE>(m_handle), INFINITE);
            GC::KeepAlive(this);
        }

        Boolean WaitOneWithTimeout(Int32 timeout)
        {
            DWORD result = NativeWaitOne(static_cast<HANDLE>(m_handle), timeout == -1 ? INFINITE : static_cast<DWORD>(timeout));
            GC::KeepAlive(this);
            return result != WAIT_TIMEOUT;
        }

        Boolean WaitTwoWithTimeout(void* other)
        {
            if (other == reinterpret_cast<void*>(-1))
            {
                NativeWaitOne(static_cast<HANDLE>(m_handle), INFINITE);
                GC::KeepAlive(this);
                return true;
            }

            DWORD result = NativeWaitTwo(static_cast<HANDLE>(m_handle), static_cast<HANDLE>(other));
            GC::KeepAlive(this);
            return result == WAIT_OBJECT_0;
        }

        void Free()
        {
            DisposeCore(true);
            GC::SuppressFinalize(this);
            GC::KeepAlive(this);
        }

        void* GetUnmanagedHandle() { return m_handle; }
        virtual String^ ToString() override { return m_name; }

        ~SafeWaitHandle()
        {
            DisposeCore(true);
        }

        !SafeWaitHandle()
        {
            CloseFromFinalizer();
        }
    };

    [Quire::Mirror::MirrorOfAttribute("utl_mgd.SafeEvent, utl-mgd")]
    public ref class SafeEvent : SafeWaitHandle
    {
    public:
        SafeEvent(String^ name)
            : SafeWaitHandle("SafeEvent " + name, NativeCreateEvent(nullptr)) {}

        void Set()
        {
            NativeSetEvent(static_cast<HANDLE>(m_handle));
            GC::KeepAlive(this);
        }

        void Reset()
        {
            NativeResetEvent(static_cast<HANDLE>(m_handle));
            GC::KeepAlive(this);
        }
    };

    [Quire::Mirror::MirrorOfAttribute("utl_mgd.SafeMutex, utl-mgd")]
    public ref class SafeMutex : SafeWaitHandle
    {
    public:
        SafeMutex(String^ name, Int32)
            : SafeWaitHandle("SafeMutex " + name, nullptr)
        {
            String^ nativeName = "Global\\" + name->Replace("\\", "_");
            pin_ptr<const wchar_t> pinnedName = PtrToStringChars(nativeName);
            m_handle = NativeCreateMutex(pinnedName);
        }

        SafeMutex(String^ name)
            : SafeWaitHandle("SafeMutex " + name, NativeCreateMutex(nullptr)) {}

        void ReleaseMutex()
        {
            NativeReleaseMutex(static_cast<HANDLE>(m_handle));
            GC::KeepAlive(this);
        }

        void EnsureReleased(UInt32 max_tries)
        {
            UInt32 count = 0;
            while (count < max_tries && NativeReleaseMutex(static_cast<HANDLE>(m_handle)))
                ++count;
            GC::KeepAlive(this);
        }

        void EnsureReleased()
        {
            UInt32 count = 0;
            while (count < 1000 && NativeReleaseMutex(static_cast<HANDLE>(m_handle)))
                ++count;
            GC::KeepAlive(this);
        }

    protected:
        virtual void DisposeCore(Boolean disposing) override
        {
            EnsureReleased();
            SafeWaitHandle::DisposeCore(disposing);
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
            volatile Boolean evenLength = s->Length % 2 == 0;
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
