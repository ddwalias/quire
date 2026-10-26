#include "mutils_mat.h"

#include <math.h>

namespace mutils {

EFI_EXPORT const char *matdbl_entropy(Efi::EfiCallContext *, const _double_mat *m, void *context, double *out) {
    check_src_f(m);
    double *data = m->data;
    int size = m->size;
    if (out == nullptr) return "null pointer";
    double sum = 0.0;
    for (int i = 0; i < size; ++i) {
        if (data[i] < 0.0) return "Illegal data value";
        sum += data[i];
    }
    if (!(fabs(sum - 1.0) <= (fabs(sum) + 1.0) * 1e-05)) return "Illegal data value";
    double total = 0.0;
    double progress = 0.0;
    total += (double)size;
    if (progress < total) {
        if (mutil_interrupt_step == 0.0) mutil_interrupt_step = 10000000.0;
        progress += mutil_interrupt_step;
    }
    *out = 0.0;
    for (int i = 0; i < size; ++i) {
        if (data[i] > 0.0) *out -= data[i] * log(data[i]);
    }
    *out /= log(2.0);
    total += (double)size;
    if (progress < total) {
        if (mutil_interrupt_step == 0.0) mutil_interrupt_step = 10000000.0;
        progress += mutil_interrupt_step;
    }
    (void)context;
    return nullptr;
}

EFI_EXPORT const char *matflt_entropy(Efi::EfiCallContext *, const _float_mat *m, void *context, double *out) {
    check_src_f(m);
    float *data = m->data;
    int size = m->size;
    if (out == nullptr) return "null pointer";
    double sum = 0.0;
    for (int i = 0; i < size; ++i) {
        if (data[i] < 0.0) return "Illegal data value";
        sum += data[i];
    }
    if (!(fabs(sum - 1.0) <= (fabs(sum) + 1.0) * 1e-05)) return "Illegal data value";
    double total = 0.0;
    double progress = 0.0;
    total += (double)size;
    if (progress < total) {
        if (mutil_interrupt_step == 0.0) mutil_interrupt_step = 10000000.0;
        progress += mutil_interrupt_step;
    }
    *out = 0.0;
    for (int i = 0; i < size; ++i) {
        if (data[i] > 0.0) *out -= data[i] * logf(data[i]);
    }
    *out /= log(2.0);
    total += (double)size;
    if (progress < total) {
        if (mutil_interrupt_step == 0.0) mutil_interrupt_step = 10000000.0;
        progress += mutil_interrupt_step;
    }
    (void)context;
    return nullptr;
}

EFI_EXPORT const char *mats32_entropy(Efi::EfiCallContext *, const _sint32_mat *m, void *context, double *out) {
    check_src_f(m);
    long *data = m->data;
    int size = m->size;
    if (out == nullptr) return "null pointer";
    int sum = 0;
    for (int i = 0; i < size; ++i) {
        if (data[i] < 0) return "Illegal data value";
        sum += data[i];
    }
    double total = 0.0;
    double progress = 0.0;
    total += (double)size;
    if (progress < total) {
        if (mutil_interrupt_step == 0.0) mutil_interrupt_step = 10000000.0;
        progress += mutil_interrupt_step;
    }
    *out = 0.0;
    for (int i = 0; i < size; ++i) {
        if (data[i] > 0) *out -= data[i] * log(data[i] / (double)sum);
    }
    *out /= (double)sum;
    *out /= log(2.0);
    total += (double)size;
    if (progress < total) {
        if (mutil_interrupt_step == 0.0) mutil_interrupt_step = 10000000.0;
        progress += mutil_interrupt_step;
    }
    (void)context;
    return nullptr;
}

EFI_EXPORT const char *mats16_entropy(Efi::EfiCallContext *, const _sint16_mat *m, void *context, double *out) {
    check_src_f(m);
    short *data = m->data;
    int size = m->size;
    if (out == nullptr) return "null pointer";
    int sum = 0;
    for (int i = 0; i < size; ++i) {
        if (data[i] < 0) return "Illegal data value";
        sum += data[i];
    }
    double total = 0.0;
    double progress = 0.0;
    total += (double)size;
    if (progress < total) {
        if (mutil_interrupt_step == 0.0) mutil_interrupt_step = 10000000.0;
        progress += mutil_interrupt_step;
    }
    *out = 0.0;
    for (int i = 0; i < size; ++i) {
        if (data[i] > 0) *out -= data[i] * log(data[i] / (double)sum);
    }
    *out /= (double)sum;
    *out /= log(2.0);
    total += (double)size;
    if (progress < total) {
        if (mutil_interrupt_step == 0.0) mutil_interrupt_step = 10000000.0;
        progress += mutil_interrupt_step;
    }
    (void)context;
    return nullptr;
}

EFI_EXPORT const char *matu32_entropy(Efi::EfiCallContext *, const _uint32_mat *m, void *context, double *out) {
    check_src_f(m);
    unsigned int *data = m->data;
    int size = m->size;
    if (out == nullptr) return "null pointer";
    int sum = 0;
    for (int i = 0; i < size; ++i) {
        if (data[i] < 0) return "Illegal data value";
        sum += data[i];
    }
    double total = 0.0;
    double progress = 0.0;
    total += (double)size;
    if (progress < total) {
        if (mutil_interrupt_step == 0.0) mutil_interrupt_step = 10000000.0;
        progress += mutil_interrupt_step;
    }
    *out = 0.0;
    for (int i = 0; i < size; ++i) {
        if (data[i] > 0) *out -= data[i] * log(data[i] / (double)sum);
    }
    *out /= (double)sum;
    *out /= log(2.0);
    total += (double)size;
    if (progress < total) {
        if (mutil_interrupt_step == 0.0) mutil_interrupt_step = 10000000.0;
        progress += mutil_interrupt_step;
    }
    (void)context;
    return nullptr;
}

EFI_EXPORT const char *matu16_entropy(Efi::EfiCallContext *, const _uint16_mat *m, void *context, double *out) {
    check_src_f(m);
    unsigned short *data = m->data;
    int size = m->size;
    if (out == nullptr) return "null pointer";
    int sum = 0;
    for (int i = 0; i < size; ++i) {
        if (data[i] < 0) return "Illegal data value";
        sum += data[i];
    }
    double total = 0.0;
    double progress = 0.0;
    total += (double)size;
    if (progress < total) {
        if (mutil_interrupt_step == 0.0) mutil_interrupt_step = 10000000.0;
        progress += mutil_interrupt_step;
    }
    *out = 0.0;
    for (int i = 0; i < size; ++i) {
        if (data[i] > 0) *out -= data[i] * log(data[i] / (double)sum);
    }
    *out /= (double)sum;
    *out /= log(2.0);
    total += (double)size;
    if (progress < total) {
        if (mutil_interrupt_step == 0.0) mutil_interrupt_step = 10000000.0;
        progress += mutil_interrupt_step;
    }
    (void)context;
    return nullptr;
}

EFI_EXPORT const char *matu8_entropy(Efi::EfiCallContext *, const _uint8_mat *m, void *context, double *out) {
    check_src_f(m);
    unsigned char *data = m->data;
    int size = m->size;
    if (out == nullptr) return "null pointer";
    int sum = 0;
    for (int i = 0; i < size; ++i) {
        if (data[i] < 0) return "Illegal data value";
        sum += data[i];
    }
    double total = 0.0;
    double progress = 0.0;
    total += (double)size;
    if (progress < total) {
        if (mutil_interrupt_step == 0.0) mutil_interrupt_step = 10000000.0;
        progress += mutil_interrupt_step;
    }
    *out = 0.0;
    for (int i = 0; i < size; ++i) {
        if (data[i] > 0) *out -= data[i] * log(data[i] / (double)sum);
    }
    *out /= (double)sum;
    *out /= log(2.0);
    total += (double)size;
    if (progress < total) {
        if (mutil_interrupt_step == 0.0) mutil_interrupt_step = 10000000.0;
        progress += mutil_interrupt_step;
    }
    (void)context;
    return nullptr;
}

}  // namespace mutils
