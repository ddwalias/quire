#pragma once

#include "efi_common.h"

namespace mutils {

// The body every typed `mat*_assign_scalar` export shares.
template <typename T>
const char *assign_scalar(T value, void *context, typed_mat<T> *m) {
    if (m == nullptr) Efi::Throw("null pointer", ~0ull);
    if (m->rows <= 0 || m->cols <= 0) Efi::Throw("array size mismatch", ~0ull);
    T *data = m->data;
    if (data == nullptr) Efi::Throw("null pointer", ~0ull);
    int size = m->size;
    if (size != m->rows * m->cols) Efi::Throw("array size mismatch", ~0ull);
    for (int i = 0; i < size; ++i) data[i] = value;
    double work = (double)size + (double)size;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *matdbl_assign_scalar(double value, void *context, _double_mat *m);
EFI_EXPORT const char *matflt_assign_scalar(float value, void *context, _float_mat *m);
EFI_EXPORT const char *mats32_assign_scalar(long value, void *context, _sint32_mat *m);
EFI_EXPORT const char *mats16_assign_scalar(short value, void *context, _sint16_mat *m);
EFI_EXPORT const char *matu32_assign_scalar(unsigned int value, void *context, _uint32_mat *m);
EFI_EXPORT const char *matu16_assign_scalar(unsigned short value, void *context, _uint16_mat *m);
EFI_EXPORT const char *matu8_assign_scalar(unsigned char value, void *context, _uint8_mat *m);
EFI_EXPORT const char *matcpx_assign_scalar(_dcomplex value, void *context, _dcomplex_mat *m);

}  // namespace mutils
