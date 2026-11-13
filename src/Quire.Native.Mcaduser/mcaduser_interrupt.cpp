#include "mcaduser.h"

namespace utl {

// Imported from utl.dll (Quire.Native.Utl); the import library is generated from utl.def.
class InterruptControl {
public:
    static __declspec(dllimport) bool IsInterrupted();
};

}  // namespace utl

extern "C" BOOL isUserInterrupted() {
    return utl::InterruptControl::IsInterrupted();
}
