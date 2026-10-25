#include "mutils_mat.h"

namespace mutils {

template <typename T>
__forceinline void check_src_f(const typed_mat<T> *m) {
    if (m == nullptr) Efi::Throw("null pointer", ~0ull);
    if (m->rows <= 0 || m->cols <= 0) Efi::Throw("array size mismatch", ~0ull);
    if (m->data == nullptr) Efi::Throw("null pointer", ~0ull);
    if (m->size != m->rows * m->cols) Efi::Throw("array size mismatch", ~0ull);
}

template <typename T>
__forceinline void check_dst_f(const typed_mat<T> *m) {
    if (m == nullptr) Efi::Throw("null pointer", ~0ull);
    int cols = m->cols;
    int rows = m->rows;
    if (rows <= 0 || cols <= 0) Efi::Throw("array size mismatch", ~0ull);
    if (m->data == nullptr) Efi::Throw("null pointer", ~0ull);
    if (m->size != rows * cols) Efi::Throw("array size mismatch", ~0ull);
}

EFI_EXPORT const char *matflt_assign_submat(const _float_mat *src, long row, long col, void *context, _float_mat *dst) {
    check_src_f(src);
    check_dst_f(dst);
    if (row < 0 || dst->rows < src->rows + row || col < 0 || dst->cols < src->cols + col) return "array size mismatch";
    if (src->data == dst->data) return "Illegal memory address";
    double total = 0.0;
    double progress = 0.0;
    for (int i = 0; i < src->rows; ++i) {
        for (int j = 0; j < src->cols; ++j) {
            dst->data[(i + row) * dst->cols + j + col] = src->data[i * src->cols + j];
        }
        total += (double)(src->cols * 8);
        if (progress < total) {
            if (mutil_interrupt_step == 0.0) mutil_interrupt_step = 10000000.0;
            progress += mutil_interrupt_step;
        }
    }
    (void)context;
    return nullptr;
}

EFI_EXPORT const char *mats32_assign_submat(const _sint32_mat *src, long row, long col, void *context, _sint32_mat *dst) {
    check_src_f(src);
    check_dst_f(dst);
    if (row < 0 || dst->rows < src->rows + row || col < 0 || dst->cols < src->cols + col) return "array size mismatch";
    if (src->data == dst->data) return "Illegal memory address";
    double total = 0.0;
    double progress = 0.0;
    for (int i = 0; i < src->rows; ++i) {
        for (int j = 0; j < src->cols; ++j) {
            dst->data[(i + row) * dst->cols + j + col] = src->data[i * src->cols + j];
        }
        total += (double)(src->cols * 8);
        if (progress < total) {
            if (mutil_interrupt_step == 0.0) mutil_interrupt_step = 10000000.0;
            progress += mutil_interrupt_step;
        }
    }
    (void)context;
    return nullptr;
}

EFI_EXPORT const char *matu32_assign_submat(const _uint32_mat *src, long row, long col, void *context, _uint32_mat *dst) {
    check_src_f(src);
    check_dst_f(dst);
    if (row < 0 || dst->rows < src->rows + row || col < 0 || dst->cols < src->cols + col) return "array size mismatch";
    if (src->data == dst->data) return "Illegal memory address";
    double total = 0.0;
    double progress = 0.0;
    for (int i = 0; i < src->rows; ++i) {
        for (int j = 0; j < src->cols; ++j) {
            dst->data[(i + row) * dst->cols + j + col] = src->data[i * src->cols + j];
        }
        total += (double)(src->cols * 8);
        if (progress < total) {
            if (mutil_interrupt_step == 0.0) mutil_interrupt_step = 10000000.0;
            progress += mutil_interrupt_step;
        }
    }
    (void)context;
    return nullptr;
}

EFI_EXPORT const char *mats16_assign_submat(const _sint16_mat *src, long row, long col, void *context, _sint16_mat *dst) {
    check_src_f(src);
    check_dst_f(dst);
    if (row < 0 || dst->rows < src->rows + row || col < 0 || dst->cols < src->cols + col) return "array size mismatch";
    if (src->data == dst->data) return "Illegal memory address";
    double total = 0.0;
    double progress = 0.0;
    for (int i = 0; i < src->rows; ++i) {
        for (int j = 0; j < src->cols; ++j) {
            dst->data[(i + row) * dst->cols + j + col] = src->data[i * src->cols + j];
        }
        total += (double)(src->cols * 8);
        if (progress < total) {
            if (mutil_interrupt_step == 0.0) mutil_interrupt_step = 10000000.0;
            progress += mutil_interrupt_step;
        }
    }
    (void)context;
    return nullptr;
}

EFI_EXPORT const char *matu16_assign_submat(const _uint16_mat *src, long row, long col, void *context, _uint16_mat *dst) {
    check_src_f(src);
    check_dst_f(dst);
    if (row < 0 || dst->rows < src->rows + row || col < 0 || dst->cols < src->cols + col) return "array size mismatch";
    if (src->data == dst->data) return "Illegal memory address";
    double total = 0.0;
    double progress = 0.0;
    for (int i = 0; i < src->rows; ++i) {
        for (int j = 0; j < src->cols; ++j) {
            dst->data[(i + row) * dst->cols + j + col] = src->data[i * src->cols + j];
        }
        total += (double)(src->cols * 8);
        if (progress < total) {
            if (mutil_interrupt_step == 0.0) mutil_interrupt_step = 10000000.0;
            progress += mutil_interrupt_step;
        }
    }
    (void)context;
    return nullptr;
}

EFI_EXPORT const char *matu8_assign_submat(const _uint8_mat *src, long row, long col, void *context, _uint8_mat *dst) {
    check_src_f(src);
    check_dst_f(dst);
    if (row < 0 || dst->rows < src->rows + row || col < 0 || dst->cols < src->cols + col) return "array size mismatch";
    if (src->data == dst->data) return "Illegal memory address";
    double total = 0.0;
    double progress = 0.0;
    for (int i = 0; i < src->rows; ++i) {
        for (int j = 0; j < src->cols; ++j) {
            dst->data[(i + row) * dst->cols + j + col] = src->data[i * src->cols + j];
        }
        total += (double)(src->cols * 8);
        if (progress < total) {
            if (mutil_interrupt_step == 0.0) mutil_interrupt_step = 10000000.0;
            progress += mutil_interrupt_step;
        }
    }
    (void)context;
    return nullptr;
}

EFI_EXPORT const char *matcpx_assign_submat(const _dcomplex_mat *src, long row, long col, void *context, _dcomplex_mat *dst) {
    check_src_f(src);
    check_dst_f(dst);
    if (row < 0 || dst->rows < src->rows + row || col < 0 || dst->cols < src->cols + col) return "array size mismatch";
    if (src->data == dst->data) return "Illegal memory address";
    double total = 0.0;
    double progress = 0.0;
    for (int i = 0; i < src->rows; ++i) {
        for (int j = 0; j < src->cols; ++j) {
            dst->data[(i + row) * dst->cols + j + col] = src->data[i * src->cols + j];
        }
        total += (double)(src->cols * 8);
        if (progress < total) {
            if (mutil_interrupt_step == 0.0) mutil_interrupt_step = 10000000.0;
            progress += mutil_interrupt_step;
        }
    }
    (void)context;
    return nullptr;
}

}  // namespace mutils
