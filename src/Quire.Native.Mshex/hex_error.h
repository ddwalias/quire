#pragma once

#include "hex_tag.h"
#include "mshex_export.h"

namespace Hex {

// Layout from the original's accessors: the source tag at 0, the error code at 0x18. The remaining members are not yet
// inferred.
class Error {
public:
    HEX_EXPORT Tag GetTag() const;
    HEX_EXPORT unsigned int GetCode() const;

private:
    Tag tag_;
    unsigned int code_;
};

}  // namespace Hex
