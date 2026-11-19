#include "Msclr.h"

namespace Mpl {

    void* pack(System::Object^ object)
    {
        System::IntPtr ptr = System::Runtime::InteropServices::GCHandle::ToIntPtr(System::Runtime::InteropServices::GCHandle::Alloc(object));
        return ptr.ToPointer();
    }

    void free(void* handle)
    {
        System::IntPtr ptr(handle);
        System::Runtime::InteropServices::GCHandle gch = System::Runtime::InteropServices::GCHandle::FromIntPtr(ptr);
        gch.Free();
    }

    Loaded* GetModuleState(const Hex::Module& module)
    {
        return static_cast<Loaded*>(module.GetState());
    }

    Hex::Heap* GetModuleHeap(const Hex::Module& module)
    {
        return static_cast<Loaded*>(module.GetState())->m_heap;
    }

    Table* GetModuleTable(const Hex::Module& module)
    {
        return static_cast<Loaded*>(module.GetState())->m_table;
    }

    void* GetModuleHandle(const Hex::Module& module)
    {
        return static_cast<Loaded*>(module.GetState())->m_loader;
    }

    void SetAssemblyPath(Substring<const char> path)
    {
        Quire::Mpl::Run::Rts::AssemblyPath = ToString(path);
    }

    void SetAssemblyPath(Substring<const wchar_t> path)
    {
        Quire::Mpl::Run::Rts::AssemblyPath = gcnew System::String(path.begin(), 0, static_cast<int>(path.size()));
    }
}
