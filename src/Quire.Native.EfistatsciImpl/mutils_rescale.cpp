#include "mutils_mat.h"

namespace mutils {

EFI_EXPORT const char *matdbl_rescale(Efi::EfiCallContext *ctx, const _double_mat *src, double lo, double hi, void *context, _double_mat *dst) {
    validate(dst);
    double min;
    double max;
    matdbl_range(ctx, src, context, &min, &max);
    if (src->rows != dst->rows || src->cols != dst->cols) return "array size mismatch";
    if (lo > hi) return "Illegal data value";
    double scale = min == max ? 0.0 : ((double)hi - (double)lo) / ((double)max - (double)min);
    for (int i = 0; i < src->size; ++i) dst->data[i] = (double)(((double)src->data[i] - (double)min) * scale + (double)lo);
    return nullptr;
}

EFI_EXPORT const char *matflt_rescale(Efi::EfiCallContext *ctx, const _float_mat *src, float lo, float hi, void *context, _float_mat *dst) {
    check_src_f(dst);
    float min;
    float max;
    matflt_range(ctx, src, context, &min, &max);
    if (src->rows != dst->rows || src->cols != dst->cols) return "array size mismatch";
    if (lo > hi) return "Illegal data value";
    double scale = min == max ? 0.0 : ((double)hi - (double)lo) / ((double)max - (double)min);
    for (int i = 0; i < src->size; ++i) dst->data[i] = (float)(((double)src->data[i] - (double)min) * scale + (double)lo);
    return nullptr;
}

EFI_EXPORT const char *mats32_rescale(Efi::EfiCallContext *ctx, const _sint32_mat *src, long lo, long hi, void *context, _sint32_mat *dst) {
    check_src_f(dst);
    long min;
    long max;
    mats32_range(ctx, src, context, &min, &max);
    if (src->rows != dst->rows || src->cols != dst->cols) return "array size mismatch";
    if (lo > hi) return "Illegal data value";
    double scale = min == max ? 0.0 : ((double)hi - (double)lo) / ((double)max - (double)min);
    for (int i = 0; i < src->size; ++i) dst->data[i] = (long)(((double)src->data[i] - (double)min) * scale + (double)lo);
    return nullptr;
}

EFI_EXPORT const char *mats16_rescale(Efi::EfiCallContext *ctx, const _sint16_mat *src, short lo, short hi, void *context, _sint16_mat *dst) {
    check_src_f(dst);
    short min;
    short max;
    mats16_range(ctx, src, context, &min, &max);
    if (src->rows != dst->rows || src->cols != dst->cols) return "array size mismatch";
    if (lo > hi) return "Illegal data value";
    double scale = min == max ? 0.0 : ((double)hi - (double)lo) / ((double)max - (double)min);
    for (int i = 0; i < src->size; ++i) dst->data[i] = (short)(((double)src->data[i] - (double)min) * scale + (double)lo);
    return nullptr;
}

EFI_EXPORT const char *matu32_rescale(Efi::EfiCallContext *ctx, const _uint32_mat *src, unsigned int lo, unsigned int hi, void *context, _uint32_mat *dst) {
    check_src_f(dst);
    unsigned int min;
    unsigned int max;
    matu32_range(ctx, src, context, &min, &max);
    if (src->rows != dst->rows || src->cols != dst->cols) return "array size mismatch";
    if (lo > hi) return "Illegal data value";
    double scale = min == max ? 0.0 : ((double)hi - (double)lo) / ((double)max - (double)min);
    for (int i = 0; i < src->size; ++i) dst->data[i] = (unsigned int)(((double)src->data[i] - (double)min) * scale + (double)lo);
    return nullptr;
}

EFI_EXPORT const char *matu16_rescale(Efi::EfiCallContext *ctx, const _uint16_mat *src, unsigned short lo, unsigned short hi, void *context, _uint16_mat *dst) {
    check_src_f(dst);
    unsigned short min;
    unsigned short max;
    matu16_range(ctx, src, context, &min, &max);
    if (src->rows != dst->rows || src->cols != dst->cols) return "array size mismatch";
    if (lo > hi) return "Illegal data value";
    double scale = min == max ? 0.0 : ((double)hi - (double)lo) / ((double)max - (double)min);
    for (int i = 0; i < src->size; ++i) dst->data[i] = (unsigned short)(((double)src->data[i] - (double)min) * scale + (double)lo);
    return nullptr;
}

EFI_EXPORT const char *matu8_rescale(Efi::EfiCallContext *ctx, const _uint8_mat *src, unsigned char lo, unsigned char hi, void *context, _uint8_mat *dst) {
    check_src_f(dst);
    unsigned char min;
    unsigned char max;
    matu8_range(ctx, src, context, &min, &max);
    if (src->rows != dst->rows || src->cols != dst->cols) return "array size mismatch";
    if (lo > hi) return "Illegal data value";
    double scale = min == max ? 0.0 : ((double)hi - (double)lo) / ((double)max - (double)min);
    for (int i = 0; i < src->size; ++i) dst->data[i] = (unsigned char)(((double)src->data[i] - (double)min) * scale + (double)lo);
    return nullptr;
}

}  // namespace mutils
