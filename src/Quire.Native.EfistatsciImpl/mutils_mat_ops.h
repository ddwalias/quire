#pragma once

#include "mutils_mat.h"

namespace mutils {

struct bit_and_op {
    template <typename T>
    T operator()(T a, T b) const {
        return static_cast<T>(a & b);
    }
};

struct bit_or_op {
    template <typename T>
    T operator()(T a, T b) const {
        return static_cast<T>(a | b);
    }
};

struct bit_xor_op {
    template <typename T>
    T operator()(T a, T b) const {
        return static_cast<T>(a ^ b);
    }
};

// Whether `out`, `a` and `b` have the same rows and columns. The exports return the mismatch message themselves: a
// message returned from an inlined helper comes back in rcx, and the original returns it in rax.
template <typename T>
__forceinline bool same_shape(const typed_mat<T> *a, const typed_mat<T> *b, const typed_mat<T> *out) {
    return out->rows == a->rows && out->cols == a->cols && a->rows == b->rows && a->cols == b->cols;
}

// Element-wise `out = op(a, b)`, then the interrupt step for the work done.
template <typename T, typename Op>
__forceinline void binary_elementwise(const typed_mat<T> *a, const typed_mat<T> *b, void *context, typed_mat<T> *out,
                                      Op op) {
    T *dst = out->data;
    const T *lhs = a->data;
    const T *rhs = b->data;
    int size = out->size;
    for (int i = 0; i < size; ++i) dst[i] = op(lhs[i], rhs[i]);
    double work = (double)size * 3.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
}

// Running total of `m`, stored to `*out` after each element, then the interrupt step for the work done.
template <typename T>
__forceinline void running_sum(const T *data, int size, void *context, T *out) {
    *out = 0;
    T total = 0;
    for (int i = 0; i < size; ++i) {
        total += data[i];
        *out = total;
    }
    double work = (double)size * 3.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
}

}  // namespace mutils
