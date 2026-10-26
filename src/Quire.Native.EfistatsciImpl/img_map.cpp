#include "mutils_mat.h"

namespace mutils {

EFI_EXPORT const char *imgdbl_map_invert(Efi::EfiCallContext *ctx, const _double_mat *src, void *context, _double_mat *dst) {
    check_src_f(src);
    check_dst_f(dst);
    if (src->rows != dst->rows || src->cols != dst->cols) return "array size mismatch";
    double min;
    double max;
    matdbl_range(ctx, src, context, &min, &max);
    int size = src->size;
    for (int i = 0; i < size; ++i) dst->data[i] = max - src->data[i] + min;
    double work = (double)size * 3.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *imgu16_map_invert(Efi::EfiCallContext *ctx, const _uint16_mat *src, void *context, _uint16_mat *dst) {
    check_src_f(src);
    check_src_f(dst);
    if (src->rows != dst->rows || src->cols != dst->cols) return "array size mismatch";
    unsigned short min;
    unsigned short max;
    matu16_range(ctx, src, context, &min, &max);
    int size = src->size;
    for (int i = 0; i < size; ++i) dst->data[i] = (unsigned short)(min + max - src->data[i]);
    double work = (double)size * 3.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *imgu8_map_invert(Efi::EfiCallContext *ctx, const _uint8_mat *src, void *context, _uint8_mat *dst) {
    check_src_f(src);
    check_src_f(dst);
    if (src->rows != dst->rows || src->cols != dst->cols) return "array size mismatch";
    unsigned char min;
    unsigned char max;
    matu8_range(ctx, src, context, &min, &max);
    int size = src->size;
    for (int i = 0; i < size; ++i) dst->data[i] = (unsigned char)(min + max - src->data[i]);
    double work = (double)size * 3.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *imguniv_map_invert(Efi::EfiCallContext *ctx, const _univ_mat *src, void *context, _univ_mat *dst) {
    if (src == nullptr || dst == nullptr) return "null pointer";
    if (src->type != dst->type) return "Illegal data type";
    switch (src->type) {
    case EFI_DOUBLE: {
        if (const char *error = imgdbl_map_invert(ctx, (const _double_mat *)src, context, (_double_mat *)dst)) return error;
    } break;
    case EFI_UINT16: {
        if (const char *error = imgu16_map_invert(ctx, (const _uint16_mat *)src, context, (_uint16_mat *)dst)) return error;
    } break;
    case EFI_UINT8: {
        if (const char *error = imgu8_map_invert(ctx, (const _uint8_mat *)src, context, (_uint8_mat *)dst)) return error;
    } break;
    default: return "Illegal data type";
    }
    return nullptr;
}

}  // namespace mutils
