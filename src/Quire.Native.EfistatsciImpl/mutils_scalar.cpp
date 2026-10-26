#include "mutils_mat.h"

namespace mutils {

EFI_EXPORT const char *matdbl_add_scalar(Efi::EfiCallContext *, const _double_mat *src, double value, void *context, _double_mat *dst) {
    check_src_f(src);
    check_dst_f(dst);
    double *in = src->data;
    double *out = dst->data;
    int size = dst->size;
    if (dst->rows != src->rows || dst->cols != src->cols) return "array size mismatch";
    for (int i = 0; i < size; ++i) out[i] = in[i] + value;
    double work = (double)size * 3.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *matflt_add_scalar(Efi::EfiCallContext *, const _float_mat *src, float value, void *context, _float_mat *dst) {
    check_src_f(src);
    check_dst_f(dst);
    float *in = src->data;
    float *out = dst->data;
    int size = dst->size;
    if (dst->rows != src->rows || dst->cols != src->cols) return "array size mismatch";
    for (int i = 0; i < size; ++i) out[i] = in[i] + value;
    double work = (double)size * 3.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *mats32_add_scalar(Efi::EfiCallContext *, const _sint32_mat *src, long value, void *context, _sint32_mat *dst) {
    check_src_f(src);
    check_dst_f(dst);
    long *in = src->data;
    long *out = dst->data;
    int size = dst->size;
    if (dst->rows != src->rows || dst->cols != src->cols) return "array size mismatch";
    for (int i = 0; i < size; ++i) out[i] = in[i] + value;
    double work = (double)size * 3.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *mats16_add_scalar(Efi::EfiCallContext *, const _sint16_mat *src, short value, void *context, _sint16_mat *dst) {
    check_src_f(src);
    check_dst_f(dst);
    short *in = src->data;
    short *out = dst->data;
    int size = dst->size;
    if (dst->rows != src->rows || dst->cols != src->cols) return "array size mismatch";
    for (int i = 0; i < size; ++i) out[i] = in[i] + value;
    double work = (double)size * 3.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *matu32_add_scalar(Efi::EfiCallContext *, const _uint32_mat *src, unsigned int value, void *context, _uint32_mat *dst) {
    check_src_f(src);
    check_dst_f(dst);
    unsigned int *in = src->data;
    unsigned int *out = dst->data;
    int size = dst->size;
    if (dst->rows != src->rows || dst->cols != src->cols) return "array size mismatch";
    for (int i = 0; i < size; ++i) out[i] = in[i] + value;
    double work = (double)size * 3.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *matu16_add_scalar(Efi::EfiCallContext *, const _uint16_mat *src, unsigned short value, void *context, _uint16_mat *dst) {
    check_src_f(src);
    check_dst_f(dst);
    unsigned short *in = src->data;
    unsigned short *out = dst->data;
    int size = dst->size;
    if (dst->rows != src->rows || dst->cols != src->cols) return "array size mismatch";
    for (int i = 0; i < size; ++i) out[i] = in[i] + value;
    double work = (double)size * 3.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *matu8_add_scalar(Efi::EfiCallContext *, const _uint8_mat *src, unsigned char value, void *context, _uint8_mat *dst) {
    check_src_f(src);
    check_dst_f(dst);
    unsigned char *in = src->data;
    unsigned char *out = dst->data;
    int size = dst->size;
    if (dst->rows != src->rows || dst->cols != src->cols) return "array size mismatch";
    for (int i = 0; i < size; ++i) out[i] = in[i] + value;
    double work = (double)size * 3.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *matdbl_multiply_scalar(Efi::EfiCallContext *, const _double_mat *src, double value, void *context, _double_mat *dst) {
    check_src_f(src);
    check_dst_f(dst);
    double *in = src->data;
    double *out = dst->data;
    int size = dst->size;
    if (dst->rows != src->rows || dst->cols != src->cols) return "array size mismatch";
    for (int i = 0; i < size; ++i) out[i] = in[i] * value;
    double work = (double)size * 3.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *matflt_multiply_scalar(Efi::EfiCallContext *, const _float_mat *src, float value, void *context, _float_mat *dst) {
    check_src_f(src);
    check_dst_f(dst);
    float *in = src->data;
    float *out = dst->data;
    int size = dst->size;
    if (dst->rows != src->rows || dst->cols != src->cols) return "array size mismatch";
    for (int i = 0; i < size; ++i) out[i] = in[i] * value;
    double work = (double)size * 3.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *mats32_multiply_scalar(Efi::EfiCallContext *, const _sint32_mat *src, long value, void *context, _sint32_mat *dst) {
    check_src_f(src);
    check_dst_f(dst);
    long *in = src->data;
    long *out = dst->data;
    int size = dst->size;
    if (dst->rows != src->rows || dst->cols != src->cols) return "array size mismatch";
    for (int i = 0; i < size; ++i) out[i] = in[i] * value;
    double work = (double)size * 3.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *mats16_multiply_scalar(Efi::EfiCallContext *, const _sint16_mat *src, short value, void *context, _sint16_mat *dst) {
    check_src_f(src);
    check_dst_f(dst);
    short *in = src->data;
    short *out = dst->data;
    int size = dst->size;
    if (dst->rows != src->rows || dst->cols != src->cols) return "array size mismatch";
    for (int i = 0; i < size; ++i) out[i] = in[i] * value;
    double work = (double)size * 3.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *matu32_multiply_scalar(Efi::EfiCallContext *, const _uint32_mat *src, unsigned int value, void *context, _uint32_mat *dst) {
    check_src_f(src);
    check_dst_f(dst);
    unsigned int *in = src->data;
    unsigned int *out = dst->data;
    int size = dst->size;
    if (dst->rows != src->rows || dst->cols != src->cols) return "array size mismatch";
    for (int i = 0; i < size; ++i) out[i] = in[i] * value;
    double work = (double)size * 3.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *matu16_multiply_scalar(Efi::EfiCallContext *, const _uint16_mat *src, unsigned short value, void *context, _uint16_mat *dst) {
    check_src_f(src);
    check_dst_f(dst);
    unsigned short *in = src->data;
    unsigned short *out = dst->data;
    int size = dst->size;
    if (dst->rows != src->rows || dst->cols != src->cols) return "array size mismatch";
    for (int i = 0; i < size; ++i) out[i] = in[i] * value;
    double work = (double)size * 3.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *matu8_multiply_scalar(Efi::EfiCallContext *, const _uint8_mat *src, unsigned char value, void *context, _uint8_mat *dst) {
    check_src_f(src);
    check_dst_f(dst);
    unsigned char *in = src->data;
    unsigned char *out = dst->data;
    int size = dst->size;
    if (dst->rows != src->rows || dst->cols != src->cols) return "array size mismatch";
    for (int i = 0; i < size; ++i) out[i] = in[i] * value;
    double work = (double)size * 3.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

}  // namespace mutils
