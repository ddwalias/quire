#include "EngutilsMgd.h"
#include "EngutilsNative.h"

namespace utl
{
    class __declspec(dllimport) PerformanceTimer
    {
    public:
        PerformanceTimer();
        void Start();
        void Stop();
        void Reset();
        double GetElapsed() const;

    private:
        long long _data[14];
    };

    class __declspec(dllimport) TimeIt
    {
    public:
        TimeIt(Substring<const char> name, bool flag);
        virtual ~TimeIt();

    private:
        long long _data[5];
    };
}

namespace Quire { namespace Engine {

    [Quire::Mirror::MirrorOfAttribute("Engine.Timing, engutils-mgd")]
    public ref class Timing
    {
    private:
        static utl::PerformanceTimer* GetTimer()
        {
            static utl::PerformanceTimer* p = new utl::PerformanceTimer();
            return p;
        }

    public:
        static void Start()
        {
            GetTimer()->Start();
        }

        static void Stop()
        {
            GetTimer()->Stop();
        }

        static void Reset()
        {
            GetTimer()->Reset();
        }

        static double GetElapsed()
        {
            return GetTimer()->GetElapsed();
        }

        Timing() {}
    };

    [Quire::Mirror::MirrorOfAttribute("Engine.Timing2, engutils-mgd")]
    public ref class Timing2
    {
    private:
        static Hashtable^ m_table = gcnew Hashtable();

    public:
        static void Start(String^ msg)
        {
            std::string s = utl::ToStrng(msg);
            utl::TimeIt* t = new utl::TimeIt(s, false);
            ValueType^ val = (Int64)t;
            m_table[msg] = val;
        }

        static void Stop(String^ msg)
        {
            utl::TimeIt* t = (utl::TimeIt*)(Int64)m_table[msg];
            delete t;
        }

        Timing2() {}
    };

}}
