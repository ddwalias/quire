#pragma once

#include "mshex_export.h"

namespace Hex {

class Signature;

// Comparison bases that Signature derives from in the original. Each has no members, so its assignments copy nothing.
template <typename T>
struct equality_comparable {
    HEX_EXPORT equality_comparable &operator=(const equality_comparable &) = default;
    HEX_EXPORT equality_comparable &operator=(equality_comparable &&) = default;
};

template <typename T>
struct less_than_comparable {
    HEX_EXPORT less_than_comparable &operator=(const less_than_comparable &) = default;
    HEX_EXPORT less_than_comparable &operator=(less_than_comparable &&) = default;
};

template <typename T>
struct totally_ordered {
    HEX_EXPORT totally_ordered &operator=(const totally_ordered &) = default;
    HEX_EXPORT totally_ordered &operator=(totally_ordered &&) = default;
};

}  // namespace Hex
