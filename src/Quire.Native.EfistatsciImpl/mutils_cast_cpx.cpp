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

EFI_EXPORT void matcpx_to_double(const _dcomplex_mat *src, void *context, _double_mat *re, _double_mat *im) {
    validate(src);
    if (re == nullptr) {
        if (im == nullptr) Efi::Throw("null pointer", ~0ull);
    } else {
        if (src->rows != re->rows || src->cols != re->cols) Efi::Throw("array size mismatch", ~0ull);
        for (int i = 0; i < src->size; ++i) re->data[i] = src->data[i].re;
    }
    double progress = 0.0;
    double work = (double)src->size + (double)src->size;
    if (progress < work) {
        if (mutil_interrupt_step == 0.0) mutil_interrupt_step = 10000000.0;
        progress += mutil_interrupt_step;
    }
    if (im != nullptr) {
        if (src->rows != im->rows || src->cols != im->cols) Efi::Throw("array size mismatch", ~0ull);
        for (int i = 0; i < src->size; ++i) im->data[i] = src->data[i].im;
    }
    work = (double)src->size + (double)src->size;
    if (progress < work) {
        if (mutil_interrupt_step == 0.0) mutil_interrupt_step = 10000000.0;
        progress += mutil_interrupt_step;
    }
    (void)context;
}

}  // namespace mutils
