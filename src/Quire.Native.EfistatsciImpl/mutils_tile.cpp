#include "mutils_mat.h"

namespace mutils {

EFI_EXPORT const char *matdbl_tile(Efi::EfiCallContext *, const _double_mat *src, long trows, long tcols, void *context, _double_mat *dst) {
    check_src_f(src);
    check_dst_f(dst);
    if (src->data == dst->data) return "Illegal memory address";
    if (trows <= 0 || tcols <= 0 || trows > src->rows || tcols > src->cols) return "Illegal data value";
    int across = src->cols / tcols;
    int tiles = src->rows / trows * across;
    int tile_size = trows * tcols;
    if (tiles != dst->rows || tile_size != dst->cols) return "array size mismatch";
    double total = 0.0;
    double progress = 0.0;
    for (int t = 0; t < tiles; ++t) {
        for (int k = 0; k < tile_size; ++k) {
            dst->data[t * dst->cols + k] = src->data[(t / across * trows + k / tcols) * src->cols + t % across * tcols + k % tcols];
        }
        total += tile_size * 15.0;
        if (progress < total) {
            if (mutil_interrupt_step == 0.0) mutil_interrupt_step = 10000000.0;
            progress += mutil_interrupt_step;
        }
    }
    (void)context;
    return nullptr;
}

EFI_EXPORT const char *matflt_tile(Efi::EfiCallContext *, const _float_mat *src, long trows, long tcols, void *context, _float_mat *dst) {
    check_src_f(src);
    check_dst_f(dst);
    if (src->data == dst->data) return "Illegal memory address";
    if (trows <= 0 || tcols <= 0 || trows > src->rows || tcols > src->cols) return "Illegal data value";
    int across = src->cols / tcols;
    int tiles = src->rows / trows * across;
    int tile_size = trows * tcols;
    if (tiles != dst->rows || tile_size != dst->cols) return "array size mismatch";
    double total = 0.0;
    double progress = 0.0;
    for (int t = 0; t < tiles; ++t) {
        for (int k = 0; k < tile_size; ++k) {
            dst->data[t * dst->cols + k] = src->data[(t / across * trows + k / tcols) * src->cols + t % across * tcols + k % tcols];
        }
        total += tile_size * 15.0;
        if (progress < total) {
            if (mutil_interrupt_step == 0.0) mutil_interrupt_step = 10000000.0;
            progress += mutil_interrupt_step;
        }
    }
    (void)context;
    return nullptr;
}

EFI_EXPORT const char *mats32_tile(Efi::EfiCallContext *, const _sint32_mat *src, long trows, long tcols, void *context, _sint32_mat *dst) {
    check_src_f(src);
    check_dst_f(dst);
    if (src->data == dst->data) return "Illegal memory address";
    if (trows <= 0 || tcols <= 0 || trows > src->rows || tcols > src->cols) return "Illegal data value";
    int across = src->cols / tcols;
    int tiles = src->rows / trows * across;
    int tile_size = trows * tcols;
    if (tiles != dst->rows || tile_size != dst->cols) return "array size mismatch";
    double total = 0.0;
    double progress = 0.0;
    for (int t = 0; t < tiles; ++t) {
        for (int k = 0; k < tile_size; ++k) {
            dst->data[t * dst->cols + k] = src->data[(t / across * trows + k / tcols) * src->cols + t % across * tcols + k % tcols];
        }
        total += tile_size * 15.0;
        if (progress < total) {
            if (mutil_interrupt_step == 0.0) mutil_interrupt_step = 10000000.0;
            progress += mutil_interrupt_step;
        }
    }
    (void)context;
    return nullptr;
}

EFI_EXPORT const char *mats16_tile(Efi::EfiCallContext *, const _sint16_mat *src, long trows, long tcols, void *context, _sint16_mat *dst) {
    check_src_f(src);
    check_dst_f(dst);
    if (src->data == dst->data) return "Illegal memory address";
    if (trows <= 0 || tcols <= 0 || trows > src->rows || tcols > src->cols) return "Illegal data value";
    int across = src->cols / tcols;
    int tiles = src->rows / trows * across;
    int tile_size = trows * tcols;
    if (tiles != dst->rows || tile_size != dst->cols) return "array size mismatch";
    double total = 0.0;
    double progress = 0.0;
    for (int t = 0; t < tiles; ++t) {
        for (int k = 0; k < tile_size; ++k) {
            dst->data[t * dst->cols + k] = src->data[(t / across * trows + k / tcols) * src->cols + t % across * tcols + k % tcols];
        }
        total += tile_size * 15.0;
        if (progress < total) {
            if (mutil_interrupt_step == 0.0) mutil_interrupt_step = 10000000.0;
            progress += mutil_interrupt_step;
        }
    }
    (void)context;
    return nullptr;
}

EFI_EXPORT const char *matu32_tile(Efi::EfiCallContext *, const _uint32_mat *src, long trows, long tcols, void *context, _uint32_mat *dst) {
    check_src_f(src);
    check_dst_f(dst);
    if (src->data == dst->data) return "Illegal memory address";
    if (trows <= 0 || tcols <= 0 || trows > src->rows || tcols > src->cols) return "Illegal data value";
    int across = src->cols / tcols;
    int tiles = src->rows / trows * across;
    int tile_size = trows * tcols;
    if (tiles != dst->rows || tile_size != dst->cols) return "array size mismatch";
    double total = 0.0;
    double progress = 0.0;
    for (int t = 0; t < tiles; ++t) {
        for (int k = 0; k < tile_size; ++k) {
            dst->data[t * dst->cols + k] = src->data[(t / across * trows + k / tcols) * src->cols + t % across * tcols + k % tcols];
        }
        total += tile_size * 15.0;
        if (progress < total) {
            if (mutil_interrupt_step == 0.0) mutil_interrupt_step = 10000000.0;
            progress += mutil_interrupt_step;
        }
    }
    (void)context;
    return nullptr;
}

EFI_EXPORT const char *matu16_tile(Efi::EfiCallContext *, const _uint16_mat *src, long trows, long tcols, void *context, _uint16_mat *dst) {
    check_src_f(src);
    check_dst_f(dst);
    if (src->data == dst->data) return "Illegal memory address";
    if (trows <= 0 || tcols <= 0 || trows > src->rows || tcols > src->cols) return "Illegal data value";
    int across = src->cols / tcols;
    int tiles = src->rows / trows * across;
    int tile_size = trows * tcols;
    if (tiles != dst->rows || tile_size != dst->cols) return "array size mismatch";
    double total = 0.0;
    double progress = 0.0;
    for (int t = 0; t < tiles; ++t) {
        for (int k = 0; k < tile_size; ++k) {
            dst->data[t * dst->cols + k] = src->data[(t / across * trows + k / tcols) * src->cols + t % across * tcols + k % tcols];
        }
        total += tile_size * 15.0;
        if (progress < total) {
            if (mutil_interrupt_step == 0.0) mutil_interrupt_step = 10000000.0;
            progress += mutil_interrupt_step;
        }
    }
    (void)context;
    return nullptr;
}

EFI_EXPORT const char *matu8_tile(Efi::EfiCallContext *, const _uint8_mat *src, long trows, long tcols, void *context, _uint8_mat *dst) {
    check_src_f(src);
    check_dst_f(dst);
    if (src->data == dst->data) return "Illegal memory address";
    if (trows <= 0 || tcols <= 0 || trows > src->rows || tcols > src->cols) return "Illegal data value";
    int across = src->cols / tcols;
    int tiles = src->rows / trows * across;
    int tile_size = trows * tcols;
    if (tiles != dst->rows || tile_size != dst->cols) return "array size mismatch";
    double total = 0.0;
    double progress = 0.0;
    for (int t = 0; t < tiles; ++t) {
        for (int k = 0; k < tile_size; ++k) {
            dst->data[t * dst->cols + k] = src->data[(t / across * trows + k / tcols) * src->cols + t % across * tcols + k % tcols];
        }
        total += tile_size * 15.0;
        if (progress < total) {
            if (mutil_interrupt_step == 0.0) mutil_interrupt_step = 10000000.0;
            progress += mutil_interrupt_step;
        }
    }
    (void)context;
    return nullptr;
}

EFI_EXPORT const char *matcpx_tile(Efi::EfiCallContext *, const _dcomplex_mat *src, long trows, long tcols, void *context, _dcomplex_mat *dst) {
    check_src_f(src);
    check_dst_f(dst);
    if (src->data == dst->data) return "Illegal memory address";
    if (trows <= 0 || tcols <= 0 || trows > src->rows || tcols > src->cols) return "Illegal data value";
    int across = src->cols / tcols;
    int tiles = src->rows / trows * across;
    int tile_size = trows * tcols;
    if (tiles != dst->rows || tile_size != dst->cols) return "array size mismatch";
    double total = 0.0;
    double progress = 0.0;
    for (int t = 0; t < tiles; ++t) {
        for (int k = 0; k < tile_size; ++k) {
            dst->data[t * dst->cols + k] = src->data[(t / across * trows + k / tcols) * src->cols + t % across * tcols + k % tcols];
        }
        total += tile_size * 15.0;
        if (progress < total) {
            if (mutil_interrupt_step == 0.0) mutil_interrupt_step = 10000000.0;
            progress += mutil_interrupt_step;
        }
    }
    (void)context;
    return nullptr;
}

}  // namespace mutils
