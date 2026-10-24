#pragma once

#include <cstdint>

#include "mshex_export.h"

namespace Hex {

// A source span: where a token starts, how long it is, and the handle of the file it came from.
class Tag {
public:
    HEX_EXPORT Tag(std::uint64_t offset, std::uint64_t extent, std::uint32_t handle)
        : offset_(offset), extent_(extent), handle_(handle) {}

    HEX_EXPORT std::uint64_t GetOffset() const { return offset_; }
    HEX_EXPORT std::uint64_t GetExtent() const { return extent_; }
    HEX_EXPORT std::uint32_t GetHandle() const { return handle_; }

    HEX_EXPORT void SetOffset(std::uint64_t offset);
    HEX_EXPORT void SetExtent(std::uint64_t extent);
    HEX_EXPORT void SetHandle(std::uint32_t handle);

    // The extent is an end position: += takes the other span's end, and + spans from the first's start to the second's end.
    HEX_EXPORT Tag &operator+=(const Tag &other);

    Tag(const Tag &) = default;
    Tag(Tag &&) = default;
    HEX_EXPORT Tag &operator=(const Tag &) = default;
    HEX_EXPORT Tag &operator=(Tag &&) = default;

private:
    std::uint64_t offset_;
    std::uint64_t extent_;
    std::uint32_t handle_;
};

}  // namespace Hex
