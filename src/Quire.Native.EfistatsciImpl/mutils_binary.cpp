#include "mutils_mat.h"

namespace mutils {

EFI_EXPORT const char *matdbl_max(Efi::EfiCallContext *, const _double_mat *a, const _double_mat *b, void *context, _double_mat *dst) {
    check_src_f(a);
    check_dst_f(b);
    check_dst_f(dst);
    double *in_a = a->data;
    double *in_b = b->data;
    double *out = dst->data;
    int size = dst->size;
    if (dst->rows != a->rows || dst->cols != a->cols || a->rows != b->rows || a->cols != b->cols) return "array size mismatch";
    for (int i = 0; i < size; ++i) out[i] = in_a[i] > in_b[i] ? in_a[i] : in_b[i];
    double work = (double)size * 3.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *matflt_max(Efi::EfiCallContext *, const _float_mat *a, const _float_mat *b, void *context, _float_mat *dst) {
    check_src_f(a);
    check_dst_f(b);
    check_dst_f(dst);
    float *in_a = a->data;
    float *in_b = b->data;
    float *out = dst->data;
    int size = dst->size;
    if (dst->rows != a->rows || dst->cols != a->cols || a->rows != b->rows || a->cols != b->cols) return "array size mismatch";
    for (int i = 0; i < size; ++i) out[i] = in_a[i] > in_b[i] ? in_a[i] : in_b[i];
    double work = (double)size * 3.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *mats32_max(Efi::EfiCallContext *, const _sint32_mat *a, const _sint32_mat *b, void *context, _sint32_mat *dst) {
    check_src_f(a);
    check_dst_f(b);
    check_dst_f(dst);
    long *in_a = a->data;
    long *in_b = b->data;
    long *out = dst->data;
    int size = dst->size;
    if (dst->rows != a->rows || dst->cols != a->cols || a->rows != b->rows || a->cols != b->cols) return "array size mismatch";
    for (int i = 0; i < size; ++i) out[i] = in_a[i] > in_b[i] ? in_a[i] : in_b[i];
    double work = (double)size * 3.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *mats16_max(Efi::EfiCallContext *, const _sint16_mat *a, const _sint16_mat *b, void *context, _sint16_mat *dst) {
    check_src_f(a);
    check_dst_f(b);
    check_dst_f(dst);
    short *in_a = a->data;
    short *in_b = b->data;
    short *out = dst->data;
    int size = dst->size;
    if (dst->rows != a->rows || dst->cols != a->cols || a->rows != b->rows || a->cols != b->cols) return "array size mismatch";
    for (int i = 0; i < size; ++i) out[i] = in_a[i] > in_b[i] ? in_a[i] : in_b[i];
    double work = (double)size * 3.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *matu32_max(Efi::EfiCallContext *, const _uint32_mat *a, const _uint32_mat *b, void *context, _uint32_mat *dst) {
    check_src_f(a);
    check_dst_f(b);
    check_dst_f(dst);
    unsigned int *in_a = a->data;
    unsigned int *in_b = b->data;
    unsigned int *out = dst->data;
    int size = dst->size;
    if (dst->rows != a->rows || dst->cols != a->cols || a->rows != b->rows || a->cols != b->cols) return "array size mismatch";
    for (int i = 0; i < size; ++i) out[i] = in_a[i] > in_b[i] ? in_a[i] : in_b[i];
    double work = (double)size * 3.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *matu16_max(Efi::EfiCallContext *, const _uint16_mat *a, const _uint16_mat *b, void *context, _uint16_mat *dst) {
    check_src_f(a);
    check_dst_f(b);
    check_dst_f(dst);
    unsigned short *in_a = a->data;
    unsigned short *in_b = b->data;
    unsigned short *out = dst->data;
    int size = dst->size;
    if (dst->rows != a->rows || dst->cols != a->cols || a->rows != b->rows || a->cols != b->cols) return "array size mismatch";
    for (int i = 0; i < size; ++i) out[i] = in_a[i] > in_b[i] ? in_a[i] : in_b[i];
    double work = (double)size * 3.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *matu8_max(Efi::EfiCallContext *, const _uint8_mat *a, const _uint8_mat *b, void *context, _uint8_mat *dst) {
    check_src_f(a);
    check_dst_f(b);
    check_dst_f(dst);
    unsigned char *in_a = a->data;
    unsigned char *in_b = b->data;
    unsigned char *out = dst->data;
    int size = dst->size;
    if (dst->rows != a->rows || dst->cols != a->cols || a->rows != b->rows || a->cols != b->cols) return "array size mismatch";
    for (int i = 0; i < size; ++i) out[i] = in_a[i] > in_b[i] ? in_a[i] : in_b[i];
    double work = (double)size * 3.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *matdbl_min(Efi::EfiCallContext *, const _double_mat *a, const _double_mat *b, void *context, _double_mat *dst) {
    check_src_f(a);
    check_dst_f(b);
    check_dst_f(dst);
    double *in_a = a->data;
    double *in_b = b->data;
    double *out = dst->data;
    int size = dst->size;
    if (dst->rows != a->rows || dst->cols != a->cols || a->rows != b->rows || a->cols != b->cols) return "array size mismatch";
    for (int i = 0; i < size; ++i) out[i] = in_a[i] > in_b[i] ? in_b[i] : in_a[i];
    double work = (double)size * 3.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *matflt_min(Efi::EfiCallContext *, const _float_mat *a, const _float_mat *b, void *context, _float_mat *dst) {
    check_src_f(a);
    check_dst_f(b);
    check_dst_f(dst);
    float *in_a = a->data;
    float *in_b = b->data;
    float *out = dst->data;
    int size = dst->size;
    if (dst->rows != a->rows || dst->cols != a->cols || a->rows != b->rows || a->cols != b->cols) return "array size mismatch";
    for (int i = 0; i < size; ++i) out[i] = in_a[i] > in_b[i] ? in_b[i] : in_a[i];
    double work = (double)size * 3.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *mats32_min(Efi::EfiCallContext *, const _sint32_mat *a, const _sint32_mat *b, void *context, _sint32_mat *dst) {
    check_src_f(a);
    check_dst_f(b);
    check_dst_f(dst);
    long *in_a = a->data;
    long *in_b = b->data;
    long *out = dst->data;
    int size = dst->size;
    if (dst->rows != a->rows || dst->cols != a->cols || a->rows != b->rows || a->cols != b->cols) return "array size mismatch";
    for (int i = 0; i < size; ++i) out[i] = in_a[i] > in_b[i] ? in_b[i] : in_a[i];
    double work = (double)size * 3.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *mats16_min(Efi::EfiCallContext *, const _sint16_mat *a, const _sint16_mat *b, void *context, _sint16_mat *dst) {
    check_src_f(a);
    check_dst_f(b);
    check_dst_f(dst);
    short *in_a = a->data;
    short *in_b = b->data;
    short *out = dst->data;
    int size = dst->size;
    if (dst->rows != a->rows || dst->cols != a->cols || a->rows != b->rows || a->cols != b->cols) return "array size mismatch";
    for (int i = 0; i < size; ++i) out[i] = in_a[i] > in_b[i] ? in_b[i] : in_a[i];
    double work = (double)size * 3.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *matu32_min(Efi::EfiCallContext *, const _uint32_mat *a, const _uint32_mat *b, void *context, _uint32_mat *dst) {
    check_src_f(a);
    check_dst_f(b);
    check_dst_f(dst);
    unsigned int *in_a = a->data;
    unsigned int *in_b = b->data;
    unsigned int *out = dst->data;
    int size = dst->size;
    if (dst->rows != a->rows || dst->cols != a->cols || a->rows != b->rows || a->cols != b->cols) return "array size mismatch";
    for (int i = 0; i < size; ++i) out[i] = in_a[i] > in_b[i] ? in_b[i] : in_a[i];
    double work = (double)size * 3.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *matu16_min(Efi::EfiCallContext *, const _uint16_mat *a, const _uint16_mat *b, void *context, _uint16_mat *dst) {
    check_src_f(a);
    check_dst_f(b);
    check_dst_f(dst);
    unsigned short *in_a = a->data;
    unsigned short *in_b = b->data;
    unsigned short *out = dst->data;
    int size = dst->size;
    if (dst->rows != a->rows || dst->cols != a->cols || a->rows != b->rows || a->cols != b->cols) return "array size mismatch";
    for (int i = 0; i < size; ++i) out[i] = in_a[i] > in_b[i] ? in_b[i] : in_a[i];
    double work = (double)size * 3.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *matu8_min(Efi::EfiCallContext *, const _uint8_mat *a, const _uint8_mat *b, void *context, _uint8_mat *dst) {
    check_src_f(a);
    check_dst_f(b);
    check_dst_f(dst);
    unsigned char *in_a = a->data;
    unsigned char *in_b = b->data;
    unsigned char *out = dst->data;
    int size = dst->size;
    if (dst->rows != a->rows || dst->cols != a->cols || a->rows != b->rows || a->cols != b->cols) return "array size mismatch";
    for (int i = 0; i < size; ++i) out[i] = in_a[i] > in_b[i] ? in_b[i] : in_a[i];
    double work = (double)size * 3.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *matdbl_add(Efi::EfiCallContext *, const _double_mat *a, const _double_mat *b, void *context, _double_mat *dst) {
    check_src_f(a);
    check_dst_f(b);
    check_dst_f(dst);
    double *in_a = a->data;
    double *in_b = b->data;
    double *out = dst->data;
    int size = dst->size;
    if (dst->rows != a->rows || dst->cols != a->cols || a->rows != b->rows || a->cols != b->cols) return "array size mismatch";
    for (int i = 0; i < size; ++i) out[i] = in_a[i] + in_b[i];
    double work = (double)size * 3.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *matflt_add(Efi::EfiCallContext *, const _float_mat *a, const _float_mat *b, void *context, _float_mat *dst) {
    check_src_f(a);
    check_dst_f(b);
    check_dst_f(dst);
    float *in_a = a->data;
    float *in_b = b->data;
    float *out = dst->data;
    int size = dst->size;
    if (dst->rows != a->rows || dst->cols != a->cols || a->rows != b->rows || a->cols != b->cols) return "array size mismatch";
    for (int i = 0; i < size; ++i) out[i] = in_a[i] + in_b[i];
    double work = (double)size * 3.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *mats32_add(Efi::EfiCallContext *, const _sint32_mat *a, const _sint32_mat *b, void *context, _sint32_mat *dst) {
    check_src_f(a);
    check_dst_f(b);
    check_dst_f(dst);
    long *in_a = a->data;
    long *in_b = b->data;
    long *out = dst->data;
    int size = dst->size;
    if (dst->rows != a->rows || dst->cols != a->cols || a->rows != b->rows || a->cols != b->cols) return "array size mismatch";
    for (int i = 0; i < size; ++i) out[i] = in_a[i] + in_b[i];
    double work = (double)size * 3.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *mats16_add(Efi::EfiCallContext *, const _sint16_mat *a, const _sint16_mat *b, void *context, _sint16_mat *dst) {
    check_src_f(a);
    check_dst_f(b);
    check_dst_f(dst);
    short *in_a = a->data;
    short *in_b = b->data;
    short *out = dst->data;
    int size = dst->size;
    if (dst->rows != a->rows || dst->cols != a->cols || a->rows != b->rows || a->cols != b->cols) return "array size mismatch";
    for (int i = 0; i < size; ++i) out[i] = in_a[i] + in_b[i];
    double work = (double)size * 3.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *matu32_add(Efi::EfiCallContext *, const _uint32_mat *a, const _uint32_mat *b, void *context, _uint32_mat *dst) {
    check_src_f(a);
    check_dst_f(b);
    check_dst_f(dst);
    unsigned int *in_a = a->data;
    unsigned int *in_b = b->data;
    unsigned int *out = dst->data;
    int size = dst->size;
    if (dst->rows != a->rows || dst->cols != a->cols || a->rows != b->rows || a->cols != b->cols) return "array size mismatch";
    for (int i = 0; i < size; ++i) out[i] = in_a[i] + in_b[i];
    double work = (double)size * 3.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *matu16_add(Efi::EfiCallContext *, const _uint16_mat *a, const _uint16_mat *b, void *context, _uint16_mat *dst) {
    check_src_f(a);
    check_dst_f(b);
    check_dst_f(dst);
    unsigned short *in_a = a->data;
    unsigned short *in_b = b->data;
    unsigned short *out = dst->data;
    int size = dst->size;
    if (dst->rows != a->rows || dst->cols != a->cols || a->rows != b->rows || a->cols != b->cols) return "array size mismatch";
    for (int i = 0; i < size; ++i) out[i] = in_a[i] + in_b[i];
    double work = (double)size * 3.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *matu8_add(Efi::EfiCallContext *, const _uint8_mat *a, const _uint8_mat *b, void *context, _uint8_mat *dst) {
    check_src_f(a);
    check_dst_f(b);
    check_dst_f(dst);
    unsigned char *in_a = a->data;
    unsigned char *in_b = b->data;
    unsigned char *out = dst->data;
    int size = dst->size;
    if (dst->rows != a->rows || dst->cols != a->cols || a->rows != b->rows || a->cols != b->cols) return "array size mismatch";
    for (int i = 0; i < size; ++i) out[i] = in_a[i] + in_b[i];
    double work = (double)size * 3.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *matdbl_subtract(Efi::EfiCallContext *, const _double_mat *a, const _double_mat *b, void *context, _double_mat *dst) {
    check_src_f(a);
    check_dst_f(b);
    check_dst_f(dst);
    double *in_a = a->data;
    double *in_b = b->data;
    double *out = dst->data;
    int size = dst->size;
    if (dst->rows != a->rows || dst->cols != a->cols || a->rows != b->rows || a->cols != b->cols) return "array size mismatch";
    for (int i = 0; i < size; ++i) out[i] = in_a[i] - in_b[i];
    double work = (double)size * 3.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *matflt_subtract(Efi::EfiCallContext *, const _float_mat *a, const _float_mat *b, void *context, _float_mat *dst) {
    check_src_f(a);
    check_dst_f(b);
    check_dst_f(dst);
    float *in_a = a->data;
    float *in_b = b->data;
    float *out = dst->data;
    int size = dst->size;
    if (dst->rows != a->rows || dst->cols != a->cols || a->rows != b->rows || a->cols != b->cols) return "array size mismatch";
    for (int i = 0; i < size; ++i) out[i] = in_a[i] - in_b[i];
    double work = (double)size * 3.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *mats32_subtract(Efi::EfiCallContext *, const _sint32_mat *a, const _sint32_mat *b, void *context, _sint32_mat *dst) {
    check_src_f(a);
    check_dst_f(b);
    check_dst_f(dst);
    long *in_a = a->data;
    long *in_b = b->data;
    long *out = dst->data;
    int size = dst->size;
    if (dst->rows != a->rows || dst->cols != a->cols || a->rows != b->rows || a->cols != b->cols) return "array size mismatch";
    for (int i = 0; i < size; ++i) out[i] = in_a[i] - in_b[i];
    double work = (double)size * 3.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *mats16_subtract(Efi::EfiCallContext *, const _sint16_mat *a, const _sint16_mat *b, void *context, _sint16_mat *dst) {
    check_src_f(a);
    check_src_f(b);
    check_dst_f(dst);
    short *in_a = a->data;
    short *in_b = b->data;
    short *out = dst->data;
    int size = dst->size;
    if (dst->rows != a->rows || dst->cols != a->cols || a->rows != b->rows || a->cols != b->cols) return "array size mismatch";
    for (int i = 0; i < size; ++i) out[i] = in_a[i] - in_b[i];
    double work = (double)size * 3.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *matu32_subtract(Efi::EfiCallContext *, const _uint32_mat *a, const _uint32_mat *b, void *context, _uint32_mat *dst) {
    check_src_f(a);
    check_dst_f(b);
    check_dst_f(dst);
    unsigned int *in_a = a->data;
    unsigned int *in_b = b->data;
    unsigned int *out = dst->data;
    int size = dst->size;
    if (dst->rows != a->rows || dst->cols != a->cols || a->rows != b->rows || a->cols != b->cols) return "array size mismatch";
    for (int i = 0; i < size; ++i) out[i] = in_a[i] - in_b[i];
    double work = (double)size * 3.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *matu16_subtract(Efi::EfiCallContext *, const _uint16_mat *a, const _uint16_mat *b, void *context, _uint16_mat *dst) {
    check_src_f(a);
    check_src_f(b);
    check_dst_f(dst);
    unsigned short *in_a = a->data;
    unsigned short *in_b = b->data;
    unsigned short *out = dst->data;
    int size = dst->size;
    if (dst->rows != a->rows || dst->cols != a->cols || a->rows != b->rows || a->cols != b->cols) return "array size mismatch";
    for (int i = 0; i < size; ++i) out[i] = in_a[i] - in_b[i];
    double work = (double)size * 3.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *matu8_subtract(Efi::EfiCallContext *, const _uint8_mat *a, const _uint8_mat *b, void *context, _uint8_mat *dst) {
    check_src_f(a);
    check_src_f(b);
    check_dst_f(dst);
    unsigned char *in_a = a->data;
    unsigned char *in_b = b->data;
    unsigned char *out = dst->data;
    int size = dst->size;
    if (dst->rows != a->rows || dst->cols != a->cols || a->rows != b->rows || a->cols != b->cols) return "array size mismatch";
    for (int i = 0; i < size; ++i) out[i] = in_a[i] - in_b[i];
    double work = (double)size * 3.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *matdbl_multiply_elem(Efi::EfiCallContext *, const _double_mat *a, const _double_mat *b, void *context, _double_mat *dst) {
    check_src_f(a);
    check_dst_f(b);
    check_dst_f(dst);
    double *in_a = a->data;
    double *in_b = b->data;
    double *out = dst->data;
    int size = dst->size;
    if (dst->rows != a->rows || dst->cols != a->cols || a->rows != b->rows || a->cols != b->cols) return "array size mismatch";
    for (int i = 0; i < size; ++i) out[i] = in_a[i] * in_b[i];
    double work = (double)size * 3.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *matflt_multiply_elem(Efi::EfiCallContext *, const _float_mat *a, const _float_mat *b, void *context, _float_mat *dst) {
    check_src_f(a);
    check_dst_f(b);
    check_dst_f(dst);
    float *in_a = a->data;
    float *in_b = b->data;
    float *out = dst->data;
    int size = dst->size;
    if (dst->rows != a->rows || dst->cols != a->cols || a->rows != b->rows || a->cols != b->cols) return "array size mismatch";
    for (int i = 0; i < size; ++i) out[i] = in_a[i] * in_b[i];
    double work = (double)size * 3.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *mats32_multiply_elem(Efi::EfiCallContext *, const _sint32_mat *a, const _sint32_mat *b, void *context, _sint32_mat *dst) {
    check_src_f(a);
    check_dst_f(b);
    check_dst_f(dst);
    long *in_a = a->data;
    long *in_b = b->data;
    long *out = dst->data;
    int size = dst->size;
    if (dst->rows != a->rows || dst->cols != a->cols || a->rows != b->rows || a->cols != b->cols) return "array size mismatch";
    for (int i = 0; i < size; ++i) out[i] = in_a[i] * in_b[i];
    double work = (double)size * 3.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *mats16_multiply_elem(Efi::EfiCallContext *, const _sint16_mat *a, const _sint16_mat *b, void *context, _sint16_mat *dst) {
    check_src_f(a);
    check_dst_f(b);
    check_dst_f(dst);
    short *in_a = a->data;
    short *in_b = b->data;
    short *out = dst->data;
    int size = dst->size;
    if (dst->rows != a->rows || dst->cols != a->cols || a->rows != b->rows || a->cols != b->cols) return "array size mismatch";
    for (int i = 0; i < size; ++i) out[i] = in_a[i] * in_b[i];
    double work = (double)size * 3.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *matu32_multiply_elem(Efi::EfiCallContext *, const _uint32_mat *a, const _uint32_mat *b, void *context, _uint32_mat *dst) {
    check_src_f(a);
    check_dst_f(b);
    check_dst_f(dst);
    unsigned int *in_a = a->data;
    unsigned int *in_b = b->data;
    unsigned int *out = dst->data;
    int size = dst->size;
    if (dst->rows != a->rows || dst->cols != a->cols || a->rows != b->rows || a->cols != b->cols) return "array size mismatch";
    for (int i = 0; i < size; ++i) out[i] = in_a[i] * in_b[i];
    double work = (double)size * 3.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *matu16_multiply_elem(Efi::EfiCallContext *, const _uint16_mat *a, const _uint16_mat *b, void *context, _uint16_mat *dst) {
    check_src_f(a);
    check_dst_f(b);
    check_dst_f(dst);
    unsigned short *in_a = a->data;
    unsigned short *in_b = b->data;
    unsigned short *out = dst->data;
    int size = dst->size;
    if (dst->rows != a->rows || dst->cols != a->cols || a->rows != b->rows || a->cols != b->cols) return "array size mismatch";
    for (int i = 0; i < size; ++i) out[i] = in_a[i] * in_b[i];
    double work = (double)size * 3.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *matu8_multiply_elem(Efi::EfiCallContext *, const _uint8_mat *a, const _uint8_mat *b, void *context, _uint8_mat *dst) {
    check_src_f(a);
    check_dst_f(b);
    check_dst_f(dst);
    unsigned char *in_a = a->data;
    unsigned char *in_b = b->data;
    unsigned char *out = dst->data;
    int size = dst->size;
    if (dst->rows != a->rows || dst->cols != a->cols || a->rows != b->rows || a->cols != b->cols) return "array size mismatch";
    for (int i = 0; i < size; ++i) out[i] = in_a[i] * in_b[i];
    double work = (double)size * 3.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

}  // namespace mutils
