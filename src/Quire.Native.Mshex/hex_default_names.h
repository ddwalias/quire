#pragma once

#include "mshex_export.h"

namespace Hex {

class DefaultNames {
public:
    // A hash functor with no members of its own in this lane.
    struct hash {
        HEX_EXPORT hash &operator=(const hash &) = default;
        HEX_EXPORT hash &operator=(hash &&) = default;
    };
};

}  // namespace Hex
