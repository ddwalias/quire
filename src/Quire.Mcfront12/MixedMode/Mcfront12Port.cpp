
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

    [Quire::Mirror::MirrorOfAttribute("Engine.IFactoryCallbacks, mcfront12")]
    public interface class IFactoryCallbacks
    {
        void OnCreate(Quire::Engine::IEngine^ e);
        void OnDestroy(Quire::Engine::IEngine^ e);
    };

    [Quire::Mirror::MirrorOfAttribute("Engine.HandleFactory, mcfront12")]
    ref class HandleFactory
    {
    private:
        long m_count;
        Quire::utl_mgd::SafeMutex^ m_mutex;

    public:
        HandleFactory()
            : m_count(0), m_mutex(gcnew Quire::utl_mgd::SafeMutex("HandleFactory mutex"))
        {
        }

        long NewHandle()
        {
            m_mutex->WaitOne();
            try
            {
                return ++m_count;
            }
            finally
            {
                m_mutex->ReleaseMutex();
            }
        }

        bool IsValid(long h)
        {
            return (h > 0 && h < m_count);
        }
    };


    [Quire::Mirror::MirrorOfAttribute("Engine.HasErrors, mcfront12")]
    ref class HasErrors : public Quire::utl_mgd::FunctionBase
    {
    public:
        virtual System::Object^ Invoke(array<System::Object^>^ args) override
        {
            System::Collections::IDictionary^ dictionary = dynamic_cast<System::Collections::IDictionary^>(args[0]);
            if (dictionary != nullptr && dictionary->Contains("%errors"))
            {
                return true;
            }
            return nullptr;
        }
    };

    [Quire::Mirror::MirrorOfAttribute("Engine.TagEncoder, mcfront12")]
    ref class TagEncoder
    {
    public:
        int region_id;

        TagEncoder(int id)
            : region_id(id)
        {
        }

        System::String^ encode(System::String^ tag)
        {
            System::String^ result = Quire::Engine::Tags::EncodeRegionID(region_id, tag);
            System::GC::KeepAlive(this);
            return result;
        }
    };

    [Quire::Mirror::MirrorOfAttribute("Engine.RekeyDict, mcfront12")]
    ref class RekeyDict : public Quire::utl_mgd::FunctionBase
    {
    public:
        virtual System::Object^ Invoke(array<System::Object^>^ args) override
        {
            System::String^ text = args[0]->ToString();
            System::Collections::IDictionary^ dictionary = dynamic_cast<System::Collections::IDictionary^>(args[1]);
            if (dictionary == nullptr)
            {
                return args[1];
            }
            Quire::utl_mgd::LabeledDictionary^ labeledDictionary = gcnew Quire::utl_mgd::LabeledDictionary();
            for each (System::Object^ key in dictionary->Keys)
            {
                System::String^ text2 = dynamic_cast<System::String^>(key);
                if (text2 != nullptr && text2->StartsWith("%"))
                {
                    labeledDictionary[text2] = dictionary[text2];
                }
            }
            labeledDictionary["type " + text] = dictionary["type _raw"];
            labeledDictionary["value " + text] = dictionary["value _raw"];
            labeledDictionary["location " + text] = dictionary["location _raw"];
            labeledDictionary["labels " + text] = dictionary["labels _raw"];
            return labeledDictionary;
        }
    };

    [Quire::Mirror::MirrorOfAttribute("Engine.MergeState, mcfront12")]
    ref class MergeState : public Quire::utl_mgd::FunctionBase
    {
    public:
        virtual System::Object^ Invoke(array<System::Object^>^ args) override
        {
            System::Collections::IDictionary^ dictionary = nullptr;
            System::Collections::IDictionary^ dictionary2 = dynamic_cast<System::Collections::IDictionary^>(args[1]);
            if (dictionary2 == nullptr)
            {
                return args[0];
            }
            Quire::utl_mgd::LabeledDictionary^ labeledDictionary = dynamic_cast<Quire::utl_mgd::LabeledDictionary^>(args[0]);
            if (labeledDictionary != nullptr)
            {
                dictionary = labeledDictionary->Copy();
            }
            else
            {
                System::Collections::IDictionary^ d = dynamic_cast<System::Collections::IDictionary^>(args[0]);
                if (d != nullptr)
                {
                    dictionary = Quire::Engine::CollectionUtils::CopyDict(d);
                }
            }
            if (dictionary == nullptr)
            {
                return nullptr;
            }
            for each (System::Object^ key in dictionary2->Keys)
            {
                if (!dictionary->Contains(key))
                {
                    dictionary[key] = dictionary2[key];
                }
            }
            return dictionary;
        }
    };

    ref class FScope;

    [Quire::Mirror::MirrorOfAttribute("Engine.NodeCollection, mcfront12")]
    ref class NodeCollection
    {
    public:
        int m_fnode;
        System::Collections::Generic::List<long>^ m_ce_nodes;
        FScope^ m_scope;
        FScope^ m_child_scope;

        NodeCollection(int fnode)
            : m_fnode(fnode), m_ce_nodes(gcnew System::Collections::Generic::List<long>())
        {
        }

        property long default[int]
        {
            long get(int i)
            {
                long result = m_ce_nodes[i];
                System::GC::KeepAlive(this);
                return result;
            }
        }

        property int Count
        {
            int get()
            {
                int count = m_ce_nodes->Count;
                System::GC::KeepAlive(this);
                return count;
            }
        }

        void Add(long h)
        {
            m_ce_nodes->Add(h);
            System::GC::KeepAlive(this);
        }
    };

    [Quire::Mirror::MirrorOfAttribute("Engine.FScope, mcfront12")]
    ref class FScope : public Quire::depman::Scope
    {
    private:
        static int s_id = 0;
        int m_id;
        System::Collections::Generic::LinkedListNode<NodeCollection^>^ m_first;
        System::Collections::Generic::LinkedListNode<NodeCollection^>^ m_last;
        System::Collections::Generic::LinkedListNode<NodeCollection^>^ m_owner;

    public:
        virtual property int Id
        {
            int get() override
            {
                return m_id;
            }
        }

        virtual property Quire::depman::Scope^ Parent
        {
            Quire::depman::Scope^ get() override
            {
                FScope^ result = (m_owner != nullptr) ? m_owner->Value->m_scope : nullptr;
                System::GC::KeepAlive(this);
                return result;
            }
        }

        FScope(System::Collections::Generic::LinkedListNode<NodeCollection^>^ owner)
            : m_owner(owner)
        {
            m_id = s_id++;
        }

        FScope()
        {
            m_id = s_id++;
        }

        System::Collections::Generic::LinkedListNode<NodeCollection^>^ First(bool bRec)
        {
            if (bRec && m_first != nullptr && m_first->Value->m_child_scope != nullptr)
            {
                System::Collections::Generic::LinkedListNode<NodeCollection^>^ result = m_first->Value->m_child_scope->First(true);
                System::GC::KeepAlive(this);
                return result;
            }
            System::GC::KeepAlive(this);
            return m_first;
        }

        System::Collections::Generic::LinkedListNode<NodeCollection^>^ FindPreviousInScope(System::Collections::Generic::LinkedListNode<NodeCollection^>^ n)
        {
            System::Collections::Generic::LinkedListNode<NodeCollection^>^ previous = n->Previous;
            if (previous->Value->m_scope != this)
            {
                do
                {
                    previous = previous->Previous;
                }
                while (previous->Value->m_scope != this);
            }
            System::GC::KeepAlive(this);
            return previous;
        }

        System::Collections::Generic::LinkedListNode<NodeCollection^>^ FindNextInScope(System::Collections::Generic::LinkedListNode<NodeCollection^>^ n)
        {
            System::Collections::Generic::LinkedListNode<NodeCollection^>^ next = n->Next;
            if (next->Value->m_scope != this)
            {
                do
                {
                    next = next->Next;
                }
                while (next->Value->m_scope != this);
            }
            System::GC::KeepAlive(this);
            return next;
        }

        System::Collections::Generic::LinkedListNode<NodeCollection^>^ Last()
        {
            return m_last;
        }

        System::Collections::Generic::LinkedListNode<NodeCollection^>^ Owner()
        {
            return m_owner;
        }

        void OnFirst(System::Collections::Generic::LinkedListNode<NodeCollection^>^ n)
        {
            m_first = n;
            m_last = n;
            n->Value->m_scope = this;
        }

        void OnNew(System::Collections::Generic::LinkedListNode<NodeCollection^>^ n)
        {
            m_last = n;
            n->Value->m_scope = this;
        }

        void OnBefore(System::Collections::Generic::LinkedListNode<NodeCollection^>^ n1, System::Collections::Generic::LinkedListNode<NodeCollection^>^ n2)
        {
            n2->Value->m_scope = this;
            if (m_last == n2)
            {
                m_last = FindPreviousInScope(n2);
            }
            if (m_first == n1)
            {
                m_first = n2;
            }
        }

        void OnAfter(System::Collections::Generic::LinkedListNode<NodeCollection^>^ n1, System::Collections::Generic::LinkedListNode<NodeCollection^>^ n2)
        {
            n2->Value->m_scope = this;
            if (m_first == n2)
            {
                m_first = FindNextInScope(n2);
            }
            if (m_last == n1)
            {
                m_last = n2;
            }
        }

        bool OnRemove(System::Collections::Generic::LinkedListNode<NodeCollection^>^ n)
        {
            if (m_first == n)
            {
                if (m_last == n)
                {
                    if (m_owner != nullptr)
                    {
                        m_owner->Value->m_child_scope = nullptr;
                    }
                    System::GC::KeepAlive(this);
                    return true;
                }
                m_first = FindNextInScope(n);
            }
            else if (m_last == n)
            {
                m_last = FindPreviousInScope(n);
            }
            return false;
        }
    };

}}
