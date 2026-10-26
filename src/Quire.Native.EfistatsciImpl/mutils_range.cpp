#include "mutils_mat.h"

namespace mutils {

EFI_EXPORT const char *matdbl_range(Efi::EfiCallContext *, const _double_mat *m, void *context, double *min, double *max) {
    check_src_f(m);
    double *data = m->data;
    int size = m->size;
    if (max == nullptr || min == nullptr) return "null pointer";
    *max = *min = data[0];
    for (int i = 1; i < size; ++i) {
        if (data[i] > *max) *max = data[i];
        if (data[i] < *min) *min = data[i];
    }
    double work = (double)size * 5.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *matflt_range(Efi::EfiCallContext *, const _float_mat *m, void *context, float *min, float *max) {
    check_src_f(m);
    float *data = m->data;
    int size = m->size;
    if (max == nullptr || min == nullptr) return "null pointer";
    *max = *min = data[0];
    for (int i = 1; i < size; ++i) {
        if (data[i] > *max) *max = data[i];
        if (data[i] < *min) *min = data[i];
    }
    double work = (double)size * 5.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *mats32_range(Efi::EfiCallContext *, const _sint32_mat *m, void *context, long *min, long *max) {
    check_src_f(m);
    long *data = m->data;
    int size = m->size;
    if (max == nullptr || min == nullptr) return "null pointer";
    *max = *min = data[0];
    for (int i = 1; i < size; ++i) {
        if (data[i] > *max) *max = data[i];
        if (data[i] < *min) *min = data[i];
    }
    double work = (double)size * 5.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *mats16_range(Efi::EfiCallContext *, const _sint16_mat *m, void *context, short *min, short *max) {
    check_src_f(m);
    short *data = m->data;
    int size = m->size;
    if (max == nullptr || min == nullptr) return "null pointer";
    *max = *min = data[0];
    for (int i = 1; i < size; ++i) {
        if (data[i] > *max) *max = data[i];
        if (data[i] < *min) *min = data[i];
    }
    double work = (double)size * 5.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *matu32_range(Efi::EfiCallContext *, const _uint32_mat *m, void *context, unsigned int *min, unsigned int *max) {
    check_src_f(m);
    unsigned int *data = m->data;
    int size = m->size;
    if (max == nullptr || min == nullptr) return "null pointer";
    *max = *min = data[0];
    for (int i = 1; i < size; ++i) {
        if (data[i] > *max) *max = data[i];
        if (data[i] < *min) *min = data[i];
    }
    double work = (double)size * 5.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *matu16_range(Efi::EfiCallContext *, const _uint16_mat *m, void *context, unsigned short *min, unsigned short *max) {
    check_src_f(m);
    unsigned short *data = m->data;
    int size = m->size;
    if (max == nullptr || min == nullptr) return "null pointer";
    *max = *min = data[0];
    for (int i = 1; i < size; ++i) {
        if (data[i] > *max) *max = data[i];
        if (data[i] < *min) *min = data[i];
    }
    double work = (double)size * 5.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *matu8_range(Efi::EfiCallContext *, const _uint8_mat *m, void *context, unsigned char *min, unsigned char *max) {
    check_src_f(m);
    unsigned char *data = m->data;
    int size = m->size;
    if (max == nullptr || min == nullptr) return "null pointer";
    *max = *min = data[0];
    for (int i = 1; i < size; ++i) {
        if (data[i] > *max) *max = data[i];
        if (data[i] < *min) *min = data[i];
    }
    double work = (double)size * 5.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

}  // namespace mutils
