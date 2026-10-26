#include "mutils_mat.h"

namespace mutils {

EFI_EXPORT const char *matdbl_sum_cols(Efi::EfiCallContext *, const _double_mat *src, void *context, _double_mat *dst) {
    check_src_f(src);
    check_dst_f(dst);
    int rows = src->rows;
    int cols = src->cols;
    if (dst->cols != cols || dst->rows != 1) return "array size mismatch";
    double total = 0.0;
    double progress = 0.0;
    for (int j = 0; j < cols; ++j) {
        double sum = 0;
        for (int i = 0; i < rows; ++i) sum += src->data[i * src->cols + j];
        dst->data[j] = sum;
        total += (double)cols * 4.0;
        if (progress < total) {
            if (mutil_interrupt_step == 0.0) mutil_interrupt_step = 10000000.0;
            progress += mutil_interrupt_step;
        }
    }
    (void)context;
    return nullptr;
}

EFI_EXPORT const char *matdbl_sum_rows(Efi::EfiCallContext *, const _double_mat *src, void *context, _double_mat *dst) {
    check_src_f(src);
    check_dst_f(dst);
    int rows = src->rows;
    int cols = src->cols;
    if (dst->rows != rows || dst->cols != 1) return "array size mismatch";
    double total = 0.0;
    double progress = 0.0;
    for (int i = 0; i < rows; ++i) {
        double sum = 0;
        for (int j = 0; j < cols; ++j) sum += src->data[i * src->cols + j];
        dst->data[i] = sum;
        total += (double)cols * 4.0;
        if (progress < total) {
            if (mutil_interrupt_step == 0.0) mutil_interrupt_step = 10000000.0;
            progress += mutil_interrupt_step;
        }
    }
    (void)context;
    return nullptr;
}

EFI_EXPORT const char *matflt_sum_cols(Efi::EfiCallContext *, const _float_mat *src, void *context, _float_mat *dst) {
    check_src_f(src);
    check_dst_f(dst);
    int rows = src->rows;
    int cols = src->cols;
    if (dst->cols != cols || dst->rows != 1) return "array size mismatch";
    double total = 0.0;
    double progress = 0.0;
    for (int j = 0; j < cols; ++j) {
        float sum = 0;
        for (int i = 0; i < rows; ++i) sum += src->data[i * src->cols + j];
        dst->data[j] = sum;
        total += (double)cols * 4.0;
        if (progress < total) {
            if (mutil_interrupt_step == 0.0) mutil_interrupt_step = 10000000.0;
            progress += mutil_interrupt_step;
        }
    }
    (void)context;
    return nullptr;
}

EFI_EXPORT const char *matflt_sum_rows(Efi::EfiCallContext *, const _float_mat *src, void *context, _float_mat *dst) {
    check_src_f(src);
    check_dst_f(dst);
    int rows = src->rows;
    int cols = src->cols;
    if (dst->rows != rows || dst->cols != 1) return "array size mismatch";
    double total = 0.0;
    double progress = 0.0;
    for (int i = 0; i < rows; ++i) {
        float sum = 0;
        for (int j = 0; j < cols; ++j) sum += src->data[i * src->cols + j];
        dst->data[i] = sum;
        total += (double)cols * 4.0;
        if (progress < total) {
            if (mutil_interrupt_step == 0.0) mutil_interrupt_step = 10000000.0;
            progress += mutil_interrupt_step;
        }
    }
    (void)context;
    return nullptr;
}

EFI_EXPORT const char *mats32_sum_cols(Efi::EfiCallContext *, const _sint32_mat *src, void *context, _sint32_mat *dst) {
    check_src_f(src);
    check_dst_f(dst);
    int rows = src->rows;
    int cols = src->cols;
    if (dst->cols != cols || dst->rows != 1) return "array size mismatch";
    double total = 0.0;
    double progress = 0.0;
    for (int j = 0; j < cols; ++j) {
        long sum = 0;
        for (int i = 0; i < rows; ++i) sum += src->data[i * src->cols + j];
        dst->data[j] = sum;
        total += (double)cols * 4.0;
        if (progress < total) {
            if (mutil_interrupt_step == 0.0) mutil_interrupt_step = 10000000.0;
            progress += mutil_interrupt_step;
        }
    }
    (void)context;
    return nullptr;
}

EFI_EXPORT const char *mats32_sum_rows(Efi::EfiCallContext *, const _sint32_mat *src, void *context, _sint32_mat *dst) {
    check_src_f(src);
    check_dst_f(dst);
    int rows = src->rows;
    int cols = src->cols;
    if (dst->rows != rows || dst->cols != 1) return "array size mismatch";
    double total = 0.0;
    double progress = 0.0;
    for (int i = 0; i < rows; ++i) {
        long sum = 0;
        for (int j = 0; j < cols; ++j) sum += src->data[i * src->cols + j];
        dst->data[i] = sum;
        total += (double)cols * 4.0;
        if (progress < total) {
            if (mutil_interrupt_step == 0.0) mutil_interrupt_step = 10000000.0;
            progress += mutil_interrupt_step;
        }
    }
    (void)context;
    return nullptr;
}

EFI_EXPORT const char *mats16_sum_cols(Efi::EfiCallContext *, const _sint16_mat *src, void *context, _sint16_mat *dst) {
    check_src_f(src);
    check_src_f(dst);
    int rows = src->rows;
    int cols = src->cols;
    if (dst->cols != cols || dst->rows != 1) return "array size mismatch";
    double total = 0.0;
    double progress = 0.0;
    for (int j = 0; j < cols; ++j) {
        short sum = 0;
        for (int i = 0; i < rows; ++i) sum += src->data[i * src->cols + j];
        dst->data[j] = sum;
        total += (double)cols * 4.0;
        if (progress < total) {
            if (mutil_interrupt_step == 0.0) mutil_interrupt_step = 10000000.0;
            progress += mutil_interrupt_step;
        }
    }
    (void)context;
    return nullptr;
}

EFI_EXPORT const char *mats16_sum_rows(Efi::EfiCallContext *, const _sint16_mat *src, void *context, _sint16_mat *dst) {
    check_src_f(src);
    check_src_f(dst);
    int rows = src->rows;
    int cols = src->cols;
    if (dst->rows != rows || dst->cols != 1) return "array size mismatch";
    double total = 0.0;
    double progress = 0.0;
    for (int i = 0; i < rows; ++i) {
        short sum = 0;
        for (int j = 0; j < cols; ++j) sum += src->data[i * src->cols + j];
        dst->data[i] = sum;
        total += (double)cols * 4.0;
        if (progress < total) {
            if (mutil_interrupt_step == 0.0) mutil_interrupt_step = 10000000.0;
            progress += mutil_interrupt_step;
        }
    }
    (void)context;
    return nullptr;
}

EFI_EXPORT const char *matu32_sum_cols(Efi::EfiCallContext *, const _uint32_mat *src, void *context, _uint32_mat *dst) {
    check_src_f(src);
    check_dst_f(dst);
    int rows = src->rows;
    int cols = src->cols;
    if (dst->cols != cols || dst->rows != 1) return "array size mismatch";
    double total = 0.0;
    double progress = 0.0;
    for (int j = 0; j < cols; ++j) {
        unsigned int sum = 0;
        for (int i = 0; i < rows; ++i) sum += src->data[i * src->cols + j];
        dst->data[j] = sum;
        total += (double)cols * 4.0;
        if (progress < total) {
            if (mutil_interrupt_step == 0.0) mutil_interrupt_step = 10000000.0;
            progress += mutil_interrupt_step;
        }
    }
    (void)context;
    return nullptr;
}

EFI_EXPORT const char *matu32_sum_rows(Efi::EfiCallContext *, const _uint32_mat *src, void *context, _uint32_mat *dst) {
    check_src_f(src);
    check_dst_f(dst);
    int rows = src->rows;
    int cols = src->cols;
    if (dst->rows != rows || dst->cols != 1) return "array size mismatch";
    double total = 0.0;
    double progress = 0.0;
    for (int i = 0; i < rows; ++i) {
        unsigned int sum = 0;
        for (int j = 0; j < cols; ++j) sum += src->data[i * src->cols + j];
        dst->data[i] = sum;
        total += (double)cols * 4.0;
        if (progress < total) {
            if (mutil_interrupt_step == 0.0) mutil_interrupt_step = 10000000.0;
            progress += mutil_interrupt_step;
        }
    }
    (void)context;
    return nullptr;
}

EFI_EXPORT const char *matu16_sum_cols(Efi::EfiCallContext *, const _uint16_mat *src, void *context, _uint16_mat *dst) {
    check_src_f(src);
    check_src_f(dst);
    int rows = src->rows;
    int cols = src->cols;
    if (dst->cols != cols || dst->rows != 1) return "array size mismatch";
    double total = 0.0;
    double progress = 0.0;
    for (int j = 0; j < cols; ++j) {
        unsigned short sum = 0;
        for (int i = 0; i < rows; ++i) sum += src->data[i * src->cols + j];
        dst->data[j] = sum;
        total += (double)cols * 4.0;
        if (progress < total) {
            if (mutil_interrupt_step == 0.0) mutil_interrupt_step = 10000000.0;
            progress += mutil_interrupt_step;
        }
    }
    (void)context;
    return nullptr;
}

EFI_EXPORT const char *matu16_sum_rows(Efi::EfiCallContext *, const _uint16_mat *src, void *context, _uint16_mat *dst) {
    check_src_f(src);
    check_src_f(dst);
    int rows = src->rows;
    int cols = src->cols;
    if (dst->rows != rows || dst->cols != 1) return "array size mismatch";
    double total = 0.0;
    double progress = 0.0;
    for (int i = 0; i < rows; ++i) {
        unsigned short sum = 0;
        for (int j = 0; j < cols; ++j) sum += src->data[i * src->cols + j];
        dst->data[i] = sum;
        total += (double)cols * 4.0;
        if (progress < total) {
            if (mutil_interrupt_step == 0.0) mutil_interrupt_step = 10000000.0;
            progress += mutil_interrupt_step;
        }
    }
    (void)context;
    return nullptr;
}

EFI_EXPORT const char *matu8_sum_cols(Efi::EfiCallContext *, const _uint8_mat *src, void *context, _uint8_mat *dst) {
    check_src_f(src);
    check_src_f(dst);
    int rows = src->rows;
    int cols = src->cols;
    if (dst->cols != cols || dst->rows != 1) return "array size mismatch";
    double total = 0.0;
    double progress = 0.0;
    for (int j = 0; j < cols; ++j) {
        unsigned char sum = 0;
        for (int i = 0; i < rows; ++i) sum += src->data[i * src->cols + j];
        dst->data[j] = sum;
        total += (double)cols * 4.0;
        if (progress < total) {
            if (mutil_interrupt_step == 0.0) mutil_interrupt_step = 10000000.0;
            progress += mutil_interrupt_step;
        }
    }
    (void)context;
    return nullptr;
}

EFI_EXPORT const char *matu8_sum_rows(Efi::EfiCallContext *, const _uint8_mat *src, void *context, _uint8_mat *dst) {
    check_src_f(src);
    check_src_f(dst);
    int rows = src->rows;
    int cols = src->cols;
    if (dst->rows != rows || dst->cols != 1) return "array size mismatch";
    double total = 0.0;
    double progress = 0.0;
    for (int i = 0; i < rows; ++i) {
        unsigned char sum = 0;
        for (int j = 0; j < cols; ++j) sum += src->data[i * src->cols + j];
        dst->data[i] = sum;
        total += (double)cols * 4.0;
        if (progress < total) {
            if (mutil_interrupt_step == 0.0) mutil_interrupt_step = 10000000.0;
            progress += mutil_interrupt_step;
        }
    }
    (void)context;
    return nullptr;
}

}  // namespace mutils
