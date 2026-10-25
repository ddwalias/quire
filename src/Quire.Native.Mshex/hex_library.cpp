#include "hex_library.h"

namespace Hex {

const GUID Library::interface_id_ = {0x7f467382, 0x0a1a, 0x11e0, {0xa2, 0x83, 0x11, 0xf8, 0xe2, 0xa8, 0xc3, 0x6c}};

const GUID &Library::GetInterfaceID() {
    return interface_id_;
}

}  // namespace Hex
