#include "hex_error.h"

namespace Hex {

Tag Error::GetTag() const {
    return tag_;
}

unsigned int Error::GetCode() const {
    return code_;
}

}  // namespace Hex
