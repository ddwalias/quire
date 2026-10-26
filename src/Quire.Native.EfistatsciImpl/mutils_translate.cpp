#include "mutils_mat.h"

namespace mutils {

EFI_EXPORT const char *matdbl_translate(const _double_mat *src, long row, long col, double value, void *context, _double_mat *dst) {
    check_src_f(src);
    check_src_f(dst);
    if (src->data == dst->data || src->rows != dst->rows || src->cols != dst->cols) return "array size mismatch";
    if (row == 0 && col == 0) {
        matdbl_assign(src, context, dst);
        return nullptr;
    }
    matdbl_assign_scalar(value, context, dst);
    int rows = src->rows;
    int cols = src->cols;
    if (row >= rows || col >= cols || -row >= rows || -col >= cols) return nullptr;
    double total = 0.0;
    double progress = 0.0;
    int first_row, last_row, out_row, first_col, last_col, out_col;
    if (row >= 0) {
        first_row = 0;
        last_row = rows - row;
        out_row = row;
    } else {
        first_row = -row;
        last_row = rows;
        out_row = 0;
    }
    if (col >= 0) {
        first_col = 0;
        last_col = cols - col;
        out_col = col;
    } else {
        first_col = -col;
        last_col = cols;
        out_col = 0;
    }
    for (int i = first_row; i < last_row; ++i) {
        for (int k = out_col, j = first_col; j < last_col; ++j, ++k) dst->data[out_row * dst->cols + k] = src->data[i * src->cols + j];
        total += (double)((last_col - first_col + 1) * 8);
        ++out_row;
        if (progress < total) {
            if (mutil_interrupt_step == 0.0) mutil_interrupt_step = 10000000.0;
            progress += mutil_interrupt_step;
        }
    }
    return nullptr;
}

EFI_EXPORT const char *matflt_translate(const _float_mat *src, long row, long col, float value, void *context, _float_mat *dst) {
    check_src_f(src);
    check_dst_f(dst);
    if (src->data == dst->data || src->rows != dst->rows || src->cols != dst->cols) return "array size mismatch";
    if (row == 0 && col == 0) {
        matflt_assign(src, context, dst);
        return nullptr;
    }
    matflt_assign_scalar(value, context, dst);
    int rows = src->rows;
    int cols = src->cols;
    if (row >= rows || col >= cols || -row >= rows || -col >= cols) return nullptr;
    double total = 0.0;
    double progress = 0.0;
    int first_row, last_row, out_row, first_col, last_col, out_col;
    if (row >= 0) {
        first_row = 0;
        last_row = rows - row;
        out_row = row;
    } else {
        first_row = -row;
        last_row = rows;
        out_row = 0;
    }
    if (col >= 0) {
        first_col = 0;
        last_col = cols - col;
        out_col = col;
    } else {
        first_col = -col;
        last_col = cols;
        out_col = 0;
    }
    for (int i = first_row; i < last_row; ++i) {
        for (int k = out_col, j = first_col; j < last_col; ++j, ++k) dst->data[out_row * dst->cols + k] = src->data[i * src->cols + j];
        total += (double)((last_col - first_col + 1) * 8);
        ++out_row;
        if (progress < total) {
            if (mutil_interrupt_step == 0.0) mutil_interrupt_step = 10000000.0;
            progress += mutil_interrupt_step;
        }
    }
    return nullptr;
}

EFI_EXPORT const char *mats32_translate(const _sint32_mat *src, long row, long col, long value, void *context, _sint32_mat *dst) {
    check_src_f(src);
    check_dst_f(dst);
    if (src->data == dst->data || src->rows != dst->rows || src->cols != dst->cols) return "array size mismatch";
    if (row == 0 && col == 0) {
        mats32_assign(src, context, dst);
        return nullptr;
    }
    mats32_assign_scalar(value, context, dst);
    int rows = src->rows;
    int cols = src->cols;
    if (row >= rows || col >= cols || -row >= rows || -col >= cols) return nullptr;
    double total = 0.0;
    double progress = 0.0;
    int first_row, last_row, out_row, first_col, last_col, out_col;
    if (row >= 0) {
        first_row = 0;
        last_row = rows - row;
        out_row = row;
    } else {
        first_row = -row;
        last_row = rows;
        out_row = 0;
    }
    if (col >= 0) {
        first_col = 0;
        last_col = cols - col;
        out_col = col;
    } else {
        first_col = -col;
        last_col = cols;
        out_col = 0;
    }
    for (int i = first_row; i < last_row; ++i) {
        for (int k = out_col, j = first_col; j < last_col; ++j, ++k) dst->data[out_row * dst->cols + k] = src->data[i * src->cols + j];
        total += (double)((last_col - first_col + 1) * 8);
        ++out_row;
        if (progress < total) {
            if (mutil_interrupt_step == 0.0) mutil_interrupt_step = 10000000.0;
            progress += mutil_interrupt_step;
        }
    }
    return nullptr;
}

EFI_EXPORT const char *mats16_translate(const _sint16_mat *src, long row, long col, short value, void *context, _sint16_mat *dst) {
    check_src_f(src);
    check_dst_f(dst);
    if (src->data == dst->data || src->rows != dst->rows || src->cols != dst->cols) return "array size mismatch";
    if (row == 0 && col == 0) {
        mats16_assign(src, context, dst);
        return nullptr;
    }
    mats16_assign_scalar(value, context, dst);
    int rows = src->rows;
    int cols = src->cols;
    if (row >= rows || col >= cols || -row >= rows || -col >= cols) return nullptr;
    double total = 0.0;
    double progress = 0.0;
    int first_row, last_row, out_row, first_col, last_col, out_col;
    if (row >= 0) {
        first_row = 0;
        last_row = rows - row;
        out_row = row;
    } else {
        first_row = -row;
        last_row = rows;
        out_row = 0;
    }
    if (col >= 0) {
        first_col = 0;
        last_col = cols - col;
        out_col = col;
    } else {
        first_col = -col;
        last_col = cols;
        out_col = 0;
    }
    for (int i = first_row; i < last_row; ++i) {
        for (int k = out_col, j = first_col; j < last_col; ++j, ++k) dst->data[out_row * dst->cols + k] = src->data[i * src->cols + j];
        total += (double)((last_col - first_col + 1) * 8);
        ++out_row;
        if (progress < total) {
            if (mutil_interrupt_step == 0.0) mutil_interrupt_step = 10000000.0;
            progress += mutil_interrupt_step;
        }
    }
    return nullptr;
}

EFI_EXPORT const char *matu32_translate(const _uint32_mat *src, long row, long col, unsigned int value, void *context, _uint32_mat *dst) {
    check_src_f(src);
    check_dst_f(dst);
    if (src->data == dst->data || src->rows != dst->rows || src->cols != dst->cols) return "array size mismatch";
    if (row == 0 && col == 0) {
        matu32_assign(src, context, dst);
        return nullptr;
    }
    matu32_assign_scalar(value, context, dst);
    int rows = src->rows;
    int cols = src->cols;
    if (row >= rows || col >= cols || -row >= rows || -col >= cols) return nullptr;
    double total = 0.0;
    double progress = 0.0;
    int first_row, last_row, out_row, first_col, last_col, out_col;
    if (row >= 0) {
        first_row = 0;
        last_row = rows - row;
        out_row = row;
    } else {
        first_row = -row;
        last_row = rows;
        out_row = 0;
    }
    if (col >= 0) {
        first_col = 0;
        last_col = cols - col;
        out_col = col;
    } else {
        first_col = -col;
        last_col = cols;
        out_col = 0;
    }
    for (int i = first_row; i < last_row; ++i) {
        for (int k = out_col, j = first_col; j < last_col; ++j, ++k) dst->data[out_row * dst->cols + k] = src->data[i * src->cols + j];
        total += (double)((last_col - first_col + 1) * 8);
        ++out_row;
        if (progress < total) {
            if (mutil_interrupt_step == 0.0) mutil_interrupt_step = 10000000.0;
            progress += mutil_interrupt_step;
        }
    }
    return nullptr;
}

EFI_EXPORT const char *matu16_translate(const _uint16_mat *src, long row, long col, unsigned short value, void *context, _uint16_mat *dst) {
    check_src_f(src);
    check_dst_f(dst);
    if (src->data == dst->data || src->rows != dst->rows || src->cols != dst->cols) return "array size mismatch";
    if (row == 0 && col == 0) {
        matu16_assign(src, context, dst);
        return nullptr;
    }
    matu16_assign_scalar(value, context, dst);
    int rows = src->rows;
    int cols = src->cols;
    if (row >= rows || col >= cols || -row >= rows || -col >= cols) return nullptr;
    double total = 0.0;
    double progress = 0.0;
    int first_row, last_row, out_row, first_col, last_col, out_col;
    if (row >= 0) {
        first_row = 0;
        last_row = rows - row;
        out_row = row;
    } else {
        first_row = -row;
        last_row = rows;
        out_row = 0;
    }
    if (col >= 0) {
        first_col = 0;
        last_col = cols - col;
        out_col = col;
    } else {
        first_col = -col;
        last_col = cols;
        out_col = 0;
    }
    for (int i = first_row; i < last_row; ++i) {
        for (int k = out_col, j = first_col; j < last_col; ++j, ++k) dst->data[out_row * dst->cols + k] = src->data[i * src->cols + j];
        total += (double)((last_col - first_col + 1) * 8);
        ++out_row;
        if (progress < total) {
            if (mutil_interrupt_step == 0.0) mutil_interrupt_step = 10000000.0;
            progress += mutil_interrupt_step;
        }
    }
    return nullptr;
}

EFI_EXPORT const char *matu8_translate(const _uint8_mat *src, long row, long col, unsigned char value, void *context, _uint8_mat *dst) {
    check_src_f(src);
    check_dst_f(dst);
    if (src->data == dst->data || src->rows != dst->rows || src->cols != dst->cols) return "array size mismatch";
    if (row == 0 && col == 0) {
        matu8_assign(src, context, dst);
        return nullptr;
    }
    matu8_assign_scalar(value, context, dst);
    int rows = src->rows;
    int cols = src->cols;
    if (row >= rows || col >= cols || -row >= rows || -col >= cols) return nullptr;
    double total = 0.0;
    double progress = 0.0;
    int first_row, last_row, out_row, first_col, last_col, out_col;
    if (row >= 0) {
        first_row = 0;
        last_row = rows - row;
        out_row = row;
    } else {
        first_row = -row;
        last_row = rows;
        out_row = 0;
    }
    if (col >= 0) {
        first_col = 0;
        last_col = cols - col;
        out_col = col;
    } else {
        first_col = -col;
        last_col = cols;
        out_col = 0;
    }
    for (int i = first_row; i < last_row; ++i) {
        for (int k = out_col, j = first_col; j < last_col; ++j, ++k) dst->data[out_row * dst->cols + k] = src->data[i * src->cols + j];
        total += (double)((last_col - first_col + 1) * 8);
        ++out_row;
        if (progress < total) {
            if (mutil_interrupt_step == 0.0) mutil_interrupt_step = 10000000.0;
            progress += mutil_interrupt_step;
        }
    }
    return nullptr;
}

}  // namespace mutils
