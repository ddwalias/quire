#pragma once

#include <cstddef>

#include "mshex_export.h"

namespace Hex {

class Lexer {
public:
    // The base lexer accepts no flags.
    HEX_EXPORT virtual bool SetFlag(std::size_t flag, unsigned int value);
    HEX_EXPORT virtual bool GetFlag(std::size_t flag, unsigned int &value) const;
};

}  // namespace Hex
