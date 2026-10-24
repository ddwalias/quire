#include "hex_lexer.h"

namespace Hex {

bool Lexer::SetFlag(std::size_t, unsigned int) {
    return false;
}

bool Lexer::GetFlag(std::size_t, unsigned int &) const {
    return false;
}

}  // namespace Hex
