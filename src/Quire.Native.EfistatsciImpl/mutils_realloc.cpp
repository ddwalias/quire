#include "mutils_mat.h"

#include <string.h>

namespace mutils {

EFI_EXPORT void matdbl_realloc(Efi::EfiCallContext *ctx, _double_mat *m, long rows, long cols) {
    check_src_f(m);
    if (rows <= 0 || cols <= 0) Efi::Throw("array size mismatch", ~0ull);
    unsigned __int64 old_bytes = m->size * sizeof(double);
    m->rows = rows;
    m->cols = cols;
    m->size = rows * cols;
    unsigned __int64 bytes = m->size * sizeof(double);
    if (bytes >= old_bytes) {
        double *data = static_cast<double *>(ctx->allocator->allocate_array(bytes));
        memcpy(data, m->data, bytes);
        m->data = data;
    }
}

EFI_EXPORT void matflt_realloc(Efi::EfiCallContext *ctx, _float_mat *m, long rows, long cols) {
    check_src_f(m);
    if (rows <= 0 || cols <= 0) Efi::Throw("array size mismatch", ~0ull);
    unsigned __int64 old_bytes = m->size * sizeof(float);
    m->rows = rows;
    m->cols = cols;
    m->size = rows * cols;
    unsigned __int64 bytes = m->size * sizeof(float);
    if (bytes >= old_bytes) {
        float *data = static_cast<float *>(ctx->allocator->allocate_array(bytes));
        memcpy(data, m->data, bytes);
        m->data = data;
    }
}

EFI_EXPORT void mats32_realloc(Efi::EfiCallContext *ctx, _sint32_mat *m, long rows, long cols) {
    check_src_f(m);
    if (rows <= 0 || cols <= 0) Efi::Throw("array size mismatch", ~0ull);
    unsigned __int64 old_bytes = m->size * sizeof(long);
    m->rows = rows;
    m->cols = cols;
    m->size = rows * cols;
    unsigned __int64 bytes = m->size * sizeof(long);
    if (bytes >= old_bytes) {
        long *data = static_cast<long *>(ctx->allocator->allocate_array(bytes));
        memcpy(data, m->data, bytes);
        m->data = data;
    }
}

EFI_EXPORT void mats16_realloc(Efi::EfiCallContext *ctx, _sint16_mat *m, long rows, long cols) {
    check_src_f(m);
    if (rows <= 0 || cols <= 0) Efi::Throw("array size mismatch", ~0ull);
    unsigned __int64 old_bytes = m->size * sizeof(short);
    m->rows = rows;
    m->cols = cols;
    m->size = rows * cols;
    unsigned __int64 bytes = m->size * sizeof(short);
    if (bytes >= old_bytes) {
        short *data = static_cast<short *>(ctx->allocator->allocate_array(bytes));
        memcpy(data, m->data, bytes);
        m->data = data;
    }
}

EFI_EXPORT void matu32_realloc(Efi::EfiCallContext *ctx, _uint32_mat *m, long rows, long cols) {
    check_src_f(m);
    if (rows <= 0 || cols <= 0) Efi::Throw("array size mismatch", ~0ull);
    unsigned __int64 old_bytes = m->size * sizeof(unsigned int);
    m->rows = rows;
    m->cols = cols;
    m->size = rows * cols;
    unsigned __int64 bytes = m->size * sizeof(unsigned int);
    if (bytes >= old_bytes) {
        unsigned int *data = static_cast<unsigned int *>(ctx->allocator->allocate_array(bytes));
        memcpy(data, m->data, bytes);
        m->data = data;
    }
}

EFI_EXPORT void matu16_realloc(Efi::EfiCallContext *ctx, _uint16_mat *m, long rows, long cols) {
    check_src_f(m);
    if (rows <= 0 || cols <= 0) Efi::Throw("array size mismatch", ~0ull);
    unsigned __int64 old_bytes = m->size * sizeof(unsigned short);
    m->rows = rows;
    m->cols = cols;
    m->size = rows * cols;
    unsigned __int64 bytes = m->size * sizeof(unsigned short);
    if (bytes >= old_bytes) {
        unsigned short *data = static_cast<unsigned short *>(ctx->allocator->allocate_array(bytes));
        memcpy(data, m->data, bytes);
        m->data = data;
    }
}

EFI_EXPORT void matu8_realloc(Efi::EfiCallContext *ctx, _uint8_mat *m, long rows, long cols) {
    check_src_f(m);
    if (rows <= 0 || cols <= 0) Efi::Throw("array size mismatch", ~0ull);
    unsigned __int64 old_bytes = m->size * sizeof(unsigned char);
    m->rows = rows;
    m->cols = cols;
    m->size = rows * cols;
    unsigned __int64 bytes = m->size * sizeof(unsigned char);
    if (bytes >= old_bytes) {
        unsigned char *data = static_cast<unsigned char *>(ctx->allocator->allocate_array(bytes));
        memcpy(data, m->data, bytes);
        m->data = data;
    }
}

EFI_EXPORT void matcpx_realloc(Efi::EfiCallContext *ctx, _dcomplex_mat *m, long rows, long cols) {
    check_src_f(m);
    if (rows <= 0 || cols <= 0) Efi::Throw("array size mismatch", ~0ull);
    unsigned __int64 old_bytes = m->size * sizeof(_dcomplex);
    m->rows = rows;
    m->cols = cols;
    m->size = rows * cols;
    unsigned __int64 bytes = m->size * sizeof(_dcomplex);
    if (bytes >= old_bytes) {
        _dcomplex *data = static_cast<_dcomplex *>(ctx->allocator->allocate_array(bytes));
        memcpy(data, m->data, bytes);
        m->data = data;
    }
}

}  // namespace mutils
