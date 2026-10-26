#include "mutils_mat.h"

#include <math.h>

namespace mutils {

// Where a point lies against a polygon: 0 inside, 1 outside, 2 on the boundary.
enum _pgn_point_contain_type { PGN_POINT_INSIDE, PGN_POINT_OUTSIDE, PGN_POINT_BOUNDARY };

EFI_EXPORT const char *pgndbl_point_test(Efi::EfiCallContext *, const _double_mat *poly, double x, double y, void *context, _pgn_point_contain_type *contain, long *winding) {
    if (contain == nullptr || winding == nullptr) return "null pointer";
    check_src_f(poly);
    int n = poly->rows;
    if (poly->cols != 2 || n < 3) return "array size mismatch";
    double px = poly->data[(n - 1) * poly->cols];
    double py = poly->data[(n - 1) * poly->cols + 1];
    bool above = py >= y;
    *winding = 0;
    *contain = PGN_POINT_OUTSIDE;
    for (int i = 0; i < n; ++i) {
        double cx = poly->data[i * poly->cols];
        double cy = poly->data[i * poly->cols + 1];
        bool c_above = cy >= y;
        if (above != c_above) {
            double lhs = (px - cx) * (cy - y);
            double rhs = (py - cy) * (cx - x);
            if (lhs == rhs) {
                *contain = PGN_POINT_BOUNDARY;
                break;
            }
            if ((lhs > rhs) == (cy >= y)) {
                if (cy > py) ++*winding;
                else --*winding;
            }
        } else if (above && y == py && y == cy) {
            if ((x >= px && cx >= x) || (px >= x && x >= cx)) {
                *contain = PGN_POINT_BOUNDARY;
                break;
            }
        }
        above = c_above;
        px = cx;
        py = cy;
    }
    double work = poly->rows * 20.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    if (*contain != PGN_POINT_BOUNDARY) {
        if (*winding & 1) *contain = PGN_POINT_INSIDE;
        else *contain = PGN_POINT_OUTSIDE;
        return nullptr;
    }
    *winding = 0;
    return nullptr;
}

EFI_EXPORT const char *pgnuniv_point_test(Efi::EfiCallContext *ctx, const _univ_mat *poly, double x, double y, void *context, _pgn_point_contain_type *contain, long *winding) {
    if (poly == nullptr) return "null pointer";
    if (poly->type != EFI_DOUBLE) return "Illegal data type";
    if (const char *error = pgndbl_point_test(ctx, (const _double_mat *)poly, x, y, context, contain, winding)) return error;
    return nullptr;
}

EFI_EXPORT const char *pgndbl_shape_features(Efi::EfiCallContext *, const _double_mat *poly, void *context, double *cx, double *cy, double *area, double *perimeter, double *xmin, double *xmax, double *ymin, double *ymax) {
    check_src_f(poly);
    int n = poly->rows;
    if (poly->cols != 2 || n < 3) return "array size mismatch";
    if (cx == nullptr || cy == nullptr || area == nullptr || perimeter == nullptr || xmin == nullptr || xmax == nullptr || ymin == nullptr || ymax == nullptr) return "null pointer";
    double x = poly->data[0];
    double y = poly->data[1];
    double px = poly->data[(n - 1) * poly->cols];
    double py = poly->data[(n - 1) * poly->cols + 1];
    *xmin = x;
    *xmax = x;
    *ymin = y;
    *ymax = y;
    double c = px * y - py * x;
    double sum = c;
    double sx = (px + x) * c;
    double sy = (py + y) * c;
    *perimeter = sqrt((x - px) * (x - px) + (y - py) * (y - py));
    for (int i = 1; i < poly->rows; ++i) {
        px = x;
        py = y;
        x = poly->data[i * poly->cols];
        y = poly->data[i * poly->cols + 1];
        if (x > *xmax) *xmax = x;
        if (y > *ymax) *ymax = y;
        if (*xmin > x) *xmin = x;
        if (*ymin > y) *ymin = y;
        c = px * y - py * x;
        sy += (py + y) * c;
        sum += c;
        sx += (px + x) * c;
        *perimeter += sqrt((y - py) * (y - py) + (x - px) * (x - px));
    }
    double work = poly->rows * 50.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    *area = sum * 0.5;
    if (*area != 0.0) {
        sum *= 3.0;
        *cx = sx / sum;
        *cy = sy / sum;
    }
    return nullptr;
}

EFI_EXPORT const char *pgnuniv_shape_features(Efi::EfiCallContext *ctx, const _univ_mat *poly, void *context, double *cx, double *cy, double *area, double *perimeter, double *xmin, double *xmax, double *ymin, double *ymax) {
    if (poly == nullptr) return "null pointer";
    if (poly->type != EFI_DOUBLE) return "Illegal data type";
    if (const char *error = pgndbl_shape_features(ctx, (const _double_mat *)poly, context, cx, cy, area, perimeter, xmin, xmax, ymin, ymax)) return error;
    return nullptr;
}

}  // namespace mutils
