#include "mutils_mat.h"

namespace mutils {

const char *matdbl_assign_scalar(double value, void *context, _double_mat *m) {
    return assign_scalar(value, context, m);
}

const char *matflt_assign_scalar(float value, void *context, _float_mat *m) {
    return assign_scalar(value, context, m);
}

const char *mats32_assign_scalar(long value, void *context, _sint32_mat *m) {
    return assign_scalar(value, context, m);
}

const char *mats16_assign_scalar(short value, void *context, _sint16_mat *m) {
    return assign_scalar(value, context, m);
}

// noinline: the original calls it from matu32 histogram and the other users; LTCG would otherwise inline it there.
__declspec(noinline) const char *matu32_assign_scalar(unsigned int value, void *context, _uint32_mat *m) {
    return assign_scalar(value, context, m);
}

const char *matu16_assign_scalar(unsigned short value, void *context, _uint16_mat *m) {
    return assign_scalar(value, context, m);
}

const char *matu8_assign_scalar(unsigned char value, void *context, _uint8_mat *m) {
    return assign_scalar(value, context, m);
}

// Not the template: through it the value splits into two doubles and the fill loses the original's rep movs.
const char *matcpx_assign_scalar(_dcomplex value, void *context, _dcomplex_mat *m) {
    if (m == nullptr) Efi::Throw("null pointer", ~0ull);
    if (m->rows <= 0 || m->cols <= 0) Efi::Throw("array size mismatch", ~0ull);
    _dcomplex *data = m->data;
    if (data == nullptr) Efi::Throw("null pointer", ~0ull);
    int size = m->size;
    if (size != m->rows * m->cols) Efi::Throw("array size mismatch", ~0ull);
    for (int i = 0; i < size; ++i) data[i] = value;
    double work = (double)size + (double)size;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

void matdbl_validate(const _double_mat *m) { validate(m); }
void matflt_validate(const _float_mat *m) { validate(m); }
void mats32_validate(const _sint32_mat *m) { validate(m); }
void mats16_validate(const _sint16_mat *m) { validate(m); }
void matu32_validate(const _uint32_mat *m) { validate(m); }
void matu16_validate(const _uint16_mat *m) { validate(m); }
void matu8_validate(const _uint8_mat *m) { validate(m); }
void matcpx_validate(const _dcomplex_mat *m) { validate(m); }

void matdbl_wrap_data(_double_mat *m, double *data, long rows, long cols) { wrap_data(m, data, rows, cols); }
void matflt_wrap_data(_float_mat *m, float *data, long rows, long cols) { wrap_data(m, data, rows, cols); }
void mats32_wrap_data(_sint32_mat *m, long *data, long rows, long cols) { wrap_data(m, data, rows, cols); }
void mats16_wrap_data(_sint16_mat *m, short *data, long rows, long cols) { wrap_data(m, data, rows, cols); }
void matu32_wrap_data(_uint32_mat *m, unsigned int *data, long rows, long cols) { wrap_data(m, data, rows, cols); }
void matu16_wrap_data(_uint16_mat *m, unsigned short *data, long rows, long cols) { wrap_data(m, data, rows, cols); }
void matu8_wrap_data(_uint8_mat *m, unsigned char *data, long rows, long cols) { wrap_data(m, data, rows, cols); }

void matdbl_malloc(Efi::EfiCallContext *ctx, _double_mat *m, long rows, long cols) { allocate_mat(ctx, m, rows, cols); }
void matflt_malloc(Efi::EfiCallContext *ctx, _float_mat *m, long rows, long cols) { allocate_mat(ctx, m, rows, cols); }
void mats32_malloc(Efi::EfiCallContext *ctx, _sint32_mat *m, long rows, long cols) { allocate_mat(ctx, m, rows, cols); }
void mats16_malloc(Efi::EfiCallContext *ctx, _sint16_mat *m, long rows, long cols) { allocate_mat(ctx, m, rows, cols); }
void matu32_malloc(Efi::EfiCallContext *ctx, _uint32_mat *m, long rows, long cols) { allocate_mat(ctx, m, rows, cols); }
void matu16_malloc(Efi::EfiCallContext *ctx, _uint16_mat *m, long rows, long cols) { allocate_mat(ctx, m, rows, cols); }
void matu8_malloc(Efi::EfiCallContext *ctx, _uint8_mat *m, long rows, long cols) { allocate_mat(ctx, m, rows, cols); }
void matcpx_malloc(Efi::EfiCallContext *ctx, _dcomplex_mat *m, long rows, long cols) { allocate_mat(ctx, m, rows, cols); }

}  // namespace mutils
