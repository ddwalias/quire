#include "utl.h"

#include <stdlib.h>

namespace utl {

void die(const wchar_t *message) {
    MessageBoxW(nullptr, message, L"Fatal Error", 0);
    exit(1);
}

int GetNumberOfProcessors() {
    SYSTEM_INFO info;
    GetSystemInfo(&info);
    return info.dwNumberOfProcessors;
}

bool BoostThreadPriority() {
    return !SetPriorityClass(GetCurrentProcess(), REALTIME_PRIORITY_CLASS) &&
           !SetThreadPriority(GetCurrentThread(), THREAD_PRIORITY_TIME_CRITICAL);
}

static DWORD s_process_priority;
static DWORD s_thread_priority;

bool RestoreThreadPriority() {
    return !SetPriorityClass(GetCurrentProcess(), s_process_priority) &&
           !SetThreadPriority(GetCurrentThread(), s_thread_priority);
}

}  // namespace utl
