#pragma once

#include <iosfwd>

#include "mshex_export.h"

namespace Hex {

// Only the vtable is inferred so far. The copy and move assignments copy no members.
class Value {
public:
    virtual ~Value() = default;

    HEX_EXPORT Value &operator=(const Value &) = default;
    HEX_EXPORT Value &operator=(Value &&) = default;

    // Writes nothing: the original's Insert is empty.
    HEX_EXPORT virtual void Insert(std::ostream &os) const;
};

}  // namespace Hex
