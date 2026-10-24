#include "hex_environment.h"

namespace Hex {

bool Environment::Visitor::Visit(const Signature &, const Value *) {
    return false;
}

bool Environment::AddValue(const Signature &, const Value *) {
    return false;
}

bool Environment::SetValue(const Signature &, const Value *) {
    return false;
}

}  // namespace Hex
