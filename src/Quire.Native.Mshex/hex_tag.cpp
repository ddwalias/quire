#include "hex_tag.h"

namespace Hex {

std::uint64_t Tag::GetOffset() const {
    return offset_;
}

std::uint64_t Tag::GetExtent() const {
    return extent_;
}

std::uint32_t Tag::GetHandle() const {
    return handle_;
}

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
