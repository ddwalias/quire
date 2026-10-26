#include "mutils_mat.h"

#include <math.h>

namespace mutils {

EFI_EXPORT const char *sigdbl_window_rectangle(void *context, _double_mat *w) {
    validate(w);
    if (w->cols != 1 && w->rows != 1) return "array size mismatch";
    for (int i = 0; i < w->size; ++i) w->data[i] = 1.0;
    double work = 0.0;
    work += w->size * 5;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *siguniv_window_rectangle(void *context, _univ_mat *w) {
    if (w == nullptr) return "null pointer";
    if (w->type != EFI_DOUBLE) return "Illegal data type";
    if (const char *error = sigdbl_window_rectangle(context, (_double_mat *)w)) return error;
    return nullptr;
}

EFI_EXPORT const char *sigdbl_window_hamming(void *context, _double_mat *w) {
    validate(w);
    if (w->cols != 1 && w->rows != 1) return "array size mismatch";
    double step = 6.283185307179586 / (w->size - 1);
    for (int i = 0; i < w->size; ++i) w->data[i] = 0.54 - 0.46 * cos(i * step);
    double work = 0.0;
    work += w->size * 50;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *siguniv_window_hamming(void *context, _univ_mat *w) {
    if (w == nullptr) return "null pointer";
    if (w->type != EFI_DOUBLE) return "Illegal data type";
    if (const char *error = sigdbl_window_hamming(context, (_double_mat *)w)) return error;
    return nullptr;
}

EFI_EXPORT const char *sigdbl_window_hanning(void *context, _double_mat *w) {
    validate(w);
    if (w->cols != 1 && w->rows != 1) return "array size mismatch";
    double step = 6.283185307179586 / (w->size + 1);
    for (int i = 0; i < w->size; ++i) w->data[i] = (1.0 - cos((i + 1) * step)) * 0.5;
    double work = 0.0;
    work += w->size * 50;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *siguniv_window_hanning(void *context, _univ_mat *w) {
    if (w == nullptr) return "null pointer";
    if (w->type != EFI_DOUBLE) return "Illegal data type";
    if (const char *error = sigdbl_window_hanning(context, (_double_mat *)w)) return error;
    return nullptr;
}

EFI_EXPORT const char *sigdbl_window_gaussian(double alpha, void *context, _double_mat *w) {
    validate(w);
    if (w->cols != 1 && w->rows != 1) return "array size mismatch";
    if (0.0 >= alpha) return "Illegal data value";
    int size = w->size;
    int half = size / 2;
    for (int i = 0; i < half; ++i) {
        double x = (0.5 - i / (double)(size - 1)) * (2.0 * alpha);
        w->data[i] = exp(-x * x / 2.0);
        w->data[size - i - 1] = w->data[i];
    }
    if (half * 2 != size) w->data[half] = 1.0;
    double work = 0.0;
    work += size * 100;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *siguniv_window_gaussian(double alpha, void *context, _univ_mat *w) {
    if (w == nullptr) return "null pointer";
    if (w->type != EFI_DOUBLE) return "Illegal data type";
    if (const char *error = sigdbl_window_gaussian(alpha, context, (_double_mat *)w)) return error;
    return nullptr;
}

EFI_EXPORT const char *sigdbl_window_triangle(void *context, _double_mat *w) {
    validate(w);
    if (w->cols != 1 && w->rows != 1) return "array size mismatch";
    double step = 2.0 / (w->size + 1);
    int half = w->size / 2;
    for (int i = 0; i < half; ++i) {
        w->data[i] = (i + 1) * step;
        w->data[w->size - i - 1] = w->data[i];
    }
    if (w->size % 2) w->data[half] = 1.0;
    double work = 0.0;
    work += w->size * 5;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *siguniv_window_triangle(void *context, _univ_mat *w) {
    if (w == nullptr) return "null pointer";
    if (w->type != EFI_DOUBLE) return "Illegal data type";
    if (const char *error = sigdbl_window_triangle(context, (_double_mat *)w)) return error;
    return nullptr;
}

EFI_EXPORT const char *sigdbl_window_born_jordan(void *context, _double_mat *w) {
    validate(w);
    if (w->cols != 1 && w->rows != 1) return "array size mismatch";
    int half = (w->size - 1) / 2;
    for (int i = 0; i <= half; ++i) {
        w->data[i] = 1.0 / ((half - i) + 1.0);
        w->data[w->size - i - 1] = w->data[i];
    }
    double work = 0.0;
    work += w->size * 5;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *siguniv_window_born_jordan(void *context, _univ_mat *w) {
    if (w == nullptr) return "null pointer";
    if (w->type != EFI_DOUBLE) return "Illegal data type";
    if (const char *error = sigdbl_window_born_jordan(context, (_double_mat *)w)) return error;
    return nullptr;
}

}  // namespace mutils
