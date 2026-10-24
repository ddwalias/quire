#pragma once

#include "efi_alloc.h"
#include "efi_export.h"

namespace mutils {

EFI_EXPORT void mutil_malloc(Efi::ImcAllocator *allocator, unsigned __int64 bytes, void **out);
EFI_EXPORT void mutil_calloc(Efi::ImcAllocator *allocator, unsigned __int64 count, unsigned __int64 size, void **out);

}  // namespace mutils
