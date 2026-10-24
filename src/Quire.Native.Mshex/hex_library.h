#pragma once

#include "mshex_export.h"

namespace Hex {

// A library with no members of its own in this lane: its assignments copy nothing.
class Library {
public:
    HEX_EXPORT Library &operator=(const Library &) = default;
    HEX_EXPORT Library &operator=(Library &&) = default;
};

}  // namespace Hex
