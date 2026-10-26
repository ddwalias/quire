#include "mutils_mat.h"

namespace mutils {

EFI_EXPORT const char *matdbl_assign_zeropad(const _double_mat *src, void *context, _double_mat *dst) {
    double total = 0.0;
    double progress = 0.0;
    matdbl_assign_submat(src, 0, 0, context, dst);
    for (int i = 0; i < src->rows; ++i) {
        for (int j = src->cols; j < dst->cols; ++j) dst->data[i * dst->cols + j] = 0;
        total += (double)((dst->cols - src->cols) * 4);
        if (progress < total) {
            if (mutil_interrupt_step == 0.0) mutil_interrupt_step = 10000000.0;
            progress += mutil_interrupt_step;
        }
    }
    for (int i = src->rows; i < dst->rows; ++i) {
        for (int j = 0; j < dst->cols; ++j) dst->data[i * dst->cols + j] = 0;
        total += (double)(dst->cols * 4);
        if (progress < total) {
            if (mutil_interrupt_step == 0.0) mutil_interrupt_step = 10000000.0;
            progress += mutil_interrupt_step;
        }
    }
    return nullptr;
}

EFI_EXPORT const char *matflt_assign_zeropad(const _float_mat *src, void *context, _float_mat *dst) {
    double total = 0.0;
    double progress = 0.0;
    matflt_assign_submat(src, 0, 0, context, dst);
    for (int i = 0; i < src->rows; ++i) {
        for (int j = src->cols; j < dst->cols; ++j) dst->data[i * dst->cols + j] = 0;
        total += (double)((dst->cols - src->cols) * 4);
        if (progress < total) {
            if (mutil_interrupt_step == 0.0) mutil_interrupt_step = 10000000.0;
            progress += mutil_interrupt_step;
        }
    }
    for (int i = src->rows; i < dst->rows; ++i) {
        for (int j = 0; j < dst->cols; ++j) dst->data[i * dst->cols + j] = 0;
        total += (double)(dst->cols * 4);
        if (progress < total) {
            if (mutil_interrupt_step == 0.0) mutil_interrupt_step = 10000000.0;
            progress += mutil_interrupt_step;
        }
    }
    return nullptr;
}

EFI_EXPORT const char *mats32_assign_zeropad(const _sint32_mat *src, void *context, _sint32_mat *dst) {
    double total = 0.0;
    double progress = 0.0;
    mats32_assign_submat(src, 0, 0, context, dst);
    for (int i = 0; i < src->rows; ++i) {
        for (int j = src->cols; j < dst->cols; ++j) dst->data[i * dst->cols + j] = 0;
        total += (double)((dst->cols - src->cols) * 4);
        if (progress < total) {
            if (mutil_interrupt_step == 0.0) mutil_interrupt_step = 10000000.0;
            progress += mutil_interrupt_step;
        }
    }
    for (int i = src->rows; i < dst->rows; ++i) {
        for (int j = 0; j < dst->cols; ++j) dst->data[i * dst->cols + j] = 0;
        total += (double)(dst->cols * 4);
        if (progress < total) {
            if (mutil_interrupt_step == 0.0) mutil_interrupt_step = 10000000.0;
            progress += mutil_interrupt_step;
        }
    }
    return nullptr;
}

EFI_EXPORT const char *mats16_assign_zeropad(const _sint16_mat *src, void *context, _sint16_mat *dst) {
    double total = 0.0;
    double progress = 0.0;
    mats16_assign_submat(src, 0, 0, context, dst);
    for (int i = 0; i < src->rows; ++i) {
        for (int j = src->cols; j < dst->cols; ++j) dst->data[i * dst->cols + j] = 0;
        total += (double)((dst->cols - src->cols) * 4);
        if (progress < total) {
            if (mutil_interrupt_step == 0.0) mutil_interrupt_step = 10000000.0;
            progress += mutil_interrupt_step;
        }
    }
    for (int i = src->rows; i < dst->rows; ++i) {
        for (int j = 0; j < dst->cols; ++j) dst->data[i * dst->cols + j] = 0;
        total += (double)(dst->cols * 4);
        if (progress < total) {
            if (mutil_interrupt_step == 0.0) mutil_interrupt_step = 10000000.0;
            progress += mutil_interrupt_step;
        }
    }
    return nullptr;
}

EFI_EXPORT const char *matu32_assign_zeropad(const _uint32_mat *src, void *context, _uint32_mat *dst) {
    double total = 0.0;
    double progress = 0.0;
    matu32_assign_submat(src, 0, 0, context, dst);
    for (int i = 0; i < src->rows; ++i) {
        for (int j = src->cols; j < dst->cols; ++j) dst->data[i * dst->cols + j] = 0;
        total += (double)((dst->cols - src->cols) * 4);
        if (progress < total) {
            if (mutil_interrupt_step == 0.0) mutil_interrupt_step = 10000000.0;
            progress += mutil_interrupt_step;
        }
    }
    for (int i = src->rows; i < dst->rows; ++i) {
        for (int j = 0; j < dst->cols; ++j) dst->data[i * dst->cols + j] = 0;
        total += (double)(dst->cols * 4);
        if (progress < total) {
            if (mutil_interrupt_step == 0.0) mutil_interrupt_step = 10000000.0;
            progress += mutil_interrupt_step;
        }
    }
    return nullptr;
}

EFI_EXPORT const char *matu16_assign_zeropad(const _uint16_mat *src, void *context, _uint16_mat *dst) {
    double total = 0.0;
    double progress = 0.0;
    matu16_assign_submat(src, 0, 0, context, dst);
    for (int i = 0; i < src->rows; ++i) {
        for (int j = src->cols; j < dst->cols; ++j) dst->data[i * dst->cols + j] = 0;
        total += (double)((dst->cols - src->cols) * 4);
        if (progress < total) {
            if (mutil_interrupt_step == 0.0) mutil_interrupt_step = 10000000.0;
            progress += mutil_interrupt_step;
        }
    }
    for (int i = src->rows; i < dst->rows; ++i) {
        for (int j = 0; j < dst->cols; ++j) dst->data[i * dst->cols + j] = 0;
        total += (double)(dst->cols * 4);
        if (progress < total) {
            if (mutil_interrupt_step == 0.0) mutil_interrupt_step = 10000000.0;
            progress += mutil_interrupt_step;
        }
    }
    return nullptr;
}

EFI_EXPORT const char *matu8_assign_zeropad(const _uint8_mat *src, void *context, _uint8_mat *dst) {
    double total = 0.0;
    double progress = 0.0;
    matu8_assign_submat(src, 0, 0, context, dst);
    for (int i = 0; i < src->rows; ++i) {
        for (int j = src->cols; j < dst->cols; ++j) dst->data[i * dst->cols + j] = 0;
        total += (double)((dst->cols - src->cols) * 4);
        if (progress < total) {
            if (mutil_interrupt_step == 0.0) mutil_interrupt_step = 10000000.0;
            progress += mutil_interrupt_step;
        }
    }
    for (int i = src->rows; i < dst->rows; ++i) {
        for (int j = 0; j < dst->cols; ++j) dst->data[i * dst->cols + j] = 0;
        total += (double)(dst->cols * 4);
        if (progress < total) {
            if (mutil_interrupt_step == 0.0) mutil_interrupt_step = 10000000.0;
            progress += mutil_interrupt_step;
        }
    }
    return nullptr;
}

}  // namespace mutils
