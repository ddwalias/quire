
namespace Quire { namespace Engine {

    enum Direction {};

    [Quire::Mirror::MirrorOfAttribute("Engine.PathComponent, mcfront12")]
    value struct PathComponent
    {
    public:
        int parent;
        int child_no;
    };

    [Quire::Mirror::MirrorOfAttribute("Engine.DCount, mcfront12")]
    value struct DCount
    {
    public:
        Direction direction;
        int count;

        DCount(Direction d, int c)
            : direction(d), count(c)
        {
        }
    };

    [Quire::Mirror::MirrorOfAttribute("Engine.Empty, mcfront12")]
    ref class Empty
    {
    private:
        static System::Collections::Generic::List<System::String^>^ s_empty_list = gcnew System::Collections::Generic::List<System::String^>();
        static System::Collections::Generic::HashSet<System::String^>^ s_empty_set = gcnew System::Collections::Generic::HashSet<System::String^>();

    public:
        static System::Collections::Generic::List<System::String^>^ List()
        {
            return s_empty_list;
        }

        static System::Collections::Generic::HashSet<System::String^>^ Set()
        {
            return s_empty_set;
        }
    };

    [Quire::Mirror::MirrorOfAttribute("Engine.IMode, mcfront12")]
    public enum class IMode
    {
        BEFORE = 0,
        AFTER = 1,
        CHILD = 2
    };

    [Quire::Mirror::MirrorOfAttribute("Engine.Task, mcfront12")]
    interface class Task
    {
        void Do();
    };

    [Quire::Mirror::MirrorOfAttribute("Engine.TaskDelegate0, mcfront12")]
    delegate void TaskDelegate0();

    [Quire::Mirror::MirrorOfAttribute("Engine.Task0, mcfront12")]
    ref class Task0 : public Task
    {
    public:
        TaskDelegate0^ dg;

        Task0(TaskDelegate0^ _dg)
            : dg(_dg)
        {
        }

        virtual void Do()
        {
            dg();
            System::GC::KeepAlive(this);
        }
    };

    generic <typename T1>
    [Quire::Mirror::MirrorOfAttribute("Engine.TaskDelegate1`1, mcfront12")]
    delegate void TaskDelegate1(T1 t1);

    generic <typename T1>
    [Quire::Mirror::MirrorOfAttribute("Engine.Task1`1, mcfront12")]
    ref class Task1 : public Task
    {
    public:
        T1 t1;
        TaskDelegate1<T1>^ dg;

        Task1(TaskDelegate1<T1>^ _dg, T1 _t1)
            : t1(_t1), dg(_dg)
        {
        }

        virtual void Do()
        {
            dg(t1);
            System::GC::KeepAlive(this);
        }
    };

    generic <typename T1, typename T2>
    [Quire::Mirror::MirrorOfAttribute("Engine.TaskDelegate2`2, mcfront12")]
    delegate void TaskDelegate2(T1 t1, T2 t2);

    generic <typename T1, typename T2>
    [Quire::Mirror::MirrorOfAttribute("Engine.Task2`2, mcfront12")]
    ref class Task2 : public Task
    {
    public:
        T1 t1;
        T2 t2;
        TaskDelegate2<T1, T2>^ dg;

        Task2(TaskDelegate2<T1, T2>^ _dg, T1 _t1, T2 _t2)
            : t1(_t1), t2(_t2), dg(_dg)
        {
        }

        virtual void Do()
        {
            dg(t1, t2);
            System::GC::KeepAlive(this);
        }
    };

    generic <typename T1, typename T2, typename T3>
    [Quire::Mirror::MirrorOfAttribute("Engine.TaskDelegate3`3, mcfront12")]
    delegate void TaskDelegate3(T1 t1, T2 t2, T3 t3);

    generic <typename T1, typename T2, typename T3>
    [Quire::Mirror::MirrorOfAttribute("Engine.Task3`3, mcfront12")]
    ref class Task3 : public Task
    {
    public:
        T1 t1;
        T2 t2;
        T3 t3;
        TaskDelegate3<T1, T2, T3>^ dg;

        Task3(TaskDelegate3<T1, T2, T3>^ _dg, T1 _t1, T2 _t2, T3 _t3)
            : t1(_t1), t2(_t2), t3(_t3), dg(_dg)
        {
        }

        virtual void Do()
        {
            dg(t1, t2, t3);
            System::GC::KeepAlive(this);
        }
    };

