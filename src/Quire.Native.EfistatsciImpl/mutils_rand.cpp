#include "mutils_mat.h"

#include <time.h>

namespace mutils {

// The generator state behind mutil_rand_*: Park-Miller with a Bays-Durham shuffle (Numerical Recipes' ran1), plus a
// saved second normal deviate.
struct mutil_rand_state {
    int seed;
    int table[32];
    int last;
    unsigned char has_spare;
    double spare;
};

__forceinline void mutil_rand_step(mutil_rand_state *st) {
    int k = st->seed / 127773;
    st->seed = 16807 * (st->seed - k * 127773) - 2836 * k;
    if (st->seed < 0) st->seed += 2147483647;
}

EFI_EXPORT const char *mutil_rand_set_seed(const void *seed, void *state) {
    if (state == nullptr) return "null pointer";
    mutil_rand_state *st = static_cast<mutil_rand_state *>(state);
    st->seed = seed != nullptr ? *static_cast<const int *>(seed) : (int)time(nullptr);
    if (st->seed == 0) st->seed = 1;
    else if (st->seed < 0) st->seed = -st->seed;
    for (int j = 32 + 7; j >= 0; --j) {
        mutil_rand_step(st);
        if (j < 32) st->table[j] = st->seed;
    }
    st->last = st->table[0];
    return nullptr;
}

EFI_EXPORT const char *mutil_rand_begin(Efi::EfiCallContext *ctx, void **state) {
    if (state == nullptr) return "null pointer";
    mutil_rand_state *st = static_cast<mutil_rand_state *>(ctx->allocator->allocate_array(sizeof(mutil_rand_state)));
    int seed = 1;
    st->has_spare = 0;
    if (const char *error = mutil_rand_set_seed(&seed, st)) return error;
    *state = st;
    return nullptr;
}

}  // namespace mutils
