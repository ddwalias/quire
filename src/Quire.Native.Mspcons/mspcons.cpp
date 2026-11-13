#include "unmanaged.h"

namespace Engine {
namespace Unmanaged {

extern "C" unsigned char notag_object[16] = {};

alloc::alloc() {
}

alloc::alloc(alloc const &) {
}

alloc::~alloc() {
}

alloc &alloc::operator=(alloc const &) {
    return *this;
}

void *alloced::operator new(unsigned __int64 bytes, alloc *allocator) {
    return allocator->allocate(bytes);
}

void alloced::operator delete(void *) {
}

alloced &alloced::operator=(alloced const &) {
    return *this;
}

alloced &alloced::operator=(alloced &&) {
    return *this;
}

anyval::anyval() {
}

anyval::anyval(anyval &&) {
}

anyval::anyval(anyval const &) {
}

anyval &anyval::operator=(anyval &&) {
    return *this;
}

anyval &anyval::operator=(anyval const &) {
    return *this;
}

void anyval::write_raw(std::basic_ostream<char> &os) const {
    write_value(os);
}

bool anyval::equals(anyval const *other) const {
    return !excludes(other) && !other->excludes(this);
}

untagged::untagged() {
}

untagged::untagged(untagged &&) {
}

untagged::untagged(untagged const &) {
}

untagged &untagged::operator=(untagged &&) {
    return *this;
}

untagged &untagged::operator=(untagged const &) {
    return *this;
}

anyval *untagged::clone(alloc *allocator, anyval *out) const {
    return clone_into(allocator, out);
}

anyval *untagged::get_tag() const {
    return reinterpret_cast<anyval *>(notag_object);
}

}  // namespace Unmanaged
}  // namespace Engine

