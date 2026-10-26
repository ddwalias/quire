#include "mutils_mat.h"

#include <math.h>

namespace mutils {

EFI_EXPORT const char *imgdbl_moment(Efi::EfiCallContext *, const _double_mat *img, long p, long q, void *context, double *out) {
    check_src_f(img);
    int rows = img->rows;
    int cols = img->cols;
    const double *data = img->data;
    if (out == nullptr) return "null pointer";
    if (p < 0 || q < 0) return "Illegal data value";
    double total = 0.0;
    double progress = 0.0;
    *out = 0.0;
    for (int i = 0; i < rows; ++i) {
        for (int j = 0; j < cols; ++j) {
            double v = data[i * img->cols + j];
            if (p != 0) v = pow((double)i, (double)p) * v;
            if (q != 0) v = pow((double)j, (double)q) * v;
            *out += v;
        }
        total += cols * 10.0;
        if (progress < total) {
            if (mutil_interrupt_step == 0.0) mutil_interrupt_step = 10000000.0;
            progress += mutil_interrupt_step;
        }
    }
    (void)context;
    return nullptr;
}

EFI_EXPORT const char *imguniv_moment(Efi::EfiCallContext *ctx, const _univ_mat *img, long p, long q, void *context, double *out) {
    if (img == nullptr) return "null pointer";
    if (img->type != EFI_DOUBLE) return "Illegal data type";
    if (const char *error = imgdbl_moment(ctx, (const _double_mat *)img, p, q, context, out)) return error;
    return nullptr;
}

}  // namespace mutils
