#pragma once

#include <cstdint>
#include <string>

#include "mshex_export.h"

namespace Hex {

// A signature is one text: the name, a separator, the type, a separator, then the module. The two offsets are where the
// name ends (one character before the separator) and where the module starts. The accessors GetName, GetType and
// GetModule are parked (see the lane checkpoint): their register allocation differs from the original's.
class Signature {
public:
    HEX_EXPORT Signature();

private:
    std::uint16_t name_end_;
    std::uint16_t module_start_;
    std::string text_;
};

}  // namespace Hex
