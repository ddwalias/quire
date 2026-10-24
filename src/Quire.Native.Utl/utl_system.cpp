#include "utl.h"

namespace utl {

int GetNumberOfProcessors() {
    SYSTEM_INFO info;
    GetSystemInfo(&info);
    return info.dwNumberOfProcessors;
}

// Returns true only when both calls fail: the original's branches return true at the end of the fall-through path.
bool BoostThreadPriority() {
    if (SetPriorityClass(GetCurrentProcess(), REALTIME_PRIORITY_CLASS)) {
        return false;
    }
    if (!SetThreadPriority(GetCurrentThread(), THREAD_PRIORITY_TIME_CRITICAL)) {
        return true;
    }
    return false;
}

}  // namespace utl
