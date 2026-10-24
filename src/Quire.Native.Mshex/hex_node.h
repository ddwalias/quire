#pragma once

#include "hex_tag.h"
#include "mshex_export.h"

#include <iosfwd>

namespace Hex {

class Error;

// Virtual order follows the vtable slots that the original's Formatter::Insert dispatches through: Insert is slot 3.
class Node {
public:
    virtual ~Node() = default;

    HEX_EXPORT Node &operator=(const Node &) = default;

    virtual Error GetError(unsigned int code) const;

    // A node without a source location reports the null tag.
    HEX_EXPORT virtual Tag GetTag() const;

    virtual void Insert(std::ostream &os) const;
};

}  // namespace Hex
