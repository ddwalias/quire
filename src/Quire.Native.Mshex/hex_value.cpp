#include "hex_value.h"

namespace Hex {

Value::Value() {}

Value::Value(const Value &) : Attachment() {}

Value::Value(Value &&) : Attachment() {}

Value &Value::operator=(const Value &) {
    return *this;
}

Value &Value::operator=(Value &&) {
    return *this;
}

void Value::Insert(std::ostream &) const {}

}  // namespace Hex
