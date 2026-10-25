#pragma once

#include <guiddef.h>

#include "mshex_export.h"

namespace Hex {

// A library with no members of its own in this lane: its assignments copy nothing. Its interface identifier is a
// constant of the original, typed from the image.
class Library {
public:
    HEX_EXPORT static const GUID &GetInterfaceID();

    HEX_EXPORT Library &operator=(const Library &) = default;
    HEX_EXPORT Library &operator=(Library &&) = default;

private:
    static const GUID interface_id_;
};

}  // namespace Hex
