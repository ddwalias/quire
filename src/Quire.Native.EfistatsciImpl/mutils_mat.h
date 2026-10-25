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

template <typename T>
void validate(const typed_mat<T> *m) {
    if (m == nullptr) Efi::Throw("null pointer", ~0ull);
    int rows = m->rows;
    int cols = m->cols;
    if (rows <= 0 || cols <= 0) Efi::Throw("array size mismatch", ~0ull);
    if (m->data == nullptr) Efi::Throw("null pointer", ~0ull);
    if (m->size != rows * cols) Efi::Throw("array size mismatch", ~0ull);
}

template <typename T>
void wrap_data(typed_mat<T> *m, T *data, int rows, int cols) {
    if (m == nullptr) Efi::Throw("null pointer", ~0ull);
    m->rows = rows;
    m->cols = cols;
    m->data = data;
    m->size = rows * cols;
    if (rows <= 0 || cols <= 0) Efi::Throw("array size mismatch", ~0ull);
    if (data == nullptr) Efi::Throw("null pointer", ~0ull);
}

template <typename T>
void allocate_mat(Efi::EfiCallContext *ctx, typed_mat<T> *m, int rows, int cols) {
    if (m == nullptr) Efi::Throw("null pointer", ~0ull);
    if (rows <= 0 || cols <= 0) Efi::Throw("array size mismatch", ~0ull);
    m->rows = rows;
    m->cols = cols;
    m->size = rows * cols;
    m->data = static_cast<T *>(ctx->allocator->allocate_array(m->size * sizeof(T)));
}

EFI_EXPORT void matdbl_validate(const _double_mat *m);
EFI_EXPORT void matflt_validate(const _float_mat *m);
EFI_EXPORT void mats32_validate(const _sint32_mat *m);
EFI_EXPORT void mats16_validate(const _sint16_mat *m);
EFI_EXPORT void matu32_validate(const _uint32_mat *m);
EFI_EXPORT void matu16_validate(const _uint16_mat *m);
EFI_EXPORT void matu8_validate(const _uint8_mat *m);
EFI_EXPORT void matcpx_validate(const _dcomplex_mat *m);

EFI_EXPORT void matdbl_wrap_data(_double_mat *m, double *data, long rows, long cols);
EFI_EXPORT void matflt_wrap_data(_float_mat *m, float *data, long rows, long cols);
EFI_EXPORT void mats32_wrap_data(_sint32_mat *m, long *data, long rows, long cols);
EFI_EXPORT void mats16_wrap_data(_sint16_mat *m, short *data, long rows, long cols);
EFI_EXPORT void matu32_wrap_data(_uint32_mat *m, unsigned int *data, long rows, long cols);
EFI_EXPORT void matu16_wrap_data(_uint16_mat *m, unsigned short *data, long rows, long cols);
EFI_EXPORT void matu8_wrap_data(_uint8_mat *m, unsigned char *data, long rows, long cols);

EFI_EXPORT void matdbl_malloc(Efi::EfiCallContext *ctx, _double_mat *m, long rows, long cols);
EFI_EXPORT void matflt_malloc(Efi::EfiCallContext *ctx, _float_mat *m, long rows, long cols);
EFI_EXPORT void mats32_malloc(Efi::EfiCallContext *ctx, _sint32_mat *m, long rows, long cols);
EFI_EXPORT void mats16_malloc(Efi::EfiCallContext *ctx, _sint16_mat *m, long rows, long cols);
EFI_EXPORT void matu32_malloc(Efi::EfiCallContext *ctx, _uint32_mat *m, long rows, long cols);
EFI_EXPORT void matu16_malloc(Efi::EfiCallContext *ctx, _uint16_mat *m, long rows, long cols);
EFI_EXPORT void matu8_malloc(Efi::EfiCallContext *ctx, _uint8_mat *m, long rows, long cols);
EFI_EXPORT void matcpx_malloc(Efi::EfiCallContext *ctx, _dcomplex_mat *m, long rows, long cols);

EFI_EXPORT const char *matdbl_assign_scalar(double value, void *context, _double_mat *m);
EFI_EXPORT const char *matflt_assign_scalar(float value, void *context, _float_mat *m);
EFI_EXPORT const char *mats32_assign_scalar(long value, void *context, _sint32_mat *m);
EFI_EXPORT const char *mats16_assign_scalar(short value, void *context, _sint16_mat *m);
EFI_EXPORT const char *matu32_assign_scalar(unsigned int value, void *context, _uint32_mat *m);
EFI_EXPORT const char *matu16_assign_scalar(unsigned short value, void *context, _uint16_mat *m);
EFI_EXPORT const char *matu8_assign_scalar(unsigned char value, void *context, _uint8_mat *m);

}  // namespace mutils
