#include "hex_token.h"

#include <cstdint>

namespace Hex {

Tag Token::GetTag() const {
    return Tag(~std::uint64_t{0}, ~std::uint64_t{0}, 0);
}

}  // namespace Hex

// Instantiates the members that the consumers link against (TokenOf<void>, see hex_token.h).
template class Hex::TokenOf<void>;
