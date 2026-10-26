#include "mutils_mat.h"

namespace mutils {

EFI_EXPORT const char *matdbl_threshold(const _double_mat *src, double lo, double hi, double off, double on, void *context, _double_mat *dst) {
    if (lo > hi) return "Illegal data value";
    check_src_f(src);
    check_dst_f(dst);
    int size = src->size;
    if (src->rows != dst->rows || src->cols != dst->cols) return "array size mismatch";
    for (int i = 0; i < size; ++i) {
        double v = src->data[i];
        if (lo > v || v > hi) dst->data[i] = off;
        else dst->data[i] = on;
    }
    double work = (double)size * 3.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *mats32_threshold(const _sint32_mat *src, double lo, double hi, long off, long on, void *context, _sint32_mat *dst) {
    if (lo > hi) return "Illegal data value";
    check_src_f(src);
    check_dst_f(dst);
    int size = src->size;
    if (src->rows != dst->rows || src->cols != dst->cols) return "array size mismatch";
    for (int i = 0; i < size; ++i) {
        double v = src->data[i];
        if (lo > v || v > hi) dst->data[i] = off;
        else dst->data[i] = on;
    }
    double work = (double)size * 3.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *matu16_threshold(const _uint16_mat *src, double lo, double hi, unsigned short off, unsigned short on, void *context, _uint16_mat *dst) {
    if (lo > hi) return "Illegal data value";
    check_src_f(src);
    check_dst_f(dst);
    int size = src->size;
    if (src->rows != dst->rows || src->cols != dst->cols) return "array size mismatch";
    for (int i = 0; i < size; ++i) {
        double v = src->data[i];
        if (lo > v || v > hi) dst->data[i] = off;
        else dst->data[i] = on;
    }
    double work = (double)size * 3.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *matu8_threshold(const _uint8_mat *src, double lo, double hi, unsigned char off, unsigned char on, void *context, _uint8_mat *dst) {
    if (lo > hi) return "Illegal data value";
    check_src_f(src);
    check_dst_f(dst);
    int size = src->size;
    if (src->rows != dst->rows || src->cols != dst->cols) return "array size mismatch";
    for (int i = 0; i < size; ++i) {
        double v = src->data[i];
        if (lo > v || v > hi) dst->data[i] = off;
        else dst->data[i] = on;
    }
    double work = (double)size * 3.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

}  // namespace mutils
