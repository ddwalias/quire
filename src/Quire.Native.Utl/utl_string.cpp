#include "utl.h"

#include <oleauto.h>

#include <string.h>

namespace utl {

wchar_t *toBSTR(unsigned int codePage, const char *source, size_t length) {
    int count = MultiByteToWideChar(codePage, 0, source, (int)length, nullptr, 0);
    BSTR text = SysAllocStringLen(nullptr, count);
    MultiByteToWideChar(codePage, 0, source, (int)length, text, count);
    return text;
}

wchar_t *toBSTR(unsigned int codePage, const char *source) {
    return toBSTR(codePage, source, strlen(source));
}

namespace {

// The original reads a digit as '0' to '9' and anything else as 'A' to 'F' (upper case only).
unsigned char HexNibble(char digit) {
    if (digit >= '0' && digit <= '9') {
        return static_cast<unsigned char>(digit - '0');
    }
    return static_cast<unsigned char>(digit - 'A' + 10);
}

}  // namespace

unsigned char *FromHex(const char *text) {
    size_t length = strlen(text) / 2;
    unsigned char *bytes = new unsigned char[length + 1];
    for (size_t i = 0; i < length; ++i) {
        unsigned char high = HexNibble(text[2 * i]);
        unsigned char low = HexNibble(text[2 * i + 1]);
        bytes[i] = static_cast<unsigned char>((high << 4) + low);
    }
    bytes[length] = 0;
    return bytes;
}

}  // namespace utl
