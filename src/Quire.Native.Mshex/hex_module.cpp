#include "hex_module.h"

namespace Hex {

unsigned int Module::GetFlags() const {
    return flags_;
}

void *Module::GetDatum() const {
    return datum_;
}

Module::State *Module::GetState() const {
    return state_;
}

bool Module::IsPersistent() const {
    return persistent_ != nullptr;
}

bool Module::IsTransient() const {
    return persistent_ == nullptr;
}

bool Module::IsLoadable() const {
    return loadable_;
}

bool Module::HasFlags(unsigned int flags) const {
    return (flags_ & flags) == flags;
}

void Module::SetLoadable(bool loadable) {
    loadable_ = loadable;
}

void Module::SetDatum(void *datum) {
    datum_ = datum;
}

unsigned int Module::SetFlags(unsigned int flags) {
    unsigned int old = flags_;
    flags_ = flags;
    return old;
}

unsigned int Module::AddFlags(unsigned int flags) {
    unsigned int old = flags_;
    flags_ |= flags;
    return old;
}

unsigned int Module::ClearFlags(unsigned int flags) {
    unsigned int old = flags_;
    flags_ &= ~flags;
    return old;
}

}  // namespace Hex
