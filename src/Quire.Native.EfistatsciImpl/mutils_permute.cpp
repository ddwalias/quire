#include "mutils_mat.h"

namespace mutils {

EFI_EXPORT const char *matdbl_permute(Efi::EfiCallContext *, const _double_mat *src, const _sint32_mat *index, void *context, _double_mat *dst) {
    check_src_f(src);
    check_dst_f(dst);
    check_dst_f(index);
    if (index->rows != dst->rows || index->cols != dst->cols) return "array size mismatch";
    for (int i = 0; i < dst->size; ++i) {
        int k = index->data[i];
        if (k < 0 || k >= src->size) return "Illegal data value";
        dst->data[i] = src->data[k];
    }
    double work = (double)dst->size + (double)dst->size;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *matflt_permute(Efi::EfiCallContext *, const _float_mat *src, const _sint32_mat *index, void *context, _float_mat *dst) {
    check_src_f(src);
    check_dst_f(dst);
    check_dst_f(index);
    if (index->rows != dst->rows || index->cols != dst->cols) return "array size mismatch";
    for (int i = 0; i < dst->size; ++i) {
        int k = index->data[i];
        if (k < 0 || k >= src->size) return "Illegal data value";
        dst->data[i] = src->data[k];
    }
    double work = (double)dst->size + (double)dst->size;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *mats32_permute(Efi::EfiCallContext *, const _sint32_mat *src, const _sint32_mat *index, void *context, _sint32_mat *dst) {
    check_src_f(src);
    check_dst_f(dst);
    check_dst_f(index);
    if (index->rows != dst->rows || index->cols != dst->cols) return "array size mismatch";
    for (int i = 0; i < dst->size; ++i) {
        int k = index->data[i];
        if (k < 0 || k >= src->size) return "Illegal data value";
        dst->data[i] = src->data[k];
    }
    double work = (double)dst->size + (double)dst->size;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *mats16_permute(Efi::EfiCallContext *, const _sint16_mat *src, const _sint32_mat *index, void *context, _sint16_mat *dst) {
    check_src_f(src);
    check_dst_f(dst);
    check_dst_f(index);
    if (index->rows != dst->rows || index->cols != dst->cols) return "array size mismatch";
    for (int i = 0; i < dst->size; ++i) {
        int k = index->data[i];
        if (k < 0 || k >= src->size) return "Illegal data value";
        dst->data[i] = src->data[k];
    }
    double work = (double)dst->size + (double)dst->size;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *matu32_permute(Efi::EfiCallContext *, const _uint32_mat *src, const _sint32_mat *index, void *context, _uint32_mat *dst) {
    check_src_f(src);
    check_dst_f(dst);
    check_dst_f(index);
    if (index->rows != dst->rows || index->cols != dst->cols) return "array size mismatch";
    for (int i = 0; i < dst->size; ++i) {
        int k = index->data[i];
        if (k < 0 || k >= src->size) return "Illegal data value";
        dst->data[i] = src->data[k];
    }
    double work = (double)dst->size + (double)dst->size;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *matu16_permute(Efi::EfiCallContext *, const _uint16_mat *src, const _sint32_mat *index, void *context, _uint16_mat *dst) {
    check_src_f(src);
    check_dst_f(dst);
    check_dst_f(index);
    if (index->rows != dst->rows || index->cols != dst->cols) return "array size mismatch";
    for (int i = 0; i < dst->size; ++i) {
        int k = index->data[i];
        if (k < 0 || k >= src->size) return "Illegal data value";
        dst->data[i] = src->data[k];
    }
    double work = (double)dst->size + (double)dst->size;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *matu8_permute(Efi::EfiCallContext *, const _uint8_mat *src, const _sint32_mat *index, void *context, _uint8_mat *dst) {
    check_src_f(src);
    check_dst_f(dst);
    check_dst_f(index);
    if (index->rows != dst->rows || index->cols != dst->cols) return "array size mismatch";
    for (int i = 0; i < dst->size; ++i) {
        int k = index->data[i];
        if (k < 0 || k >= src->size) return "Illegal data value";
        dst->data[i] = src->data[k];
    }
    double work = (double)dst->size + (double)dst->size;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

}  // namespace mutils
