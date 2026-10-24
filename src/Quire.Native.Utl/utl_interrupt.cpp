#include "utl.h"

namespace utl {

InterruptControl &InterruptControl::operator=(const InterruptControl &other) {
    interrupted = other.interrupted;
    return *this;
}

InterruptControl &InterruptControl::operator=(InterruptControl &&other) {
    interrupted = other.interrupted;
    return *this;
}

void InterruptControl::RequestInterrupt(bool requested) {
    interrupted = requested;
}

}  // namespace utl
