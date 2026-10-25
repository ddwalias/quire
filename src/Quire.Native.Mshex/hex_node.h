#pragma once

#include <iosfwd>

#include "hex_tag.h"
#include "mshex_export.h"

namespace Hex {

class Error;

// Virtual order is the vtable order: the destructor, GetTag, GetError, Insert. Insert is slot 3, which Formatter::Insert
// dispatches through.
class Node {
public:
    virtual ~Node() = default;

    HEX_EXPORT Node &operator=(const Node &) = default;

    // A node without a source location reports the null tag.
    HEX_EXPORT virtual Tag GetTag() const;

    // Parked: the original's GetError inlines Error(code, tag), whose destructor (exported, ??1Error) does not match yet.
    virtual Error GetError(unsigned int code) const;

    virtual void Insert(std::ostream &os) const;
};

}  // namespace Hex
