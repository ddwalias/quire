#include "EngutilsMgd.h"

namespace Quire { namespace Engine {

    [Quire::Mirror::MirrorOfAttribute("Engine.CachingWrapper, engutils-mgd")]
    ref class CachingWrapper : public IMathcadComponent2
    {
    private:
        System::Collections::Generic::Dictionary<String^, Mathcad::IValue^>^ m_cache = gcnew System::Collections::Generic::Dictionary<String^, Mathcad::IValue^>();
        IMathcadComponent^ m_component;
        bool m_locked;
        bool m_needs_commit;

    public:
        CachingWrapper(IMathcadComponent^ c)
            : m_component(c)
        {
        }

        IMathcadComponent^ Unwrap()
        {
            return m_component;
        }

        virtual bool LockComponent()
        {
            m_needs_commit = false;
            m_locked = m_component->LockComponent();
            return m_locked;
        }

        virtual bool LockComponent(bool wait)
        {
            m_needs_commit = false;
            m_locked = m_component->LockComponent(wait);
            return m_locked;
        }

        virtual void UnlockComponent()
        {
            try
            {
                if (m_needs_commit)
                {
                    m_component->Commit();
                }
            }
            finally
            {
                if (m_locked)
                {
                    m_component->UnlockComponent();
                }
            }
        }

        virtual void SetValue(String^ place, Mathcad::IValue^ value)
        {
            if (m_component->ShouldSkipUnchangedValue(place))
            {
                Mathcad::IValue^ cached = nullptr;
                if (m_cache->TryGetValue(place, cached) && cached->Equals(value))
                {
                    return;
                }
            }
            m_cache[place] = value;
            m_component->SetValue(place, value);
            m_needs_commit = true;
        }

        virtual Mathcad::IValue^ GetValue(String^ place)
        {
            return m_component->GetValue(place);
        }
    };

    [Quire::Mirror::MirrorOfAttribute("Engine.ComponentSupport, engutils-mgd")]
    [StructLayout(LayoutKind::Sequential, Size = 1)]
    public value struct ComponentSupport
    {
    private:
        static System::Collections::Generic::Dictionary<int, System::Collections::Generic::Dictionary<int, CachingWrapper^>^>^ s_engine_map =
            gcnew System::Collections::Generic::Dictionary<int, System::Collections::Generic::Dictionary<int, CachingWrapper^>^>();

        static System::Collections::Generic::Dictionary<int, CachingWrapper^>^ GetComponentMap(int engine_id)
        {
            System::Collections::Generic::Dictionary<int, CachingWrapper^>^ map = nullptr;
            if (s_engine_map->TryGetValue(engine_id, map))
            {
                return map;
            }
            map = gcnew System::Collections::Generic::Dictionary<int, CachingWrapper^>();
            s_engine_map[engine_id] = map;
            return map;
        }

        static int GetNewHandle(System::Collections::Generic::Dictionary<int, CachingWrapper^>^ m)
        {
            int handle = m->Count + 1000;
            while (m->ContainsKey(handle))
            {
                handle++;
            }
            return handle;
        }

        static CachingWrapper^ Lookup(int engine_id, int handle, bool bThrow)
        {
            System::Collections::Generic::Dictionary<int, CachingWrapper^>^ map = GetComponentMap(engine_id);
            CachingWrapper^ wrapper = nullptr;
            if (map->TryGetValue(handle, wrapper))
            {
                return wrapper;
            }
            if (bThrow)
            {
                throw gcnew Exception("invalid component handle");
            }
            return nullptr;
        }

    public:
        static int RegisterComponent(int engine_id, IMathcadComponent^ component)
        {
            System::Collections::Generic::Dictionary<int, CachingWrapper^>^ map = GetComponentMap(engine_id);
            int handle = GetNewHandle(map);
            map[handle] = gcnew CachingWrapper(component);
            return handle;
        }

        static int RegisterComponent(int engine_id, IMathcadComponent^ component, int handle)
        {
            System::Collections::Generic::Dictionary<int, CachingWrapper^>^ map = GetComponentMap(engine_id);
            if (map->ContainsKey(handle))
            {
                throw gcnew Exception("handle unavailable");
            }
            map[handle] = gcnew CachingWrapper(component);
            return handle;
        }

        static IMathcadComponent^ LookupComponent(int engine_id, int handle)
        {
            CachingWrapper^ wrapper = Lookup(engine_id, handle, false);
            return (wrapper == nullptr) ? nullptr : wrapper->Unwrap();
        }

        static IMathcadComponent2^ LookupComponent2(int engine_id, int handle)
        {
            return Lookup(engine_id, handle, false);
        }

        static IMathcadComponent2^ LookupComponentOrThrow2(int engine_id, int handle)
        {
            return Lookup(engine_id, handle, true);
        }

        static IMathcadComponent^ UnregisterComponent(int engine_id, int handle)
        {
            System::Collections::Generic::Dictionary<int, CachingWrapper^>^ map = GetComponentMap(engine_id);
            CachingWrapper^ wrapper = nullptr;
            if (map->TryGetValue(handle, wrapper))
            {
                map->Remove(handle);
                return wrapper->Unwrap();
            }
            return nullptr;
        }
    };

}}
