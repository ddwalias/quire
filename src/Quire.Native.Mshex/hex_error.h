#pragma once

#include <cstdint>
#include <string>

#include "hex_tag.h"
#include "mshex_export.h"

namespace Hex {

// Layout from the original's accessors and constructors: the source tag at 0, the error code at 0x18, and the details
// string at 0x20.
class Error {
public:
    HEX_EXPORT Tag GetTag() const;
    HEX_EXPORT unsigned int GetCode() const;
    HEX_EXPORT std::uint64_t GetOffset() const;
    HEX_EXPORT std::uint64_t GetExtent() const;
    HEX_EXPORT std::uint32_t GetHandle() const;

private:
    Tag tag_;
    unsigned int code_;
    std::string details_;
};

}  // namespace Hex
