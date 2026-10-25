#pragma once

#include <iosfwd>

#include "hex_attachment.h"
#include "mshex_export.h"

namespace Hex {

// A value derives from Attachment at offset 0. Copies and assignments do not copy the attachment: each object gets its own.
class Value : public ::Attachment {
public:
    HEX_EXPORT Value();
    HEX_EXPORT Value(const Value &);
    HEX_EXPORT Value(Value &&);
    HEX_EXPORT virtual ~Value() = default;

    HEX_EXPORT Value &operator=(const Value &);
    HEX_EXPORT Value &operator=(Value &&);

    // Writes nothing: the original's Insert is empty.
    HEX_EXPORT virtual void Insert(std::ostream &os) const;
};

}  // namespace Hex
