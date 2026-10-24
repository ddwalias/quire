#include "hex_formatter.h"

#include "hex_component.h"
#include "hex_node.h"

namespace Hex {

void Formatter::Insert(std::ostream &os, const Node &node) const {
    node.Insert(os);
}

void Formatter::Insert(std::ostream &os, const Component &component) const {
    component.Insert(os);
}

}  // namespace Hex
