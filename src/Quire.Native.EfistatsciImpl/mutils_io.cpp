#include "mutils_mat.h"

#include <stdio.h>
#include <string.h>

namespace mutils {

EFI_EXPORT const char *matu8_write_raw(Efi::EfiCallContext *, FILE *fp, const _uint8_mat *m, void *context) {
    if (fp == nullptr) return "null pointer";
    check_src_f(m);
    for (int done = 0; done < m->size;) {
        int left = m->size - done;
        size_t chunk = (unsigned int)left < 0xFFFFFFFFu ? (size_t)left : 0xFFFFFFFFu;
        if (fwrite(m->data + done, 1, chunk, fp) != chunk) return "Input-output error";
        done += (int)chunk;
    }
    double work = 0.0;
    work += m->size * 1000;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}


// The original zeroes the swap index before branching on the matrix size.
EFI_EXPORT const char *mats16_write_raw(Efi::EfiCallContext *ctx, FILE *fp, const _sint16_mat *m, unsigned char swap, void *context) {
    if (fp == nullptr) return "null pointer";
    check_src_f(m);
    int size = m->size;
    int bytes = size * (int)sizeof(short);
    unsigned char *buf = static_cast<unsigned char *>(ctx->allocator->allocate_array(bytes));
    const unsigned char *in = (const unsigned char *)m->data;
    if (in == nullptr || buf == nullptr) Efi::Throw("null pointer", ~0ull);
    int n = m->size;
    int i = 0;
    if (n > 0) {
    if (!swap) {
        memmove(buf, in, n * sizeof(short));
    } else {
        int limit = n * (int)sizeof(short);
        for (; i < limit; i += 2) {
            buf[i + 0] = *(in + i + 1);
            buf[i + 1] = *(in + i + 0);
        }
    }
    } else {
        Efi::Throw("Illegal data value", ~0ull);
    }
    int count = m->size;
    for (int done = 0; done < bytes;) {
        int left = bytes - done;
        size_t chunk = (unsigned int)left < 0xFFFFFFFFu ? (size_t)left : 0xFFFFFFFFu;
        if (fwrite(buf + done, 1, chunk, fp) != chunk) return "Input-output error";
        done += (int)chunk;
    }
    double work = 0.0;
    work += count * 2;
    work += size * 2000;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}


EFI_EXPORT const char *matdbl_write_raw(Efi::EfiCallContext *ctx, FILE *fp, const _double_mat *m, unsigned char swap, void *context) {
    if (fp == nullptr) return "null pointer";
    check_src_f(m);
    int size = m->size;
    int bytes = size * (int)sizeof(double);
    unsigned char *buf = static_cast<unsigned char *>(ctx->allocator->allocate_array(bytes));
    const unsigned char *in = (const unsigned char *)m->data;
    if (in == nullptr || buf == nullptr) Efi::Throw("null pointer", ~0ull);
    int n = m->size;
    int i = 0;
    if (n > 0) {
    if (!swap) {
        memmove(buf, in, n * sizeof(double));
    } else {
        for (; i < n * (int)sizeof(double); i += 8) {
            buf[i + 0] = *(in + i + 7);
            buf[i + 1] = *(in + i + 6);
            buf[i + 2] = *(in + i + 5);
            buf[i + 3] = *(in + i + 4);
            buf[i + 4] = *(in + i + 3);
            buf[i + 5] = *(in + i + 2);
            buf[i + 6] = *(in + i + 1);
            buf[i + 7] = *(in + i + 0);
        }
    }
    } else {
        Efi::Throw("Illegal data value", ~0ull);
    }
    int count = m->size;
    for (int done = 0; done < bytes;) {
        int left = bytes - done;
        size_t chunk = (unsigned int)left < 0xFFFFFFFFu ? (size_t)left : 0xFFFFFFFFu;
        if (fwrite(buf + done, 1, chunk, fp) != chunk) return "Input-output error";
        done += (int)chunk;
    }
    double work = 0.0;
    work += count + count;
    work += size * 8000;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *matflt_write_raw(Efi::EfiCallContext *ctx, FILE *fp, const _float_mat *m, unsigned char swap, void *context) {
    if (fp == nullptr) return "null pointer";
    check_src_f(m);
    int size = m->size;
    int bytes = size * (int)sizeof(float);
    unsigned char *buf = static_cast<unsigned char *>(ctx->allocator->allocate_array(bytes));
    const unsigned char *in = (const unsigned char *)m->data;
    if (in == nullptr || buf == nullptr) Efi::Throw("null pointer", ~0ull);
    int n = m->size;
    int i = 0;
    if (n > 0) {
    if (!swap) {
        memmove(buf, in, n * sizeof(float));
    } else {
        for (; i < n * (int)sizeof(float); i += 4) {
            buf[i + 0] = *(in + i + 3);
            buf[i + 1] = *(in + i + 2);
            buf[i + 2] = *(in + i + 1);
            buf[i + 3] = *(in + i + 0);
        }
    }
    } else {
        Efi::Throw("Illegal data value", ~0ull);
    }
    int count = m->size;
    for (int done = 0; done < bytes;) {
        int left = bytes - done;
        size_t chunk = (unsigned int)left < 0xFFFFFFFFu ? (size_t)left : 0xFFFFFFFFu;
        if (fwrite(buf + done, 1, chunk, fp) != chunk) return "Input-output error";
        done += (int)chunk;
    }
    double work = 0.0;
    work += count + count;
    work += size * 4000;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *mats32_write_raw(Efi::EfiCallContext *ctx, FILE *fp, const _sint32_mat *m, unsigned char swap, void *context) {
    if (fp == nullptr) return "null pointer";
    check_src_f(m);
    int size = m->size;
    int bytes = size * (int)sizeof(long);
    unsigned char *buf = static_cast<unsigned char *>(ctx->allocator->allocate_array(bytes));
    const unsigned char *in = (const unsigned char *)m->data;
    if (in == nullptr || buf == nullptr) Efi::Throw("null pointer", ~0ull);
    int n = m->size;
    int i = 0;
    if (n > 0) {
    if (!swap) {
        memmove(buf, in, n * sizeof(long));
    } else {
        for (; i < n * (int)sizeof(long); i += 4) {
            buf[i + 0] = *(in + i + 3);
            buf[i + 1] = *(in + i + 2);
            buf[i + 2] = *(in + i + 1);
            buf[i + 3] = *(in + i + 0);
        }
    }
    } else {
        Efi::Throw("Illegal data value", ~0ull);
    }
    int count = m->size;
    for (int done = 0; done < bytes;) {
        int left = bytes - done;
        size_t chunk = (unsigned int)left < 0xFFFFFFFFu ? (size_t)left : 0xFFFFFFFFu;
        if (fwrite(buf + done, 1, chunk, fp) != chunk) return "Input-output error";
        done += (int)chunk;
    }
    double work = 0.0;
    work += count * 4;
    work += size * 4000;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *matu32_write_raw(Efi::EfiCallContext *ctx, FILE *fp, const _uint32_mat *m, unsigned char swap, void *context) {
    if (fp == nullptr) return "null pointer";
    check_src_f(m);
    int size = m->size;
    int bytes = size * (int)sizeof(unsigned int);
    unsigned char *buf = static_cast<unsigned char *>(ctx->allocator->allocate_array(bytes));
    const unsigned char *in = (const unsigned char *)m->data;
    if (in == nullptr || buf == nullptr) Efi::Throw("null pointer", ~0ull);
    int n = m->size;
    int i = 0;
    if (n > 0) {
    if (!swap) {
        memmove(buf, in, n * sizeof(unsigned int));
    } else {
        for (; i < n * (int)sizeof(unsigned int); i += 4) {
            buf[i + 0] = *(in + i + 3);
            buf[i + 1] = *(in + i + 2);
            buf[i + 2] = *(in + i + 1);
            buf[i + 3] = *(in + i + 0);
        }
    }
    } else {
        Efi::Throw("Illegal data value", ~0ull);
    }
    int count = m->size;
    for (int done = 0; done < bytes;) {
        int left = bytes - done;
        size_t chunk = (unsigned int)left < 0xFFFFFFFFu ? (size_t)left : 0xFFFFFFFFu;
        if (fwrite(buf + done, 1, chunk, fp) != chunk) return "Input-output error";
        done += (int)chunk;
    }
    double work = 0.0;
    work += count * 4;
    work += size * 4000;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

EFI_EXPORT const char *matu16_write_raw(Efi::EfiCallContext *ctx, FILE *fp, const _uint16_mat *m, unsigned char swap, void *context) {
    if (fp == nullptr) return "null pointer";
    check_src_f(m);
    int size = m->size;
    int bytes = size * (int)sizeof(unsigned short);
    unsigned char *buf = static_cast<unsigned char *>(ctx->allocator->allocate_array(bytes));
    const unsigned char *in = (const unsigned char *)m->data;
    if (in == nullptr || buf == nullptr) Efi::Throw("null pointer", ~0ull);
    int n = m->size;
    int i = 0;
    if (n > 0) {
    if (!swap) {
        memmove(buf, in, n * sizeof(unsigned short));
    } else {
        for (; i < n * (int)sizeof(unsigned short); i += 2) {
            buf[i + 0] = *(in + i + 1);
            buf[i + 1] = *(in + i + 0);
        }
    }
    } else {
        Efi::Throw("Illegal data value", ~0ull);
    }
    int count = m->size;
    for (int done = 0; done < bytes;) {
        int left = bytes - done;
        size_t chunk = (unsigned int)left < 0xFFFFFFFFu ? (size_t)left : 0xFFFFFFFFu;
        if (fwrite(buf + done, 1, chunk, fp) != chunk) return "Input-output error";
        done += (int)chunk;
    }
    double work = 0.0;
    work += count * 2;
    work += size * 2000;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}

}  // namespace mutils
