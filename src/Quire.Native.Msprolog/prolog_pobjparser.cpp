#include "prolog.h"

namespace Prolog {

PObjParser &PObjParser::operator=(PObjParser &&) {
    return *this;
}

PObjParser &PObjParser::operator=(PObjParser const &) {
    return *this;
}

#include <string.h>

unsigned char PObjParser::format_code(Engine::Unmanaged::anyval const *value) {
    if (auto const *text = dynamic_cast<Engine::Unmanaged::pstring const *>(value)) {
        if (text->text && strlen(text->text) == 2 && text->text[0] == '%') {
            return text->text[1];
        }
    }
    return '0';
}

}
