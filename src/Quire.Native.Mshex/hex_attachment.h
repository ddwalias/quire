#pragma once

class CriticalSection;

// The virtual base of Component, Value, Environment and the other Hex classes. Mathcad keeps it in the global namespace
// (RTTI `.?AVAttachment@@`). Its functions live in hex_attachment.cpp, which is compiled with /EHsc while the rest of
// mshex uses /EHa (decision D28).
class Attachment {
public:
    Attachment();
    virtual ~Attachment();
    virtual void Destroy();

private:
    int references_;
    CriticalSection *lock_;
};
