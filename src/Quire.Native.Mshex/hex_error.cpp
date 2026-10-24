#include "hex_error.h"

namespace Hex {

Tag Error::GetTag() const {
    return tag_;
}

unsigned int Error::GetCode() const {
    return code_;
}

std::uint64_t Error::GetOffset() const {
    return tag_.GetOffset();
}

std::uint64_t Error::GetExtent() const {
    return tag_.GetExtent();
}

std::uint32_t Error::GetHandle() const {
    return tag_.GetHandle();
}

}  // namespace Hex
