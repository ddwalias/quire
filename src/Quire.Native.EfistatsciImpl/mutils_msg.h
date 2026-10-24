#pragma once

#include "efi_export.h"

// Declarations for efistatsci_.dll's mutils message and utility exports. Signatures come from the export
// names (docs/mirror/native/exports.txt); the enum values are not yet inferred from machine code.

namespace mutils {

enum _mutil_msg_level {
    MUTIL_MSG_LEVEL_UNINFERRED = 0
};

EFI_EXPORT void mutil_abort(void);
EFI_EXPORT void mutil_msg_print(enum _mutil_msg_level level, const char *file, const char *func, int line);
EFI_EXPORT void mutil_msg_print_line(unsigned char c);
EFI_EXPORT void mutil_msg_print_time(unsigned char c);
EFI_EXPORT void mutil_msg_set_level(enum _mutil_msg_level level);
EFI_EXPORT const char *mutil_rand_end(void *state);

}  // namespace mutils
