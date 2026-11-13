#include <ctype.h>

// Parses an optionally negative decimal integer of at most maxDigits digits; the whole string must be consumed.
bool parse_int(const char *text, unsigned int maxDigits, int &value) {
    unsigned int i;
    int sign;
    if (text[0] == '-' && text[1] != '\0') {
        sign = -1;
        i = 1;
    } else {
        sign = 1;
        i = 0;
    }
    int number = 0;
    if (text[i] != '\0') {
        do {
            if (!isdigit(static_cast<unsigned char>(text[i])) || i >= maxDigits) {
                break;
            }
            number = number * 10 + (text[i] - '0');
            ++i;
        } while (text[i] != '\0');
    }
    if (text[i] != '\0') {
        return false;
    }
    value = number * sign;
    return true;
}
