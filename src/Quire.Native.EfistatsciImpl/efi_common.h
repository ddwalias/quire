#pragma once

#include "efi_alloc.h"
#include "efi_export.h"

namespace Efi {

[[noreturn]] __declspec(dllimport) void Throw(const char *message, unsigned __int64 code);

struct EfiCallContext {
    // Only the allocator at +80h is read by this DLL; the fields before it are not inferred yet.
    unsigned char unknown_[0x80];
    ImcAllocator *allocator;
};

}  // namespace Efi

// Every element type's matrix has the same header: 32-bit rows, cols and size, then the data pointer at +10h.
template <typename T>
struct typed_mat {
    int rows;
    int cols;
    int size;
    T *data;
};

struct _double_mat : typed_mat<double> {};
struct _float_mat : typed_mat<float> {};
struct _sint32_mat : typed_mat<long> {};
struct _sint16_mat : typed_mat<short> {};
struct _uint32_mat : typed_mat<unsigned int> {};
struct _uint16_mat : typed_mat<unsigned short> {};
struct _uint8_mat : typed_mat<unsigned char> {};

// Boundary handling for neighbourhood filters (global, as in the exports' signatures). Only the zero (0) and
// replicate (3) rules are handled by the filters mirrored so far; they reject any other value.
enum _mutil_boundary_type { MUTIL_BOUNDARY_ZERO = 0, MUTIL_BOUNDARY_REPLICATE = 3 };

// The element type tags of _univ_mat (and the other univ containers). EFI_SINT8 has no typed operations.
enum efi_type { EFI_UINT8, EFI_SINT8, EFI_UINT16, EFI_SINT16, EFI_UINT32, EFI_SINT32, EFI_FLOAT, EFI_DOUBLE, EFI_DCOMPLEX };

// A matrix of any element type: the header of typed_mat, then the element type tag (efi_type) at +18h.
struct _univ_mat {
    int rows;
    int cols;
    int size;
    void *data;
    int type;
};

struct _dcomplex {
    double re;
    double im;
};
struct _dcomplex_mat : typed_mat<_dcomplex> {};

// A scalar of any element type: the value, then its efi_type tag at +10h.
struct _univ_scalar {
    union {
        unsigned char u8;
        unsigned short u16;
        short s16;
        unsigned int u32;
        long s32;
        float flt;
        double dbl;
        _dcomplex cpx;
    };
    int type;
};

namespace mutils {

// One definition serves every object, as the original's single static does. The image starts at 1e6, and only a zeroed
// step is raised to 1e7.
inline double mutil_interrupt_step = 1000000.0;

inline unsigned char mutil_interrupt_inline(double work, double *progress, void *) {
    if (mutil_interrupt_step == 0.0) mutil_interrupt_step = 10000000.0;
    if (progress != nullptr && work > *progress) *progress += mutil_interrupt_step;
    return 0;
}

}  // namespace mutils
