#include "mutils_mat.h"

namespace mutils {

EFI_EXPORT const char *matu8_bit_shift_left(const _uint8_mat *src, unsigned char value, void *context, _uint8_mat *dst) {
    check_src_f(src);
    check_dst_f(dst);
    unsigned char *in = src->data;
    unsigned char *out = dst->data;
    int size = dst->size;
    if (dst->rows != src->rows || dst->cols != src->cols) return "array size mismatch";
    for (int i = 0; i < size; ++i) out[i] = in[i] << value;
    double work = (double)size * 3.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *matu16_bit_shift_left(const _uint16_mat *src, unsigned short value, void *context, _uint16_mat *dst) {
    check_src_f(src);
    check_dst_f(dst);
    unsigned short *in = src->data;
    unsigned short *out = dst->data;
    int size = dst->size;
    if (dst->rows != src->rows || dst->cols != src->cols) return "array size mismatch";
    for (int i = 0; i < size; ++i) out[i] = in[i] << value;
    double work = (double)size * 3.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *matu32_bit_shift_left(const _uint32_mat *src, unsigned int value, void *context, _uint32_mat *dst) {
    check_src_f(src);
    check_dst_f(dst);
    unsigned int *in = src->data;
    unsigned int *out = dst->data;
    int size = dst->size;
    if (dst->rows != src->rows || dst->cols != src->cols) return "array size mismatch";
    for (int i = 0; i < size; ++i) out[i] = in[i] << value;
    double work = (double)size * 3.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *matu8_bit_shift_right(const _uint8_mat *src, unsigned char value, void *context, _uint8_mat *dst) {
    check_src_f(src);
    check_dst_f(dst);
    unsigned char *in = src->data;
    unsigned char *out = dst->data;
    int size = dst->size;
    if (dst->rows != src->rows || dst->cols != src->cols) return "array size mismatch";
    for (int i = 0; i < size; ++i) out[i] = in[i] >> value;
    double work = (double)size * 3.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *matu16_bit_shift_right(const _uint16_mat *src, unsigned short value, void *context, _uint16_mat *dst) {
    check_src_f(src);
    check_dst_f(dst);
    unsigned short *in = src->data;
    unsigned short *out = dst->data;
    int size = dst->size;
    if (dst->rows != src->rows || dst->cols != src->cols) return "array size mismatch";
    for (int i = 0; i < size; ++i) out[i] = in[i] >> value;
    double work = (double)size * 3.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *matu32_bit_shift_right(const _uint32_mat *src, unsigned int value, void *context, _uint32_mat *dst) {
    check_src_f(src);
    check_dst_f(dst);
    unsigned int *in = src->data;
    unsigned int *out = dst->data;
    int size = dst->size;
    if (dst->rows != src->rows || dst->cols != src->cols) return "array size mismatch";
    for (int i = 0; i < size; ++i) out[i] = in[i] >> value;
    double work = (double)size * 3.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *matu8_bit_and_scalar(const _uint8_mat *src, unsigned char value, void *context, _uint8_mat *dst) {
    check_src_f(src);
    check_dst_f(dst);
    unsigned char *in = src->data;
    unsigned char *out = dst->data;
    int size = dst->size;
    if (dst->rows != src->rows || dst->cols != src->cols) return "array size mismatch";
    for (int i = 0; i < size; ++i) out[i] = in[i] & value;
    double work = (double)size * 3.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *matu16_bit_and_scalar(const _uint16_mat *src, unsigned short value, void *context, _uint16_mat *dst) {
    check_src_f(src);
    check_dst_f(dst);
    unsigned short *in = src->data;
    unsigned short *out = dst->data;
    int size = dst->size;
    if (dst->rows != src->rows || dst->cols != src->cols) return "array size mismatch";
    for (int i = 0; i < size; ++i) out[i] = in[i] & value;
    double work = (double)size * 3.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *matu32_bit_and_scalar(const _uint32_mat *src, unsigned int value, void *context, _uint32_mat *dst) {
    check_src_f(src);
    check_dst_f(dst);
    unsigned int *in = src->data;
    unsigned int *out = dst->data;
    int size = dst->size;
    if (dst->rows != src->rows || dst->cols != src->cols) return "array size mismatch";
    for (int i = 0; i < size; ++i) out[i] = in[i] & value;
    double work = (double)size * 3.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *matu8_bit_or_scalar(const _uint8_mat *src, unsigned char value, void *context, _uint8_mat *dst) {
    check_src_f(src);
    check_dst_f(dst);
    unsigned char *in = src->data;
    unsigned char *out = dst->data;
    int size = dst->size;
    if (dst->rows != src->rows || dst->cols != src->cols) return "array size mismatch";
    for (int i = 0; i < size; ++i) out[i] = in[i] | value;
    double work = (double)size * 3.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *matu16_bit_or_scalar(const _uint16_mat *src, unsigned short value, void *context, _uint16_mat *dst) {
    check_src_f(src);
    check_dst_f(dst);
    unsigned short *in = src->data;
    unsigned short *out = dst->data;
    int size = dst->size;
    if (dst->rows != src->rows || dst->cols != src->cols) return "array size mismatch";
    for (int i = 0; i < size; ++i) out[i] = in[i] | value;
    double work = (double)size * 3.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *matu32_bit_or_scalar(const _uint32_mat *src, unsigned int value, void *context, _uint32_mat *dst) {
    check_src_f(src);
    check_dst_f(dst);
    unsigned int *in = src->data;
    unsigned int *out = dst->data;
    int size = dst->size;
    if (dst->rows != src->rows || dst->cols != src->cols) return "array size mismatch";
    for (int i = 0; i < size; ++i) out[i] = in[i] | value;
    double work = (double)size * 3.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *matu8_bit_xor_scalar(const _uint8_mat *src, unsigned char value, void *context, _uint8_mat *dst) {
    check_src_f(src);
    check_dst_f(dst);
    unsigned char *in = src->data;
    unsigned char *out = dst->data;
    int size = dst->size;
    if (dst->rows != src->rows || dst->cols != src->cols) return "array size mismatch";
    for (int i = 0; i < size; ++i) out[i] = in[i] ^ value;
    double work = (double)size * 3.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *matu16_bit_xor_scalar(const _uint16_mat *src, unsigned short value, void *context, _uint16_mat *dst) {
    check_src_f(src);
    check_dst_f(dst);
    unsigned short *in = src->data;
    unsigned short *out = dst->data;
    int size = dst->size;
    if (dst->rows != src->rows || dst->cols != src->cols) return "array size mismatch";
    for (int i = 0; i < size; ++i) out[i] = in[i] ^ value;
    double work = (double)size * 3.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *matu32_bit_xor_scalar(const _uint32_mat *src, unsigned int value, void *context, _uint32_mat *dst) {
    check_src_f(src);
    check_dst_f(dst);
    unsigned int *in = src->data;
    unsigned int *out = dst->data;
    int size = dst->size;
    if (dst->rows != src->rows || dst->cols != src->cols) return "array size mismatch";
    for (int i = 0; i < size; ++i) out[i] = in[i] ^ value;
    double work = (double)size * 3.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *matu8_bit_and(const _uint8_mat *a, const _uint8_mat *b, void *context, _uint8_mat *dst) {
    check_src_f(a);
    check_dst_f(b);
    check_dst_f(dst);
    unsigned char *in_a = a->data;
    unsigned char *in_b = b->data;
    unsigned char *out = dst->data;
    int size = dst->size;
    if (dst->rows != a->rows || dst->cols != a->cols || a->rows != b->rows || a->cols != b->cols) return "array size mismatch";
    for (int i = 0; i < size; ++i) out[i] = in_a[i] & in_b[i];
    double work = (double)size * 3.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *matu16_bit_and(const _uint16_mat *a, const _uint16_mat *b, void *context, _uint16_mat *dst) {
    check_src_f(a);
    check_dst_f(b);
    check_dst_f(dst);
    unsigned short *in_a = a->data;
    unsigned short *in_b = b->data;
    unsigned short *out = dst->data;
    int size = dst->size;
    if (dst->rows != a->rows || dst->cols != a->cols || a->rows != b->rows || a->cols != b->cols) return "array size mismatch";
    for (int i = 0; i < size; ++i) out[i] = in_a[i] & in_b[i];
    double work = (double)size * 3.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *matu32_bit_and(const _uint32_mat *a, const _uint32_mat *b, void *context, _uint32_mat *dst) {
    check_src_f(a);
    check_dst_f(b);
    check_dst_f(dst);
    unsigned int *in_a = a->data;
    unsigned int *in_b = b->data;
    unsigned int *out = dst->data;
    int size = dst->size;
    if (dst->rows != a->rows || dst->cols != a->cols || a->rows != b->rows || a->cols != b->cols) return "array size mismatch";
    for (int i = 0; i < size; ++i) out[i] = in_a[i] & in_b[i];
    double work = (double)size * 3.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *matu8_bit_or(const _uint8_mat *a, const _uint8_mat *b, void *context, _uint8_mat *dst) {
    check_src_f(a);
    check_dst_f(b);
    check_dst_f(dst);
    unsigned char *in_a = a->data;
    unsigned char *in_b = b->data;
    unsigned char *out = dst->data;
    int size = dst->size;
    if (dst->rows != a->rows || dst->cols != a->cols || a->rows != b->rows || a->cols != b->cols) return "array size mismatch";
    for (int i = 0; i < size; ++i) out[i] = in_a[i] | in_b[i];
    double work = (double)size * 3.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *matu16_bit_or(const _uint16_mat *a, const _uint16_mat *b, void *context, _uint16_mat *dst) {
    check_src_f(a);
    check_dst_f(b);
    check_dst_f(dst);
    unsigned short *in_a = a->data;
    unsigned short *in_b = b->data;
    unsigned short *out = dst->data;
    int size = dst->size;
    if (dst->rows != a->rows || dst->cols != a->cols || a->rows != b->rows || a->cols != b->cols) return "array size mismatch";
    for (int i = 0; i < size; ++i) out[i] = in_a[i] | in_b[i];
    double work = (double)size * 3.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *matu32_bit_or(const _uint32_mat *a, const _uint32_mat *b, void *context, _uint32_mat *dst) {
    check_src_f(a);
    check_dst_f(b);
    check_dst_f(dst);
    unsigned int *in_a = a->data;
    unsigned int *in_b = b->data;
    unsigned int *out = dst->data;
    int size = dst->size;
    if (dst->rows != a->rows || dst->cols != a->cols || a->rows != b->rows || a->cols != b->cols) return "array size mismatch";
    for (int i = 0; i < size; ++i) out[i] = in_a[i] | in_b[i];
    double work = (double)size * 3.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *matu8_bit_xor(const _uint8_mat *a, const _uint8_mat *b, void *context, _uint8_mat *dst) {
    check_src_f(a);
    check_dst_f(b);
    check_dst_f(dst);
    unsigned char *in_a = a->data;
    unsigned char *in_b = b->data;
    unsigned char *out = dst->data;
    int size = dst->size;
    if (dst->rows != a->rows || dst->cols != a->cols || a->rows != b->rows || a->cols != b->cols) return "array size mismatch";
    for (int i = 0; i < size; ++i) out[i] = in_a[i] ^ in_b[i];
    double work = (double)size * 3.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *matu16_bit_xor(const _uint16_mat *a, const _uint16_mat *b, void *context, _uint16_mat *dst) {
    check_src_f(a);
    check_dst_f(b);
    check_dst_f(dst);
    unsigned short *in_a = a->data;
    unsigned short *in_b = b->data;
    unsigned short *out = dst->data;
    int size = dst->size;
    if (dst->rows != a->rows || dst->cols != a->cols || a->rows != b->rows || a->cols != b->cols) return "array size mismatch";
    for (int i = 0; i < size; ++i) out[i] = in_a[i] ^ in_b[i];
    double work = (double)size * 3.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *matu32_bit_xor(const _uint32_mat *a, const _uint32_mat *b, void *context, _uint32_mat *dst) {
    check_src_f(a);
    check_dst_f(b);
    check_dst_f(dst);
    unsigned int *in_a = a->data;
    unsigned int *in_b = b->data;
    unsigned int *out = dst->data;
    int size = dst->size;
    if (dst->rows != a->rows || dst->cols != a->cols || a->rows != b->rows || a->cols != b->cols) return "array size mismatch";
    for (int i = 0; i < size; ++i) out[i] = in_a[i] ^ in_b[i];
    double work = (double)size * 3.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

}  // namespace mutils
