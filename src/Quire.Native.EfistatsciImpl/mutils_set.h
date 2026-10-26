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

// The same checks with every field read where it is used (matset_resize, matset_verify_aresame).
__forceinline void check_set_f(const _mat_set *s) {
    if (s == nullptr) Efi::Throw("null pointer", ~0ull);
    if (s->dims == nullptr) Efi::Throw("null pointer", ~0ull);
    if (s->matrices == nullptr) Efi::Throw("null pointer", ~0ull);
    if (s->count <= 0) Efi::Throw("array size mismatch", ~0ull);
    if (s->total <= 0) Efi::Throw("array size mismatch", ~0ull);
    int total = 1;
    for (int i = 0; i < s->count; ++i) {
        if (s->dims[i] <= 0) Efi::Throw("array size mismatch", ~0ull);
        total *= s->dims[i];
    }
    if (total != s->total) Efi::Throw("Illegal data value", ~0ull);
}

namespace mutils {

struct _pgn_chain {
    // The first 8 bytes are not inferred yet; then the link count, one int and one direction code per link.
    unsigned char unknown_[8];
    int count;
    unsigned char unknown2_[4];
    int *points;
    unsigned char *codes;
};

EFI_EXPORT void matset_malloc(Efi::EfiCallContext *ctx, _mat_set *s, long count, const long *dims);
EFI_EXPORT void matset_free(Efi::EfiCallContext *ctx, _mat_set *s);
EFI_EXPORT void matset_free_matrices(Efi::EfiCallContext *ctx, _mat_set *s);
EFI_EXPORT void matset_validate(const _mat_set *s);
EFI_EXPORT void matset_resize(Efi::EfiCallContext *ctx, long count, long *dims, _mat_set *s);
EFI_EXPORT const char *pgn_chain_free(Efi::EfiCallContext *ctx, _pgn_chain *chain);

}  // namespace mutils
