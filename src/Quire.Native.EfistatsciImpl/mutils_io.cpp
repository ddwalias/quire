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

}  // namespace mutils
