#pragma once

#include "hex_tag.h"
#include "mshex_export.h"

namespace Hex {

class Node {
public:
    virtual ~Node() = default;

    // A node without a source location reports the null tag.
    HEX_EXPORT virtual Tag GetTag() const;
};

}  // namespace Hex
