#define WIN32_LEAN_AND_MEAN
#define NOMINMAX
#include <sdkddkver.h>

#pragma warning(disable: 4100)

namespace Quire { namespace msclr {

    [Quire::Mirror::MirrorOfAttribute("msclr._detail_class, mccore12")]
    value class _detail_class
    {
    public:
        [Quire::Mirror::MirrorOfAttribute("msclr._detail_class+dummy_struct, mccore12")]
        value class dummy_struct
        {
        public:
            static initonly System::String^ dummy_string = "";
        };

        typedef System::String^ _safe_bool;
        static _safe_bool const _safe_true = dummy_struct::dummy_string;
        static _safe_bool const _safe_false = nullptr;
    };

    [Quire::Mirror::MirrorOfAttribute("msclr.lock, mccore12")]
    ref class lock
    {
    private:
        System::Object^ m_object;
        bool m_locked;

    public:
        lock(Quire::Mcd::RuntimeOptions^ _object)
            : m_object(_object), m_locked(false)
        {
            acquire(-1);
        }

        ~lock()
        {
            if (m_locked)
            {
                System::Threading::Monitor::Exit(m_object);
                m_locked = false;
            }
        }

        bool is_locked()
        {
            return m_locked;
        }

        operator _detail_class::_safe_bool()
        {
            return is_locked() ? _detail_class::_safe_true : _detail_class::_safe_false;
        }

        void acquire(System::TimeSpan _timeout)
        {
            if (!m_locked)
            {
                System::Threading::Monitor::TryEnter(m_object, _timeout, m_locked);
                if (!m_locked)
                {
                    throw System::Runtime::InteropServices::Marshal::GetExceptionForHR(-2147024638);
                }
            }
            System::GC::KeepAlive(this);
        }

        void acquire()
        {
            if (!m_locked)
            {
                System::Threading::Monitor::TryEnter(m_object, -1, m_locked);
                if (!m_locked)
                {
                    throw System::Runtime::InteropServices::Marshal::GetExceptionForHR(-2147024638);
                }
            }
            System::GC::KeepAlive(this);
        }

        void acquire(int _timeout)
        {
            if (!m_locked)
            {
                System::Threading::Monitor::TryEnter(m_object, _timeout, m_locked);
                if (!m_locked)
                {
                    throw System::Runtime::InteropServices::Marshal::GetExceptionForHR(-2147024638);
                }
            }
            System::GC::KeepAlive(this);
        }

        bool try_acquire(System::TimeSpan _timeout)
        {
            if (!m_locked)
            {
                System::Threading::Monitor::TryEnter(m_object, _timeout, m_locked);
                if (!m_locked)
                {
                    return false;
                }
            }
            return true;
        }

        bool try_acquire(int _timeout)
        {
            if (!m_locked)
            {
                System::Threading::Monitor::TryEnter(m_object, _timeout, m_locked);
                if (!m_locked)
                {
                    return false;
                }
            }
            return true;
        }

        void release()
        {
            if (m_locked)
            {
                System::Threading::Monitor::Exit(m_object);
                m_locked = false;
            }
        }
    };

}}

namespace Quire { namespace Engine {

    [Quire::Mirror::MirrorOfAttribute("Engine.DetectAlgos, mccore12")]
    ref class DetectAlgos : Quire::depman::Action
    {
    private:
        System::Collections::IDictionary^ m_algos;
        System::Collections::IDictionary^ m_result;
        static System::Collections::Hashtable^ m_algos_detected = nullptr;
        static System::Object^ ms_lock = gcnew System::Object();

        System::Collections::ICollection^ _DetectAlgos(Quire::depman::Environment^ A_0)
        {
            System::Collections::ArrayList^ arrayList = nullptr;
            System::Threading::Monitor::Enter(ms_lock);
            try
            {
                if (m_algos_detected != nullptr)
                {
                    for each (System::Collections::DictionaryEntry dictionaryEntry in m_algos_detected)
                    {
                        if (dynamic_cast<System::String^>(dictionaryEntry.Key) != nullptr)
                        {
                            Quire::Engine::AlgoInfo^ value = dynamic_cast<Quire::Engine::AlgoInfo^>(dictionaryEntry.Value);
                            if (value != nullptr)
                            {
                                if (arrayList == nullptr)
                                {
                                    arrayList = gcnew System::Collections::ArrayList();
                                }
                                arrayList->Add(value);
                            }
                        }
                    }
                    m_algos_detected = nullptr;
                }
            }
            finally
            {
                System::Threading::Monitor::Exit(ms_lock);
            }
            System::GC::KeepAlive(this);
            return arrayList;
        }

    public:
        DetectAlgos(System::Collections::IDictionary^ A_0, System::Collections::IDictionary^ A_1)
            : m_algos(A_0), m_result(A_1)
        {
        }

        ~DetectAlgos()
        {
        }

        virtual Quire::depman::ActionResult Invoke(Quire::depman::Environment^ env, System::Object^ state)
        {
            System::Collections::ICollection^ collection = _DetectAlgos(env);
            if (collection != nullptr)
            {
                m_result["%algorithms"] = collection;
            }
            else
            {
                m_result->Remove("%algorithms");
            }
            return Quire::depman::ActionResult(m_result, state);
        }

        virtual System::Collections::ICollection^ GetDefs()
        {
            return nullptr;
        }

        virtual bool IsUndefine()
        {
            return false;
        }

        virtual bool IsChildOnly()
        {
            return false;
        }

        static void DeleteAlgos()
        {
            m_algos_detected = nullptr;
        }
    };

}}
