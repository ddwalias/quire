#include "mutils_set.h"

namespace mutils {

void matset_malloc(Efi::EfiCallContext *ctx, _mat_set *s, long count, const long *dims) {
    if (s == nullptr) Efi::Throw("null pointer", ~0ull);
    if (dims == nullptr) Efi::Throw("null pointer", ~0ull);
    if (count <= 0) Efi::Throw("array size mismatch", ~0ull);
    s->dims = static_cast<int *>(ctx->allocator->allocate_array(count * sizeof(int)));
    s->total = 1;
    s->count = count;
    for (int i = 0; i < count; ++i) {
        if (dims[i] <= 0) Efi::Throw("array size mismatch", ~0ull);
        s->dims[i] = dims[i];
        s->total *= dims[i];
    }
    s->matrices = ctx->allocator->allocate_array(static_cast<unsigned __int64>(s->total) * 32);
}

void matset_free(Efi::EfiCallContext *, _mat_set *s) {
    if (s == nullptr) Efi::Throw("null pointer", ~0ull);
    s->matrices = nullptr;
    s->total = 0;
    s->dims = nullptr;
    s->count = 0;
}

void matset_free_matrices(Efi::EfiCallContext *, _mat_set *s) { validate(s); }

void matset_validate(const _mat_set *s) { validate(s); }

const char *pgn_chain_free(Efi::EfiCallContext *, _pgn_chain *chain) {
    if (chain == nullptr || chain->count == 0) return nullptr;
    chain->points = nullptr;
    chain->codes = nullptr;
    chain->count = 0;
    return nullptr;
}

void matset_resize(Efi::EfiCallContext *ctx, long count, long *dims, _mat_set *s) {
    check_set_f(s);
    if (dims == nullptr) Efi::Throw("null pointer", ~0ull);
    if (count < 1) Efi::Throw("Illegal data value", ~0ull);
    int product = 1;
    for (int i = 0; i < count; ++i) product *= dims[i];
    if (product != s->total) Efi::Throw("array size mismatch", ~0ull);
    int *d = static_cast<int *>(ctx->allocator->allocate_array((int)(count * sizeof(int))));
    for (int i = 0; i < count; ++i) d[i] = dims[i];
    s->dims = d;
    s->count = count;
}

}  // namespace mutils
