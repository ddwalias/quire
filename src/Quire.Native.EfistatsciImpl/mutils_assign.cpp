#include "mutils_mat.h"

#include <string.h>

namespace mutils {

EFI_EXPORT const char *matdbl_assign(const _double_mat *src, void *context, _double_mat *dst) {
    check_src_f(src);
    check_src_f(dst);
    if (src->rows != dst->rows || src->cols != dst->cols) return "array size mismatch";
    if (dst->data == src->data) return nullptr;
    int bytes = src->size * sizeof(double);
    // Without the casts the compiler turns this memmove of doubles into memcpy.
    if (memmove((void *)dst->data, (const void *)src->data, bytes) == nullptr || dst->data == nullptr) return "out of memory";
    double work = (double)src->size;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *matflt_assign(const _float_mat *src, void *context, _float_mat *dst) {
    check_src_f(src);
    check_src_f(dst);
    if (src->rows != dst->rows || src->cols != dst->cols) return "array size mismatch";
    if (dst->data == src->data) return nullptr;
    int bytes = src->size * sizeof(float);
    if (memmove(dst->data, src->data, bytes) == nullptr || dst->data == nullptr) return "out of memory";
    double work = (double)src->size;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *mats32_assign(const _sint32_mat *src, void *context, _sint32_mat *dst) {
    check_src_f(src);
    check_src_f(dst);
    if (src->rows != dst->rows || src->cols != dst->cols) return "array size mismatch";
    if (dst->data == src->data) return nullptr;
    int bytes = src->size * sizeof(long);
    if (memmove(dst->data, src->data, bytes) == nullptr || dst->data == nullptr) return "out of memory";
    double work = (double)src->size;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *mats16_assign(const _sint16_mat *src, void *context, _sint16_mat *dst) {
    check_src_f(src);
    check_src_f(dst);
    if (src->rows != dst->rows || src->cols != dst->cols) return "array size mismatch";
    if (dst->data == src->data) return nullptr;
    int bytes = src->size * sizeof(short);
    if (memmove(dst->data, src->data, bytes) == nullptr || dst->data == nullptr) return "out of memory";
    double work = (double)src->size;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *matu32_assign(const _uint32_mat *src, void *context, _uint32_mat *dst) {
    check_src_f(src);
    check_src_f(dst);
    if (src->rows != dst->rows || src->cols != dst->cols) return "array size mismatch";
    if (dst->data == src->data) return nullptr;
    int bytes = src->size * sizeof(unsigned int);
    if (memmove(dst->data, src->data, bytes) == nullptr || dst->data == nullptr) return "out of memory";
    double work = (double)src->size;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *matu16_assign(const _uint16_mat *src, void *context, _uint16_mat *dst) {
    check_src_f(src);
    check_src_f(dst);
    if (src->rows != dst->rows || src->cols != dst->cols) return "array size mismatch";
    if (dst->data == src->data) return nullptr;
    int bytes = src->size * sizeof(unsigned short);
    if (memmove(dst->data, src->data, bytes) == nullptr || dst->data == nullptr) return "out of memory";
    double work = (double)src->size;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *matu8_assign(const _uint8_mat *src, void *context, _uint8_mat *dst) {
    check_src_f(src);
    check_dst_f(dst);  // the original reads cols before rows here only
    if (src->rows != dst->rows || src->cols != dst->cols) return "array size mismatch";
    if (dst->data == src->data) return nullptr;
    int bytes = src->size * sizeof(unsigned char);
    if (memmove(dst->data, src->data, bytes) == nullptr || dst->data == nullptr) return "out of memory";
    double work = (double)src->size;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

}  // namespace mutils
