#include "mutils_mat.h"

namespace mutils {

EFI_EXPORT const char *imgs32_binary_dilate(Efi::EfiCallContext *, const _sint32_mat *src, const _uint8_mat *se, long se_row, long se_col, long fg, long bg, void *context, _sint32_mat *dst) {
    check_src_f(src);
    check_src_f(se);
    check_dst_f(dst);
    int rows = src->rows;
    int cols = src->cols;
    if (rows != dst->rows || cols != dst->cols) return "array size mismatch";
    if (src->data == dst->data || (const void *)se->data == (const void *)dst->data) return "Illegal memory address";
    if (fg == bg) return "Illegal data value";
    int se_rows = se->rows;
    int se_cols = se->cols;
    double total = 0.0;
    double progress = 0.0;
    double work = ((se_cols * 3 + 3.0) * se_rows + 2.0) * cols;
    const long *in = src->data;
    mats32_assign_scalar(bg, context, dst);
    for (int i = 0; i < rows; ++i) {
        for (int j = 0; j < cols; ++j) {
            if (in[i * src->cols + j] != fg) continue;
            for (int k = 0, r = i - se_row; k < se_rows; ++k, ++r) {
                if (r < 0 || r >= rows) continue;
                const unsigned char *s = se->data + k * se->cols;
                long *d = dst->data + r * dst->cols;
                for (int l = 0, c = j - se_col; l < se_cols; ++l, ++c) {
                    if (c < 0 || c >= cols) continue;
                    if (s[l] != 0) d[c] = fg;
                }
            }
        }
        total += work;
        if (progress < total) {
            if (mutil_interrupt_step == 0.0) mutil_interrupt_step = 10000000.0;
            progress += mutil_interrupt_step;
        }
    }
    return nullptr;
}

EFI_EXPORT const char *imgu16_binary_dilate(Efi::EfiCallContext *, const _uint16_mat *src, const _uint8_mat *se, long se_row, long se_col, unsigned short fg, unsigned short bg, void *context, _uint16_mat *dst) {
    check_src_f(src);
    check_src_f(se);
    check_dst_f(dst);
    int rows = src->rows;
    int cols = src->cols;
    if (rows != dst->rows || cols != dst->cols) return "array size mismatch";
    if (src->data == dst->data || (const void *)se->data == (const void *)dst->data) return "Illegal memory address";
    if (fg == bg) return "Illegal data value";
    int se_rows = se->rows;
    int se_cols = se->cols;
    double total = 0.0;
    double progress = 0.0;
    double work = ((se_cols * 3 + 3.0) * se_rows + 2.0) * cols;
    const unsigned short *in = src->data;
    matu16_assign_scalar(bg, context, dst);
    for (int i = 0; i < rows; ++i) {
        for (int j = 0; j < cols; ++j) {
            if (in[i * src->cols + j] != fg) continue;
            for (int k = 0, r = i - se_row; k < se_rows; ++k, ++r) {
                if (r < 0 || r >= rows) continue;
                const unsigned char *s = se->data + k * se->cols;
                unsigned short *d = dst->data + r * dst->cols;
                for (int l = 0, c = j - se_col; l < se_cols; ++l, ++c) {
                    if (c < 0 || c >= cols) continue;
                    if (s[l] != 0) d[c] = fg;
                }
            }
        }
        total += work;
        if (progress < total) {
            if (mutil_interrupt_step == 0.0) mutil_interrupt_step = 10000000.0;
            progress += mutil_interrupt_step;
        }
    }
    return nullptr;
}

EFI_EXPORT const char *imgu8_binary_dilate(Efi::EfiCallContext *, const _uint8_mat *src, const _uint8_mat *se, long se_row, long se_col, unsigned char fg, unsigned char bg, void *context, _uint8_mat *dst) {
    check_src_f(src);
    check_src_f(se);
    check_dst_f(dst);
    // The original keeps rows as a 64-bit loop bound.
    __int64 rows = src->rows;
    int cols = src->cols;
    if (rows != dst->rows || cols != dst->cols) return "array size mismatch";
    // The original captures dst->data before the alias checks.
    auto *destination = dst->data;
    if (src->data == destination || (const void *)se->data == (const void *)destination) return "Illegal memory address";
    if (fg == bg) return "Illegal data value";
    int se_rows = se->rows;
    int se_cols = se->cols;
    // The original initializes progress before the row-work total.
    double progress = 0.0;
    double total = 0.0;
    double work = ((se_cols * 3 + 3.0) * se_rows + 2.0) * cols;
    const unsigned char *in = src->data;
    matu8_assign_scalar(bg, context, dst);
    for (int i = 0; i < rows; ++i) {
        for (int j = 0; j < cols; ++j) {
            if (in[i * src->cols + j] != fg) continue;
            for (int k = 0, r = i - se_row; k < se_rows; ++k, ++r) {
                if (r < 0 || r >= rows) continue;
                const unsigned char *s = se->data + k * se->cols;
                unsigned char *d = dst->data + r * dst->cols;
                for (int l = 0, c = j - se_col; l < se_cols; ++l, ++c) {
                    if (c < 0 || c >= cols) continue;
                    if (s[l] != 0) d[c] = fg;
                }
            }
        }
        total += work;
        if (progress < total) {
            if (mutil_interrupt_step == 0.0) mutil_interrupt_step = 10000000.0;
            progress += mutil_interrupt_step;
        }
    }
    return nullptr;
}

}  // namespace mutils
