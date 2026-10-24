#pragma once

#include <iosfwd>

#include "mshex_export.h"

namespace Hex {

class Component;
class Node;

class Formatter {
public:
    HEX_EXPORT virtual void Insert(std::ostream &os, const Node &node) const;
    HEX_EXPORT virtual void Insert(std::ostream &os, const Component &component) const;
};

}  // namespace Hex
