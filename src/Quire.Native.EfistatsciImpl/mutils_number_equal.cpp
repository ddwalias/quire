#include "mutils_mat.h"

namespace mutils {

EFI_EXPORT const char *matdbl_number_equal(Efi::EfiCallContext *, const _double_mat *a, const _double_mat *b, void *context, unsigned int *count) {
    check_src_f(a);
    check_dst_f(b);
    double *a_data = a->data;
    double *b_data = b->data;
    int size = a->size;
    if (count == nullptr) return "null pointer";
    if (a->rows != b->rows || a->cols != b->cols) return "array size mismatch";
    unsigned int equal = 0;
    for (int i = 0; i < size; ++i) {
        if (a_data[i] == b_data[i]) ++equal;
    }
    *count = equal;
    double work = (double)size * 3.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *matflt_number_equal(Efi::EfiCallContext *, const _float_mat *a, const _float_mat *b, void *context, unsigned int *count) {
    check_src_f(a);
    check_dst_f(b);
    float *a_data = a->data;
    float *b_data = b->data;
    int size = a->size;
    if (count == nullptr) return "null pointer";
    if (a->rows != b->rows || a->cols != b->cols) return "array size mismatch";
    unsigned int equal = 0;
    for (int i = 0; i < size; ++i) {
        if (a_data[i] == b_data[i]) ++equal;
    }
    *count = equal;
    double work = (double)size * 3.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *mats32_number_equal(Efi::EfiCallContext *, const _sint32_mat *a, const _sint32_mat *b, void *context, unsigned int *count) {
    check_src_f(a);
    check_dst_f(b);
    long *a_data = a->data;
    long *b_data = b->data;
    int size = a->size;
    if (count == nullptr) return "null pointer";
    if (a->rows != b->rows || a->cols != b->cols) return "array size mismatch";
    unsigned int equal = 0;
    for (int i = 0; i < size; ++i) {
        if (a_data[i] == b_data[i]) ++equal;
    }
    *count = equal;
    double work = (double)size * 3.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *mats16_number_equal(Efi::EfiCallContext *, const _sint16_mat *a, const _sint16_mat *b, void *context, unsigned int *count) {
    check_src_f(a);
    check_dst_f(b);
    short *a_data = a->data;
    short *b_data = b->data;
    int size = a->size;
    if (count == nullptr) return "null pointer";
    if (a->rows != b->rows || a->cols != b->cols) return "array size mismatch";
    unsigned int equal = 0;
    for (int i = 0; i < size; ++i) {
        if (a_data[i] == b_data[i]) ++equal;
    }
    *count = equal;
    double work = (double)size * 3.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *matu32_number_equal(Efi::EfiCallContext *, const _uint32_mat *a, const _uint32_mat *b, void *context, unsigned int *count) {
    check_src_f(a);
    check_dst_f(b);
    unsigned int *a_data = a->data;
    unsigned int *b_data = b->data;
    int size = a->size;
    if (count == nullptr) return "null pointer";
    if (a->rows != b->rows || a->cols != b->cols) return "array size mismatch";
    unsigned int equal = 0;
    for (int i = 0; i < size; ++i) {
        if (a_data[i] == b_data[i]) ++equal;
    }
    *count = equal;
    double work = (double)size * 3.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *matu16_number_equal(Efi::EfiCallContext *, const _uint16_mat *a, const _uint16_mat *b, void *context, unsigned int *count) {
    check_src_f(a);
    check_dst_f(b);
    unsigned short *a_data = a->data;
    unsigned short *b_data = b->data;
    int size = a->size;
    if (count == nullptr) return "null pointer";
    if (a->rows != b->rows || a->cols != b->cols) return "array size mismatch";
    unsigned int equal = 0;
    for (int i = 0; i < size; ++i) {
        if (a_data[i] == b_data[i]) ++equal;
    }
    *count = equal;
    double work = (double)size * 3.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *matu8_number_equal(Efi::EfiCallContext *, const _uint8_mat *a, const _uint8_mat *b, void *context, unsigned int *count) {
    check_src_f(a);
    check_dst_f(b);
    unsigned char *a_data = a->data;
    unsigned char *b_data = b->data;
    int size = a->size;
    if (count == nullptr) return "null pointer";
    if (a->rows != b->rows || a->cols != b->cols) return "array size mismatch";
    unsigned int equal = 0;
    for (int i = 0; i < size; ++i) {
        if (a_data[i] == b_data[i]) ++equal;
    }
    *count = equal;
    double work = (double)size * 3.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

}  // namespace mutils
