#include "hex_node.h"

#include <cstdint>

namespace Hex {

Tag Node::GetTag() const {
    return Tag(~std::uint64_t{0}, ~std::uint64_t{0}, 0);
}

}  // namespace Hex
