#include "hex_tag.h"

namespace Hex {

void Tag::SetOffset(std::uint64_t offset) {
    offset_ = offset;
}

void Tag::SetExtent(std::uint64_t extent) {
    extent_ = extent;
}

void Tag::SetHandle(std::uint32_t handle) {
    handle_ = handle;
}

}  // namespace Hex
