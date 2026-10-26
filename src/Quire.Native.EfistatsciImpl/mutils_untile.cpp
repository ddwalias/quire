#include "mutils_mat.h"

namespace mutils {

EFI_EXPORT const char *matdbl_untile(Efi::EfiCallContext *, const _double_mat *src, long trows, long tcols, void *context, _double_mat *dst) {
    check_src_f(src);
    check_dst_f(dst);
    if (src->data == dst->data) return "Illegal memory address";
    if (trows <= 0 || tcols <= 0 || trows > dst->rows || tcols > dst->cols) return "Illegal data value";
    int across = dst->cols / tcols;
    if (src->rows != dst->rows / trows * across) return "array size mismatch";
    int tile_size = trows * tcols;
    if (src->cols != tile_size) return "array size mismatch";
    double total = 0.0;
    double progress = 0.0;
    for (int i = 0; i < dst->rows; ++i) {
        for (int j = 0; j < dst->cols; ++j) {
            dst->data[i * dst->cols + j] = src->data[(i / trows * across + j / tcols) * tile_size + i % trows * tcols + j % tcols];
        }
        total += dst->cols * 15.0;
        if (progress < total) {
            if (mutil_interrupt_step == 0.0) mutil_interrupt_step = 10000000.0;
            progress += mutil_interrupt_step;
        }
    }
    (void)context;
    return nullptr;
}

EFI_EXPORT const char *matflt_untile(Efi::EfiCallContext *, const _float_mat *src, long trows, long tcols, void *context, _float_mat *dst) {
    check_src_f(src);
    check_dst_f(dst);
    if (src->data == dst->data) return "Illegal memory address";
    if (trows <= 0 || tcols <= 0 || trows > dst->rows || tcols > dst->cols) return "Illegal data value";
    int across = dst->cols / tcols;
    if (src->rows != dst->rows / trows * across) return "array size mismatch";
    int tile_size = trows * tcols;
    if (src->cols != tile_size) return "array size mismatch";
    double total = 0.0;
    double progress = 0.0;
    for (int i = 0; i < dst->rows; ++i) {
        for (int j = 0; j < dst->cols; ++j) {
            dst->data[i * dst->cols + j] = src->data[(i / trows * across + j / tcols) * tile_size + i % trows * tcols + j % tcols];
        }
        total += dst->cols * 15.0;
        if (progress < total) {
            if (mutil_interrupt_step == 0.0) mutil_interrupt_step = 10000000.0;
            progress += mutil_interrupt_step;
        }
    }
    (void)context;
    return nullptr;
}

EFI_EXPORT const char *mats32_untile(Efi::EfiCallContext *, const _sint32_mat *src, long trows, long tcols, void *context, _sint32_mat *dst) {
    check_src_f(src);
    check_dst_f(dst);
    if (src->data == dst->data) return "Illegal memory address";
    if (trows <= 0 || tcols <= 0 || trows > dst->rows || tcols > dst->cols) return "Illegal data value";
    int across = dst->cols / tcols;
    if (src->rows != dst->rows / trows * across) return "array size mismatch";
    int tile_size = trows * tcols;
    if (src->cols != tile_size) return "array size mismatch";
    double total = 0.0;
    double progress = 0.0;
    for (int i = 0; i < dst->rows; ++i) {
        for (int j = 0; j < dst->cols; ++j) {
            dst->data[i * dst->cols + j] = src->data[(i / trows * across + j / tcols) * tile_size + i % trows * tcols + j % tcols];
        }
        total += dst->cols * 15.0;
        if (progress < total) {
            if (mutil_interrupt_step == 0.0) mutil_interrupt_step = 10000000.0;
            progress += mutil_interrupt_step;
        }
    }
    (void)context;
    return nullptr;
}

EFI_EXPORT const char *mats16_untile(Efi::EfiCallContext *, const _sint16_mat *src, long trows, long tcols, void *context, _sint16_mat *dst) {
    check_src_f(src);
    check_dst_f(dst);
    if (src->data == dst->data) return "Illegal memory address";
    if (trows <= 0 || tcols <= 0 || trows > dst->rows || tcols > dst->cols) return "Illegal data value";
    int across = dst->cols / tcols;
    if (src->rows != dst->rows / trows * across) return "array size mismatch";
    int tile_size = trows * tcols;
    if (src->cols != tile_size) return "array size mismatch";
    double total = 0.0;
    double progress = 0.0;
    for (int i = 0; i < dst->rows; ++i) {
        for (int j = 0; j < dst->cols; ++j) {
            dst->data[i * dst->cols + j] = src->data[(i / trows * across + j / tcols) * tile_size + i % trows * tcols + j % tcols];
        }
        total += dst->cols * 15.0;
        if (progress < total) {
            if (mutil_interrupt_step == 0.0) mutil_interrupt_step = 10000000.0;
            progress += mutil_interrupt_step;
        }
    }
    (void)context;
    return nullptr;
}

EFI_EXPORT const char *matu32_untile(Efi::EfiCallContext *, const _uint32_mat *src, long trows, long tcols, void *context, _uint32_mat *dst) {
    check_src_f(src);
    check_dst_f(dst);
    if (src->data == dst->data) return "Illegal memory address";
    if (trows <= 0 || tcols <= 0 || trows > dst->rows || tcols > dst->cols) return "Illegal data value";
    int across = dst->cols / tcols;
    if (src->rows != dst->rows / trows * across) return "array size mismatch";
    int tile_size = trows * tcols;
    if (src->cols != tile_size) return "array size mismatch";
    double total = 0.0;
    double progress = 0.0;
    for (int i = 0; i < dst->rows; ++i) {
        for (int j = 0; j < dst->cols; ++j) {
            dst->data[i * dst->cols + j] = src->data[(i / trows * across + j / tcols) * tile_size + i % trows * tcols + j % tcols];
        }
        total += dst->cols * 15.0;
        if (progress < total) {
            if (mutil_interrupt_step == 0.0) mutil_interrupt_step = 10000000.0;
            progress += mutil_interrupt_step;
        }
    }
    (void)context;
    return nullptr;
}

EFI_EXPORT const char *matu16_untile(Efi::EfiCallContext *, const _uint16_mat *src, long trows, long tcols, void *context, _uint16_mat *dst) {
    check_src_f(src);
    check_dst_f(dst);
    if (src->data == dst->data) return "Illegal memory address";
    if (trows <= 0 || tcols <= 0 || trows > dst->rows || tcols > dst->cols) return "Illegal data value";
    int across = dst->cols / tcols;
    if (src->rows != dst->rows / trows * across) return "array size mismatch";
    int tile_size = trows * tcols;
    if (src->cols != tile_size) return "array size mismatch";
    double total = 0.0;
    double progress = 0.0;
    for (int i = 0; i < dst->rows; ++i) {
        for (int j = 0; j < dst->cols; ++j) {
            dst->data[i * dst->cols + j] = src->data[(i / trows * across + j / tcols) * tile_size + i % trows * tcols + j % tcols];
        }
        total += dst->cols * 15.0;
        if (progress < total) {
            if (mutil_interrupt_step == 0.0) mutil_interrupt_step = 10000000.0;
            progress += mutil_interrupt_step;
        }
    }
    (void)context;
    return nullptr;
}

EFI_EXPORT const char *matu8_untile(Efi::EfiCallContext *, const _uint8_mat *src, long trows, long tcols, void *context, _uint8_mat *dst) {
    check_src_f(src);
    check_dst_f(dst);
    if (src->data == dst->data) return "Illegal memory address";
    if (trows <= 0 || tcols <= 0 || trows > dst->rows || tcols > dst->cols) return "Illegal data value";
    int across = dst->cols / tcols;
    if (src->rows != dst->rows / trows * across) return "array size mismatch";
    int tile_size = trows * tcols;
    if (src->cols != tile_size) return "array size mismatch";
    double total = 0.0;
    double progress = 0.0;
    for (int i = 0; i < dst->rows; ++i) {
        for (int j = 0; j < dst->cols; ++j) {
            dst->data[i * dst->cols + j] = src->data[(i / trows * across + j / tcols) * tile_size + i % trows * tcols + j % tcols];
        }
        total += dst->cols * 15.0;
        if (progress < total) {
            if (mutil_interrupt_step == 0.0) mutil_interrupt_step = 10000000.0;
            progress += mutil_interrupt_step;
        }
    }
    (void)context;
    return nullptr;
}

EFI_EXPORT const char *matcpx_untile(Efi::EfiCallContext *, const _dcomplex_mat *src, long trows, long tcols, void *context, _dcomplex_mat *dst) {
    check_src_f(src);
    check_dst_f(dst);
    if (src->data == dst->data) return "Illegal memory address";
    if (trows <= 0 || tcols <= 0 || trows > dst->rows || tcols > dst->cols) return "Illegal data value";
    int across = dst->cols / tcols;
    if (src->rows != dst->rows / trows * across) return "array size mismatch";
    int tile_size = trows * tcols;
    if (src->cols != tile_size) return "array size mismatch";
    double total = 0.0;
    double progress = 0.0;
    for (int i = 0; i < dst->rows; ++i) {
        for (int j = 0; j < dst->cols; ++j) {
            dst->data[i * dst->cols + j] = src->data[(i / trows * across + j / tcols) * tile_size + i % trows * tcols + j % tcols];
        }
        total += dst->cols * 15.0;
        if (progress < total) {
            if (mutil_interrupt_step == 0.0) mutil_interrupt_step = 10000000.0;
            progress += mutil_interrupt_step;
        }
    }
    (void)context;
    return nullptr;
}

}  // namespace mutils
