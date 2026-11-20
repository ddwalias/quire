#include "hex_signature.h"

namespace Hex {

Signature::Signature() : name_end_(1), module_start_(2), text_(2, '\0') {}

bool operator==(const Signature &a, const Signature &b) {
    return a.text_ == b.text_;
}

bool operator<(const Signature &a, const Signature &b) {
    return a.text_ < b.text_;
}

}  // namespace Hex
