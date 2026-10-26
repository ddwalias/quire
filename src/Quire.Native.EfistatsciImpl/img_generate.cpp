#include "mutils_mat.h"

#include <math.h>

namespace mutils {

EFI_EXPORT const char *imgdbl_generate_rectangle(Efi::EfiCallContext *, long row, long col, long height, long width, double fg, double bg, void *context, _double_mat *dst) {
    check_src_f(dst);
    if (height < 1 || width < 1) return "Illegal data type";
    if (fg == bg) return "Illegal data value";
    int row_end = row + height;
    if (row_end - 1 < 0 || row >= dst->rows) return "array size mismatch";
    int col_end = col + width;
    if (col_end - 1 < 0 || col >= dst->cols) return "array size mismatch";
    double total = 0.0;
    double progress = 0.0;
    matdbl_assign_scalar(bg, context, dst);
    int last_row = row_end > dst->rows ? dst->rows : row_end;
    int last_col = col_end > dst->cols ? dst->cols : col_end;
    for (int i = row > 0 ? row : 0; i < last_row; ++i) {
        for (int j = col > 0 ? col : 0; j < last_col; ++j) dst->data[i * dst->cols + j] = fg;
        total += height * 3.0 * width;
        if (progress < total) {
            if (mutil_interrupt_step == 0.0) mutil_interrupt_step = 10000000.0;
            progress += mutil_interrupt_step;
        }
    }
    return nullptr;
}

EFI_EXPORT const char *imgs32_generate_rectangle(Efi::EfiCallContext *, long row, long col, long height, long width, long fg, long bg, void *context, _sint32_mat *dst) {
    check_src_f(dst);
    if (height < 1 || width < 1) return "Illegal data type";
    if (fg == bg) return "Illegal data value";
    int row_end = row + height;
    if (row_end - 1 < 0 || row >= dst->rows) return "array size mismatch";
    int col_end = col + width;
    if (col_end - 1 < 0 || col >= dst->cols) return "array size mismatch";
    double total = 0.0;
    double progress = 0.0;
    mats32_assign_scalar(bg, context, dst);
    int last_row = row_end > dst->rows ? dst->rows : row_end;
    int last_col = col_end > dst->cols ? dst->cols : col_end;
    for (int i = row > 0 ? row : 0; i < last_row; ++i) {
        for (int j = col > 0 ? col : 0; j < last_col; ++j) dst->data[i * dst->cols + j] = fg;
        total += height * 3.0 * width;
        if (progress < total) {
            if (mutil_interrupt_step == 0.0) mutil_interrupt_step = 10000000.0;
            progress += mutil_interrupt_step;
        }
    }
    return nullptr;
}

EFI_EXPORT const char *imgu16_generate_rectangle(Efi::EfiCallContext *, long row, long col, long height, long width, unsigned short fg, unsigned short bg, void *context, _uint16_mat *dst) {
    check_src_f(dst);
    if (height < 1 || width < 1) return "Illegal data type";
    if (fg == bg) return "Illegal data value";
    int row_end = row + height;
    if (row_end - 1 < 0 || row >= dst->rows) return "array size mismatch";
    int col_end = col + width;
    if (col_end - 1 < 0 || col >= dst->cols) return "array size mismatch";
    double total = 0.0;
    double progress = 0.0;
    matu16_assign_scalar(bg, context, dst);
    int last_row = row_end > dst->rows ? dst->rows : row_end;
    int last_col = col_end > dst->cols ? dst->cols : col_end;
    for (int i = row > 0 ? row : 0; i < last_row; ++i) {
        for (int j = col > 0 ? col : 0; j < last_col; ++j) dst->data[i * dst->cols + j] = fg;
        total += height * 3.0 * width;
        if (progress < total) {
            if (mutil_interrupt_step == 0.0) mutil_interrupt_step = 10000000.0;
            progress += mutil_interrupt_step;
        }
    }
    return nullptr;
}

EFI_EXPORT const char *imgu8_generate_rectangle(Efi::EfiCallContext *, long row, long col, long height, long width, unsigned char fg, unsigned char bg, void *context, _uint8_mat *dst) {
    check_src_f(dst);
    if (height < 1 || width < 1) return "Illegal data type";
    if (fg == bg) return "Illegal data value";
    int row_end = row + height;
    if (row_end - 1 < 0 || row >= dst->rows) return "array size mismatch";
    int col_end = col + width;
    if (col_end - 1 < 0 || col >= dst->cols) return "array size mismatch";
    double total = 0.0;
    double progress = 0.0;
    matu8_assign_scalar(bg, context, dst);
    int last_row = row_end > dst->rows ? dst->rows : row_end;
    int last_col = col_end > dst->cols ? dst->cols : col_end;
    for (int i = row > 0 ? row : 0; i < last_row; ++i) {
        for (int j = col > 0 ? col : 0; j < last_col; ++j) dst->data[i * dst->cols + j] = fg;
        total += height * 3.0 * width;
        if (progress < total) {
            if (mutil_interrupt_step == 0.0) mutil_interrupt_step = 10000000.0;
            progress += mutil_interrupt_step;
        }
    }
    return nullptr;
}

EFI_EXPORT const char *imguniv_generate_rectangle(Efi::EfiCallContext *ctx, long row, long col, long height, long width, _univ_scalar fg, _univ_scalar bg, void *context, _univ_mat *dst) {
    if (dst == nullptr) return "null pointer";
    // The original tests the by-value scalars' hidden pointers.
    if (&fg == nullptr || dst->type != fg.type || &bg == nullptr || dst->type != bg.type) return "Illegal data type";
    switch (dst->type) {
    case EFI_DOUBLE: {
        if (const char *error = imgdbl_generate_rectangle(ctx, row, col, height, width, fg.dbl, bg.dbl, context, (_double_mat *)dst)) return error;
    } break;
    case EFI_SINT32: {
        if (const char *error = imgs32_generate_rectangle(ctx, row, col, height, width, fg.s32, bg.s32, context, (_sint32_mat *)dst)) return error;
    } break;
    case EFI_UINT16: {
        if (const char *error = imgu16_generate_rectangle(ctx, row, col, height, width, fg.u16, bg.u16, context, (_uint16_mat *)dst)) return error;
    } break;
    case EFI_UINT8: {
        if (const char *error = imgu8_generate_rectangle(ctx, row, col, height, width, fg.u8, bg.u8, context, (_uint8_mat *)dst)) return error;
    } break;
    default: return "Illegal data type";
    }
    return nullptr;
}

}  // namespace mutils
