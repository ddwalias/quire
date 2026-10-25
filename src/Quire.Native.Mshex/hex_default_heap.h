#pragma once

#include <cstdint>

#include "mshex_export.h"

namespace Hex {

// Only the alignment constant is inferred so far; the heap's base classes and members are not yet in this lane.
class DefaultHeap {
protected:
    HEX_EXPORT static const std::uint64_t alignment;
};

}  // namespace Hex
