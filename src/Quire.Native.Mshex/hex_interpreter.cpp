#include "hex_interpreter.h"

namespace Hex {

bool Interpreter::SetOption(std::size_t, std::istream &) {
    return false;
}

bool Interpreter::GetOption(std::size_t, std::ostream &) const {
    return false;
}

bool Interpreter::SetEnvironment(Environment *) {
    return false;
}

}  // namespace Hex
