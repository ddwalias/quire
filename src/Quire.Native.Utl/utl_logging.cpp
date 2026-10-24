#include "utl.h"

// urlmon.h, pulled in by Windows.h, defines IsLoggingEnabled as a macro for IsLoggingEnabledW.
#undef IsLoggingEnabled

namespace utl {

namespace {

__declspec(noinline) bool ReadRegistryFlag(const wchar_t *name) {
    HKEY key;
    DWORD value;
    DWORD size;
    if (RegOpenKeyExW(HKEY_CURRENT_USER, L"Software\\Mathsoft\\Mathcad Engine 1\\AppSettings", 0, KEY_READ, &key) ==
        ERROR_SUCCESS) {
        size = sizeof(value);
        if (RegQueryValueExW(key, name, nullptr, nullptr, reinterpret_cast<LPBYTE>(&value), &size) == ERROR_SUCCESS) {
            bool enabled = value != 0;
            RegCloseKey(key);
            return enabled;
        }
        RegCloseKey(key);
    }
    if (RegOpenKeyExW(HKEY_LOCAL_MACHINE, L"Software\\Mathsoft\\Mathcad Engine 1\\Default\\AppSettings", 0, KEY_READ,
                      &key) != ERROR_SUCCESS) {
        return false;
    }
    size = sizeof(value);
    if (RegQueryValueExW(key, name, nullptr, nullptr, reinterpret_cast<LPBYTE>(&value), &size) == ERROR_SUCCESS) {
        bool enabled = value != 0;
        RegCloseKey(key);
        return enabled;
    }
    RegCloseKey(key);
    return false;
}

}  // namespace

bool IsLoggingEnabled() {
    static bool initialized = false;
    static bool enabled = false;
    if (!initialized) {
        enabled = ReadRegistryFlag(L"EngineLogging");
        initialized = true;
    }
    return enabled;
}

}  // namespace utl
