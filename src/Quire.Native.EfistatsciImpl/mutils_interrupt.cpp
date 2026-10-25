#include "efi_common.h"

namespace mutils {

EFI_EXPORT unsigned char mutil_interrupt(double work, double *progress, void *context) {
    return mutil_interrupt_inline(work, progress, context);
}

}  // namespace mutils
