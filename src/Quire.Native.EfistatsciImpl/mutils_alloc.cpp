#include "mutils_alloc.h"

namespace mutils {

void mutil_malloc(Efi::ImcAllocator *allocator, unsigned __int64 bytes, void **out) {
    *out = allocator->allocate_array(bytes);
}

void mutil_calloc(Efi::ImcAllocator *allocator, unsigned __int64 count, unsigned __int64 size, void **out) {
    *out = allocator->allocate_array(count * size);
}

}  // namespace mutils
