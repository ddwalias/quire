#include "mutils_mat.h"

#include <math.h>

namespace mutils {

EFI_EXPORT const char *matdbl_histogram(Efi::EfiCallContext *, const _double_mat *src, double lo, double hi, unsigned char include_lo, void *context, _uint32_mat *hist) {
    check_src_f(src);
    check_src_f(hist);
    if (hist->cols != 1) return "array size mismatch";
    if (lo >= hi) return "Illegal data value";
    matu32_assign_scalar(0, context, hist);
    int bins = hist->rows;
    int size = src->size;
    double scale = bins / (hi - lo);
    for (int i = 0; i < size; ++i) {
        double v = src->data[i];
        int bin;
        if (v == lo) {
            if (!include_lo) continue;
            bin = 0;
        } else {
            if (lo > v || v > hi) continue;
            bin = (int)ceil((v - lo) * scale) - 1;
            if (bin < 0) continue;
        }
        if (bin < bins) ++hist->data[bin];
    }
    double work = (double)size * 4.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *mats32_histogram(Efi::EfiCallContext *, const _sint32_mat *src, double lo, double hi, unsigned char include_lo, void *context, _uint32_mat *hist) {
    check_src_f(src);
    check_src_f(hist);
    if (hist->cols != 1) return "array size mismatch";
    if (lo >= hi) return "Illegal data value";
    matu32_assign_scalar(0, context, hist);
    int bins = hist->rows;
    int size = src->size;
    double scale = bins / (hi - lo);
    for (int i = 0; i < size; ++i) {
        double v = src->data[i];
        int bin;
        if (v == lo) {
            if (!include_lo) continue;
            bin = 0;
        } else {
            if (lo > v || v > hi) continue;
            bin = (int)ceil((v - lo) * scale) - 1;
            if (bin < 0) continue;
        }
        if (bin < bins) ++hist->data[bin];
    }
    double work = (double)size * 4.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *matu16_histogram(Efi::EfiCallContext *, const _uint16_mat *src, double lo, double hi, unsigned char include_lo, void *context, _uint32_mat *hist) {
    check_src_f(src);
    check_src_f(hist);
    if (hist->cols != 1) return "array size mismatch";
    if (lo >= hi) return "Illegal data value";
    matu32_assign_scalar(0, context, hist);
    int bins = hist->rows;
    int size = src->size;
    double scale = bins / (hi - lo);
    for (int i = 0; i < size; ++i) {
        double v = src->data[i];
        int bin;
        if (v == lo) {
            if (!include_lo) continue;
            bin = 0;
        } else {
            if (lo > v || v > hi) continue;
            bin = (int)ceil((v - lo) * scale) - 1;
            if (bin < 0) continue;
        }
        if (bin < bins) ++hist->data[bin];
    }
    double work = (double)size * 4.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *matu8_histogram(Efi::EfiCallContext *, const _uint8_mat *src, double lo, double hi, unsigned char include_lo, void *context, _uint32_mat *hist) {
    check_src_f(src);
    check_src_f(hist);
    if (hist->cols != 1) return "array size mismatch";
    if (lo >= hi) return "Illegal data value";
    matu32_assign_scalar(0, context, hist);
    int bins = hist->rows;
    int size = src->size;
    double scale = bins / (hi - lo);
    for (int i = 0; i < size; ++i) {
        double v = src->data[i];
        int bin;
        if (v == lo) {
            if (!include_lo) continue;
            bin = 0;
        } else {
            if (lo > v || v > hi) continue;
            bin = (int)ceil((v - lo) * scale) - 1;
            if (bin < 0) continue;
        }
        if (bin < bins) ++hist->data[bin];
    }
    double work = (double)size * 4.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

}  // namespace mutils
