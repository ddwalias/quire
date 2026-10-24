#include "utl.h"

#include <oleauto.h>

namespace utl {

wchar_t *toBSTR(unsigned int codePage, const char *source, size_t length) {
    int count = MultiByteToWideChar(codePage, 0, source, (int)length, nullptr, 0);
    BSTR text = SysAllocStringLen(nullptr, count);
    MultiByteToWideChar(codePage, 0, source, (int)length, text, count);
    return text;
}

}  // namespace utl
