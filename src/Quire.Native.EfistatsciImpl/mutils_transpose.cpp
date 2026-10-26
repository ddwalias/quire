#include "mutils_mat.h"

namespace mutils {

EFI_EXPORT const char *matdbl_transpose(const _double_mat *src, void *context, _double_mat *dst) {
    check_src_f(src);
    check_dst_f(dst);
    int rows = dst->rows;
    int cols = dst->cols;
    if (src->data == dst->data) {
        // In place only a vector transposes, by swapping its dimensions.
        if (src->rows != 1 && src->cols != 1) return "array size mismatch";
        dst->rows = cols;
        dst->cols = rows;
        return nullptr;
    }
    if (rows != src->cols || cols != src->rows) return "array size mismatch";
    double total = 0.0;
    double progress = 0.0;
    for (int i = 0; i < rows; ++i) {
        for (int j = 0; j < cols; ++j) {
            dst->data[i * dst->cols + j] = src->data[j * src->cols + i];
        }
        total += (double)(cols * 8);
        if (progress < total) {
            if (mutil_interrupt_step == 0.0) mutil_interrupt_step = 10000000.0;
            progress += mutil_interrupt_step;
        }
    }
    (void)context;
    return nullptr;
}

EFI_EXPORT const char *matflt_transpose(const _float_mat *src, void *context, _float_mat *dst) {
    check_src_f(src);
    check_dst_f(dst);
    int rows = dst->rows;
    int cols = dst->cols;
    if (src->data == dst->data) {
        if (src->rows != 1 && src->cols != 1) return "array size mismatch";
        dst->rows = cols;
        dst->cols = rows;
        return nullptr;
    }
    if (rows != src->cols || cols != src->rows) return "array size mismatch";
    double total = 0.0;
    double progress = 0.0;
    for (int i = 0; i < rows; ++i) {
        for (int j = 0; j < cols; ++j) {
            dst->data[i * dst->cols + j] = src->data[j * src->cols + i];
        }
        total += (double)(cols * 8);
        if (progress < total) {
            if (mutil_interrupt_step == 0.0) mutil_interrupt_step = 10000000.0;
            progress += mutil_interrupt_step;
        }
    }
    (void)context;
    return nullptr;
}

EFI_EXPORT const char *mats32_transpose(const _sint32_mat *src, void *context, _sint32_mat *dst) {
    check_src_f(src);
    check_dst_f(dst);
    int rows = dst->rows;
    int cols = dst->cols;
    if (src->data == dst->data) {
        if (src->rows != 1 && src->cols != 1) return "array size mismatch";
        dst->rows = cols;
        dst->cols = rows;
        return nullptr;
    }
    if (rows != src->cols || cols != src->rows) return "array size mismatch";
    double total = 0.0;
    double progress = 0.0;
    for (int i = 0; i < rows; ++i) {
        for (int j = 0; j < cols; ++j) {
            dst->data[i * dst->cols + j] = src->data[j * src->cols + i];
        }
        total += (double)(cols * 8);
        if (progress < total) {
            if (mutil_interrupt_step == 0.0) mutil_interrupt_step = 10000000.0;
            progress += mutil_interrupt_step;
        }
    }
    (void)context;
    return nullptr;
}

EFI_EXPORT const char *mats16_transpose(const _sint16_mat *src, void *context, _sint16_mat *dst) {
    check_src_f(src);
    check_dst_f(dst);
    int rows = dst->rows;
    int cols = dst->cols;
    if (src->data == dst->data) {
        if (src->rows != 1 && src->cols != 1) return "array size mismatch";
        dst->rows = cols;
        dst->cols = rows;
        return nullptr;
    }
    if (rows != src->cols || cols != src->rows) return "array size mismatch";
    double total = 0.0;
    double progress = 0.0;
    for (int i = 0; i < rows; ++i) {
        for (int j = 0; j < cols; ++j) {
            dst->data[i * dst->cols + j] = src->data[j * src->cols + i];
        }
        total += (double)(cols * 8);
        if (progress < total) {
            if (mutil_interrupt_step == 0.0) mutil_interrupt_step = 10000000.0;
            progress += mutil_interrupt_step;
        }
    }
    (void)context;
    return nullptr;
}

EFI_EXPORT const char *matu32_transpose(const _uint32_mat *src, void *context, _uint32_mat *dst) {
    check_src_f(src);
    check_dst_f(dst);
    int rows = dst->rows;
    int cols = dst->cols;
    if (src->data == dst->data) {
        if (src->rows != 1 && src->cols != 1) return "array size mismatch";
        dst->rows = cols;
        dst->cols = rows;
        return nullptr;
    }
    if (rows != src->cols || cols != src->rows) return "array size mismatch";
    double total = 0.0;
    double progress = 0.0;
    for (int i = 0; i < rows; ++i) {
        for (int j = 0; j < cols; ++j) {
            dst->data[i * dst->cols + j] = src->data[j * src->cols + i];
        }
        total += (double)(cols * 8);
        if (progress < total) {
            if (mutil_interrupt_step == 0.0) mutil_interrupt_step = 10000000.0;
            progress += mutil_interrupt_step;
        }
    }
    (void)context;
    return nullptr;
}

EFI_EXPORT const char *matu16_transpose(const _uint16_mat *src, void *context, _uint16_mat *dst) {
    check_src_f(src);
    check_dst_f(dst);
    int rows = dst->rows;
    int cols = dst->cols;
    if (src->data == dst->data) {
        if (src->rows != 1 && src->cols != 1) return "array size mismatch";
        dst->rows = cols;
        dst->cols = rows;
        return nullptr;
    }
    if (rows != src->cols || cols != src->rows) return "array size mismatch";
    double total = 0.0;
    double progress = 0.0;
    for (int i = 0; i < rows; ++i) {
        for (int j = 0; j < cols; ++j) {
            dst->data[i * dst->cols + j] = src->data[j * src->cols + i];
        }
        total += (double)(cols * 8);
        if (progress < total) {
            if (mutil_interrupt_step == 0.0) mutil_interrupt_step = 10000000.0;
            progress += mutil_interrupt_step;
        }
    }
    (void)context;
    return nullptr;
}

EFI_EXPORT const char *matu8_transpose(const _uint8_mat *src, void *context, _uint8_mat *dst) {
    check_src_f(src);
    check_dst_f(dst);
    int rows = dst->rows;
    int cols = dst->cols;
    if (src->data == dst->data) {
        if (src->rows != 1 && src->cols != 1) return "array size mismatch";
        dst->rows = cols;
        dst->cols = rows;
        return nullptr;
    }
    if (rows != src->cols || cols != src->rows) return "array size mismatch";
    double total = 0.0;
    double progress = 0.0;
    for (int i = 0; i < rows; ++i) {
        for (int j = 0; j < cols; ++j) {
            dst->data[i * dst->cols + j] = src->data[j * src->cols + i];
        }
        total += (double)(cols * 8);
        if (progress < total) {
            if (mutil_interrupt_step == 0.0) mutil_interrupt_step = 10000000.0;
            progress += mutil_interrupt_step;
        }
    }
    (void)context;
    return nullptr;
}

EFI_EXPORT const char *matcpx_transpose(const _dcomplex_mat *src, void *context, _dcomplex_mat *dst) {
    check_src_f(src);
    check_dst_f(dst);
    int rows = dst->rows;
    int cols = dst->cols;
    if (src->data == dst->data) {
        if (src->rows != 1 && src->cols != 1) return "array size mismatch";
        dst->rows = cols;
        dst->cols = rows;
        return nullptr;
    }
    if (rows != src->cols || cols != src->rows) return "array size mismatch";
    double total = 0.0;
    double progress = 0.0;
    for (int i = 0; i < rows; ++i) {
        for (int j = 0; j < cols; ++j) {
            dst->data[i * dst->cols + j] = src->data[j * src->cols + i];
        }
        total += (double)(cols * 8);
        if (progress < total) {
            if (mutil_interrupt_step == 0.0) mutil_interrupt_step = 10000000.0;
            progress += mutil_interrupt_step;
        }
    }
    (void)context;
    return nullptr;
}

}  // namespace mutils
