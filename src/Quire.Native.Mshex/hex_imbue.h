#pragma once

#include "mshex_export.h"

namespace Hex {

class Interpreter;

class imbue {
public:
    HEX_EXPORT explicit imbue(const Interpreter *interpreter);

private:
    const Interpreter *interpreter_;
};

}  // namespace Hex
