#pragma once

#include <ostream>

// Engine::Unmanaged classes exported by mspcons.dll. Layouts and virtual slot order are read from the original's
// vftables and RTTI (docs/mirror/native/exports.txt, .mirror-cache). Members without a visible export name are
// named by their role in the code that calls them, marked "role inferred".

namespace Engine {
namespace Unmanaged {

class alloc {
public:
    alloc();
    alloc(alloc const &);
    virtual ~alloc();
    alloc &operator=(alloc const &);

    virtual void *allocate(unsigned __int64 bytes) = 0;
};

class alloced {
public:
    alloced &operator=(alloced const &);
    alloced &operator=(alloced &&);

    static void *operator new(unsigned __int64 bytes, alloc *allocator);
    static void operator delete(void *p);
};

// anyval is abstract: slots 0-4 and 6 are pure in its vftable (_purecall); slot 5 is defined here.
class anyval : public alloced {
public:
    anyval();
    anyval(anyval &&);
    anyval(anyval const &);
    anyval &operator=(anyval &&);
    anyval &operator=(anyval const &);

    virtual anyval *clone(alloc *allocator, anyval *out) const = 0;                // slot 0
    virtual anyval *clone_into(alloc *allocator, anyval *out) const = 0;           // slot 1, role inferred
    virtual void reserved_2() const = 0;                                           // slot 2, role not inferred
    virtual bool excludes(anyval const *other) const = 0;                          // slot 3, role inferred
    virtual void write_value(std::basic_ostream<char> &os) const = 0;              // slot 4, role inferred
    virtual void write_raw(std::basic_ostream<char> &os) const;                    // slot 5
    virtual anyval *get_tag() const = 0;                                           // slot 6

    bool equals(anyval const *other) const;
};

// untagged overrides clone (slot 0), write_raw (5) and get_tag (6). Slots 1-4 stay pure, so it is abstract too.
class untagged : public anyval {
public:
    untagged();
    untagged(untagged &&);
    untagged(untagged const &);
    untagged &operator=(untagged &&);
    untagged &operator=(untagged const &);

    anyval *clone(alloc *allocator, anyval *out) const override;
    anyval *get_tag() const override;
};

}  // namespace Unmanaged
}  // namespace Engine

