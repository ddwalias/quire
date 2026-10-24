#pragma once

#include <cstdint>

#include "mshex_export.h"

namespace Hex {

// A source span: where a token starts, how long it is, and the handle of the file it came from.
class Tag {
public:
    HEX_EXPORT Tag(std::uint64_t offset, std::uint64_t extent, std::uint32_t handle)
        : offset_(offset), extent_(extent), handle_(handle) {}

    HEX_EXPORT std::uint64_t GetOffset() const;
    HEX_EXPORT std::uint64_t GetExtent() const;
    HEX_EXPORT std::uint32_t GetHandle() const;

    HEX_EXPORT void SetOffset(std::uint64_t offset);
    HEX_EXPORT void SetExtent(std::uint64_t extent);
    HEX_EXPORT void SetHandle(std::uint32_t handle);

private:
    std::uint64_t offset_;
    std::uint64_t extent_;
    std::uint32_t handle_;
};

}  // namespace Hex
