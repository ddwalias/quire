#include <Windows.h>
#include <vcclr.h>

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
using namespace System::Reflection;

[assembly:AssemblyVersionAttribute("0.1.0.0")];

namespace Quire { namespace utl_mgd {

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
