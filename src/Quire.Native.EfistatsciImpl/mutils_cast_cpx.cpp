#include "mutils_mat.h"

namespace mutils {

EFI_EXPORT void matcpx_cast_to_dbl(const _dcomplex_mat *src, unsigned char clamp, void *context, _double_mat *dst) {
    check_src_f(src);
    check_dst_f(dst);
    if (src->rows != dst->rows || src->cols != dst->cols) Efi::Throw("array size mismatch", ~0ull);
    if (clamp) {
        for (int i = 0; i < src->size; ++i) dst->data[i] = src->data[i].re;
    } else {
        for (int i = 0; i < src->size; ++i) {
            if (src->data[i].im != 0.0) Efi::Throw("overflow", ~0ull);
            dst->data[i] = src->data[i].re;
        }
    }
    double work = (double)src->size * 4.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
}

EFI_EXPORT void matdbl_cast_to_cpx(const _double_mat *src, void *context, _dcomplex_mat *dst) {
    check_src_f(src);
    check_dst_f(dst);
    if (src->rows != dst->rows || src->cols != dst->cols) Efi::Throw("array size mismatch", ~0ull);
    for (int i = 0; i < src->size; ++i) {
        dst->data[i].re = src->data[i];
        dst->data[i].im = 0.0;
    }
    double work = (double)src->size * 4.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
}

EFI_EXPORT void matflt_cast_to_cpx(const _float_mat *src, void *context, _dcomplex_mat *dst) {
    check_src_f(src);
    check_dst_f(dst);
    if (src->rows != dst->rows || src->cols != dst->cols) Efi::Throw("array size mismatch", ~0ull);
    for (int i = 0; i < src->size; ++i) {
        dst->data[i].re = src->data[i];
        dst->data[i].im = 0.0;
    }
    double work = (double)src->size * 4.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
}

}  // namespace mutils
