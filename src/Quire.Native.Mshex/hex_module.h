#pragma once

#include "mshex_export.h"

namespace Hex {

// Layout from the original's accessors. Bytes 0x08..0x47 and 0x50..0x77 are not yet inferred and stay as padding.
class Module {
public:
    struct State;

    virtual ~Module() = default;

    HEX_EXPORT unsigned int GetFlags() const;
    HEX_EXPORT void *GetDatum() const;
    HEX_EXPORT State *GetState() const;
    HEX_EXPORT bool IsPersistent() const;
    HEX_EXPORT bool IsTransient() const;
    HEX_EXPORT bool IsLoadable() const;
    HEX_EXPORT bool HasFlags(unsigned int flags) const;

    HEX_EXPORT void SetLoadable(bool loadable);
    HEX_EXPORT void SetDatum(void *datum);
    HEX_EXPORT unsigned int SetFlags(unsigned int flags);
    HEX_EXPORT unsigned int AddFlags(unsigned int flags);
    HEX_EXPORT unsigned int ClearFlags(unsigned int flags);

private:
    unsigned char unknown_08_[0x40];
    void *persistent_;  // 0x48: non-null for a persistent module
    unsigned char unknown_50_[0x28];
    unsigned int flags_;  // 0x78
    void *datum_;         // 0x80
    State *state_;        // 0x88
    bool loadable_;       // 0x90
};

}  // namespace Hex
