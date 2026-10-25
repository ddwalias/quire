#include "hex_attachment.h"

#include <Windows.h>

class CriticalSection {
public:
    CriticalSection() { InitializeCriticalSection(&section_); }
    ~CriticalSection() { DeleteCriticalSection(&section_); }

private:
    CRITICAL_SECTION section_;
};

Attachment::Attachment() : references_(1), lock_(new CriticalSection) {}

Attachment::~Attachment() {
    delete lock_;
}

void Attachment::Destroy() {
    delete this;
}
