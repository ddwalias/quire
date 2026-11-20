#define WIN32_LEAN_AND_MEAN
#define NOMINMAX
#include <sdkddkver.h>

#include <iostream>
#include <ostream>
#include <string>
#include <vcclr.h>

#pragma warning(disable: 4100 4400 4679 4691 4945)

namespace
{
    int dummy_anon = 0;
}

namespace ms
{
    template <typename T>
    struct Range
    {
        T* first;
        T* last;
    };
}

template <typename T>
class Substring : public ms::Range<T>
{
public:
    Substring(T* p, size_t n) : ms::Range<T>{p, p + n} {}
    Substring(T* s) : Substring(s, std::char_traits<std::remove_const_t<T>>::length(s)) {}
    Substring(const std::basic_string<std::remove_const_t<T>>& s) : Substring{s.c_str(), s.size()} {}
    Substring(const ms::Range<T>& r) : ms::Range<T>(r) {}

    T* begin() const { return this->first; }
    T* end() const { return this->last; }
    size_t size() const { return this->last - this->first; }
};

namespace utl
{
    std::string ToStrng(System::Object^ o)
    {
        if (o == nullptr)
            return "<null>";
        System::String^ s = o->ToString();
        if (s->Length > 0)
        {
            pin_ptr<System::Byte> p = &System::Text::Encoding::UTF8->GetBytes(s)[0];
            return reinterpret_cast<const char*>(p);
        }
        return "";
    }

    __declspec(dllimport) std::ostream& __cdecl Log(Substring<const char> name);

    __declspec(dllimport) bool __cdecl IsLoggingEnabled();
}

namespace Quire { namespace Engine {

    [Quire::Mirror::MirrorOfAttribute("Engine.FactoryImp, mceng")]
    public ref class FactoryImp sealed : public Quire::Engine::IFactory, public Quire::Engine::IFactoryCallbacks
    {
    private:
        Quire::utl_mgd::SafeMutex^ m_engine_creation_mutex = gcnew Quire::utl_mgd::SafeMutex("EngineCreation");
        long long m_engine_last_requested;
        System::Collections::Hashtable^ m_engines = gcnew System::Collections::Hashtable();
        int m_worker_stack_size;
        int m_engine_count = 0;
        System::String^ m_appData;
        System::String^ m_assemblyPath;

        Quire::Engine::IEngine^ CreateEngine(System::String^ A_0)
        {
            m_engine_creation_mutex->WaitOne();
            try
            {
                int engine_count = m_engine_count;
                m_engine_count++;
                int worker_stack_size = m_worker_stack_size;
                Quire::Engine::IEnginePriv^ enginePriv = gcnew Quire::Engine::MCFront12(this, Quire::Engine::MCCore12::Instance(), engine_count, worker_stack_size);
                enginePriv->Initialize(A_0);
                return enginePriv;
            }
            finally
            {
                m_engine_creation_mutex->ReleaseMutex();
            }
        }

    public:
        FactoryImp()
        {
            int max_stack = Quire::Mpl::Run::Rts::m_max_stack;
            m_worker_stack_size = (int)((double)max_stack * 12582.912 + 0.5);
            System::GC::KeepAlive(this);
        }

        ~FactoryImp()
        {
            m_engine_creation_mutex->WaitOne();
            try
            {
                if (m_engines->Count > 0 && ::utl::IsLoggingEnabled())
                {
                    ::utl::Log("debug") << "WARNING: factory destruction: " << m_engines->Count << "undisposed engines exist; " << "terminating program...";
                }
            }
            finally
            {
                m_engine_creation_mutex->ReleaseMutex();
            }
            System::GC::KeepAlive(this);
        }

        virtual void Initialize(System::String^ appData)
        {
            if (m_appData != nullptr)
            {
                throw gcnew System::Exception("FactoryImp::Initialize called twice!");
            }
            m_appData = appData;
        }

        virtual bool VerifyIntegrity()
        {
            static bool bNeedInit = true;
            if (bNeedInit)
            {
                bNeedInit = false;
                try
                {
                    Quire::Engine::MCFront12^ mCFront = gcnew Quire::Engine::MCFront12(this, Quire::Engine::MCCore12::Instance(), -1, m_worker_stack_size);
                    mCFront->Initialize(m_appData);
                    delete mCFront;
                    return true;
                }
                catch (System::Exception^ ex)
                {
                    ::utl::Log("eng") << "Engine factory: integrity check failed" << std::endl << std::endl;
                    ::utl::Log("eng") << ::utl::ToStrng(ex->Message) << std::endl << std::endl;
                    ::utl::Log("eng") << "Stack trace:" << std::endl;
                    ::utl::Log("eng") << ::utl::ToStrng(ex->StackTrace) << std::endl;
                    return false;
                }
                ::utl::Log("eng") << "Engine factory: integrity check failed " << "(unknown cause)" << std::endl;
                System::GC::KeepAlive(this);
                return false;
            }
            return true;
        }

        property int WorkerStackSize
        {
            virtual int get()
            {
                return m_worker_stack_size;
            }
            virtual void set(int n)
            {
                int num = (m_worker_stack_size = n * 3);
                if (num == 0)
                    num = 3145728;
                Quire::Mpl::Run::Rts::m_max_stack = (int)((double)num / 12582.912 + 0.5);
                Quire::Mpl::Run::Rts::m_worker_stack_size = m_worker_stack_size;
            }
        }

        virtual Quire::Engine::IEngine^ NewEngine()
        {
            return CreateEngine(m_appData);
        }

        virtual System::Collections::ICollection^ GetEngines()
        {
            m_engine_creation_mutex->WaitOne();
            try
            {
                return Quire::Engine::CollectionUtils::Copy(m_engines->Keys);
            }
            finally
            {
                m_engine_creation_mutex->ReleaseMutex();
            }
        }

        virtual void OnCreate(Quire::Engine::IEngine^ e)
        {
            m_engine_creation_mutex->WaitOne();
            try
            {
                m_engines[e] = true;
            }
            finally
            {
                m_engine_creation_mutex->ReleaseMutex();
            }
        }

        virtual void OnDestroy(Quire::Engine::IEngine^ e)
        {
            m_engine_creation_mutex->WaitOne();
            try
            {
                m_engines->Remove(e);
            }
            finally
            {
                m_engine_creation_mutex->ReleaseMutex();
            }
        }
    };

}}
