#include <string>
#include <vcclr.h>

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
                        System::Collections::Generic::List<System::String^>^ definitions = m_defined;
                        System::String^ item = obj->ToString();
                        definitions->Add(item);
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
    public ref class HasErrors : public Quire::utl_mgd::FunctionBase
    {
    public:
        HasErrors() {}

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
    public ref class RekeyDict : public Quire::utl_mgd::FunctionBase
    {
    public:
        RekeyDict() {}

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
                System::String^ dictionaryKey = dynamic_cast<System::String^>(key);
                if (dictionaryKey && dictionaryKey->StartsWith("%"))
                {
                    labeledDictionary[dictionaryKey] = dictionary[dictionaryKey];
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
    public ref class MergeState : public Quire::utl_mgd::FunctionBase
    {
    public:
        MergeState() {}

        virtual System::Object^ Invoke(array<System::Object^>^ args) override
        {
            System::Collections::IDictionary^ dictionary = nullptr;
            System::Collections::IDictionary^ rightDictionary = dynamic_cast<System::Collections::IDictionary^>(args[1]);
            if (rightDictionary == nullptr)
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

    [Quire::Mirror::MirrorOfAttribute("Engine.GetRuntimeOptionsNumber, mcfront12")]
    public ref class GetRuntimeOptionsNumber : public Quire::utl_mgd::FunctionBase
    {
    public:
        GetRuntimeOptionsNumber() {}

        virtual System::Object^ Invoke(array<System::Object^>^ args) override
        {
            return (dynamic_cast<::Mcd::RuntimeOptions^>(args[0]))->m_ro_number;
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

    [Quire::Mirror::MirrorOfAttribute("Engine.NodeMap, mcfront12")]
    public ref class NodeMap
    {
    private:
        ::depman::Depman^ m_depman;
        long m_top_ce_node;
        FScope^ m_top_scope;
        long m_lasttoptop;
        System::Collections::Generic::Dictionary<long, long>^ m_ce_to_front;
        System::Collections::Generic::Dictionary<long, System::Collections::Generic::LinkedListNode<NodeCollection^>^>^ m_front_to_nc;
        System::Collections::Generic::LinkedList<NodeCollection^>^ m_ce_nodes;

        long last_ce_node(System::Collections::Generic::LinkedListNode<NodeCollection^>^ n)
        {
            while (n != nullptr)
            {
                NodeCollection^ nc = n->Value;
                if (nc->Count > 0)
                {
                    long res = nc[nc->Count - 1];
                    System::GC::KeepAlive(this);
                    return res;
                }
                n = n->Previous;
            }
            System::GC::KeepAlive(this);
            return m_top_ce_node;
        }

        void _DeleteCENode(long h)
        {
            m_depman->Remove(h);
            m_ce_to_front->Remove(h);
            System::GC::KeepAlive(this);
        }

        void ll_move_before(System::Collections::Generic::LinkedListNode<NodeCollection^>^ n1, System::Collections::Generic::LinkedListNode<NodeCollection^>^ n2)
        {
            System::Collections::Generic::LinkedListNode<NodeCollection^>^ linkedListNode = (n2->Value->m_child_scope != nullptr) ? n2->Value->m_child_scope->First(true) : n2;
            System::Collections::Generic::LinkedListNode<NodeCollection^>^ nodeToMove = n2;
            System::Collections::Generic::LinkedListNode<NodeCollection^>^ previous = n2->Previous;
            m_ce_nodes->Remove(n2);
            m_ce_nodes->AddBefore(n1, n2);
            if (n2 != linkedListNode)
            {
                do
                {
                    System::Collections::Generic::LinkedListNode<NodeCollection^>^ node = nodeToMove;
                    nodeToMove = previous;
                    previous = previous->Previous;
                    m_ce_nodes->Remove(nodeToMove);
                    m_ce_nodes->AddBefore(node, nodeToMove);
                }
                while (nodeToMove != linkedListNode);
            }
            System::GC::KeepAlive(this);
        }

        void ll_move_after(System::Collections::Generic::LinkedListNode<NodeCollection^>^ n1, System::Collections::Generic::LinkedListNode<NodeCollection^>^ n2)
        {
            System::Collections::Generic::LinkedListNode<NodeCollection^>^ linkedListNode = (n2->Value->m_child_scope != nullptr) ? n2->Value->m_child_scope->First(true) : n2;
            System::Collections::Generic::LinkedListNode<NodeCollection^>^ nodeToMove = linkedListNode;
            System::Collections::Generic::LinkedListNode<NodeCollection^>^ next = linkedListNode->Next;
            m_ce_nodes->Remove(linkedListNode);
            m_ce_nodes->AddAfter(n1, linkedListNode);
            if (linkedListNode != n2)
            {
                do
                {
                    System::Collections::Generic::LinkedListNode<NodeCollection^>^ node = nodeToMove;
                    nodeToMove = next;
                    next = next->Next;
                    m_ce_nodes->Remove(nodeToMove);
                    m_ce_nodes->AddAfter(node, nodeToMove);
                }
                while (nodeToMove != n2);
            }
            System::GC::KeepAlive(this);
        }

        void scope_detach(System::Collections::Generic::LinkedListNode<NodeCollection^>^ n)
        {
            if (n->Value->m_scope->OnRemove(n))
            {
                System::Collections::Generic::LinkedListNode<NodeCollection^>^ linkedListNode = n->Value->m_scope->Owner();
                if (linkedListNode != nullptr)
                {
                    ce_sync_child(linkedListNode);
                }
                n->Value->m_scope = nullptr;
            }
            System::GC::KeepAlive(this);
        }

        void ce_sync(System::Collections::Generic::LinkedListNode<NodeCollection^>^ n)
        {
            System::Collections::Generic::LinkedListNode<NodeCollection^>^ linkedListNode = (n->Value->m_child_scope != nullptr) ? n->Value->m_child_scope->First(true) : n;
            System::Collections::Generic::LinkedListNode<NodeCollection^>^ currentNode = linkedListNode;
            long h = last_ce_node(linkedListNode->Previous);
            while (true)
            {
                for each (long current in currentNode->Value->m_ce_nodes)
                {
                    m_depman->MoveAfter(h, current, reinterpret_cast<::depman::Scope^>(currentNode->Value->m_scope));
                    h = current;
                }
                if (currentNode == n)
                {
                    break;
                }
                currentNode = currentNode->Next;
            }
            System::GC::KeepAlive(this);
        }

        void ce_sync_child(System::Collections::Generic::LinkedListNode<NodeCollection^>^ n)
        {
            for each (long current in n->Value->m_ce_nodes)
            {
                m_depman->SetEnvScope(current, reinterpret_cast<::depman::Scope^>(n->Value->m_child_scope));
            }
            System::GC::KeepAlive(this);
        }

        int get_children(System::Collections::Generic::LinkedListNode<NodeCollection^>^ n, System::Collections::IList^ out, int seek)
        {
            if (n->Value->m_child_scope == nullptr)
            {
                System::GC::KeepAlive(this);
                return 0;
            }
            FScope^ child_scope = n->Value->m_child_scope;
            int num = 0;
            System::Collections::Generic::LinkedListNode<NodeCollection^>^ linkedListNode = child_scope->First(false);
            if (linkedListNode != n)
            {
                do
                {
                    if (linkedListNode->Value->m_scope == child_scope)
                    {
                        if (num == seek)
                        {
                            return (int)linkedListNode->Value->m_fnode;
                        }
                        num++;
                        if (out != nullptr)
                        {
                            out->Add(linkedListNode->Value->m_fnode);
                        }
                    }
                    linkedListNode = linkedListNode->Next;
                }
                while (linkedListNode != n);
            }
            System::GC::KeepAlive(this);
            return num;
        }

    public:
        void RemoveFrontNode(long fnode, bool bChildrenOnly)
        {
            System::Collections::Generic::LinkedListNode<NodeCollection^>^ n = m_front_to_nc[fnode];
            RemoveFrontNode(n, bChildrenOnly);
        }

    private:
        void RemoveFrontNode(System::Collections::Generic::LinkedListNode<NodeCollection^>^ n, bool bChildrenOnly)
        {
            System::Collections::Generic::LinkedListNode<NodeCollection^>^ linkedListNode = (n->Value->m_child_scope != nullptr) ? n->Value->m_child_scope->First(true) : n;
            System::Collections::Generic::LinkedListNode<NodeCollection^>^ nextNode = linkedListNode;
            System::Collections::Generic::LinkedListNode<NodeCollection^>^ nodeToRemove = linkedListNode;
            while (true)
            {
                nextNode = nextNode->Next;
                if (nodeToRemove == n)
                {
                    scope_detach(nodeToRemove);
                    if (bChildrenOnly)
                    {
                        break;
                    }
                }
                DeleteCENodes(nodeToRemove);
                System::Collections::Generic::Dictionary<long, System::Collections::Generic::LinkedListNode<NodeCollection^>^>^ front_to_nc = m_front_to_nc;
                int fnode = (int)nodeToRemove->Value->m_fnode;
                front_to_nc->Remove(fnode);
                m_ce_nodes->Remove(nodeToRemove);
                if (nodeToRemove == n)
                {
                    break;
                }
                nodeToRemove = nextNode;
            }
            System::GC::KeepAlive(this);
        }

    public:
        void DeleteCENodes(long fnode)
        {
            System::Collections::Generic::LinkedListNode<NodeCollection^>^ n = m_front_to_nc[fnode];
            DeleteCENodes(n);
        }

    private:
        void DeleteCENodes(System::Collections::Generic::LinkedListNode<NodeCollection^>^ n)
        {
            NodeCollection^ value = n->Value;
            for each (long current in value->m_ce_nodes)
            {
                _DeleteCENode(current);
            }
            value->m_ce_nodes->Clear();
            System::GC::KeepAlive(this);
        }

        static System::Collections::Generic::LinkedListNode<NodeCollection^>^ First(System::Collections::Generic::LinkedListNode<NodeCollection^>^ A_0, bool A_1)
        {
            if (A_0->Value->m_child_scope != nullptr)
            {
                return A_0->Value->m_child_scope->First(A_1);
            }
            return A_0;
        }

    public:
        NodeMap(::depman::Depman^ d, long top_ce, ::depman::Scope^ top_scope)
            : m_depman(d),
              m_top_ce_node(top_ce),
              m_top_scope(dynamic_cast<FScope^>(top_scope)),
              m_lasttoptop(2),
              m_ce_to_front(gcnew System::Collections::Generic::Dictionary<long, long>()),
              m_front_to_nc(gcnew System::Collections::Generic::Dictionary<long, System::Collections::Generic::LinkedListNode<NodeCollection^>^>()),
              m_ce_nodes(gcnew System::Collections::Generic::LinkedList<NodeCollection^>())
        {
        }

        int^ GetFrontNode(long cenode)
        {
            long value = 0;
            if (m_ce_to_front->TryGetValue(cenode, value))
            {
                System::GC::KeepAlive(this);
                return (int)value;
            }
            return nullptr;
        }


        long NewCENodeGen(long fnode, int priority, System::Collections::ICollection^ static_deps, System::Collections::ICollection^ static_defs, ::depman::Action^ action)
        {
            System::Collections::Generic::LinkedListNode<NodeCollection^>^ linkedListNode = m_front_to_nc[fnode];
            long h = m_top_ce_node;
            System::Collections::Generic::LinkedListNode<NodeCollection^>^ currentNode = linkedListNode;
            if (linkedListNode != nullptr)
            {
                do
                {
                    NodeCollection^ value = currentNode->Value;
                    if (value->Count <= 0)
                    {
                        currentNode = currentNode->Previous;
                        continue;
                    }
                    h = value[value->Count - 1];
                    break;
                }
                while (currentNode != nullptr);
            }
            ::depman::Depman^ depman = m_depman;
            FScope^ scope = linkedListNode->Value->m_scope;
            long num = depman->AddAfterWithDef(h, reinterpret_cast<::depman::Scope^>(scope), action, static_deps, static_defs, priority, fnode);
            linkedListNode->Value->Add(num);
            if (linkedListNode->Value->m_child_scope != nullptr)
            {
                m_depman->SetEnvScope(num, reinterpret_cast<::depman::Scope^>(linkedListNode->Value->m_child_scope));
            }
            m_ce_to_front[num] = fnode;
            System::GC::KeepAlive(this);
            return num;
        }

        long NewCENode(long fnode, int priority, System::Collections::ICollection^ static_deps, ::depman::Action^ action)
        {
            System::Collections::ArrayList^ deps = Quire::Engine::CollectionUtils::Copy(static_deps);
            if (m_lasttoptop > 0 && fnode > m_lasttoptop)
            {
                deps->Add("_toptop");
            }
            // Stored in a local first: returning the call directly makes the compiler add a KeepAlive(this) the original does not have.
            long result = NewCENodeGen(fnode, priority, deps, nullptr, action);
            return result;
        }

        long NewCENodeTopTop(long fnode, int priority, System::Collections::ICollection^ static_deps, ::depman::Action^ action)
        {
            System::Collections::ArrayList^ imports = Quire::Engine::CollectionUtils::Copy(static_deps);
            imports->Add("_imports");
            if (fnode > m_lasttoptop)
            {
                m_lasttoptop = fnode;
            }
            System::Collections::ArrayList^ toptop = gcnew System::Collections::ArrayList(1);
            toptop->Add("_toptop");
            // Stored in a local first: returning the call directly makes the compiler add a KeepAlive(this) the original does not have.
            long result = NewCENodeGen(fnode, priority, imports, toptop, action);
            return result;
        }

        void UpdateCENode(long ce_node, int priority, System::Collections::ICollection^ static_deps, ::depman::Action^ action)
        {
            m_depman->Update(ce_node, action, static_deps, priority);
            System::GC::KeepAlive(this);
        }

        void DeleteCENode(long fnode, long cenode)
        {
            System::Collections::Generic::LinkedListNode<NodeCollection^>^ linkedListNode = m_front_to_nc[fnode];
            if (linkedListNode != nullptr)
            {
                linkedListNode->Value->m_ce_nodes->Remove(cenode);
                _DeleteCENode(cenode);
            }
        }

        System::Collections::IEnumerable^ GetCENodes(long fnode)
        {
            System::Collections::Generic::List<long>^ ce_nodes = m_front_to_nc[fnode]->Value->m_ce_nodes;
            System::GC::KeepAlive(this);
            return ce_nodes;
        }

        void LockCENodes(long fnode)
        {
            for each (long current in m_front_to_nc[fnode]->Value->m_ce_nodes)
            {
                m_depman->Lock(current);
            }
            System::GC::KeepAlive(this);
        }

        void LockCEInvalidNodes(long fnode)
        {
            for each (long current in m_front_to_nc[fnode]->Value->m_ce_nodes)
            {
                m_depman->LockInvalid(current);
            }
            System::GC::KeepAlive(this);
        }

        void UnLockCENodes(long fnode)
        {
            for each (long current in m_front_to_nc[fnode]->Value->m_ce_nodes)
            {
                m_depman->UnLock(current);
            }
            System::GC::KeepAlive(this);
        }

        void LockCENodesAfterNode(long fnode, NodeMap globalmap)
        {
            if (!m_depman->GetAutoCalc())
            {
                System::Collections::Generic::LinkedListNode<NodeCollection^>^ linkedListNode = m_front_to_nc[fnode];
                System::Collections::Generic::LinkedListNode<NodeCollection^>^ lastNode = linkedListNode->Value->m_scope->Last();
                System::Collections::Generic::LinkedListNode<NodeCollection^>^ firstNode = linkedListNode->Value->m_scope->First(false);
                System::Collections::Generic::LinkedListNode<NodeCollection^>^ currentNode = linkedListNode;
                if (linkedListNode != nullptr)
                {
                    do
                    {
                        UnLockCENodes(currentNode->Value->m_fnode);
                        if (currentNode == firstNode)
                        {
                            break;
                        }
                        currentNode = currentNode->Previous;
                    }
                    while (currentNode != nullptr);
                }
                if (linkedListNode == lastNode)
                {
                    System::GC::KeepAlive(this);
                    return;
                }
                System::Collections::Generic::LinkedListNode<NodeCollection^>^ next = linkedListNode->Next;
                if (next != nullptr)
                {
                    do
                    {
                        long nextFrontNode = next->Value->m_fnode;
                        if (!globalmap.m_ce_to_front->ContainsValue(nextFrontNode))
                        {
                            LockCEInvalidNodes(nextFrontNode);
                        }
                        if (next == lastNode)
                        {
                            break;
                        }
                        next = next->Next;
                    }
                    while (next != nullptr);
                }
            }
            System::GC::KeepAlive(this);
        }

        void InsertFrontNode(long fnode, long newFnode, IMode mode)
        {
            System::Collections::Generic::LinkedListNode<NodeCollection^>^ linkedListNode = nullptr;
            System::Collections::Generic::LinkedListNode<NodeCollection^>^ insertedNode = gcnew System::Collections::Generic::LinkedListNode<NodeCollection^>(gcnew NodeCollection(newFnode));
            m_front_to_nc[newFnode] = insertedNode;
            linkedListNode = nullptr;
            if (m_front_to_nc->TryGetValue(fnode, linkedListNode))
            {
                if (mode == IMode::CHILD)
                {
                    if (linkedListNode->Value->m_child_scope == nullptr)
                    {
                        FScope^ child_scope = gcnew FScope(linkedListNode);
                        linkedListNode->Value->m_child_scope = child_scope;
                        linkedListNode->Value->m_child_scope->OnFirst(insertedNode);
                        ce_sync_child(linkedListNode);
                    }
                    else
                    {
                        linkedListNode->Value->m_child_scope->OnNew(insertedNode);
                    }
                    m_ce_nodes->AddBefore(linkedListNode, insertedNode);
                }
                else if (mode == IMode::BEFORE)
                {
                    System::Collections::Generic::LinkedList<NodeCollection^>^ ce_nodes = m_ce_nodes;
                    System::Collections::Generic::LinkedListNode<NodeCollection^>^ beforeNode = linkedListNode;
                    System::Collections::Generic::LinkedListNode<NodeCollection^>^ node = (beforeNode->Value->m_child_scope != nullptr) ? beforeNode->Value->m_child_scope->First(true) : beforeNode;
                    ce_nodes->AddBefore(node, insertedNode);
                    linkedListNode->Value->m_scope->OnBefore(linkedListNode, insertedNode);
                }
                else
                {
                    m_ce_nodes->AddAfter(linkedListNode, insertedNode);
                    linkedListNode->Value->m_scope->OnAfter(linkedListNode, insertedNode);
                }
            }
            else
            {
                if (mode == IMode::CHILD)
                {
                    throw gcnew System::Exception("illegal child");
                }
                FScope^ top_scope = m_top_scope;
                insertedNode->Value->m_scope = top_scope;
                if (mode == IMode::AFTER)
                {
                    m_ce_nodes->AddLast(insertedNode);
                }
                else
                {
                    m_ce_nodes->AddFirst(insertedNode);
                }
            }
            System::GC::KeepAlive(this);
        }

        void MoveFrontNode(long fnode1, long fnode2, IMode mode)
        {
            System::Collections::Generic::LinkedListNode<NodeCollection^>^ linkedListNode = m_front_to_nc[fnode1];
            System::Collections::Generic::LinkedListNode<NodeCollection^>^ targetNode = m_front_to_nc[fnode2];
            if (mode == IMode::CHILD)
            {
                if (targetNode->Value->m_scope == linkedListNode->Value->m_child_scope)
                {
                    System::GC::KeepAlive(this);
                    return;
                }
            }
            if (linkedListNode->Value->m_scope == targetNode->Value->m_scope)
            {
                if (mode == IMode::AFTER)
                {
                    System::Collections::Generic::LinkedListNode<NodeCollection^>^ targetScopeFirst = (targetNode->Value->m_child_scope != nullptr) ? targetNode->Value->m_child_scope->First(true) : targetNode;
                    if (linkedListNode->Next == targetScopeFirst)
                    {
                        System::GC::KeepAlive(this);
                        return;
                    }
                }
                else if (mode == IMode::BEFORE)
                {
                    if (((linkedListNode->Value->m_child_scope != nullptr) ? linkedListNode->Value->m_child_scope->First(true) : linkedListNode)->Previous == targetNode)
                    {
                        System::GC::KeepAlive(this);
                        return;
                    }
                }
            }
            if (mode == IMode::CHILD)
            {
                scope_detach(targetNode);
                if (linkedListNode->Value->m_child_scope == nullptr)
                {
                    FScope^ childScope = gcnew FScope(linkedListNode);
                    linkedListNode->Value->m_child_scope = childScope;
                    linkedListNode->Value->m_child_scope->OnFirst(targetNode);
                    ce_sync_child(linkedListNode);
                }
                else
                {
                    linkedListNode->Value->m_child_scope->OnNew(targetNode);
                }
                ll_move_before(linkedListNode, targetNode);
            }
            else if (mode == IMode::BEFORE)
            {
                if (targetNode->Value->m_scope != linkedListNode->Value->m_scope)
                {
                    scope_detach(targetNode);
                }
                linkedListNode->Value->m_scope->OnBefore(linkedListNode, targetNode);
                System::Collections::Generic::LinkedListNode<NodeCollection^>^ n = (linkedListNode->Value->m_child_scope != nullptr) ? linkedListNode->Value->m_child_scope->First(true) : linkedListNode;
                ll_move_before(n, targetNode);
            }
            else
            {
                if (targetNode->Value->m_scope != linkedListNode->Value->m_scope)
                {
                    scope_detach(targetNode);
                }
                linkedListNode->Value->m_scope->OnAfter(linkedListNode, targetNode);
                ll_move_after(linkedListNode, targetNode);
            }
            ce_sync(targetNode);
        }


        int GetChildren(long fnode, System::Object^ out)
        {
            System::Collections::Generic::LinkedListNode<NodeCollection^>^ n = m_front_to_nc[fnode];
            System::Collections::IList^ list = dynamic_cast<System::Collections::IList^>(out);
            System::GC::KeepAlive(this);
            return get_children(n, list, -1);
        }

        int GetChild(long fnode, long index)
        {
            System::Collections::Generic::LinkedListNode<NodeCollection^>^ n = m_front_to_nc[fnode];
            System::GC::KeepAlive(this);
            return get_children(n, nullptr, (int)index);
        }

        int GetParent(long fnode)
        {
            System::Collections::Generic::LinkedListNode<NodeCollection^>^ linkedListNode = m_front_to_nc[fnode]->Value->m_scope->Owner();
            if (linkedListNode != nullptr)
            {
                int parentFnode = (int)linkedListNode->Value->m_fnode;
                System::GC::KeepAlive(this);
                return parentFnode;
            }
            System::GC::KeepAlive(this);
            return -1;
        }

        int GetPrevious(long fnode)
        {
            System::Collections::Generic::LinkedListNode<NodeCollection^>^ linkedListNode = m_front_to_nc[fnode];
            System::Collections::Generic::LinkedListNode<NodeCollection^>^ firstScopeNode = linkedListNode->Value->m_scope->First(false);
            if (linkedListNode == firstScopeNode)
            {
                System::GC::KeepAlive(this);
                return -1;
            }
            System::Collections::Generic::LinkedListNode<NodeCollection^>^ previous = linkedListNode->Previous;
            if (previous != nullptr)
            {
                do
                {
                    FScope^% scope = previous->Value->m_scope;
                    FScope^% targetScope = linkedListNode->Value->m_scope;
                    if (scope != targetScope && previous != firstScopeNode)
                    {
                        previous = previous->Previous;
                        continue;
                    }
                    break;
                }
                while (previous != nullptr);
                if (previous != nullptr)
                {
                    int previousFnode = (int)previous->Value->m_fnode;
                    System::GC::KeepAlive(this);
                    return previousFnode;
                }
            }
            System::GC::KeepAlive(this);
            return -1;
        }

        int GetNext(long fnode)
        {
            System::Collections::Generic::LinkedListNode<NodeCollection^>^ linkedListNode = m_front_to_nc[fnode];
            System::Collections::Generic::LinkedListNode<NodeCollection^>^ lastScopeNode = linkedListNode->Value->m_scope->Last();
            if (linkedListNode == lastScopeNode)
            {
                System::GC::KeepAlive(this);
                return -1;
            }
            System::Collections::Generic::LinkedListNode<NodeCollection^>^ next = linkedListNode->Next;
            if (next != nullptr)
            {
                do
                {
                    FScope^% scope = next->Value->m_scope;
                    FScope^% targetScope = linkedListNode->Value->m_scope;
                    if (scope != targetScope && next != lastScopeNode)
                    {
                        next = next->Next;
                        continue;
                    }
                    break;
                }
                while (next != nullptr);
                if (next != nullptr)
                {
                    int nextFnode = (int)next->Value->m_fnode;
                    System::GC::KeepAlive(this);
                    return nextFnode;
                }
            }
            System::GC::KeepAlive(this);
            return -1;
        }

        bool Exists(long fnode)
        {
            bool result = m_front_to_nc->ContainsKey(fnode);
            System::GC::KeepAlive(this);
            return result;
        }

        int GetTopNodes(System::Object^ out)
        {
            if (m_ce_nodes->Count > 0)
            {
                System::Collections::Generic::LinkedListNode<NodeCollection^>^ linkedListNode = m_ce_nodes->First;
                System::Collections::IList^ list = dynamic_cast<System::Collections::IList^>(out);
                if (linkedListNode != nullptr)
                {
                    do
                    {
                        if (linkedListNode->Value->m_scope == m_top_scope)
                        {
                            int fnode = (int)linkedListNode->Value->m_fnode;
                            list->Add(fnode);
                        }
                        linkedListNode = linkedListNode->Next;
                    }
                    while (linkedListNode != nullptr);
                }
            }
            int count = m_ce_nodes->Count;
            System::GC::KeepAlive(this);
            return count;
        }

        void mapNodes_(Quire::utl_mgd::Function^ A_0)
        {
            if (m_ce_nodes->Count > 0)
            {
                System::Collections::Generic::LinkedListNode<NodeCollection^>^ linkedListNode = m_ce_nodes->First;
                if (linkedListNode != nullptr)
                {
                    do
                    {
                        System::ValueType^ a_ = linkedListNode->Value->m_scope->Id;
                        System::ValueType^ nodeHandle = (System::ValueType^)linkedListNode->Value->m_fnode;
                        A_0->Invoke2(nodeHandle, a_);
                        linkedListNode = linkedListNode->Next;
                    }
                    while (linkedListNode != nullptr);
                }
            }
            System::GC::KeepAlive(this);
        }

        static ::depman::Scope^ NewScope()
        {
            return reinterpret_cast<::depman::Scope^>(gcnew FScope());
        }

        static System::Collections::IList^ NewChildCollection()
        {
            return gcnew System::Collections::ArrayList();
        }

    internal:
        System::Collections::Generic::List<PathComponent>^ NodePath(long fnode)
        {
            System::Collections::Generic::List<PathComponent>^ list = gcnew System::Collections::Generic::List<PathComponent>();
            if (fnode == 0)
            {
                return list;
            }
            System::Collections::Generic::LinkedListNode<NodeCollection^>^ linkedListNode = m_front_to_nc[fnode];
            while (true)
            {
                PathComponent item;
                ::depman::Scope^ scope = reinterpret_cast<::depman::Scope^>(linkedListNode->Value->m_scope);
                System::Collections::Generic::LinkedListNode<NodeCollection^>^ firstScopeNode = linkedListNode->Value->m_scope->First(false);
                int num = 0;
                System::Collections::Generic::LinkedListNode<NodeCollection^>^ currentNode = linkedListNode;
                if (linkedListNode != nullptr)
                {
                    do
                    {
                        if (reinterpret_cast<::depman::Scope^>(currentNode->Value->m_scope) == scope)
                        {
                            num++;
                        }
                        if (currentNode == firstScopeNode)
                        {
                            break;
                        }
                        currentNode = currentNode->Previous;
                    }
                    while (currentNode != nullptr);
                }
                item.child_no = num - 1;
                System::Collections::Generic::LinkedListNode<NodeCollection^>^ scopeOwner = linkedListNode->Value->m_scope->Owner();
                int parent = (!scopeOwner) ? -1 : scopeOwner->Value->m_fnode;
                item.parent = parent;
                list->Add(item);
                if (scopeOwner == nullptr)
                {
                    break;
                }
                linkedListNode = scopeOwner;
            }
            list->Reverse();
            System::GC::KeepAlive(this);
            return list;
        }
    };

    [Quire::Mirror::MirrorOfAttribute("Engine.NodeNavigation, mcfront12")]
    ref class NodeNavigation
    {
    public:
        static System::Collections::Generic::List<DCount>^ NavigateTo(System::Object^ nodes, long from, long to)
        {
            System::Collections::Generic::List<DCount>^ list = gcnew System::Collections::Generic::List<DCount>();
            NodeMap^ nodeMap = dynamic_cast<NodeMap^>(nodes);
            System::Collections::Generic::List<PathComponent>^ sourcePath = nodeMap->NodePath(from);
            System::Collections::Generic::List<PathComponent>^ targetPath = nodeMap->NodePath(to);
            int commonPathLimit = System::Math::Min(sourcePath->Count, targetPath->Count);
            int commonPathLength = 0;
            if (0 < commonPathLimit)
            {
                do
                {
                    PathComponent sourceComponent = sourcePath[commonPathLength];
                    PathComponent targetComponent = targetPath[commonPathLength];
                    if (sourceComponent.parent != targetComponent.parent)
                    {
                        break;
                    }
                    commonPathLength++;
                }
                while (commonPathLength < commonPathLimit);
            }
            DCount upwardCount((Direction)0, sourcePath->Count - commonPathLength);
            list->Add(upwardCount);
            int sourceChildNumber = sourcePath[commonPathLength - 1].child_no;
            int targetChildNumber = targetPath[commonPathLength - 1].child_no;
            if (targetChildNumber > sourceChildNumber)
            {
                DCount rightwardCount((Direction)2, targetChildNumber - sourceChildNumber);
                list->Add(rightwardCount);
            }
            else if (targetChildNumber < sourceChildNumber)
            {
                DCount leftwardCount((Direction)3, sourceChildNumber - targetChildNumber);
                list->Add(leftwardCount);
            }
            int targetPathIndex = commonPathLength;
            if (commonPathLength < targetPath->Count)
            {
                do
                {
                    DCount downwardCount((Direction)1, 1);
                    list->Add(downwardCount);
                    PathComponent targetComponent = targetPath[targetPathIndex];
                    DCount childCount((Direction)2, targetComponent.child_no);
                    list->Add(childCount);
                    targetPathIndex++;
                }
                while (targetPathIndex < targetPath->Count);
            }
            return list;
        }

        static System::Collections::Generic::List<PathComponent>^ NodePath(System::Object^ nodes, long h)
        {
            return (dynamic_cast<NodeMap^>(nodes))->NodePath(h);
        }

        [System::Runtime::CompilerServices::MethodImpl(System::Runtime::CompilerServices::MethodImplOptions::NoInlining)]
        static long GetParent(System::Object^ nodes, long h)
        {
            return (dynamic_cast<NodeMap^>(nodes))->GetParent(h);
        }

        static System::Collections::Generic::List<long>^ GetChildren(System::Object^ nodes, long h)
        {
            NodeMap^ nodeMap = dynamic_cast<NodeMap^>(nodes);
            System::Collections::Generic::List<long>^ list = gcnew System::Collections::Generic::List<long>();
            nodeMap->GetChildren(h, list);
            return list;
        }

        NodeNavigation()
        {
        }
    };

    [Quire::Mirror::MirrorOfAttribute("Engine.GlobalMap, mcfront12")]
    public ref class GlobalMap : public NodeTracker
    {
    private:
        [Quire::Mirror::MirrorOfAttribute("Engine.GlobalMap+SetZone, mcfront12")]
        ref class SetZone
        {
        private:
            System::Object^ zone;
            NodeTracker^ tracker;

        public:
            SetZone(System::Object^ z, NodeTracker^ tr)
                : zone(z), tracker(tr)
            {
            }

            void Operate(NodeData^ n)
            {
                n->SetZone(zone);
                tracker->OnOutOfDate(n);
                System::GC::KeepAlive(this);
            }
        };

        System::Collections::Generic::Dictionary<long, NodeData^>^ m_handles;
        System::Object^ m_global_zone;
        System::Object^ m_normal_zone;
        System::Collections::Generic::Dictionary<System::String^, System::Collections::Generic::HashSet<NodeData^>^>^ m_global_map;
        System::Collections::Generic::Dictionary<System::String^, System::Collections::Generic::HashSet<NodeData^>^>^ m_normal_map;
        System::Collections::Generic::HashSet<NodeData^>^ m_outofdate_nodes;
        System::Collections::Generic::HashSet<System::String^>^ m_outofdate_vars;

        static System::Collections::Generic::HashSet<NodeData^>^ s_empty_set;
        static System::String^ s_too_many_globals;
        static System::String^ s_normals_and_globals;
        static System::String^ s_global_not_top_level;

        void AddDefinition(System::String^ var, NodeData^ position, System::Collections::Generic::Dictionary<System::String^, System::Collections::Generic::HashSet<NodeData^>^>^ map)
        {
            System::Collections::Generic::HashSet<NodeData^>^ value = nullptr;
            if (!map->TryGetValue(var, value))
            {
                value = gcnew System::Collections::Generic::HashSet<NodeData^>();
                map[var] = value;
            }
            value->Add(position);
            System::GC::KeepAlive(this);
        }

        void RemoveDefinition(System::String^ var, NodeData^ position, System::Collections::Generic::Dictionary<System::String^, System::Collections::Generic::HashSet<NodeData^>^>^ map)
        {
            System::Collections::Generic::HashSet<NodeData^>^ value = nullptr;
            if (!map->TryGetValue(var, value))
            {
                System::GC::KeepAlive(this);
                return;
            }
            value->Remove(position);
            if (value->Count == 0)
            {
                map->Remove(var);
            }
            System::GC::KeepAlive(this);
        }

        System::Collections::Generic::HashSet<NodeData^>^ Lookup(System::String^ var, System::Collections::Generic::Dictionary<System::String^, System::Collections::Generic::HashSet<NodeData^>^>^ map)
        {
            System::Collections::Generic::HashSet<NodeData^>^ value = nullptr;
            if (map->TryGetValue(var, value))
            {
                System::GC::KeepAlive(this);
                return value;
            }
            return s_empty_set;
        }

        [System::Runtime::CompilerServices::MethodImpl(System::Runtime::CompilerServices::MethodImplOptions::NoInlining)]
        System::Collections::Generic::HashSet<NodeData^>^ GetGlobals(System::String^ var)
        {
            System::Collections::Generic::Dictionary<System::String^, System::Collections::Generic::HashSet<NodeData^>^>^ map = m_global_map;
            System::Collections::Generic::HashSet<NodeData^>^ value = nullptr;
            System::Collections::Generic::HashSet<NodeData^>^ result;
            if (map->TryGetValue(var, value))
            {
                System::GC::KeepAlive(this);
                result = value;
            }
            else
            {
                result = s_empty_set;
            }
            return result;
        }

        System::Collections::Generic::HashSet<NodeData^>^ GetNormals(System::String^ var)
        {
            System::Collections::Generic::Dictionary<System::String^, System::Collections::Generic::HashSet<NodeData^>^>^ map = m_normal_map;
            System::Collections::Generic::HashSet<NodeData^>^ value = nullptr;
            System::Collections::Generic::HashSet<NodeData^>^ n;
            if (map->TryGetValue(var, value))
            {
                System::GC::KeepAlive(this);
                n = value;
            }
            else
            {
                n = s_empty_set;
            }
            return FilterTopLevel(n);
        }

        void CheckGlobals()
        {
            System::Collections::Generic::HashSet<System::String^>::Enumerator enumerator = m_outofdate_vars->GetEnumerator();
            if (enumerator.MoveNext())
            {
                do
                {
                    System::String^ current = enumerator.Current;
                    CheckGlobals(current);
                }
                while (enumerator.MoveNext());
            }
            m_outofdate_vars->Clear();
            System::GC::KeepAlive(this);
        }

        void CheckGlobals(System::String^ var)
        {
            System::String^ text = nullptr;
            System::String^ multipleGlobalsError = nullptr;
            int count = FilterTopLevel(GetGlobals(var))->Count;
            if (count > 1)
            {
                multipleGlobalsError = s_too_many_globals;
            }
            else if (count == 1)
            {
                text = s_normals_and_globals;
            }
            System::Collections::Generic::HashSet<NodeData^>::Enumerator enumerator = GetGlobals(var)->GetEnumerator();
            if (enumerator.MoveNext())
            {
                do
                {
                    NodeData^ current = enumerator.Current;
                    long handle = current->Handle();
                    System::String^ error = (IsTopLevel(handle)) ? multipleGlobalsError : s_global_not_top_level;
                    current->SetError(error);
                    m_outofdate_nodes->Add(current);
                }
                while (enumerator.MoveNext());
            }
            System::Collections::Generic::HashSet<NodeData^>::Enumerator normalEnumerator = GetNormals(var)->GetEnumerator();
            if (normalEnumerator.MoveNext())
            {
                do
                {
                    NodeData^ normalNode = normalEnumerator.Current;
                    long handle = normalNode->Handle();
                    System::String^ normalError = (IsTopLevel(handle)) ? text : nullptr;
                    normalNode->SetError(normalError);
                    m_outofdate_nodes->Add(normalNode);
                }
                while (normalEnumerator.MoveNext());
            }
            System::GC::KeepAlive(this);
        }

        NodeData^ FromHandle(long h)
        {
            NodeData^ value = nullptr;
            if (m_handles->TryGetValue(h, value))
            {
                System::GC::KeepAlive(this);
                return value;
            }
            return nullptr;
        }

        void StartTracking(NodeData^ n)
        {
            m_handles[n->Handle()] = n;
            System::Collections::Generic::Dictionary<System::String^, System::Collections::Generic::HashSet<NodeData^>^>^ map = (n->IsGlobal()) ? m_global_map : m_normal_map;
            System::Collections::Generic::List<System::String^>::Enumerator enumerator = n->GetDefinitions()->GetEnumerator();
            if (enumerator.MoveNext())
            {
                do
                {
                    System::String^ current = enumerator.Current;
                    AddDefinition(current, n, map);
                    m_outofdate_vars->Add(current);
                }
                while (enumerator.MoveNext());
            }
            m_outofdate_nodes->Add(n);
            System::GC::KeepAlive(this);
        }

        void StopTracking(NodeData^ n)
        {
            System::Collections::Generic::Dictionary<System::String^, System::Collections::Generic::HashSet<NodeData^>^>^ map = (n->IsGlobal()) ? m_global_map : m_normal_map;
            System::Collections::Generic::List<System::String^>::Enumerator enumerator = n->GetDefinitions()->GetEnumerator();
            if (enumerator.MoveNext())
            {
                do
                {
                    System::String^ current = enumerator.Current;
                    RemoveDefinition(current, n, map);
                    m_outofdate_vars->Add(current);
                }
                while (enumerator.MoveNext());
            }
            m_outofdate_nodes->Remove(n);
            m_handles->Remove(n->Handle());
            System::GC::KeepAlive(this);
        }

        long GetParent(long h)
        {
            long parent = NodeNavigation::GetParent(m_normal_zone, h);
            System::GC::KeepAlive(this);
            return parent;
        }

        long GetTopAncestor(long h)
        {
            long parent = NodeNavigation::GetParent(m_normal_zone, h);
            System::GC::KeepAlive(this);
            long num = parent;
            if (parent != -1)
            {
                long parentHandle;
                do
                {
                    h = num;
                    parentHandle = NodeNavigation::GetParent(m_normal_zone, num);
                    System::GC::KeepAlive(this);
                    num = parentHandle;
                }
                while (parentHandle != -1);
            }
            return h;
        }

        bool IsTopLevel(long h)
        {
            return GetTopAncestor(h) == h;
        }

        System::Collections::Generic::HashSet<NodeData^>^ FilterTopLevel(System::Collections::Generic::HashSet<NodeData^>^ n)
        {
            System::Collections::Generic::HashSet<NodeData^>^ hashSet = gcnew System::Collections::Generic::HashSet<NodeData^>();
            System::Collections::Generic::HashSet<NodeData^>::Enumerator enumerator = n->GetEnumerator();
            if (enumerator.MoveNext())
            {
                do
                {
                    NodeData^ current = enumerator.Current;
                    long num = current->Handle();
                    if (IsTopLevel(num))
                    {
                        hashSet->Add(current);
                    }
                }
                while (enumerator.MoveNext());
            }
            System::GC::KeepAlive(this);
            return hashSet;
        }

        void OperateRecursively(UnaryNodeDataFunc^ f, long h)
        {
            System::Collections::Generic::Stack<long>^ stack = gcnew System::Collections::Generic::Stack<long>();
            System::Collections::Generic::Stack<NodeData^>^ pendingNodes = gcnew System::Collections::Generic::Stack<NodeData^>();
            stack->Push(h);
            if (stack->Count > 0)
            {
                do
                {
                    long num = stack->Pop();
                    NodeData^ nodeData = nullptr;
                    NodeData^ resolvedNode;
                    if (m_handles->TryGetValue(num, nodeData))
                    {
                        System::GC::KeepAlive(this);
                        resolvedNode = nodeData;
                    }
                    else
                    {
                        resolvedNode = nullptr;
                    }
                    if (resolvedNode != nullptr)
                    {
                        pendingNodes->Push(resolvedNode);
                    }
                    System::Collections::Generic::List<long>::Enumerator enumerator = NodeNavigation::GetChildren(m_normal_zone, num)->GetEnumerator();
                    if (enumerator.MoveNext())
                    {
                        do
                        {
                            long current = enumerator.Current;
                            stack->Push(current);
                        }
                        while (enumerator.MoveNext());
                    }
                }
                while (stack->Count > 0);
            }
            if (pendingNodes->Count > 0)
            {
                do
                {
                    NodeData^ n = pendingNodes->Pop();
                    f(n);
                }
                while (pendingNodes->Count > 0);
            }
            System::GC::KeepAlive(this);
        }

        void SetZoneRecursive(long fnode)
        {
            long topAncestor = GetTopAncestor(fnode);
            NodeData^ nodeData = nullptr;
            NodeData^ resolvedNode;
            if (m_handles->TryGetValue(topAncestor, nodeData))
            {
                System::GC::KeepAlive(this);
                resolvedNode = nodeData;
            }
            else
            {
                resolvedNode = nullptr;
            }
            System::Object^ z = (resolvedNode == nullptr || !resolvedNode->IsGlobal()) ? m_normal_zone : m_global_zone;
            SetZone^ a_ = gcnew SetZone(z, this);
            OperateRecursively(gcnew UnaryNodeDataFunc(a_, &SetZone::Operate), fnode);
        }

        void OnRemoveRecursive(long fnode)
        {
            OperateRecursively(gcnew UnaryNodeDataFunc(this, &GlobalMap::OnRemove), fnode);
        }

        void Update()
        {
            CheckGlobals();
            System::Collections::Generic::HashSet<NodeData^>::Enumerator enumerator = m_outofdate_nodes->GetEnumerator();
            if (enumerator.MoveNext())
            {
                do
                {
                    NodeData^ current = enumerator.Current;
                    current->Update();
                }
                while (enumerator.MoveNext());
            }
            m_outofdate_nodes->Clear();
            System::GC::KeepAlive(this);
        }

    public:
        GlobalMap(System::Object^ global_zone, System::Object^ normal_zone)
            : m_handles(gcnew System::Collections::Generic::Dictionary<long, NodeData^>()),
              m_global_zone(global_zone),
              m_normal_zone(normal_zone),
              m_global_map(gcnew System::Collections::Generic::Dictionary<System::String^, System::Collections::Generic::HashSet<NodeData^>^>()),
              m_normal_map(gcnew System::Collections::Generic::Dictionary<System::String^, System::Collections::Generic::HashSet<NodeData^>^>()),
              m_outofdate_nodes(gcnew System::Collections::Generic::HashSet<NodeData^>()),
              m_outofdate_vars(gcnew System::Collections::Generic::HashSet<System::String^>())
        {
        }

        void OnUpdate(long fnode, NodeData^ _new)
        {
            NodeData^ value = nullptr;
            NodeData^ nodeData;
            if (m_handles->TryGetValue(fnode, value))
            {
                System::GC::KeepAlive(this);
                nodeData = value;
            }
            else
            {
                nodeData = nullptr;
            }
            if (nodeData != nullptr)
            {
                StopTracking(nodeData);
            }
            StartTracking(_new);
            SetZoneRecursive(fnode);
            Update();
        }

        void BeforeRemove(long fnode)
        {
            OperateRecursively(gcnew UnaryNodeDataFunc(this, &GlobalMap::OnRemove), fnode);
        }

        void AfterRemove(long /* fnode */)
        {
            Update();
        }

        void BeforeMove(long /* fnode */)
        {
        }

        void AfterMove(long fnode)
        {
            SetZoneRecursive(fnode);
            Update();
        }

        virtual void OnOutOfDate(NodeData^ n)
        {
            m_outofdate_nodes->Add(n);
            System::GC::KeepAlive(this);
        }

        virtual void OnRemove(NodeData^ n)
        {
            StopTracking(n);
        }

    private:
        static GlobalMap()
        {
            s_empty_set = gcnew System::Collections::Generic::HashSet<NodeData^>();
            s_too_many_globals = "multiple global definitions";
            s_normals_and_globals = "normal and global definitions";
            s_global_not_top_level = "illegal context for global definition";
        }
    };

}}

#pragma comment(linker, "/include:?ToActionResult@Engine@@$$FYM?AVActionResult@depman@Quire@@PE$AAVObject@System@@0@Z")
#pragma comment(linker, "/include:?ErrorDictFromMessage@Engine@@$$FYMPE$AAVObject@System@@PE$AAVString@3@II@Z")
#pragma comment(linker, "/include:?ToStrng@utl@@$$FYM?AV?$basic_string@DU?$char_traits@D@std@@V?$allocator@D@2@@std@@PE$AAVObject@System@@@Z")
#pragma comment(linker, "/include:?UintFromObject@Engine@@$$FYMIPE$AAVObject@System@@@Z")
#pragma comment(linker, "/include:?ErrorDictFromCondition@Engine@@$$FYMPE$AAVObject@System@@PE$AAVCondition@mslisp@Quire@@@Z")
#pragma comment(linker, "/include:?ErrorDictFromObject@Engine@@$$FYMPE$AAVObject@System@@PE$AAV23@@Z")
// The original's global functions live in its <Module> under their unqualified-to-Quire names (Engine.X).
namespace Engine {

    Quire::depman::ActionResult ToActionResult(System::Object^ o, System::Object^ state)
    {
        Quire::depman::Action^ action = dynamic_cast<Quire::depman::Action^>(o);
        if (action != nullptr)
        {
            return Quire::depman::ActionResult(action, state);
        }
        Quire::depman::ActionResult^ boxed = dynamic_cast<Quire::depman::ActionResult^>(o);
        if (boxed != nullptr)
        {
            return *boxed;
        }
        return Quire::depman::ActionResult(o, state);
    }

    System::Object^ ErrorDictFromMessage(System::String^ msg, unsigned int argument, unsigned int handle)
    {
        System::Collections::ArrayList^ errors = gcnew System::Collections::ArrayList();
        errors->Add(gcnew ::Mpl::Run::AppException(msg, argument, handle));
        System::Collections::Hashtable^ dictionary = gcnew System::Collections::Hashtable(1);
        dictionary["%errors"] = errors;
        return dictionary;
    }

}


// Engine-side copy of utl::ToStrng(object) for the mixed DLL. The original is a <Module> global of utl.
namespace Engine {

    unsigned int ParseHex(System::String^ ss);

    unsigned int UintFromObject(System::Object^ x)
    {
        if (x == nullptr)
        {
            return 0;
        }
        System::String^ s = x->ToString();
        if (s->StartsWith("0x"))
        {
            return ParseHex(s);
        }
        return *static_cast<int^>(Quire::Engine::StringUtils::ParseInt(s));
    }

    System::Object^ ErrorDictFromCondition(Quire::mslisp::Condition^ c)
    {
        System::Collections::IDictionary^ dictionary = dynamic_cast<System::Collections::IDictionary^>(c->args[0]);
        if (dictionary != nullptr)
        {
            return dictionary;
        }
        System::String^ message = c->args[0]->ToString();
        unsigned int handle = 0;
        unsigned int argument = 0;
        if (c->args->Length > 1)
        {
            handle = UintFromObject(c->args[1]);
        }
        if (c->args->Length > 2)
        {
            argument = UintFromObject(c->args[2]);
        }
        return ErrorDictFromMessage(message, argument, handle);
    }

    System::Object^ ErrorDictFromObject(System::Object^ o)
    {
        Quire::mslisp::Condition^ condition = dynamic_cast<Quire::mslisp::Condition^>(o);
        if (condition != nullptr)
        {
            return ErrorDictFromCondition(condition);
        }
        System::Exception^ exception = dynamic_cast<System::Exception^>(o);
        if (exception != nullptr)
        {
            return ErrorDictFromMessage(exception->Message, 0, 0);
        }
        return ErrorDictFromMessage(o->ToString(), 0, 0);
    }

}

namespace Engine {

    bool HasErrors(Quire::depman::ActionResult r)
    {
        if (r.GetNext() != nullptr)
        {
            return false;
        }
        System::Object^ result = r.GetResult();
        if (result != nullptr)
        {
            System::Collections::IDictionary^ dictionary = dynamic_cast<System::Collections::IDictionary^>(result);
            if (dictionary != nullptr && dictionary->Contains("%errors"))
            {
                return true;
            }
        }
        return false;
    }

}

#pragma comment(linker, "/include:?HasErrors@Engine@@$$FYM_NVActionResult@depman@Quire@@@Z")