    generic <typename T1, typename T2, typename T3, typename T4>
    [Quire::Mirror::MirrorOfAttribute("Engine.TaskDelegate4`4, mcfront12")]
    delegate void TaskDelegate4(T1 t1, T2 t2, T3 t3, T4 t4);

    generic <typename T1, typename T2, typename T3, typename T4>
    [Quire::Mirror::MirrorOfAttribute("Engine.Task4`4, mcfront12")]
    ref class Task4 : public Task
    {
    public:
        T1 t1;
        T2 t2;
        T3 t3;
        T4 t4;
        TaskDelegate4<T1, T2, T3, T4>^ dg;

        Task4(TaskDelegate4<T1, T2, T3, T4>^ _dg, T1 _t1, T2 _t2, T3 _t3, T4 _t4)
            : t1(_t1), t2(_t2), t3(_t3), t4(_t4), dg(_dg)
        {
        }

        virtual void Do()
        {
            dg(t1, t2, t3, t4);
            System::GC::KeepAlive(this);
        }
    };

    generic <typename T1, typename T2, typename T3, typename T4, typename T5>
    [Quire::Mirror::MirrorOfAttribute("Engine.TaskDelegate5`5, mcfront12")]
    delegate void TaskDelegate5(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5);

    generic <typename T1, typename T2, typename T3, typename T4, typename T5>
    [Quire::Mirror::MirrorOfAttribute("Engine.Task5`5, mcfront12")]
    ref class Task5 : public Task
    {
    public:
        T1 t1;
        T2 t2;
        T3 t3;
        T4 t4;
        T5 t5;
        TaskDelegate5<T1, T2, T3, T4, T5>^ dg;

        Task5(TaskDelegate5<T1, T2, T3, T4, T5>^ _dg, T1 _t1, T2 _t2, T3 _t3, T4 _t4, T5 _t5)
            : t1(_t1), t2(_t2), t3(_t3), t4(_t4), t5(_t5), dg(_dg)
        {
        }

        virtual void Do()
        {
            dg(t1, t2, t3, t4, t5);
            System::GC::KeepAlive(this);
        }
    };

    generic <typename T1, typename T2, typename T3, typename T4, typename T5, typename T6>
    [Quire::Mirror::MirrorOfAttribute("Engine.TaskDelegate6`6, mcfront12")]
    delegate void TaskDelegate6(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6);

    generic <typename T1, typename T2, typename T3, typename T4, typename T5, typename T6>
    [Quire::Mirror::MirrorOfAttribute("Engine.Task6`6, mcfront12")]
    ref class Task6 : public Task
    {
    public:
        T1 t1;
        T2 t2;
        T3 t3;
        T4 t4;
        T5 t5;
        T6 t6;
        TaskDelegate6<T1, T2, T3, T4, T5, T6>^ dg;

        Task6(TaskDelegate6<T1, T2, T3, T4, T5, T6>^ _dg, T1 _t1, T2 _t2, T3 _t3, T4 _t4, T5 _t5, T6 _t6)
            : t1(_t1), t2(_t2), t3(_t3), t4(_t4), t5(_t5), t6(_t6), dg(_dg)
        {
        }

        virtual void Do()
        {
            dg(t1, t2, t3, t4, t5, t6);
            System::GC::KeepAlive(this);
        }
    };

    [Quire::Mirror::MirrorOfAttribute("Engine.DefinitionAction, mcfront12")]
    public ref class DefinitionAction : public Quire::depman::Action
    {
    protected:
        System::Collections::ICollection^ m_defs;
        Quire::depman::Action^ m_action;
        bool m_child_only;

    public:
        DefinitionAction(System::Collections::ICollection^ defs, Quire::depman::Action^ a, bool b)
            : m_defs(defs), m_action(a), m_child_only(b)
        {
        }

        ~DefinitionAction()
        {
            delete m_action;
        }

        virtual Quire::depman::ActionResult Invoke(Quire::depman::Environment^ env, System::Object^ state)
        {
            return m_action->Invoke(env, state);
        }

        virtual System::Collections::ICollection^ GetDefs()
        {
            return m_defs;
        }

        virtual bool IsUndefine()
        {
            return false;
        }

        virtual bool IsChildOnly()
        {
            return m_child_only;
        }

        virtual System::String^ ToString() override
        {
            System::Text::StringBuilder^ stringBuilder = gcnew System::Text::StringBuilder();
            stringBuilder->Append("{DefinitionAction (defines");
            for each (System::String^ def in m_defs)
            {
                stringBuilder->Append(" \"");
                stringBuilder->Append(def);
                stringBuilder->Append("\"");
            }
            stringBuilder->Append("): ");
            stringBuilder->Append(m_action->ToString());
            stringBuilder->Append("}");
            return stringBuilder->ToString();
        }
    };

