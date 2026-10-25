#include "mutils_mat.h"

namespace mutils {

template <typename T>
__forceinline void chk_s(const typed_mat<T> *m) {
    if (m == nullptr) Efi::Throw("null pointer", ~0ull);
    if (m->rows <= 0 || m->cols <= 0) Efi::Throw("array size mismatch", ~0ull);
    if (m->data == nullptr) Efi::Throw("null pointer", ~0ull);
    if (m->size != m->rows * m->cols) Efi::Throw("array size mismatch", ~0ull);
}

template <typename T>
__forceinline void chk_d(const typed_mat<T> *m) {
    if (m == nullptr) Efi::Throw("null pointer", ~0ull);
    int cols = m->cols;
    int rows = m->rows;
    if (rows <= 0 || cols <= 0) Efi::Throw("array size mismatch", ~0ull);
    if (m->data == nullptr) Efi::Throw("null pointer", ~0ull);
    if (m->size != rows * cols) Efi::Throw("array size mismatch", ~0ull);
}

EFI_EXPORT void matu8_cast_to_dbl(const _uint8_mat *src, void *context, _double_mat *dst) {
    chk_s(src);
    chk_d(dst);
    if (src->rows != dst->rows || src->cols != dst->cols) Efi::Throw("array size mismatch", ~0ull);
    int i = 0;
    if (0 < src->size) {
        do {
            dst->data[i] = static_cast<double>(src->data[i]);
            ++i;
        } while (i < src->size);
    }
    double work = (double)src->size + (double)src->size;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
}

EFI_EXPORT void matu8_cast_to_flt(const _uint8_mat *src, void *context, _float_mat *dst) {
    chk_s(src);
    chk_d(dst);
    if (src->rows != dst->rows || src->cols != dst->cols) Efi::Throw("array size mismatch", ~0ull);
    int i = 0;
    if (0 < src->size) {
        do {
            dst->data[i] = static_cast<float>(src->data[i]);
            ++i;
        } while (i < src->size);
    }
    double work = (double)src->size + (double)src->size;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
}

EFI_EXPORT void matu8_cast_to_s16(const _uint8_mat *src, void *context, _sint16_mat *dst) {
    chk_s(src);
    chk_d(dst);
    if (src->rows != dst->rows || src->cols != dst->cols) Efi::Throw("array size mismatch", ~0ull);
    int i = 0;
    if (0 < src->size) {
        do {
            dst->data[i] = static_cast<short>(src->data[i]);
            ++i;
        } while (i < src->size);
    }
    double work = (double)src->size + (double)src->size;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
}

EFI_EXPORT void matu8_cast_to_u16(const _uint8_mat *src, void *context, _uint16_mat *dst) {
    chk_s(src);
    chk_d(dst);
    if (src->rows != dst->rows || src->cols != dst->cols) Efi::Throw("array size mismatch", ~0ull);
    int i = 0;
    if (0 < src->size) {
        do {
            dst->data[i] = static_cast<unsigned short>(src->data[i]);
            ++i;
        } while (i < src->size);
    }
    double work = (double)src->size + (double)src->size;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
}

EFI_EXPORT void matu8_cast_to_s32(const _uint8_mat *src, void *context, _sint32_mat *dst) {
    chk_s(src);
    chk_d(dst);
    if (src->rows != dst->rows || src->cols != dst->cols) Efi::Throw("array size mismatch", ~0ull);
    int i = 0;
    if (0 < src->size) {
        do {
            dst->data[i] = static_cast<long>(src->data[i]);
            ++i;
        } while (i < src->size);
    }
    double work = (double)src->size + (double)src->size;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
}

EFI_EXPORT void matu8_cast_to_u32(const _uint8_mat *src, void *context, _uint32_mat *dst) {
    chk_s(src);
    chk_d(dst);
    if (src->rows != dst->rows || src->cols != dst->cols) Efi::Throw("array size mismatch", ~0ull);
    int i = 0;
    if (0 < src->size) {
        do {
            dst->data[i] = static_cast<unsigned int>(src->data[i]);
            ++i;
        } while (i < src->size);
    }
    double work = (double)src->size + (double)src->size;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
}

}  // namespace mutils
