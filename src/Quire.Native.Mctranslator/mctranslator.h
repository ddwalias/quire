#pragma once

namespace Hex {
    class Environment;
}

namespace Engine {

class ModuleEnv {
public:
    Hex::Environment* GetHexEnv() const;
    void SetBuilderFactory(void* factory);
    void SetModuleInit(void* init);

private:
    char pad_00_[0x58];
    Hex::Environment* m_hexEnv;
    void* pad_60_;
    void* m_builderFactory;
    void* m_moduleInit;
};

}
