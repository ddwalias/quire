#include "hex_environment.h"

namespace Hex {

bool Environment::Visitor::Visit(const Signature &, const Value *) {
    return false;
}

bool Environment::Visitor::Alter(const Signature &signature, const Value *&value) {
    return Visit(signature, value);
}

bool Environment::AddValue(const Signature &, const Value *) {
    return false;
}

bool Environment::SetValue(const Signature &, const Value *) {
    return false;
}

}  // namespace Hex
