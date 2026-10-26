#include "mutils_mat.h"

namespace mutils {

template <typename M>
__forceinline void check_sum_input(const M *m) {
    if (m == nullptr) Efi::Throw("null pointer", ~0ull);
    int cols = m->cols;
    if (m->rows <= 0 || cols <= 0) Efi::Throw("array size mismatch", ~0ull);
    if (m->data == nullptr) Efi::Throw("null pointer", ~0ull);
    if (m->size != m->rows * cols) Efi::Throw("array size mismatch", ~0ull);
}

template <typename T>
__forceinline void running_sum(const T *data, int size, T *out) {
    *out = 0;
    for (int i = 0; i < size; ++i) *out += data[i];
}

EFI_EXPORT const char *matdbl_sum(Efi::EfiCallContext *, const _double_mat *m, void *context, double *out) {
    check_sum_input(m);
    auto *data = m->data;
    int size = m->size;
    if (out == nullptr) return "null pointer";
    running_sum(data, size, out);
    double work = (double)size * 3.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *matflt_sum(Efi::EfiCallContext *, const _float_mat *m, void *context, float *out) {
    check_sum_input(m);
    auto *data = m->data;
    int size = m->size;
    if (out == nullptr) return "null pointer";
    running_sum(data, size, out);
    double work = (double)size * 3.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *mats32_sum(Efi::EfiCallContext *, const _sint32_mat *m, void *context, long *out) {
    check_sum_input(m);
    auto *data = m->data;
    int size = m->size;
    if (out == nullptr) return "null pointer";
    running_sum(data, size, out);
    double work = (double)size * 3.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *mats16_sum(Efi::EfiCallContext *, const _sint16_mat *m, void *context, short *out) {
    check_sum_input(m);
    auto *data = m->data;
    int size = m->size;
    if (out == nullptr) return "null pointer";
    running_sum(data, size, out);
    double work = (double)size * 3.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *matu32_sum(Efi::EfiCallContext *, const _uint32_mat *m, void *context, unsigned int *out) {
    check_sum_input(m);
    auto *data = m->data;
    int size = m->size;
    if (out == nullptr) return "null pointer";
    running_sum(data, size, out);
    double work = (double)size * 3.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *matu16_sum(Efi::EfiCallContext *, const _uint16_mat *m, void *context, unsigned short *out) {
    check_sum_input(m);
    auto *data = m->data;
    int size = m->size;
    if (out == nullptr) return "null pointer";
    running_sum(data, size, out);
    double work = (double)size * 3.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *matu8_sum(Efi::EfiCallContext *, const _uint8_mat *m, void *context, unsigned char *out) {
    check_sum_input(m);
    auto *data = m->data;
    int size = m->size;
    if (out == nullptr) return "null pointer";
    running_sum(data, size, out);
    double work = (double)size * 3.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

}  // namespace mutils
