#include "mutils_mat.h"

namespace mutils {

EFI_EXPORT const char *matdbl_abs(Efi::EfiCallContext *, const _double_mat *src, void *context, _double_mat *dst) {
    check_src_f(src);
    check_dst_f(dst);
    double *in = src->data;
    double *out = dst->data;
    int size = dst->size;
    if (dst->rows != src->rows || dst->cols != src->cols) return "array size mismatch";
    for (int i = 0; i < size; ++i) out[i] = in[i] >= 0 ? in[i] : -in[i];
    double work = (double)size * 3.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *matflt_abs(Efi::EfiCallContext *, const _float_mat *src, void *context, _float_mat *dst) {
    check_src_f(src);
    check_dst_f(dst);
    float *in = src->data;
    float *out = dst->data;
    int size = dst->size;
    if (dst->rows != src->rows || dst->cols != src->cols) return "array size mismatch";
    for (int i = 0; i < size; ++i) out[i] = in[i] >= 0 ? in[i] : -in[i];
    double work = (double)size * 3.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *mats32_abs(Efi::EfiCallContext *, const _sint32_mat *src, void *context, _sint32_mat *dst) {
    check_src_f(src);
    check_dst_f(dst);
    long *in = src->data;
    long *out = dst->data;
    int size = dst->size;
    if (dst->rows != src->rows || dst->cols != src->cols) return "array size mismatch";
    for (int i = 0; i < size; ++i) out[i] = in[i] >= 0 ? in[i] : -in[i];
    double work = (double)size * 3.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *mats16_abs(Efi::EfiCallContext *, const _sint16_mat *src, void *context, _sint16_mat *dst) {
    check_src_f(src);
    check_dst_f(dst);
    short *in = src->data;
    short *out = dst->data;
    int size = dst->size;
    if (dst->rows != src->rows || dst->cols != src->cols) return "array size mismatch";
    for (int i = 0; i < size; ++i) out[i] = in[i] >= 0 ? in[i] : -in[i];
    double work = (double)size * 3.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

}  // namespace mutils
