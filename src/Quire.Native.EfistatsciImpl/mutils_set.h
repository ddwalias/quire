#pragma once

#include "efi_common.h"

// A set of matrices: `count` dimensions in `dims`, whose product must equal `total`. Fields after `count` are read as
// they are used; +18h is not inferred beyond being a pointer.
struct _mat_set {
    int count;
    int *dims;
    int total;
    void *matrices;
};

__forceinline void validate(const _mat_set *s) {
    if (s == nullptr) Efi::Throw("null pointer", ~0ull);
    if (s->dims == nullptr) Efi::Throw("null pointer", ~0ull);
    if (s->matrices == nullptr) Efi::Throw("null pointer", ~0ull);
    int count = s->count;
    if (count <= 0) Efi::Throw("array size mismatch", ~0ull);
    int total = s->total;
    if (total <= 0) Efi::Throw("array size mismatch", ~0ull);
    int product = 1;
    for (int i = 0; i < count; ++i) {
        int dim = s->dims[i];
        if (dim <= 0) Efi::Throw("array size mismatch", ~0ull);
        product *= dim;
    }
    if (product != total) Efi::Throw("Illegal data value", ~0ull);
}

namespace mutils {

struct _pgn_chain {
    // Fields other than the count at +8 are not inferred yet.
    unsigned char unknown_[8];
    int count;
    unsigned char unknown2_[4];
    void *head_;
    void *tail_;
};

EFI_EXPORT void matset_free(Efi::EfiCallContext *ctx, _mat_set *s);
EFI_EXPORT void matset_free_matrices(Efi::EfiCallContext *ctx, _mat_set *s);
EFI_EXPORT void matset_validate(const _mat_set *s);
EFI_EXPORT const char *pgn_chain_free(Efi::EfiCallContext *ctx, _pgn_chain *chain);

}  // namespace mutils
