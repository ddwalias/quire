#include "mutils_set.h"

namespace mutils {

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
    chain->head_ = nullptr;
    chain->tail_ = nullptr;
    chain->count = 0;
    return nullptr;
}

}  // namespace mutils
