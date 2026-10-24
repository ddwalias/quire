#pragma once

#include <cstddef>
#include <iosfwd>

#include "mshex_export.h"

namespace Hex {

class Environment;

class Interpreter {
public:
    // The base interpreter accepts no options and no environment.
    HEX_EXPORT virtual bool SetOption(std::size_t option, std::istream &in);
    HEX_EXPORT virtual bool GetOption(std::size_t option, std::ostream &out) const;
    HEX_EXPORT virtual bool SetEnvironment(Environment *environment);
};

}  // namespace Hex