    [Quire::Mirror::MirrorOfAttribute("Engine.NodeData, mcfront12")]
    public ref class NodeData
    {
    private:
        long m_fnode;
        Quire::utl_mgd::Function^ m_add;
        Quire::utl_mgd::Function^ m_error;
        Quire::utl_mgd::Function^ m_remove;
        System::Object^ m_zone;
        System::String^ m_error_msg;
        bool m_uptodate;

    public:
        NodeData(long fnode, Quire::utl_mgd::Function^ add, Quire::utl_mgd::Function^ error, Quire::utl_mgd::Function^ remove)
            : m_fnode(fnode), m_add(add), m_error(error), m_remove(remove)
        {
        }

        void SetZone(System::Object^ zone)
        {
            if (zone != m_zone)
            {
                m_zone = zone;
                m_uptodate = false;
            }
        }

        void SetError(System::String^ msg)
        {
            if (msg != m_error_msg)
            {
                m_error_msg = msg;
                m_uptodate = false;
            }
        }

        virtual void Update()
        {
            if (!m_uptodate)
            {
                m_remove->Invoke0();
                if ((System::Object^)m_error_msg != nullptr)
                {
                    m_error->Invoke2(m_zone, m_error_msg);
                }
                else
                {
                    m_add->Invoke1(m_zone);
                }
                m_uptodate = true;
            }
        }

        long Handle()
        {
            return m_fnode;
        }

        virtual bool IsGlobal()
        {
            return false;
        }

        virtual System::Collections::Generic::List<System::String^>^ GetDefinitions()
        {
            return Empty::List();
        }
    };

    [Quire::Mirror::MirrorOfAttribute("Engine.NormalDefinition, mcfront12")]
    public ref class NormalDefinition : public NodeData
    {
    private:
        System::Collections::Generic::List<System::String^>^ m_defined;

    public:
        NormalDefinition(long fnode, array<System::Object^>^ defined, Quire::utl_mgd::Function^ add, Quire::utl_mgd::Function^ error, Quire::utl_mgd::Function^ remove)
            : NodeData(fnode, add, error, remove)
        {
            if (defined != nullptr)
            {
                m_defined = gcnew System::Collections::Generic::List<System::String^>(defined->Length);
                int num = 0;
                if (num < defined->Length)
                {
                    do
                    {
                        System::Object^ obj = defined[num];
                        System::Collections::Generic::List<System::String^>^ defined2 = m_defined;
                        System::String^ item = obj->ToString();
                        defined2->Add(item);
                        num++;
                    }
                    while (num < defined->Length);
                }
            }
            System::GC::KeepAlive(this);
        }

        virtual System::Collections::Generic::List<System::String^>^ GetDefinitions() override
        {
            return (m_defined != nullptr) ? m_defined : Empty::List();
        }
    };

    [Quire::Mirror::MirrorOfAttribute("Engine.GlobalDefinition, mcfront12")]
    public ref class GlobalDefinition : public NormalDefinition
    {
    public:
        GlobalDefinition(long fnode, array<System::Object^>^ defined, Quire::utl_mgd::Function^ add, Quire::utl_mgd::Function^ error, Quire::utl_mgd::Function^ remove)
            : NormalDefinition(fnode, defined, add, error, remove)
        {
        }

        virtual bool IsGlobal() override
        {
            return true;
        }
    };

    [Quire::Mirror::MirrorOfAttribute("Engine.UnaryNodeDataFunc, mcfront12")]
    delegate void UnaryNodeDataFunc(NodeData^ n);

    [Quire::Mirror::MirrorOfAttribute("Engine.NodeTracker, mcfront12")]
    interface class NodeTracker
    {
        void OnOutOfDate(NodeData^ n);
        void OnRemove(NodeData^ n);
    };

}}
