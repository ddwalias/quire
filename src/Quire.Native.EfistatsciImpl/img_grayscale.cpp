#include "mutils_mat.h"

// Boundary handling for neighbourhood filters. Only the zero (0) and replicate (3) rules are handled by the
// grayscale morphology; any other value is rejected.
enum _mutil_boundary_type { MUTIL_BOUNDARY_ZERO = 0, MUTIL_BOUNDARY_REPLICATE = 3 };

namespace mutils {

// The pixel at (i, j) of `in` (rows x cols, row stride m->cols) under `boundary`; false for an unknown rule.
template <typename T>
__forceinline bool boundary_pixel(const typed_mat<T> *m, const T *in, int rows, int cols, int i, int j, _mutil_boundary_type boundary, T *value) {
    if (boundary == MUTIL_BOUNDARY_ZERO) {
        if (i >= 0 && j >= 0 && i < rows && j < cols) *value = in[i * m->cols + j];
        else *value = 0;
        return true;
    }
    if (boundary != MUTIL_BOUNDARY_REPLICATE) return false;
    i = i < 0 ? 0 : (i > rows - 1 ? rows - 1 : i);
    j = j < 0 ? 0 : (j > cols - 1 ? cols - 1 : j);
    *value = in[i * m->cols + j];
    return true;
}


EFI_EXPORT const char *imgdbl_grayscale_erode(Efi::EfiCallContext *, const _double_mat *src, const _double_mat *se, long se_row, long se_col, _mutil_boundary_type boundary, void *context, _double_mat *dst) {
    check_src_f(src);
    check_src_f(se);
    check_src_f(dst);
    int rows = src->rows;
    int cols = src->cols;
    if (rows != dst->rows || cols != dst->cols) return "array size mismatch";
    if (src->data == dst->data || se->data == dst->data) return "Illegal memory address";
    const double *in = src->data;
    const double *s = se->data;
    double *out = dst->data;
    int se_rows = se->rows;
    int se_cols = se->cols;
    double total = 0.0;
    double progress = 0.0;
    for (int r = 0; r < rows; ++r) {
        for (int c = 0; c < cols; ++c) {
            double v;
            int i, j;
            i = r - se_row;
            j = c - se_col;
            switch (boundary) {
            case MUTIL_BOUNDARY_ZERO:
                v = (i >= 0 && j >= 0 && i < rows && j < cols) ? in[i * src->cols + j] : 0;
                break;
            case MUTIL_BOUNDARY_REPLICATE:
                i = i < 0 ? 0 : (i > rows - 1 ? rows - 1 : i);
                j = j < 0 ? 0 : (j > cols - 1 ? cols - 1 : j);
                v = in[i * src->cols + j];
                break;
            default:
                return "Illegal data value";
            }
            double best = (double)(v - s[0]);
            for (int k = 0; k < se_rows; ++k) {
                for (int l = 0; l < se_cols; ++l) {
                    if (k == 0 && l == 0) continue;
                    i = r + k - se_row;
                    j = c + l - se_col;
                    switch (boundary) {
                    case MUTIL_BOUNDARY_ZERO:
                        v = (i >= 0 && j >= 0 && i < rows && j < cols) ? in[i * src->cols + j] : 0;
                        break;
                    case MUTIL_BOUNDARY_REPLICATE:
                        i = i < 0 ? 0 : (i > rows - 1 ? rows - 1 : i);
                        j = j < 0 ? 0 : (j > cols - 1 ? cols - 1 : j);
                        v = in[i * src->cols + j];
                        break;
                    default:
                        return "Illegal data value";
                    }
                    double x = (double)(v - s[k * se->cols + l]);
                    if (x < best) best = x;
                }
            }
            out[r * dst->cols + c] = best;
        }
        total += se_rows * 8.0 * se_cols * cols + 8.0;
        if (progress < total) {
            if (mutil_interrupt_step == 0.0) mutil_interrupt_step = 10000000.0;
            progress += mutil_interrupt_step;
        }
    }
    (void)context;
    return nullptr;
}

EFI_EXPORT const char *imgs32_grayscale_erode(Efi::EfiCallContext *, const _sint32_mat *src, const _sint32_mat *se, long se_row, long se_col, _mutil_boundary_type boundary, void *context, _sint32_mat *dst) {
    check_src_f(src);
    check_src_f(se);
    check_src_f(dst);
    int rows = src->rows;
    int cols = src->cols;
    if (rows != dst->rows || cols != dst->cols) return "array size mismatch";
    if (src->data == dst->data || se->data == dst->data) return "Illegal memory address";
    const long *in = src->data;
    const long *s = se->data;
    long *out = dst->data;
    int se_rows = se->rows;
    int se_cols = se->cols;
    double total = 0.0;
    double progress = 0.0;
    for (int r = 0; r < rows; ++r) {
        for (int c = 0; c < cols; ++c) {
            long v;
            int i, j;
            i = r - se_row;
            j = c - se_col;
            switch (boundary) {
            case MUTIL_BOUNDARY_ZERO:
                v = (i >= 0 && j >= 0 && i < rows && j < cols) ? in[i * src->cols + j] : 0;
                break;
            case MUTIL_BOUNDARY_REPLICATE:
                i = i < 0 ? 0 : (i > rows - 1 ? rows - 1 : i);
                j = j < 0 ? 0 : (j > cols - 1 ? cols - 1 : j);
                v = in[i * src->cols + j];
                break;
            default:
                return "Illegal data value";
            }
            long best = (long)(v - s[0]);
            for (int k = 0; k < se_rows; ++k) {
                for (int l = 0; l < se_cols; ++l) {
                    if (k == 0 && l == 0) continue;
                    i = r + k - se_row;
                    j = c + l - se_col;
                    switch (boundary) {
                    case MUTIL_BOUNDARY_ZERO:
                        v = (i >= 0 && j >= 0 && i < rows && j < cols) ? in[i * src->cols + j] : 0;
                        break;
                    case MUTIL_BOUNDARY_REPLICATE:
                        i = i < 0 ? 0 : (i > rows - 1 ? rows - 1 : i);
                        j = j < 0 ? 0 : (j > cols - 1 ? cols - 1 : j);
                        v = in[i * src->cols + j];
                        break;
                    default:
                        return "Illegal data value";
                    }
                    long x = (long)(v - s[k * se->cols + l]);
                    if (x < best) best = x;
                }
            }
            out[r * dst->cols + c] = best;
        }
        total += se_rows * 8.0 * se_cols * cols + 8.0;
        if (progress < total) {
            if (mutil_interrupt_step == 0.0) mutil_interrupt_step = 10000000.0;
            progress += mutil_interrupt_step;
        }
    }
    (void)context;
    return nullptr;
}

EFI_EXPORT const char *imgu8_grayscale_erode(Efi::EfiCallContext *, const _uint8_mat *src, const _uint8_mat *se, long se_row, long se_col, _mutil_boundary_type boundary, void *context, _uint8_mat *dst) {
    check_src_f(src);
    check_src_f(se);
    check_src_f(dst);
    int rows = src->rows;
    int cols = src->cols;
    if (rows != dst->rows || cols != dst->cols) return "array size mismatch";
    if (src->data == dst->data || se->data == dst->data) return "Illegal memory address";
    const unsigned char *in = src->data;
    const unsigned char *s = se->data;
    unsigned char *out = dst->data;
    int se_rows = se->rows;
    int se_cols = se->cols;
    double total = 0.0;
    double progress = 0.0;
    for (int r = 0; r < rows; ++r) {
        for (int c = 0; c < cols; ++c) {
            unsigned char v;
            int i, j;
            i = r - se_row;
            j = c - se_col;
            switch (boundary) {
            case MUTIL_BOUNDARY_ZERO:
                v = (i >= 0 && j >= 0 && i < rows && j < cols) ? in[i * src->cols + j] : 0;
                break;
            case MUTIL_BOUNDARY_REPLICATE:
                i = i < 0 ? 0 : (i > rows - 1 ? rows - 1 : i);
                j = j < 0 ? 0 : (j > cols - 1 ? cols - 1 : j);
                v = in[i * src->cols + j];
                break;
            default:
                return "Illegal data value";
            }
            unsigned char best = (unsigned char)(v - s[0]);
            for (int k = 0; k < se_rows; ++k) {
                for (int l = 0; l < se_cols; ++l) {
                    if (k == 0 && l == 0) continue;
                    i = r + k - se_row;
                    j = c + l - se_col;
                    switch (boundary) {
                    case MUTIL_BOUNDARY_ZERO:
                        v = (i >= 0 && j >= 0 && i < rows && j < cols) ? in[i * src->cols + j] : 0;
                        break;
                    case MUTIL_BOUNDARY_REPLICATE:
                        i = i < 0 ? 0 : (i > rows - 1 ? rows - 1 : i);
                        j = j < 0 ? 0 : (j > cols - 1 ? cols - 1 : j);
                        v = in[i * src->cols + j];
                        break;
                    default:
                        return "Illegal data value";
                    }
                    unsigned char x = (unsigned char)(v - s[k * se->cols + l]);
                    if (x < best) best = x;
                }
            }
            out[r * dst->cols + c] = best;
        }
        total += se_rows * 8.0 * se_cols * cols + 8.0;
        if (progress < total) {
            if (mutil_interrupt_step == 0.0) mutil_interrupt_step = 10000000.0;
            progress += mutil_interrupt_step;
        }
    }
    (void)context;
    return nullptr;
}

EFI_EXPORT const char *imgu16_grayscale_erode(Efi::EfiCallContext *, const _uint16_mat *src, const _uint16_mat *se, long se_row, long se_col, _mutil_boundary_type boundary, void *context, _uint16_mat *dst) {
    check_src_f(src);
    check_src_f(se);
    check_src_f(dst);
    int rows = src->rows;
    int cols = src->cols;
    if (rows != dst->rows || cols != dst->cols) return "array size mismatch";
    if (src->data == dst->data || se->data == dst->data) return "Illegal memory address";
    const unsigned short *in = src->data;
    const unsigned short *s = se->data;
    unsigned short *out = dst->data;
    int se_rows = se->rows;
    int se_cols = se->cols;
    double total = 0.0;
    double progress = 0.0;
    for (int r = 0; r < rows; ++r) {
        for (int c = 0; c < cols; ++c) {
            unsigned short v;
            int i, j;
            i = r - se_row;
            j = c - se_col;
            switch (boundary) {
            case MUTIL_BOUNDARY_ZERO:
                v = (i >= 0 && j >= 0 && i < rows && j < cols) ? in[i * src->cols + j] : 0;
                break;
            case MUTIL_BOUNDARY_REPLICATE:
                i = i < 0 ? 0 : (i > rows - 1 ? rows - 1 : i);
                j = j < 0 ? 0 : (j > cols - 1 ? cols - 1 : j);
                v = in[i * src->cols + j];
                break;
            default:
                return "Illegal data value";
            }
            unsigned short best = (unsigned short)(v - s[0]);
            for (int k = 0; k < se_rows; ++k) {
                for (int l = 0; l < se_cols; ++l) {
                    if (k == 0 && l == 0) continue;
                    i = r + k - se_row;
                    j = c + l - se_col;
                    switch (boundary) {
                    case MUTIL_BOUNDARY_ZERO:
                        v = (i >= 0 && j >= 0 && i < rows && j < cols) ? in[i * src->cols + j] : 0;
                        break;
                    case MUTIL_BOUNDARY_REPLICATE:
                        i = i < 0 ? 0 : (i > rows - 1 ? rows - 1 : i);
                        j = j < 0 ? 0 : (j > cols - 1 ? cols - 1 : j);
                        v = in[i * src->cols + j];
                        break;
                    default:
                        return "Illegal data value";
                    }
                    unsigned short x = (unsigned short)(v - s[k * se->cols + l]);
                    if (x < best) best = x;
                }
            }
            out[r * dst->cols + c] = best;
        }
        total += se_rows * 8.0 * se_cols * cols + 8.0;
        if (progress < total) {
            if (mutil_interrupt_step == 0.0) mutil_interrupt_step = 10000000.0;
            progress += mutil_interrupt_step;
        }
    }
    (void)context;
    return nullptr;
}

}  // namespace mutils
