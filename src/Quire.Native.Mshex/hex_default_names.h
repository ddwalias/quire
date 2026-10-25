#pragma once

#include "mshex_export.h"

namespace Hex {

class DefaultNames {
public:
    // A hash functor. Its bucket constants are exported data; it has no other members in this lane.
    struct hash {
        HEX_EXPORT static const unsigned int bucket_size;
        HEX_EXPORT static const unsigned int min_buckets;

        HEX_EXPORT hash &operator=(const hash &) = default;
        HEX_EXPORT hash &operator=(hash &&) = default;
    };
};

}  // namespace Hex
