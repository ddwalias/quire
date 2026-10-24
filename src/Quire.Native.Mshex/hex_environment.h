#pragma once

#include "mshex_export.h"

namespace Hex {

class Signature;
class Value;

// Virtual members of the original that take a signature and a value. Only the members the lane has inferred are here.
class Environment {
public:
    struct Visitor {
        virtual ~Visitor() = default;

        HEX_EXPORT Visitor &operator=(const Visitor &) = default;
        HEX_EXPORT Visitor &operator=(Visitor &&) = default;

        HEX_EXPORT virtual bool Visit(const Signature &signature, const Value *value);
    };

    virtual ~Environment() = default;

    HEX_EXPORT Environment &operator=(const Environment &) = default;
    HEX_EXPORT Environment &operator=(Environment &&) = default;

    HEX_EXPORT virtual bool AddValue(const Signature &signature, const Value *value);
    HEX_EXPORT virtual bool SetValue(const Signature &signature, const Value *value);
};

}  // namespace Hex
