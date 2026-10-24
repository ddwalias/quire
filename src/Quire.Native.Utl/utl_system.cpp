#include "utl.h"

namespace utl {

int GetNumberOfProcessors() {
    SYSTEM_INFO info;
    GetSystemInfo(&info);
    return info.dwNumberOfProcessors;
}

}  // namespace utl
