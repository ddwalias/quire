#include "mutils_mat.h"

#include <math.h>

namespace mutils {

EFI_EXPORT void matdbl_cast_to_u8(const _double_mat *src, unsigned char clamp, void *context, _uint8_mat *dst) {
    check_src_f(src);
    check_dst_f(dst);
    if (src->rows != dst->rows || src->cols != dst->cols) Efi::Throw("array size mismatch", ~0ull);
    for (int i = 0; i < src->size; ++i) {
        double v = floor(src->data[i] + 0.5);
        if (v < 0.0) {
            if (!clamp) Efi::Throw("overflow", ~0ull);
            dst->data[i] = 0;
        } else if (v > 255.0) {
            if (!clamp) Efi::Throw("overflow", ~0ull);
            dst->data[i] = 255;
        } else {
            dst->data[i] = (unsigned char)v;
        }
    }
    double work = (double)src->size * 5.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
}

EFI_EXPORT void matflt_cast_to_u8(const _float_mat *src, unsigned char clamp, void *context, _uint8_mat *dst) {
    check_src_f(src);
    check_dst_f(dst);
    if (src->rows != dst->rows || src->cols != dst->cols) Efi::Throw("array size mismatch", ~0ull);
    for (int i = 0; i < src->size; ++i) {
        double v = floor(src->data[i] + 0.5);
        if (v < 0.0) {
            if (!clamp) Efi::Throw("overflow", ~0ull);
            dst->data[i] = 0;
        } else if (v > 255.0) {
            if (!clamp) Efi::Throw("overflow", ~0ull);
            dst->data[i] = 255;
        } else {
            dst->data[i] = (unsigned char)v;
        }
    }
    double work = (double)src->size * 5.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
}

EFI_EXPORT void mats16_cast_to_u8(const _sint16_mat *src, unsigned char clamp, void *context, _uint8_mat *dst) {
    check_src_f(src);
    check_dst_f(dst);
    if (src->rows != dst->rows || src->cols != dst->cols) Efi::Throw("array size mismatch", ~0ull);
    for (int i = 0; i < src->size; ++i) {
        short v = src->data[i];
        if (v < 0) {
            if (!clamp) Efi::Throw("overflow", ~0ull);
            dst->data[i] = 0;
        } else if (v > 255) {
            if (!clamp) Efi::Throw("overflow", ~0ull);
            dst->data[i] = 255;
        } else {
            dst->data[i] = (unsigned char)v;
        }
    }
    double work = (double)src->size * 5.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
}

EFI_EXPORT void mats32_cast_to_u8(const _sint32_mat *src, unsigned char clamp, void *context, _uint8_mat *dst) {
    check_src_f(src);
    check_dst_f(dst);
    if (src->rows != dst->rows || src->cols != dst->cols) Efi::Throw("array size mismatch", ~0ull);
    for (int i = 0; i < src->size; ++i) {
        long v = src->data[i];
        if (v < 0) {
            if (!clamp) Efi::Throw("overflow", ~0ull);
            dst->data[i] = 0;
        } else if (v > 255) {
            if (!clamp) Efi::Throw("overflow", ~0ull);
            dst->data[i] = 255;
        } else {
            dst->data[i] = (unsigned char)v;
        }
    }
    double work = (double)src->size * 5.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
}

EFI_EXPORT void matu16_cast_to_u8(const _uint16_mat *src, unsigned char clamp, void *context, _uint8_mat *dst) {
    check_src_f(src);
    check_dst_f(dst);
    if (src->rows != dst->rows || src->cols != dst->cols) Efi::Throw("array size mismatch", ~0ull);
    for (int i = 0; i < src->size; ++i) {
        unsigned short v = src->data[i];
        if (v > 255) {
            if (!clamp) Efi::Throw("overflow", ~0ull);
            dst->data[i] = 255;
        } else {
            dst->data[i] = (unsigned char)v;
        }
    }
    double work = (double)src->size * 5.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
}

EFI_EXPORT void matu32_cast_to_u8(const _uint32_mat *src, unsigned char clamp, void *context, _uint8_mat *dst) {
    check_src_f(src);
    check_dst_f(dst);
    if (src->rows != dst->rows || src->cols != dst->cols) Efi::Throw("array size mismatch", ~0ull);
    for (int i = 0; i < src->size; ++i) {
        unsigned int v = src->data[i];
        if (v > 255) {
            if (!clamp) Efi::Throw("overflow", ~0ull);
            dst->data[i] = 255;
        } else {
            dst->data[i] = (unsigned char)v;
        }
    }
    double work = (double)src->size * 5.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
}

EFI_EXPORT void matdbl_cast_to_u16(const _double_mat *src, unsigned char clamp, void *context, _uint16_mat *dst) {
    check_src_f(src);
    check_dst_f(dst);
    if (src->rows != dst->rows || src->cols != dst->cols) Efi::Throw("array size mismatch", ~0ull);
    for (int i = 0; i < src->size; ++i) {
        double v = floor(src->data[i] + 0.5);
        if (v < 0.0) {
            if (!clamp) Efi::Throw("overflow", ~0ull);
            dst->data[i] = 0;
        } else if (v > 65535.0) {
            if (!clamp) Efi::Throw("overflow", ~0ull);
            dst->data[i] = 65535;
        } else {
            dst->data[i] = (unsigned short)v;
        }
    }
    double work = (double)src->size * 5.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
}

EFI_EXPORT void matflt_cast_to_u16(const _float_mat *src, unsigned char clamp, void *context, _uint16_mat *dst) {
    check_src_f(src);
    check_dst_f(dst);
    if (src->rows != dst->rows || src->cols != dst->cols) Efi::Throw("array size mismatch", ~0ull);
    for (int i = 0; i < src->size; ++i) {
        double v = floor(src->data[i] + 0.5);
        if (v < 0.0) {
            if (!clamp) Efi::Throw("overflow", ~0ull);
            dst->data[i] = 0;
        } else if (v > 65535.0) {
            if (!clamp) Efi::Throw("overflow", ~0ull);
            dst->data[i] = 65535;
        } else {
            dst->data[i] = (unsigned short)v;
        }
    }
    double work = (double)src->size * 5.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
}

EFI_EXPORT void mats16_cast_to_u16(const _sint16_mat *src, unsigned char clamp, void *context, _uint16_mat *dst) {
    check_src_f(src);
    check_dst_f(dst);
    if (src->rows != dst->rows || src->cols != dst->cols) Efi::Throw("array size mismatch", ~0ull);
    for (int i = 0; i < src->size; ++i) {
        short v = src->data[i];
        if (v < 0) {
            if (!clamp) Efi::Throw("overflow", ~0ull);
            dst->data[i] = 0;
        } else {
            dst->data[i] = (unsigned short)v;
        }
    }
    double work = (double)src->size * 5.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
}

EFI_EXPORT void mats32_cast_to_u16(const _sint32_mat *src, unsigned char clamp, void *context, _uint16_mat *dst) {
    check_src_f(src);
    check_dst_f(dst);
    if (src->rows != dst->rows || src->cols != dst->cols) Efi::Throw("array size mismatch", ~0ull);
    for (int i = 0; i < src->size; ++i) {
        long v = src->data[i];
        if (v < 0) {
            if (!clamp) Efi::Throw("overflow", ~0ull);
            dst->data[i] = 0;
        } else if (v > 65535) {
            if (!clamp) Efi::Throw("overflow", ~0ull);
            dst->data[i] = 65535;
        } else {
            dst->data[i] = (unsigned short)v;
        }
    }
    double work = (double)src->size * 5.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
}

EFI_EXPORT void matu32_cast_to_u16(const _uint32_mat *src, unsigned char clamp, void *context, _uint16_mat *dst) {
    check_src_f(src);
    check_dst_f(dst);
    if (src->rows != dst->rows || src->cols != dst->cols) Efi::Throw("array size mismatch", ~0ull);
    for (int i = 0; i < src->size; ++i) {
        unsigned int v = src->data[i];
        if (v > 65535) {
            if (!clamp) Efi::Throw("overflow", ~0ull);
            dst->data[i] = 65535;
        } else {
            dst->data[i] = (unsigned short)v;
        }
    }
    double work = (double)src->size * 5.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
}

EFI_EXPORT void mats32_cast_to_s16(const _sint32_mat *src, unsigned char clamp, void *context, _sint16_mat *dst) {
    check_src_f(src);
    check_dst_f(dst);
    if (src->rows != dst->rows || src->cols != dst->cols) Efi::Throw("array size mismatch", ~0ull);
    for (int i = 0; i < src->size; ++i) {
        long v = src->data[i];
        if (v < -32767) {
            if (!clamp) Efi::Throw("overflow", ~0ull);
            dst->data[i] = -32767;
        } else if (v > 32767) {
            if (!clamp) Efi::Throw("overflow", ~0ull);
            dst->data[i] = 32767;
        } else {
            dst->data[i] = (short)v;
        }
    }
    double work = (double)src->size * 5.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
}

EFI_EXPORT void matu16_cast_to_s16(const _uint16_mat *src, unsigned char clamp, void *context, _sint16_mat *dst) {
    check_src_f(src);
    check_dst_f(dst);
    if (src->rows != dst->rows || src->cols != dst->cols) Efi::Throw("array size mismatch", ~0ull);
    for (int i = 0; i < src->size; ++i) {
        unsigned short v = src->data[i];
        if (v > 32767) {
            if (!clamp) Efi::Throw("overflow", ~0ull);
            dst->data[i] = 32767;
        } else {
            dst->data[i] = (short)v;
        }
    }
    double work = (double)src->size * 5.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
}

EFI_EXPORT void matu32_cast_to_s16(const _uint32_mat *src, unsigned char clamp, void *context, _sint16_mat *dst) {
    check_src_f(src);
    check_dst_f(dst);
    if (src->rows != dst->rows || src->cols != dst->cols) Efi::Throw("array size mismatch", ~0ull);
    for (int i = 0; i < src->size; ++i) {
        unsigned int v = src->data[i];
        if (v > 32767) {
            if (!clamp) Efi::Throw("overflow", ~0ull);
            dst->data[i] = 32767;
        } else {
            dst->data[i] = (short)v;
        }
    }
    double work = (double)src->size * 5.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
}

EFI_EXPORT void mats16_cast_to_u32(const _sint16_mat *src, unsigned char clamp, void *context, _uint32_mat *dst) {
    check_src_f(src);
    check_dst_f(dst);
    if (src->rows != dst->rows || src->cols != dst->cols) Efi::Throw("array size mismatch", ~0ull);
    for (int i = 0; i < src->size; ++i) {
        short v = src->data[i];
        if (v < 0) {
            if (!clamp) Efi::Throw("overflow", ~0ull);
            dst->data[i] = 0;
        } else {
            dst->data[i] = (unsigned int)v;
        }
    }
    double work = (double)src->size * 5.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
}

EFI_EXPORT void mats32_cast_to_u32(const _sint32_mat *src, unsigned char clamp, void *context, _uint32_mat *dst) {
    check_src_f(src);
    check_dst_f(dst);
    if (src->rows != dst->rows || src->cols != dst->cols) Efi::Throw("array size mismatch", ~0ull);
    for (int i = 0; i < src->size; ++i) {
        long v = src->data[i];
        if (v < 0) {
            if (!clamp) Efi::Throw("overflow", ~0ull);
            dst->data[i] = 0;
        } else {
            dst->data[i] = (unsigned int)v;
        }
    }
    double work = (double)src->size * 5.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
}

EFI_EXPORT void matu32_cast_to_s32(const _uint32_mat *src, unsigned char clamp, void *context, _sint32_mat *dst) {
    check_src_f(src);
    check_dst_f(dst);
    if (src->rows != dst->rows || src->cols != dst->cols) Efi::Throw("array size mismatch", ~0ull);
    for (int i = 0; i < src->size; ++i) {
        unsigned int v = src->data[i];
        if (v > 2147483647) {
            if (!clamp) Efi::Throw("overflow", ~0ull);
            dst->data[i] = 2147483647;
        } else {
            dst->data[i] = (long)v;
        }
    }
    double work = (double)src->size * 5.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
}

EFI_EXPORT void matdbl_cast_to_u32(const _double_mat *src, unsigned char clamp, void *context, _uint32_mat *dst) {
    check_src_f(src);
    check_dst_f(dst);
    if (src->rows != dst->rows || src->cols != dst->cols) Efi::Throw("array size mismatch", ~0ull);
    for (int i = 0; i < src->size; ++i) {
        double v = floor(src->data[i] + 0.5);
        if (v < 0.0) {
            if (!clamp) Efi::Throw("overflow", ~0ull);
            dst->data[i] = 0;
        } else if (v > 4294967295.0) {
            if (!clamp) Efi::Throw("overflow", ~0ull);
            dst->data[i] = 4294967295u;
        } else {
            dst->data[i] = (unsigned int)v;
        }
    }
    double work = (double)src->size * 5.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
}

EFI_EXPORT void matflt_cast_to_u32(const _float_mat *src, unsigned char clamp, void *context, _uint32_mat *dst) {
    check_src_f(src);
    check_dst_f(dst);
    if (src->rows != dst->rows || src->cols != dst->cols) Efi::Throw("array size mismatch", ~0ull);
    for (int i = 0; i < src->size; ++i) {
        double v = floor(src->data[i] + 0.5);
        if (v < 0.0) {
            if (!clamp) Efi::Throw("overflow", ~0ull);
            dst->data[i] = 0;
        } else if (v > 4294967295.0) {
            if (!clamp) Efi::Throw("overflow", ~0ull);
            dst->data[i] = 4294967295u;
        } else {
            dst->data[i] = (unsigned int)v;
        }
    }
    double work = (double)src->size * 5.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
}

}  // namespace mutils
