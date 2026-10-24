#pragma once

// efiutl.dll's allocator, as used by efistatsci_. Only the member imported here is declared; its layout is not needed
// by the callers in this file.

namespace Efi {

struct ImcAllocator {
    __declspec(dllimport) void *allocate_array(unsigned __int64 bytes);
};

}  // namespace Efi
