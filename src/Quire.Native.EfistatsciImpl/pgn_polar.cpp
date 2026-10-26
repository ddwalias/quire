#include "mutils_mat.h"

#include <math.h>

namespace mutils {

EFI_EXPORT const char *pgndbl_rectangular_to_polar(Efi::EfiCallContext *, const _double_mat *src, double cx, double cy, void *context, _double_mat *dst) {
    check_src_f(src);
    check_dst_f(dst);
    if (src->cols != 2 || src->rows != dst->rows || dst->cols != src->cols) return "array size mismatch";
    for (int i = 0; i < src->rows; ++i) {
        double dx = src->data[i * src->cols] - cx;
        double dy = src->data[i * src->cols + 1] - cy;
        double r = sqrt(dx * dx + dy * dy);
        double angle;
        if (dx == 0.0) {
            angle = dy >= 0.0 ? 1.5707963267948966 : 4.71238898038469;
        } else {
            angle = atan(dy / dx);
            if (0.0 > dx) angle += 3.141592653589793;
            if (0.0 > angle) angle += 6.283185307179586;
        }
        dst->data[i * src->cols] = r;
        dst->data[i * src->cols + 1] = angle;
    }
    double work = src->rows * 20.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *pgnuniv_rectangular_to_polar(Efi::EfiCallContext *ctx, const _univ_mat *src, double cx, double cy, void *context, _univ_mat *dst) {
    if (src == nullptr || dst == nullptr) return "null pointer";
    if (src->type != dst->type || src->type != EFI_DOUBLE) return "Illegal data type";
    if (const char *error = pgndbl_rectangular_to_polar(ctx, (const _double_mat *)src, cx, cy, context, (_double_mat *)dst)) return error;
    return nullptr;
}

}  // namespace mutils
