#pragma once

#include <iosfwd>

#include "mshex_export.h"

namespace Hex {

class Error;

// Virtual order follows the vtable slots that the original's Formatter::Insert dispatches through: Insert is slot 0.
class Component {
public:
    virtual void Insert(std::ostream &os) const;
    virtual void OnError(Error &error) const;
    virtual ~Component() = default;
};

}  // namespace Hex
