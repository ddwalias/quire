#pragma once

#include <cstdint>

#include "hex_tag.h"
#include "mshex_export.h"

namespace Hex {

class Token {
public:
    virtual ~Token() = default;

    HEX_EXPORT Token &operator=(const Token &) = default;

    HEX_EXPORT virtual Tag GetTag() const;
};

// Token payload with a source tag. The export names are for TokenOf<void>, the instance the consumers link against.
template <typename T>
class TokenOf : public Token {
public:
    HEX_EXPORT virtual std::uint16_t GetToken() const { return token_; }
    HEX_EXPORT Tag &GetTag() { return tag_; }
    HEX_EXPORT virtual Tag GetTag() const { return tag_; }

private:
    std::uint16_t token_;
    Tag tag_;
};

}  // namespace Hex
