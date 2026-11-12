#pragma once

#include <guiddef.h>

#include "mshex_export.h"

namespace Hex {

// An interface with no data of its own. Its interface identifier is a constant of the original, typed from the image.
// The four virtual members are all pure in the original (its vtable holds four pure-call slots); their names are not
// yet recovered, so they are placeholders that fix the slot count.
class Library {
public:
    HEX_EXPORT static const GUID &GetInterfaceID();

    HEX_EXPORT Library() = default;
    HEX_EXPORT Library(const Library &) = default;
    HEX_EXPORT Library(Library &&) = default;
    HEX_EXPORT Library &operator=(const Library &) = default;
    HEX_EXPORT Library &operator=(Library &&) = default;

    virtual void UnidentifiedSlot0() = 0;
    virtual void UnidentifiedSlot1() = 0;
    virtual void UnidentifiedSlot2() = 0;
    virtual void UnidentifiedSlot3() = 0;

private:
    static const GUID interface_id_;
};

}  // namespace Hex
