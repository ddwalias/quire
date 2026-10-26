#include "mutils_mat.h"

namespace mutils {

EFI_EXPORT const char *matdbl_extract(const _double_mat *src, long row, long col, void *context, _double_mat *dst) {
    check_src_f(src);
    check_src_f(dst);
    if (src->data == dst->data) return "Illegal memory address";
    int last_row = row + dst->rows - 1;
    int last_col = col + dst->cols - 1;
    if (row < 0 || last_row >= src->rows || col < 0 || last_col >= src->cols) return "array size mismatch";
    double total = 0.0;
    double progress = 0.0;
    for (int c = col, j = 0; c <= last_col; ++c, ++j) {
        for (int r = row, i = 0; r <= last_row; ++r, ++i) {
            dst->data[i * dst->cols + j] = src->data[r * src->cols + c];
        }
        total += (double)((last_row - row) * 8);
        if (progress < total) {
            if (mutil_interrupt_step == 0.0) mutil_interrupt_step = 10000000.0;
            progress += mutil_interrupt_step;
        }
    }
    (void)context;
    return nullptr;
}

EFI_EXPORT const char *matflt_extract(const _float_mat *src, long row, long col, void *context, _float_mat *dst) {
    check_src_f(src);
    check_src_f(dst);
    if (src->data == dst->data) return "Illegal memory address";
    int last_row = row + dst->rows - 1;
    int last_col = col + dst->cols - 1;
    if (row < 0 || last_row >= src->rows || col < 0 || last_col >= src->cols) return "array size mismatch";
    double total = 0.0;
    double progress = 0.0;
    for (int c = col, j = 0; c <= last_col; ++c, ++j) {
        for (int r = row, i = 0; r <= last_row; ++r, ++i) {
            dst->data[i * dst->cols + j] = src->data[r * src->cols + c];
        }
        total += (double)((last_row - row) * 8);
        if (progress < total) {
            if (mutil_interrupt_step == 0.0) mutil_interrupt_step = 10000000.0;
            progress += mutil_interrupt_step;
        }
    }
    (void)context;
    return nullptr;
}

EFI_EXPORT const char *mats32_extract(const _sint32_mat *src, long row, long col, void *context, _sint32_mat *dst) {
    check_src_f(src);
    check_src_f(dst);
    if (src->data == dst->data) return "Illegal memory address";
    int last_row = row + dst->rows - 1;
    int last_col = col + dst->cols - 1;
    if (row < 0 || last_row >= src->rows || col < 0 || last_col >= src->cols) return "array size mismatch";
    double total = 0.0;
    double progress = 0.0;
    for (int c = col, j = 0; c <= last_col; ++c, ++j) {
        for (int r = row, i = 0; r <= last_row; ++r, ++i) {
            dst->data[i * dst->cols + j] = src->data[r * src->cols + c];
        }
        total += (double)((last_row - row) * 8);
        if (progress < total) {
            if (mutil_interrupt_step == 0.0) mutil_interrupt_step = 10000000.0;
            progress += mutil_interrupt_step;
        }
    }
    (void)context;
    return nullptr;
}

EFI_EXPORT const char *mats16_extract(const _sint16_mat *src, long row, long col, void *context, _sint16_mat *dst) {
    check_src_f(src);
    check_src_f(dst);
    if (src->data == dst->data) return "Illegal memory address";
    int last_row = row + dst->rows - 1;
    int last_col = col + dst->cols - 1;
    if (row < 0 || last_row >= src->rows || col < 0 || last_col >= src->cols) return "array size mismatch";
    double total = 0.0;
    double progress = 0.0;
    for (int c = col, j = 0; c <= last_col; ++c, ++j) {
        for (int r = row, i = 0; r <= last_row; ++r, ++i) {
            dst->data[i * dst->cols + j] = src->data[r * src->cols + c];
        }
        total += (double)((last_row - row) * 8);
        if (progress < total) {
            if (mutil_interrupt_step == 0.0) mutil_interrupt_step = 10000000.0;
            progress += mutil_interrupt_step;
        }
    }
    (void)context;
    return nullptr;
}

EFI_EXPORT const char *matu32_extract(const _uint32_mat *src, long row, long col, void *context, _uint32_mat *dst) {
    check_src_f(src);
    check_src_f(dst);
    if (src->data == dst->data) return "Illegal memory address";
    int last_row = row + dst->rows - 1;
    int last_col = col + dst->cols - 1;
    if (row < 0 || last_row >= src->rows || col < 0 || last_col >= src->cols) return "array size mismatch";
    double total = 0.0;
    double progress = 0.0;
    for (int c = col, j = 0; c <= last_col; ++c, ++j) {
        for (int r = row, i = 0; r <= last_row; ++r, ++i) {
            dst->data[i * dst->cols + j] = src->data[r * src->cols + c];
        }
        total += (double)((last_row - row) * 8);
        if (progress < total) {
            if (mutil_interrupt_step == 0.0) mutil_interrupt_step = 10000000.0;
            progress += mutil_interrupt_step;
        }
    }
    (void)context;
    return nullptr;
}

EFI_EXPORT const char *matu16_extract(const _uint16_mat *src, long row, long col, void *context, _uint16_mat *dst) {
    check_src_f(src);
    check_src_f(dst);
    if (src->data == dst->data) return "Illegal memory address";
    int last_row = row + dst->rows - 1;
    int last_col = col + dst->cols - 1;
    if (row < 0 || last_row >= src->rows || col < 0 || last_col >= src->cols) return "array size mismatch";
    double total = 0.0;
    double progress = 0.0;
    for (int c = col, j = 0; c <= last_col; ++c, ++j) {
        for (int r = row, i = 0; r <= last_row; ++r, ++i) {
            dst->data[i * dst->cols + j] = src->data[r * src->cols + c];
        }
        total += (double)((last_row - row) * 8);
        if (progress < total) {
            if (mutil_interrupt_step == 0.0) mutil_interrupt_step = 10000000.0;
            progress += mutil_interrupt_step;
        }
    }
    (void)context;
    return nullptr;
}

EFI_EXPORT const char *matu8_extract(const _uint8_mat *src, long row, long col, void *context, _uint8_mat *dst) {
    check_src_f(src);
    check_src_f(dst);
    if (src->data == dst->data) return "Illegal memory address";
    int last_row = row + dst->rows - 1;
    int last_col = col + dst->cols - 1;
    if (row < 0 || last_row >= src->rows || col < 0 || last_col >= src->cols) return "array size mismatch";
    double total = 0.0;
    double progress = 0.0;
    for (int c = col, j = 0; c <= last_col; ++c, ++j) {
        for (int r = row, i = 0; r <= last_row; ++r, ++i) {
            dst->data[i * dst->cols + j] = src->data[r * src->cols + c];
        }
        total += (double)((last_row - row) * 8);
        if (progress < total) {
            if (mutil_interrupt_step == 0.0) mutil_interrupt_step = 10000000.0;
            progress += mutil_interrupt_step;
        }
    }
    (void)context;
    return nullptr;
}

EFI_EXPORT const char *matcpx_extract(const _dcomplex_mat *src, long row, long col, void *context, _dcomplex_mat *dst) {
    check_src_f(src);
    check_src_f(dst);
    if (src->data == dst->data) return "Illegal memory address";
    int last_row = row + dst->rows - 1;
    int last_col = col + dst->cols - 1;
    if (row < 0 || last_row >= src->rows || col < 0 || last_col >= src->cols) return "array size mismatch";
    double total = 0.0;
    double progress = 0.0;
    for (int c = col, j = 0; c <= last_col; ++c, ++j) {
        for (int r = row, i = 0; r <= last_row; ++r, ++i) {
            dst->data[i * dst->cols + j] = src->data[r * src->cols + c];
        }
        total += (double)((last_row - row) * 8);
        if (progress < total) {
            if (mutil_interrupt_step == 0.0) mutil_interrupt_step = 10000000.0;
            progress += mutil_interrupt_step;
        }
    }
    (void)context;
    return nullptr;
}

}  // namespace mutils
