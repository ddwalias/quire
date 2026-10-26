#include "mutils_mat.h"

namespace mutils {

EFI_EXPORT const char *matdbl_cumulative_sum(Efi::EfiCallContext *, const _double_mat *src, void *context, _double_mat *dst) {
    check_src_f(src);
    validate(dst);
    if (src->size != dst->size) return "array size mismatch";
    matdbl_assign(src, context, dst);
    for (int i = 1; i < dst->size; ++i) dst->data[i] = dst->data[i - 1] + dst->data[i];
    double work = 0.0;
    work += (double)dst->size;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *matflt_cumulative_sum(Efi::EfiCallContext *, const _float_mat *src, void *context, _float_mat *dst) {
    check_src_f(src);
    validate(dst);
    if (src->size != dst->size) return "array size mismatch";
    matflt_assign(src, context, dst);
    for (int i = 1; i < dst->size; ++i) dst->data[i] = dst->data[i - 1] + dst->data[i];
    double work = 0.0;
    work += (double)dst->size;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *mats32_cumulative_sum(Efi::EfiCallContext *, const _sint32_mat *src, void *context, _sint32_mat *dst) {
    check_src_f(src);
    validate(dst);
    if (src->size != dst->size) return "array size mismatch";
    mats32_assign(src, context, dst);
    for (int i = 1; i < dst->size; ++i) dst->data[i] = dst->data[i - 1] + dst->data[i];
    double work = 0.0;
    work += (double)dst->size;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *mats16_cumulative_sum(Efi::EfiCallContext *, const _sint16_mat *src, void *context, _sint16_mat *dst) {
    check_src_f(src);
    check_src_f(dst);
    if (src->size != dst->size) return "array size mismatch";
    mats16_assign(src, context, dst);
    for (int i = 1; i < dst->size; ++i) dst->data[i] = dst->data[i - 1] + dst->data[i];
    double work = 0.0;
    work += (double)dst->size;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *matu32_cumulative_sum(Efi::EfiCallContext *, const _uint32_mat *src, void *context, _uint32_mat *dst) {
    check_src_f(src);
    validate(dst);
    if (src->size != dst->size) return "array size mismatch";
    matu32_assign(src, context, dst);
    for (int i = 1; i < dst->size; ++i) dst->data[i] = dst->data[i - 1] + dst->data[i];
    double work = 0.0;
    work += (double)dst->size;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *matu16_cumulative_sum(Efi::EfiCallContext *, const _uint16_mat *src, void *context, _uint16_mat *dst) {
    check_src_f(src);
    check_src_f(dst);
    if (src->size != dst->size) return "array size mismatch";
    matu16_assign(src, context, dst);
    for (int i = 1; i < dst->size; ++i) dst->data[i] = dst->data[i - 1] + dst->data[i];
    double work = 0.0;
    work += (double)dst->size;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *matu8_cumulative_sum(Efi::EfiCallContext *, const _uint8_mat *src, void *context, _uint8_mat *dst) {
    check_src_f(src);
    check_src_f(dst);
    if (src->size != dst->size) return "array size mismatch";
    matu8_assign(src, context, dst);
    for (int i = 1; i < dst->size; ++i) dst->data[i] = dst->data[i - 1] + dst->data[i];
    double work = 0.0;
    work += (double)dst->size;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

}  // namespace mutils
