#include "mctranslator.h"

namespace Engine {

Hex::Environment* ModuleEnv::GetHexEnv() const {
    return m_hexEnv;
}

void ModuleEnv::SetBuilderFactory(void* factory) {
    m_builderFactory = factory;
}

void ModuleEnv::SetModuleInit(void* init) {
    m_moduleInit = init;
}

}
