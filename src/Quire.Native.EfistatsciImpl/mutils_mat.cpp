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

const char *matu32_assign_scalar(unsigned int value, void *context, _uint32_mat *m) {
    return assign_scalar(value, context, m);
}

const char *matu16_assign_scalar(unsigned short value, void *context, _uint16_mat *m) {
    return assign_scalar(value, context, m);
}

const char *matu8_assign_scalar(unsigned char value, void *context, _uint8_mat *m) {
    return assign_scalar(value, context, m);
}

const char *matcpx_assign_scalar(_dcomplex value, void *context, _dcomplex_mat *m) {
    return assign_scalar(value, context, m);
}

}  // namespace mutils
