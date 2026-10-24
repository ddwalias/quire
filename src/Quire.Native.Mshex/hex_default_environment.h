#pragma once

#include "hex_environment.h"
#include "mshex_export.h"

namespace Hex {

// Layout from the accessor: the environment's vptr, then an unidentified span, then the signature count at 0x20.
class DefaultEnvironment : public Environment {
public:
    HEX_EXPORT virtual unsigned int GetSignatures() const;

private:
    unsigned char unknown_08_[0x18];
    unsigned int signatures_;
};

}  // namespace Hex
