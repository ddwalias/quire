#include "mutils_mat.h"

#include <math.h>

namespace mutils {

EFI_EXPORT const char *matdbl_flip_left_right(const _double_mat *src, void *context, _double_mat *dst) {
    check_src_f(src);
    check_src_f(dst);
    int rows = dst->rows;
    int cols = dst->cols;
    if (src->rows != rows || src->cols != cols) return "array size mismatch";
    double *in = src->data;
    double *out = dst->data;
    double total = 0.0;
    double progress = 0.0;
    for (int i = 0; i < rows; ++i) {
        int left = i * cols;
        int right = (i + 1) * cols - 1;
        for (int j = 0; j < ceil(cols * 0.5); ++j) {
            double t = in[left + j];
            out[left + j] = in[right - j];
            out[right - j] = t;
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

EFI_EXPORT const char *matflt_flip_left_right(const _float_mat *src, void *context, _float_mat *dst) {
    check_src_f(src);
    check_src_f(dst);
    int rows = dst->rows;
    int cols = dst->cols;
    if (src->rows != rows || src->cols != cols) return "array size mismatch";
    float *in = src->data;
    float *out = dst->data;
    double total = 0.0;
    double progress = 0.0;
    for (int i = 0; i < rows; ++i) {
        int left = i * cols;
        int right = (i + 1) * cols - 1;
        for (int j = 0; j < ceil(cols * 0.5); ++j) {
            float t = in[left + j];
            out[left + j] = in[right - j];
            out[right - j] = t;
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

EFI_EXPORT const char *mats32_flip_left_right(const _sint32_mat *src, void *context, _sint32_mat *dst) {
    check_src_f(src);
    check_src_f(dst);
    int rows = dst->rows;
    int cols = dst->cols;
    if (src->rows != rows || src->cols != cols) return "array size mismatch";
    long *in = src->data;
    long *out = dst->data;
    double total = 0.0;
    double progress = 0.0;
    for (int i = 0; i < rows; ++i) {
        int left = i * cols;
        int right = (i + 1) * cols - 1;
        for (int j = 0; j < ceil(cols * 0.5); ++j) {
            long t = in[left + j];
            out[left + j] = in[right - j];
            out[right - j] = t;
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

EFI_EXPORT const char *mats16_flip_left_right(const _sint16_mat *src, void *context, _sint16_mat *dst) {
    check_src_f(src);
    check_src_f(dst);
    int rows = dst->rows;
    int cols = dst->cols;
    if (src->rows != rows || src->cols != cols) return "array size mismatch";
    short *in = src->data;
    short *out = dst->data;
    double total = 0.0;
    double progress = 0.0;
    for (int i = 0; i < rows; ++i) {
        int left = i * cols;
        int right = (i + 1) * cols - 1;
        for (int j = 0; j < ceil(cols * 0.5); ++j) {
            short t = in[left + j];
            out[left + j] = in[right - j];
            out[right - j] = t;
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

EFI_EXPORT const char *matu32_flip_left_right(const _uint32_mat *src, void *context, _uint32_mat *dst) {
    check_src_f(src);
    check_src_f(dst);
    int rows = dst->rows;
    int cols = dst->cols;
    if (src->rows != rows || src->cols != cols) return "array size mismatch";
    unsigned int *in = src->data;
    unsigned int *out = dst->data;
    double total = 0.0;
    double progress = 0.0;
    for (int i = 0; i < rows; ++i) {
        int left = i * cols;
        int right = (i + 1) * cols - 1;
        for (int j = 0; j < ceil(cols * 0.5); ++j) {
            unsigned int t = in[left + j];
            out[left + j] = in[right - j];
            out[right - j] = t;
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

EFI_EXPORT const char *matu16_flip_left_right(const _uint16_mat *src, void *context, _uint16_mat *dst) {
    check_src_f(src);
    check_src_f(dst);
    int rows = dst->rows;
    int cols = dst->cols;
    if (src->rows != rows || src->cols != cols) return "array size mismatch";
    unsigned short *in = src->data;
    unsigned short *out = dst->data;
    double total = 0.0;
    double progress = 0.0;
    for (int i = 0; i < rows; ++i) {
        int left = i * cols;
        int right = (i + 1) * cols - 1;
        for (int j = 0; j < ceil(cols * 0.5); ++j) {
            unsigned short t = in[left + j];
            out[left + j] = in[right - j];
            out[right - j] = t;
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

EFI_EXPORT const char *matu8_flip_left_right(const _uint8_mat *src, void *context, _uint8_mat *dst) {
    check_src_f(src);
    check_src_f(dst);
    int rows = dst->rows;
    int cols = dst->cols;
    if (src->rows != rows || src->cols != cols) return "array size mismatch";
    unsigned char *in = src->data;
    unsigned char *out = dst->data;
    double total = 0.0;
    double progress = 0.0;
    for (int i = 0; i < rows; ++i) {
        int left = i * cols;
        int right = (i + 1) * cols - 1;
        for (int j = 0; j < ceil(cols * 0.5); ++j) {
            unsigned char t = in[left + j];
            out[left + j] = in[right - j];
            out[right - j] = t;
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

EFI_EXPORT const char *matcpx_flip_left_right(const _dcomplex_mat *src, void *context, _dcomplex_mat *dst) {
    check_src_f(src);
    check_src_f(dst);
    int rows = dst->rows;
    int cols = dst->cols;
    if (src->rows != rows || src->cols != cols) return "array size mismatch";
    _dcomplex *in = src->data;
    _dcomplex *out = dst->data;
    double total = 0.0;
    double progress = 0.0;
    for (int i = 0; i < rows; ++i) {
        int left = i * cols;
        int right = (i + 1) * cols - 1;
        for (int j = 0; j < ceil(cols * 0.5); ++j) {
            _dcomplex t = in[left + j];
            out[left + j] = in[right - j];
            out[right - j] = t;
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

EFI_EXPORT const char *matdbl_flip_up_down(const _double_mat *src, void *context, _double_mat *dst) {
    check_src_f(src);
    check_src_f(dst);
    int rows = dst->rows;
    int cols = dst->cols;
    if (src->rows != rows || src->cols != cols) return "array size mismatch";
    double *in = src->data;
    double *out = dst->data;
    double total = 0.0;
    double progress = 0.0;
    for (int i = 0; i < ceil(rows * 0.5); ++i) {
        int top = i * cols;
        int bottom = (rows - i - 1) * cols;
        for (int j = 0; j < cols; ++j) {
            double t = in[top + j];
            out[top + j] = in[bottom + j];
            out[bottom + j] = t;
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

EFI_EXPORT const char *matflt_flip_up_down(const _float_mat *src, void *context, _float_mat *dst) {
    check_src_f(src);
    check_src_f(dst);
    int rows = dst->rows;
    int cols = dst->cols;
    if (src->rows != rows || src->cols != cols) return "array size mismatch";
    float *in = src->data;
    float *out = dst->data;
    double total = 0.0;
    double progress = 0.0;
    for (int i = 0; i < ceil(rows * 0.5); ++i) {
        int top = i * cols;
        int bottom = (rows - i - 1) * cols;
        for (int j = 0; j < cols; ++j) {
            float t = in[top + j];
            out[top + j] = in[bottom + j];
            out[bottom + j] = t;
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

EFI_EXPORT const char *mats32_flip_up_down(const _sint32_mat *src, void *context, _sint32_mat *dst) {
    check_src_f(src);
    check_src_f(dst);
    int rows = dst->rows;
    int cols = dst->cols;
    if (src->rows != rows || src->cols != cols) return "array size mismatch";
    long *in = src->data;
    long *out = dst->data;
    double total = 0.0;
    double progress = 0.0;
    for (int i = 0; i < ceil(rows * 0.5); ++i) {
        int top = i * cols;
        int bottom = (rows - i - 1) * cols;
        for (int j = 0; j < cols; ++j) {
            long t = in[top + j];
            out[top + j] = in[bottom + j];
            out[bottom + j] = t;
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

EFI_EXPORT const char *mats16_flip_up_down(const _sint16_mat *src, void *context, _sint16_mat *dst) {
    check_src_f(src);
    check_src_f(dst);
    int rows = dst->rows;
    int cols = dst->cols;
    if (src->rows != rows || src->cols != cols) return "array size mismatch";
    short *in = src->data;
    short *out = dst->data;
    double total = 0.0;
    double progress = 0.0;
    for (int i = 0; i < ceil(rows * 0.5); ++i) {
        int top = i * cols;
        int bottom = (rows - i - 1) * cols;
        for (int j = 0; j < cols; ++j) {
            short t = in[top + j];
            out[top + j] = in[bottom + j];
            out[bottom + j] = t;
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

EFI_EXPORT const char *matu32_flip_up_down(const _uint32_mat *src, void *context, _uint32_mat *dst) {
    check_src_f(src);
    check_src_f(dst);
    int rows = dst->rows;
    int cols = dst->cols;
    if (src->rows != rows || src->cols != cols) return "array size mismatch";
    unsigned int *in = src->data;
    unsigned int *out = dst->data;
    double total = 0.0;
    double progress = 0.0;
    for (int i = 0; i < ceil(rows * 0.5); ++i) {
        int top = i * cols;
        int bottom = (rows - i - 1) * cols;
        for (int j = 0; j < cols; ++j) {
            unsigned int t = in[top + j];
            out[top + j] = in[bottom + j];
            out[bottom + j] = t;
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

EFI_EXPORT const char *matu16_flip_up_down(const _uint16_mat *src, void *context, _uint16_mat *dst) {
    check_src_f(src);
    check_src_f(dst);
    int rows = dst->rows;
    int cols = dst->cols;
    if (src->rows != rows || src->cols != cols) return "array size mismatch";
    unsigned short *in = src->data;
    unsigned short *out = dst->data;
    double total = 0.0;
    double progress = 0.0;
    for (int i = 0; i < ceil(rows * 0.5); ++i) {
        int top = i * cols;
        int bottom = (rows - i - 1) * cols;
        for (int j = 0; j < cols; ++j) {
            unsigned short t = in[top + j];
            out[top + j] = in[bottom + j];
            out[bottom + j] = t;
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

EFI_EXPORT const char *matu8_flip_up_down(const _uint8_mat *src, void *context, _uint8_mat *dst) {
    check_src_f(src);
    check_src_f(dst);
    int rows = dst->rows;
    int cols = dst->cols;
    if (src->rows != rows || src->cols != cols) return "array size mismatch";
    unsigned char *in = src->data;
    unsigned char *out = dst->data;
    double total = 0.0;
    double progress = 0.0;
    for (int i = 0; i < ceil(rows * 0.5); ++i) {
        int top = i * cols;
        int bottom = (rows - i - 1) * cols;
        for (int j = 0; j < cols; ++j) {
            unsigned char t = in[top + j];
            out[top + j] = in[bottom + j];
            out[bottom + j] = t;
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
