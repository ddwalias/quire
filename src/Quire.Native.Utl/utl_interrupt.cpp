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

bool InterruptControl::IsInterrupted() {
    InterruptControl *control = static_cast<InterruptControl *>(TlsGetValue(s_tls_index));
    return control && control->interrupted;
}

void InterruptControl::ThreadInitialize(InterruptControl *control) {
    TlsSetValue(s_tls_index, control);
}

// Set by the TLS allocation in the original's static initialisation, which is not yet ported.
DWORD InterruptControl::s_tls_index;

}  // namespace utl
